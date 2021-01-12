# Shopify-challenge
This is a simple Image Repository API built on dotnet core 3.1
It currently runs on localhost
It has two user profiles i.e Admin and Customer
Only Customers are allowed to Create images

Create A Customer : /api/users/customer
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "password": "string",
  "confirm_Password": "string"
}

Authorize using : /api/users/auth
Email
Password


Add an Image: /api/users/add-image
{
  "name": "string",
  "file": "string"
}

NB: your file string must be in base64
Thank you !
