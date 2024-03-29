# Kronocrypt API

## Description
This is a tool that will allow users to be able to easily and securely share secrets. To ensure security, secrets are stored encrypted, the key used to decrypt is never stored, and the secret can only be retrieved once and must be done before the secret expires. This is useful if you want to share sensitive information such as credentials without worrying about them being able to be read by an unwanted party. 

The encryption schemes used are as follows:
- The [Rijndael algorithm](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rijndael?view=net-5.0) is used to encrypt the message
- [RSA](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa?view=net-5.0) is used to encrypt the key generated by the Rijandael algorithm
- [SHA-512/RSA](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsapkcs1signatureformatter?view=net-5.0) is used to hash the encrypted message to ensure it was not modified

Once a secret is created, it will produce an ID, a key, and an expire datetime. The ID and key are to be used to decrypt the message. The key is never stored, so if this is lost then the message will *not* be recoverable. The expire datetime indicates when the secret will expire; the secret will not be decrypted once the expire datetime has passed. 

## Usage

### Secret Creation

To create a secret, make the following call:

```
POST /api/secrets

{
    "message": "Hello world!",
    "expireMinutes": 1
}
```

This will encrypt the message `Hello world!` and will give the secret a lifespan of 60 seconds. 

An example response body is below:
```
201 Created
{
  "secretId": "dde83fa5-334a-464a-8132-f376f73a648b",
  "key": "MIICXAIBAAKBgQDX0hQdKn1RynetmIU+bEYsbtYVfqTacKaONTh4V739n7wJgeWsmgveL1f/pYrPaabNigN4DjDzPVwH5DuF1umJCXo/KraBbbMPLP+cmO4awc8wHNTBZdi6JjT1lufj8Dz4MQUuGmIt89PcYUFChrnfEONof1it5JkvVybPPbl4XQIDAQABAoGAP+O/8pZmfPUMEsbpAv64k2TIWZqhIM1icQzOR8npw5Aq8UGUBVGhG8g2K8cM3bPwHj6yIjNJSGisuuVUvZ1OiqiN+PIuqBLhY3IudBK8DnA0UBpl/+BeZug75lHsgeA9OjL8ivC4SIuY5A/FbZ11xMd2BDppWUwE6Hy6jrZ8A10CQQD0eX9HU5Q4lDpvvvr3duUhiA/EoZ8Ep38BcKMe+QYUEEelV4jTzpHgZ68EpQM+qeCT66F7brK0X86suJugA2STAkEA4f7DgG1+GMwlRnUa9URCsvcsLzbP56Jt0Wb1dTeWh7lU2XDlWRzm9c6J53T5IyaD4BfHpJxU0Yhe3OAH35k1TwJAMDX1f59fz3iLvZWv8DUmImKumVw1+7j8NtB7mpQJOtOrDVQhy4MlCVfpD8VqymS9wO3qvmiqHR/3peAR8JA6uQJBANyxDDubMRuIKBKBA11mVngsNgK5VRgPn4xxLdxU93P9ASYCQIXgWo7KLhNQQIcZ8ohg0H4oiA/CS1kIkcfB9rMCQAqA2/Msn3fXU/ODqVMS8PaRfVU9n3hnZtRoo+ttSxNzGaJCxvZs015DRRzqjeV5IhsJKkeXZufFjMhF780qwWU=",
  "expireDateTime": "2023-06-18T17:51:50.525267+00:00",
  "expireDateTimeEpoch": 1687110710
}
```

Note: The `expireDateTime` value is in UTC time. 


### Secret Retrieval

To retrieve a secret, make the following call:

```
GET /api/secrets/{id}?key={key}
```

Following the example above, the call would look like this:

```
GET /api/secrets/dde83fa5-334a-464a-8132-f376f73a648b?key=MIICXAIBAAKBgQDX0hQdKn1RynetmIU+bEYsbtYVfqTacKaONTh4V739n7wJgeWsmgveL1f/pYrPaabNigN4DjDzPVwH5DuF1umJCXo/KraBbbMPLP+cmO4awc8wHNTBZdi6JjT1lufj8Dz4MQUuGmIt89PcYUFChrnfEONof1it5JkvVybPPbl4XQIDAQABAoGAP+O/8pZmfPUMEsbpAv64k2TIWZqhIM1icQzOR8npw5Aq8UGUBVGhG8g2K8cM3bPwHj6yIjNJSGisuuVUvZ1OiqiN+PIuqBLhY3IudBK8DnA0UBpl/+BeZug75lHsgeA9OjL8ivC4SIuY5A/FbZ11xMd2BDppWUwE6Hy6jrZ8A10CQQD0eX9HU5Q4lDpvvvr3duUhiA/EoZ8Ep38BcKMe+QYUEEelV4jTzpHgZ68EpQM+qeCT66F7brK0X86suJugA2STAkEA4f7DgG1+GMwlRnUa9URCsvcsLzbP56Jt0Wb1dTeWh7lU2XDlWRzm9c6J53T5IyaD4BfHpJxU0Yhe3OAH35k1TwJAMDX1f59fz3iLvZWv8DUmImKumVw1+7j8NtB7mpQJOtOrDVQhy4MlCVfpD8VqymS9wO3qvmiqHR/3peAR8JA6uQJBANyxDDubMRuIKBKBA11mVngsNgK5VRgPn4xxLdxU93P9ASYCQIXgWo7KLhNQQIcZ8ohg0H4oiA/CS1kIkcfB9rMCQAqA2/Msn3fXU/ODqVMS8PaRfVU9n3hnZtRoo+ttSxNzGaJCxvZs015DRRzqjeV5IhsJKkeXZufFjMhF780qwWU=
```

If the secret retrieval is successful, the response body should be:

```
200 OK
{
    "message": "Hello world!"
}
```

If the secret retrieval is *not* successful, the response body will be:

```
404 Not Found
{
    "message": "The ID provided could either not be found or has expired."
}
```

A secret retrieval may fail if either of the following scenarios occur:
- The secret with the ID provided does not exist
- The secret has already been retrieved
- The secret has expired
- The number of allowed attempts to access the secret has been hit
- The key provided is incorrect

