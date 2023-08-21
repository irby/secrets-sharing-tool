# Create Secrets -- Test Invalid Input

POST http://localhost:5000/api/secrets
{
  "message": "",
  "expireMinutes": 10
}

HTTP 400

POST http://localhost:5000/api/secrets
{
  "message": "Hello, World!",
  "expireMinutes": 0
}

HTTP 400

POST http://localhost:5000/api/secrets
{
  "message": "Hello, World!",
  "expireMinutes": 10080
}

HTTP 400




# Create and Retrieve Secrets

POST http://localhost:5000/api/secrets
{
  "message": "Hello, World!",
  "expireMinutes": 10
}

HTTP 201
[Captures]
secret_id: jsonpath "$['secretId']"
key: jsonpath "$['key']"


GET http://localhost:5000/api/secrets/{{secret_id}}?key={{key}}

HTTP 200
[Asserts]
jsonpath "$['message']" == "Hello, World!"


# Getting the same secret again should fail
GET http://localhost:5000/api/secrets/{{secret_id}}?key={{key}}

HTTP 404

