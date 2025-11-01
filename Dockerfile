# escape=`
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR C:\src
COPY . .
RUN nuget restore src\OrderSubmissionSystem\OrderSubmissionSystem.Api\OrderSubmissionSystem.Api.csproj -PackagesDirectory C:\packages
RUN msbuild src\OrderSubmissionSystem\OrderSubmissionSystem.Api\OrderSubmissionSystem.Api.csproj /p:Configuration=Release /p:OutputPath=C:\out

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]
WORKDIR C:\inetpub\wwwroot
COPY --from=build C:\out\ .

ENV SeqUrl=http://seq:5341
ENV RedisConnectionString=redis:6379
ENV ProcessorType=Ftp
ENV OrderFileFormat=Xml
ENV FtpUploaderType=Local
ENV ApiKeyCachePrefix=api-key:

EXPOSE 80
