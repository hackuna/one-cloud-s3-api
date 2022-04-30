# 1Cloud.S3.API

Пример взаимодействия с облачным хранилищем от [1cloud.ru](https://1cloud.ru/ref/339507) по протоколу Amazon S3, с использованием с# и asp net core.

Настройки в `appsettings.json`:

```
"StorageOptions": {
    "AccessKey": "{YourAccesKey}",          // Здесь ваш ключ доступа
    "SecretKey": "{YourSecretKey}",         // Здесь ваш секретный ключ
    "ServiceUrl": "https://1cloud.store"    // Здесь endpoint
}
```