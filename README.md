# Description
Because of my Latin interest, I decided to create a Latin-inspired API containing classical Latin works and authors such as De Bello Gallico by Caesar and Cicero's Epistulae. The API is up and running (at least as of April 2020) at https://latin.sommarvind.tech.

# Resources and endpoints
## Users
| Method | Endpoint                  | Description                      |
|--------|---------------------------|----------------------------------|
| GET    | /api/v1/users             | Get all users.                   |
| GET    | /api/v1/users/:id         | Get user by id.                  |
| GET    | /api/v1/users/:id/works   | Get works added by user by id.   |
| GET    | /api/v1/users/:id/authors | Get authors added by user by id. |
| POST   | /api/v1/users             | Register new user.               |
| POST   | /api/v1/users/login       | Login.                           |

## Latin Works
| Method | Endpoint          | Description        |
|--------|-------------------|--------------------|
| GET    | /api/v1/works     | Get all works.     |
| GET    | /api/v1/works/:id | Get work by id.    |
| POST   | /api/v1/works     | Add new work.      |
| DELETE | /api/v1/works/:id | Delete work by id. |
| PUT    | /api/v1/works/:id | Update work by id. |

## Latin Authors
| Method | Endpoint                  | Description                |
|--------|---------------------------|----------------------------|
| GET    | /api/v1/authors           | Get all authors.           |
| GET    | /api/v1/authors/:id       | Get author by id.          |
| GET    | /api/v1/authors/:id/works | Get works by author by id. |
| POST   | /api/v1/authors           | Add new author.            |
| DELETE | /api/v1/authors/:id       | Delete author by id.       |
| PUT    | /api/v1/authors/:id       | Update author by id.       |

## Webhooks
| Method  | Endpoint          | Description          |
|---------|-------------------|----------------------|
| POST    | /api/v1/webhooks  | Register new webhook.|

# Technical details
## HATEOAS
I have implemented HATEOAS (Hypermedia as the Engine of Application State) by adding a _links property to all documents, with a self property, containing a href and a rel property. This way, users or services consuming the API can simply navigate to the next or related resource. There is one exception to this rule, however, webhooks do not have any _links and self properties because I chose not to implement GET for this particular resource.

## JWT
The main advantage of JWT is its ease of use and its portability, but also the fact that it is an Internet standard (RFC 7519). Compared to cookies, JWT is stateless in the sense that the server does not have to keep track of the sessions, everything is stored inside the token (much like a personal identification document), which cannot be tampered with because it is cryptographically signed. The disadvantage with JWT is that it has to be stored somewhere (local or session storage, where it can be accessed by JS), and also that it stays valid until expiry (which would be a bad thing if someone hijacked it).

## Webhooks
A new webhook can be created by POSTing to /api/v1/webhooks with the body parameters "event" (should be one of the event types 'NewAuthor', 'EditedAuthor', 'NewWork' or 'EditedWork') and "callbackURL" (a valid URL listening to POST requests). Whenever the event occurs, a POST request is sent to all event subscribers' callback URLs through the WebhookService implementing the IWebhookService interface (dependency injected into all controllers).

## Install
The project requires a Secrets.json file in project root with the key "SecurityKey" as a JWT signing key.

## Testing
Either manually at https://latin.sommarvind.tech, or by importing the Latin Works.postman_collection.json and running the collection runner.
