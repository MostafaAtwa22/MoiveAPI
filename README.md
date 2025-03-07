# 🎬 Movies.API

Movies.API is a robust movie site built with ASP.NET Core, following the **Onion Architecture** to ensure maintainability and scalability. It supports **JWT authentication**, **role management**, **Google authentication**, **image uploads to the database**, and **movie genre classification**.  

## 📌 Features

- 🔹 **Movie & Genre Management**: CRUD operations for movies and genres.  
- 🔹 **Image Upload**: Stores images directly in the database.  
- 🔹 **Onion Architecture**: Decoupled layers with clear responsibilities.  
- 🔹 **Entity Framework Core**: Uses **migrations** for database schema management.  
- 🔹 **Custom Attributes & Validation**: Ensures file uploads meet specific criteria.  
- 🔹 **Authentication & Authorization**:  
  - JWT-based authentication  
  - Role-based access control  
  - Google authentication  
  - Forgot password functionality  
- 🔹 **Automapper**: Simplifies object mapping.  
- 🔹 **DTOs (Data Transfer Objects)**: Ensures clean and efficient data exchange.  
- 🔹 **Repository Pattern & Unit of Work**: Implements best practices for data access.  

## 🏛 Project Architecture

This project follows **Onion Architecture**, which enforces separation of concerns:

📂 **Movies.API** (Presentation Layer)  
- Controllers: `MoviesController`, `GenresController`, `AccountsController`, `RolesController`  

📂 **Movies.Core** (Domain Layer)  
- Models: `Movie`, `Genre`, `ApplicationUser`, `BaseEntity`  
- Interfaces: `IBaseRepository`, `IUnitOfWork`  

📂 **Movies.Repository** (Data Access Layer)  
- Database Context & Migrations  
- Repository Implementations  

📂 **Movies.Services** (Business Logic Layer)  
- DTOs for Movies, Genres, Accounts, and Roles  
- Helper classes (`MappingProfile.cs` for AutoMapper)  
- Custom attributes for file validation (`AllowedExtensionsAttribute.cs`, `MaxFileSizeAttribute.cs`)  

## ⚙️ Technologies Used

- **ASP.NET Core 7+**
- **Entity Framework Core**
- **JWT Authentication**
- **Google Authentication**
- **AutoMapper**
- **Repository Pattern & Unit of Work**
- **Fluent Validation & Custom Attributes**
- **SQL Server**
- **Swagger for API Documentation**

