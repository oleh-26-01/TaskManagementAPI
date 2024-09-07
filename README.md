# Task Management API

## Project Overview

This is a .NET 8 Web API for managing tasks. It provides endpoints for user registration, login, and task management (CRUD operations, filtering, sorting, and pagination).

## Features

- User registration and login with JWT authentication.
- Task creation, retrieval, update, and deletion.
- Filtering tasks by status, priority, and due date.
- Sorting tasks by due date or priority.
- Pagination for retrieving tasks.

## Technologies Used

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens)
- Swagger/OpenAPI

## Setup Instructions

1. **Clone the repository:**

   ```bash
   git clone https://github.com/oleh-26-01/TaskManagementAPI.git
   ```

2. **Configure the database connection string:**

   - Open the `appsettings.json` file and update the `ConnectionStrings` section with your SQL Server connection string.

3. **Run database migrations:**

   - Open the Package Manager Console in Visual Studio.
   - Navigate to the project directory.
   - Run the following command:

     ```
     Update-Database
     ```

4. **Run the application:**

   - Press F5 in Visual Studio or run the following command in the terminal:

     ```bash
     dotnet run
     ```

## Authentication

The API uses JWT (JSON Web Token) authentication. To access protected endpoints, you need to include a valid JWT token in the `Authorization` header of your requests.

**Obtaining a JWT Token:**

1. **Register a new user:** Use the `/api/users/register` endpoint to create a new user account.
2. **Log in:** Use the `/api/users/login` endpoint to log in with your username and password. The response will include a JWT token.

**Using the JWT Token:**

- Include the JWT token in the `Authorization` header of your requests, prefixed with the word "Bearer" and a space. For example:

  ```
  Authorization: Bearer your-jwt-token
  ```

**Swagger Authorization:**

- You can easily authorize your requests in Swagger UI by clicking the "Authorize" button at the top of the page.
- Enter your JWT token in the input field and click "Authorize".
- Swagger will automatically include the token in the Authorization header of subsequent requests.

## API Documentation

The API documentation is available through Swagger UI:

- **Local:** [https://localhost:7248/swagger/index.html](https://localhost:7248/swagger/index.html)
