# Expense Tracker

A full-stack web application for tracking personal expenses, organizing spending by category, and analyzing spending habits. Built with Angular, ASP.NET Core Web API, and PostgreSQL.

**Deployed App**: Expense Tracker

## Features

- **Expense Management:** Create, view, update, and delete expenses.
- **Category Organization:** Organize expenses into categories such as Food, Transportation, Entertainment, and Utilities.
- **Expense Filtering:** Filter expenses by supported criteria, such as category and date range.
- **Spending Summaries:** Calculate total spending and analyze expenses by category or month.
- **User Authentication:** Register and log in securely.
- **Data Persistence:** Store expenses and related information in a PostgreSQL database.
- **Responsive Interface:** Access the application from desktop and mobile browsers.

## Tech Stack

| Component | Technology |
|---|---|
| Frontend | Angular, TypeScript |
| Backend | C#, ASP.NET Core Web API |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| API Architecture | REST |
| Authentication | JWT |
| Testing | xUnit |
| API Development and Testing | Postman |
| Version Control | Git, GitHub |
| Frontend Deployment | Vercel |
| Backend Deployment | Render |
| PostgreSQL Database Host | Neon |

## Architecture

The application uses a three-layer architecture to separate the user interface, application logic, and data storage.

```text
User
  |
  v
Angular Frontend
  |
  | HTTP / REST API
  v
ASP.NET Core Web API
  |
  | Entity Framework Core
  v
PostgreSQL Database
```

1. **Frontend:** Angular components provide the user interface and communicate with the backend through HTTP requests.
2. **Backend:** ASP.NET Core handles API requests, validates input, enforces authorization, and executes application logic.
3. **Data Access:** Entity Framework Core maps application models to database records and translates database operations into SQL.
4. **Database:** PostgreSQL stores users, expenses, categories, and their relationships.

## Database Design

The database uses relational tables to organize application data.

### Users

Stores registered user information.

- `Id` — Primary key
- `Email` — User email address
- `PasswordHash` — Hashed password
- `CreatedAt` — Account creation timestamp

### Categories

Stores expense categories.

- `Id` — Primary key
- `Name` — Category name

### Expenses

Stores individual expense records.

- `Id` — Primary key
- `UserId` — Foreign key referencing the user
- `CategoryId` — Foreign key referencing the category
- `Amount` — Expense amount
- `Description` — Expense description
- `ExpenseDate` — Date the expense occurred
- `CreatedAt` — Record creation timestamp

The relationships between these tables allow expenses to be associated with users and categories.

## SQL Concepts Demonstrated

The project uses a relational database to support expense management and reporting.

- `SELECT` and `WHERE` for retrieving and filtering records
- `JOIN` for combining related records across tables
- `GROUP BY` and aggregate functions such as `SUM` for spending summaries
- `ORDER BY` for sorting expenses
- Primary keys and foreign keys for relational integrity
- Indexes for frequently queried columns, if implemented

## API Endpoints

The API exposes REST endpoints for expense management and supporting functionality.

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/auth/register` | Register a user |
| `POST` | `/api/auth/login` | Authenticate a user |
| `GET` | `/api/expenses` | Retrieve expenses |
| `GET` | `/api/expenses/{id}` | Retrieve an expense |
| `POST` | `/api/expenses` | Create an expense |
| `PUT` | `/api/expenses/{id}` | Update an expense |
| `DELETE` | `/api/expenses/{id}` | Delete an expense |
| `GET` | `/api/categories` | Retrieve categories |
| `GET` | `/api/expenses/summary` | Retrieve spending summaries |
