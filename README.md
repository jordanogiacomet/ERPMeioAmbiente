# ERPMeioAmbienteAPI

ERPMeioAmbienteAPI é uma API RESTful projetada para gerenciar clientes, coletas, funcionários e produtos para um sistema de gerenciamento ambiental. Esta API inclui autenticação e autorização usando tokens JWT e controle de acesso baseado em funções.

## Índice
- [Primeiros Passos](#primeiros-passos)
- [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
- [Autenticação e Autorização](#autenticação-e-autorização)
- [Endpoints da API](#endpoints-da-api)
  - [Auth](#auth)
  - [Cliente](#cliente)
  - [Coleta](#coleta)
  - [Funcionário](#funcionário)
  - [Produto](#produto)
- [Documentação Swagger](#documentação-swagger)
- [Registro de Logs](#registro-de-logs)
- [Contribuindo](#contribuindo)

## Primeiros Passos

### Pré-requisitos
- .NET 8.0 SDK
- Servidor MySQL

### Instalação
1. Clone o repositório
   ```sh
   git clone https://github.com/seuusuario/ERPMeioAmbienteAPI.git
   ```
2. Navegue até o diretório do projeto
   ```sh
   cd ERPMeioAmbienteAPI
   ```
3. Instale os pacotes necessários
   ```sh
   dotnet restore
   ```

### Configuração
Atualize o arquivo `appsettings.json` com sua string de conexão do banco de dados e configurações JWT:
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

### Executando a Aplicação
1. Atualize o banco de dados
   ```sh
   dotnet ef database update
   ```
2. Execute a aplicação
   ```sh
   dotnet run
   ```

## Autenticação e Autorização
A API usa tokens JWT para autenticação e controle de acesso baseado em funções. As funções disponíveis são:
- Admin
- Cliente
- Funcionario

## Endpoints da API

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

## Documentação Swagger
A documentação da API está disponível via Swagger. Navegue até `https://localhost:7186/swagger` para acessar a UI do Swagger.

## Registro de Logs
A aplicação usa o registro de logs padrão fornecido pelo ASP.NET Core. Os logs mostrarão informações importantes sobre a criação de funções, registro de usuários e erros.

## Contribuindo
Contribuições são bem-vindas! Por favor, abra um issue ou envie um pull request no GitHub.

## Licença
Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.
