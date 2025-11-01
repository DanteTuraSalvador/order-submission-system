using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace OrderSubmissionSystem.Api.Security
{
    public class FileApiKeyStore : IApiKeyStore, IDisposable
    {
        private readonly string _filePath;
        private readonly ILogger _logger = Log.ForContext<FileApiKeyStore>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private HashSet<string> _keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly FileSystemWatcher _watcher;

        public FileApiKeyStore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            _filePath = filePath;
            LoadKeys();

            var directory = Path.GetDirectoryName(_filePath);
            var fileName = Path.GetFileName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory) && !string.IsNullOrWhiteSpace(fileName))
            {
                _watcher = new FileSystemWatcher(directory, fileName)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
                };

                _watcher.Changed += OnConfigChanged;
                _watcher.Created += OnConfigChanged;
                _watcher.Renamed += OnConfigChanged;
                _watcher.EnableRaisingEvents = true;
            }
        }

        public bool IsValid(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            _lock.EnterReadLock();
            try
            {
                return _keys.Contains(apiKey);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            // Give the file system a short moment to release the handle
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            LoadKeys();
        }

        private void LoadKeys()
        {
            try
            {
                var keys = ReadKeysFromFile();

                _lock.EnterWriteLock();
                _keys = new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load API keys from {FilePath}", _filePath);
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private IEnumerable<string> ReadKeysFromFile()
        {
            if (!File.Exists(_filePath))
            {
                _logger.Warning("API key file {FilePath} does not exist", _filePath);
                return Array.Empty<string>();
            }

            var text = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            var config = JsonConvert.DeserializeObject<ApiKeyFile>(text);
            if (config == null)
            {
                return Array.Empty<string>();
            }

            var keys = new List<string>();

            if (config.Keys != null)
            {
                keys.AddRange(config.Keys
                    .Where(k => k.Enabled.GetValueOrDefault(true))
                    .Select(k => k.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v)));
            }

            if (config.Values != null)
            {
                keys.AddRange(config.Values.Where(v => !string.IsNullOrWhiteSpace(v)));
            }

            return keys;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _lock.Dispose();
        }

        private class ApiKeyFile
        {
            [JsonProperty("keys")]
            public List<ApiKeyEntry> Keys { get; set; }

            [JsonProperty("values")]
            public List<string> Values { get; set; }
        }

        private class ApiKeyEntry
        {
            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("enabled")]
            public bool? Enabled { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}
