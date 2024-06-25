
# ERPMeioAmbienteAPI

ERPMeioAmbienteAPI is a RESTful API designed to manage clients, collections, employees, and products for an environmental management system. This API includes authentication and authorization using JWT tokens and role-based access control.

## Table of Contents
- [Getting Started](#getting-started)
- [Database Configuration](#database-configuration)
- [Authentication and Authorization](#authentication-and-authorization)
- [API Endpoints](#api-endpoints)
  - [Auth](#auth)
  - [Cliente](#cliente)
  - [Coleta](#coleta)
  - [Funcionario](#funcionario)
  - [Produto](#produto)
- [Swagger Documentation](#swagger-documentation)
- [Logging](#logging)
- [Contributing](#contributing)

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- MySQL Server

### Installation
1. Clone the repository
   ```sh
   git clone https://github.com/yourusername/ERPMeioAmbienteAPI.git
   ```
2. Navigate to the project directory
   ```sh
   cd ERPMeioAmbienteAPI
   ```
3. Install the required packages
   ```sh
   dotnet restore
   ```

### Configuration
Update the `appsettings.json` file with your database connection string and JWT settings:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MeioAmbienteConnection": "server=localhost;database=ERPMeioAmbiente;user=root;password=root"
  },
  "AuthSettings": {
    "Key": "This is the key that will be use in the encryption",
    "Audience": "",
    "Issuer": ""
  }
}
```

### Running the Application
1. Update the database
   ```sh
   dotnet ef database update
   ```
2. Run the application
   ```sh
   dotnet run
   ```

## Database Configuration
The database is configured using Entity Framework Core with MySQL. Ensure your `appsettings.json` file has the correct connection string.

## Authentication and Authorization
The API uses JWT tokens for authentication and role-based access control. The roles available are:
- Admin
- Cliente
- Funcionario

## API Endpoints

### Auth
- **Register**
  - `POST /api/Auth/Register`
  - Registers a new user.
- **Login**
  - `POST /api/Auth/Login`
  - Logs in a user and returns a JWT token.

### Cliente
- **Adiciona Cliente**
  - `POST /api/Cliente`
  - Adds a new client. (Admin or Funcionario only)
- **Recupera Clientes**
  - `GET /api/Cliente`
  - Retrieves all clients. (Admin or Funcionario only)
- **Recupera Cliente Atual**
  - `GET /api/Cliente/me`
  - Retrieves the currently authenticated client's details. (Cliente only)
- **Atualiza Cliente Atual**
  - `PUT /api/Cliente/me`
  - Updates the currently authenticated client's details. (Cliente only)
- **Recupera Cliente por ID**
  - `GET /api/Cliente/{id}`
  - Retrieves a client by ID. (Admin or Funcionario only)
- **Atualiza Cliente por ID**
  - `PUT /api/Cliente/{id}`
  - Updates a client by ID. (Admin or Funcionario only)
- **Deleta Cliente por ID**
  - `DELETE /api/Cliente/{id}`
  - Deletes a client by ID. (Admin only)
- **Solicita Coleta**
  - `POST /api/Cliente/me/solicita-coleta`
  - Requests a new collection. (Cliente only)
- **Recupera Coletas do Cliente**
  - `GET /api/Cliente/me/coletas`
  - Retrieves all collections of the authenticated client. (Cliente only)
- **Recupera Coleta do Cliente por ID**
  - `GET /api/Cliente/me/coletas/{id}`
  - Retrieves a collection of the authenticated client by ID. (Cliente only)
- **Atualiza Coleta do Cliente**
  - `PUT /api/Cliente/me/coletas/{id}`
  - Updates a collection of the authenticated client. (Cliente only)
- **Deleta Coleta do Cliente**
  - `DELETE /api/Cliente/me/coletas/{id}`
  - Deletes a collection of the authenticated client. (Cliente only)

### Coleta
- **Adiciona Coleta**
  - `POST /api/Coleta`
  - Adds a new collection. (Admin or Funcionario only)
- **Recupera Coletas**
  - `GET /api/Coleta`
  - Retrieves all collections. (Admin or Funcionario only)
- **Recupera Coleta por ID**
  - `GET /api/Coleta/{id}`
  - Retrieves a collection by ID. (Admin or Funcionario only)
- **Atualiza Coleta por ID**
  - `PUT /api/Coleta/{id}`
  - Updates a collection by ID. (Admin or Funcionario only)
- **Deleta Coleta por ID**
  - `DELETE /api/Coleta/{id}`
  - Deletes a collection by ID. (Admin only)

### Funcionario
- **Adiciona Funcionario**
  - `POST /api/Funcionario`
  - Adds a new employee. (Admin only)
- **Recupera Funcionarios**
  - `GET /api/Funcionario`
  - Retrieves all employees. (Admin only)
- **Recupera Funcionario por ID**
  - `GET /api/Funcionario/{id}`
  - Retrieves an employee by ID. (Admin only)
- **Atualiza Funcionario por ID**
  - `PUT /api/Funcionario/{id}`
  - Updates an employee by ID. (Admin only)
- **Deleta Funcionario por ID**
  - `DELETE /api/Funcionario/{id}`
  - Deletes an employee by ID. (Admin only)

### Produto
- **Adiciona Produto**
  - `POST /api/Produto`
  - Adds a new product.
- **Recupera Produtos**
  - `GET /api/Produto`
  - Retrieves all products.
- **Recupera Produto por ID**
  - `GET /api/Produto/{id}`
  - Retrieves a product by ID.
- **Atualiza Produto por ID**
  - `PUT /api/Produto/{id}`
  - Updates a product by ID.
- **Deleta Produto por ID**
  - `DELETE /api/Produto/{id}`
  - Deletes a product by ID.

## Swagger Documentation
The API documentation is available via Swagger. Navigate to `https://localhost:7186/swagger` to access the Swagger UI.

## Logging
The application uses the default logging provided by ASP.NET Core. Logs will show important information about role creation, user registration, and errors.

## Contributing
Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
