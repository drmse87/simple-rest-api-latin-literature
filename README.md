# Simple Web Api Latin Literature

Simple [ASP.NET Core WebAPI](https://asp.net) for Latin Literature.

# Resources and endpoints

## Users

| Method | Endpoint                  | Description                      |
| ------ | ------------------------- | -------------------------------- |
| GET    | /api/v1/users             | Get all users.                   |
| GET    | /api/v1/users/:id         | Get user by id.                  |
| GET    | /api/v1/users/:id/works   | Get works added by user by id.   |
| GET    | /api/v1/users/:id/authors | Get authors added by user by id. |
| POST   | /api/v1/users             | Register new user.               |
| POST   | /api/v1/users/login       | Login.                           |

## Latin Works

| Method | Endpoint          | Description        |
| ------ | ----------------- | ------------------ |
| GET    | /api/v1/works     | Get all works.     |
| GET    | /api/v1/works/:id | Get work by id.    |
| POST   | /api/v1/works     | Add new work.      |
| DELETE | /api/v1/works/:id | Delete work by id. |
| PUT    | /api/v1/works/:id | Update work by id. |

## Latin Authors

| Method | Endpoint                  | Description                |
| ------ | ------------------------- | -------------------------- |
| GET    | /api/v1/authors           | Get all authors.           |
| GET    | /api/v1/authors/:id       | Get author by id.          |
| GET    | /api/v1/authors/:id/works | Get works by author by id. |
| POST   | /api/v1/authors           | Add new author.            |
| DELETE | /api/v1/authors/:id       | Delete author by id.       |
| PUT    | /api/v1/authors/:id       | Update author by id.       |

## Webhooks

| Method | Endpoint         | Description           |
| ------ | ---------------- | --------------------- |
| POST   | /api/v1/webhooks | Register new webhook. |

# Technical details

## HATEOAS

I have implemented HATEOAS (Hypermedia as the Engine of Application State) by adding a \_links property to all documents, with a self property, containing a href and a rel property. This way, users or services consuming the API can simply navigate to the next or related resource. There is one exception to this rule: webhooks do not have any \_links and self properties because I chose not to implement GET for this particular resource.

## Authentication with JWT

The main advantage of JWT is its ease of use and its portability, but also the fact that it is an Internet standard (RFC 7519). Compared to cookies, JWT is stateless in the sense that the server does not have to keep track of the sessions, everything is stored inside the token (much like a personal identification document), which cannot be tampered with because it is cryptographically signed. The disadvantage with JWT is that it has to be stored somewhere (local or session storage, where it can be accessed by JS), and also that it stays valid until expiry (which would be a bad thing if someone hijacked it). Other authentication options could have been, for example, Basic Auth, OAuth2, or an API key.

## Webhooks

A new webhook can be created by POSTing to /api/v1/webhooks with the body parameters "event" (should be one of the event types 'NewAuthor', 'EditedAuthor', 'NewWork' or 'EditedWork') and "callbackURL" (a valid URL listening to POST requests). Whenever the event occurs, a POST request is sent to all event subscribers' callback URLs through the WebhookService implementing the IWebhookService interface (dependency injected into all controllers).

## Representation

The resources are currently represented in JSON format because the requests are sent with the Accept: application/json header. To allow other representations, the client should change the Accept header into Accept: application/xml (for XML), and then the API (depending on which framework is used and if this is done automatically or not) will represent (i.e. serialize) the resources accordingly.

## Install

The project requires a Secrets.json file in project root with the key "SecurityKey" as a JWT signing key.

## Testing

Testing can be performed by importing the Latin Works.postman_collection.json and running the collection runner.

# Lessons learned

In retrospect, I would have wanted to add more data to the database. I would also have wanted to add a /user endpoint to get the current user's document. I would also have wanted to add a /user/webhooks store, to get a list of the current user's webhooks. I would also have used ASP.NET's Webhooks instead of writing my own webhook functionality. Finally, for the mapping done in the static ToModel and FromModel methods, I would have wanted to use AutoMapper instead.
