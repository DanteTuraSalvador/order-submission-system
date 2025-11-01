using Prometheus;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderSubmissionSystem.Api.Controllers
{
    [RoutePrefix("")]
    public class MetricsController : ApiController
    {
        [HttpGet]
        [Route("metrics")]
        public async Task<HttpResponseMessage> GetMetrics()
        {
            using (var stream = new MemoryStream())
            {
                await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream).ConfigureAwait(false);
                stream.Position = 0;
                var metrics = Encoding.UTF8.GetString(stream.ToArray());

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(metrics, Encoding.UTF8, "text/plain")
                };

                return response;
            }
        }
    }
}
