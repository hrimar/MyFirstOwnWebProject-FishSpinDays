# MyFirstOwnWebProject-FishSpinDays

FishSpinDays is my first own web project with roles - a blog for fishing stories and events.

## Features & User Roles

The application is separated into multiple Layers (Data, Services, Web) with Areas that are accessible by users with different access levels:

- **All users** could read all the available information, but they haven't permission to write anything.
- **Logged in users** could write publications in different fishing sections chosen by dropdown list (sea fishing, freshwater fishing, rods and reels, lures, handmade lures, fishing schools and etc.); could like publications, but not their own publications; could make some comments and like or dislike comments; could use our real-time communication chat channel and use the API of the application.
- **Admin users** could create new main sections and subsections in publications of different types; could write publications and edit the existed ones; could delete the comments; could see the list of all users with his publications and make Ban of user.

## Architecture & Design Patterns

- **Layered Architecture** - Multi-layer separation (Data, Services, Web)
- **Service Layer Pattern** - Business logic encapsulation in service classes
- **Dependency Injection** - Built-in ASP.NET Core DI container
- **AutoMapper** - Object-to-object mapping for ViewModels and DTOs
- **Area-based Organization** - Separate areas for Admin and Identity features

## Tech Stack
### Backend
- **.NET 8** - Latest framework version
- **ASP.NET Core** - Web framework with Razor Pages and MVC
- **Entity Framework Core** - ORM with Code First approach and direct DbContext usage
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer Authentication** - API authentication
- **SignalR** - Real-time communication (chat functionality)

### Frontend
- **Razor Pages & MVC Views** - Server-side rendering
- **Bootstrap 3.3.7** - CSS framework
- **jQuery** - JavaScript library
- **AJAX** - Asynchronous requests
- **Summernote** - Rich text editor for publications
- **Custom CSS** - Additional styling

### Security Features
- **CSRF Protection** - Anti-forgery tokens on all forms
- **Security Headers** - HSTS, CSP, X-Frame-Options, etc.
- **Input Validation** - Data annotations and model validation
- **User Secrets** - Secure configuration management
- **CORS Policy** - Environment-specific origin restrictions

### Database & Storage
- **SQL Server** - Primary database (Local & Azure)
- **Azure SQL Database** - Production database
- **Entity Framework Migrations** - Database schema management

### API & Integration
- **RESTful API** - JSON endpoints for external consumption
- **Swagger/OpenAPI** - API documentation
- **External Authentication** - Facebook, Google, GitHub OAuth
- **Weather API Integration** - Real-time weather display

### Development & Deployment
- **Azure App Service** - Cloud hosting
- **GitHub** - Source control
- **User Secrets** - Development configuration
- **Environment Variables** - Production configuration
- **Azure Active Directory** - Production authentication

### Testing & Quality
- **Structured Logging** - Comprehensive application logging
- **Performance Monitoring** - Operation timing and metrics
- **Error Handling** - Custom error pages and logging
- **Build Validation** - Compilation checks

## Project Structure

```
FishSpinDays/
├── FishSpinDays.Common/     # Shared constants, extensions, ViewModels
├── FishSpinDays.Data/       # Entity Framework context and migrations
├── FishSpinDays.Models/     # Domain entities
├── FishSpinDays.Services/   # Business logic layer
├── FishSpinDays.Web/		 # Web application (Razor Pages + MVC)
└── FishSpinDays.Tests/		 # Unit and integration tests
```

## Live Demo

Production: `https://fishspindays-prod-ne-app.azurewebsites.net`
Custom domains: `fishspindays.com`, `www.fishspindays.com` // still not configurated