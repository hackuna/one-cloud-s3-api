# 1cloud S3 API

An example of interaction with cloud object storage from [1cloud.ru](https://1cloud.ru/ref/339507) using the Amazon S3 protocol, using C# and .NET 7.

## Configuration

### Environment variables:

```
S3_SERVICE_URL=https://1cloud.store
S3_ACCESS_KEY={YourAccesKey}
S3_SECRET_KEY={YourSecretKey}
```

### Local development in IDE

Configure Environment variables at  ```"environmentVariables"``` section in file ```Properties/launchSettings.json```

```
"environmentVariables": {
	"ASPNETCORE_ENVIRONMENT": "Development",
	"S3_SERVICE_URL": "https://1cloud.store",
	"S3_ACCESS_KEY": "{YourDevAccesKey}",
	"S3_SECRET_KEY": "{YourDevSecretKey}"
}
```

## Docker image

| Container image repository | Last Build |
| -- | -- |
| [ghcr.io/hackuna/one-cloud-s3-api](https://github.com/hackuna/one-cloud-s3-api/pkgs/container/one-cloud-s3-api) | [![Build & Publish](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hackuna/one-cloud-s3-api/actions/workflows/dotnet.yml) |

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
--env=ASPNETCORE_ENVIRONMENT=Production \
--env=S3_SERVICE_URL=https://1cloud.store \
--env=S3_ACCESS_KEY={YourAccesKey} \
--env=S3_SECRET_KEY={YourSecretKey} \
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