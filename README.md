# Remote Browser System

1. **Dispatcher** - API service that manages browsing requests and stores results. Client creates a browse request, recieves a task id, polls for that task.
2. **Node** - Service that handles browsing tasks from the Dispatcher and manages Payload containers
3. **Payload** - Node.js service using Puppeteer to perform the actual web browsing

### Dispatcher

- Receives URLs from users and dispatches them to Nodes using RabbitMQ
- Stores browsing results in PostgreSQL database

### Node

- Receives browsing tasks from the Dispatcher
- Dynamically manages Docker containers running the Payload service
- Sends browsing results back to the Dispatcher

### Payload

- Node.js service using Puppeteer for browser automation
- Runs in isolated Docker containers
- Processes URLs and extracts HTML content

### Database: PostgreSQL

- Relational database, benefical for this structured data app

### Message Bus

- RabbitMQ, app can be improved by ensuring idempotency with a idempotency header key (inbox pattern), so the payload is not stressed by duplicate requests

### Logging

### Testing

- Unit tests using xUnit (ProcessBrowsingRequest_ReturnsHtmlContent_WhenUrlIsValid)

## Running the Project

- Build all services using docker-compose, the build dependencies are configured, db migrations are applied automatically, then test via Dispatcher controller using SwaggerUI or simple curl
