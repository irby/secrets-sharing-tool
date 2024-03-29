[![Backend Tests](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-api.yml/badge.svg)](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-api.yml) [![Frontend Tests](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-spa.yml/badge.svg)](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-spa.yml) [![End-to-end Tests](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-e2e.yml/badge.svg)](https://github.com/irby/secrets-sharing-tool/actions/workflows/pr-e2e.yml)

# Kronocrypt

Kronocrypt allows users to easily and securely share secret information across the internet. Secret information is stored encrypted and can only be accessed using the link generated when the secret is created – not even someone with database access can read the secret information. Users can easily configure how long they want the secret to last, and once the secret has been accessed or passes its expiry datetime the secret contents are erased forever. This guarantees complete privacy between you and whomever you want to share your secrets with.

Kronocrypt is entirely open-source and free to extend.

Additional documentation:
- [API](./src/api/README.md)
- [Website](./src/spa/README.md)
