# 1cloud S3 API

Easy to use REST API Gateway for [1cloud.ru](https://1cloud.ru/ref/339507) object storage (S3). Based on Microsoft.NET.Sdk.Web, Amazon.S3.SDK, Swagger, OpenApi.

## Configuration

### Environment variables:

```
S3_SERVICE_URL=https://1cloud.store
S3_ACCESS_KEY={YourAccessKey}
S3_SECRET_KEY={YourSecretKey}
SERVICE_API_URL=https://api.1cloud.ru",
SERVICE_API_KEY={YourApiKey}
```

### Local development in IDE

Configure Environment variables at  `"environmentVariables"` section in file `Properties/launchSettings.json`

```
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "S3_SERVICE_URL": "https://1cloud.store",
    "S3_ACCESS_KEY": "{YourAccessKey}",
    "S3_SECRET_KEY": "{YourSecretKey}",
    "SERVICE_API_URL": "https://api.1cloud.ru",
    "SERVICE_API_KEY": "{YourApiKey}"
}
```

## Docker image

Based on [nightly/runtime-deps:7.0-jammy-chiseled](https://mcr.microsoft.com/en-us/product/dotnet/nightly/runtime-deps/about) (Ubuntu 22.04, ×64)

### Build container image command

```
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer -c Release --self-contained true -p:PublishSingleFile=true
```

| Container Image | Last Build | [CodeQL](https://codeql.github.com/) | Size |
| -- | -- | -- | -- |
| 🐳 [ghcr.io/hackuna/one-cloud-s3-api:latest](https://github.com/hackuna/one-cloud-s3-api/pkgs/container/one-cloud-s3-api) | [![Build & Publish](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/dotnet.yml) | [![CodeQL](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/github-code-scanning/codeql) | 51.11 Mb |

Pull image command:

```
docker pull ghcr.io/hackuna/one-cloud-s3-api:latest
```

Run container command:

```
docker run -d \
--restart always \
--name storage-api \
-p 5000:8080 \
--volume=storage-secrets:/root/.aspnet/DataProtection-Keys \
--env=ASPNETCORE_ENVIRONMENT=Development \
--env=ASPNETCORE_URLS=http://+:8080 \
--env=S3_SERVICE_URL=https://1cloud.store \
--env=S3_ACCESS_KEY={YourAccessKey} \
--env=S3_SECRET_KEY={YourSecretKey} \
--env=SERVICE_API_URL=https://api.1cloud.ru \
--env=SERVICE_API_KEY={YourApiKey} \
ghcr.io/hackuna/one-cloud-s3-api:latest
```

Stop container command:

```
docker stop storage-api
```

Remove container command:

```
docker rm storage-api
```
