# .NET PetClinic

A complete rewrite of the [Spring PetClinic](https://github.com/spring-projects/spring-petclinic) Java application to **.NET 10** using **GitHub Copilot CLI**.

## About This Project

This project demonstrates how GitHub Copilot can assist in migrating a full-stack Java/Spring Boot application to .NET/ASP.NET Core while maintaining the same user experience, database schema, and functionality.


The initial prompt used to start the migration process was :

```markdown
/plan I want to rewrite the application from JavaSpringBoot located in @spring-petclinic-main/ to a new DOTNET 10 application that you will put in @dotnet-petclinic/ 
- use 5 agents to analyze the codebase (db, back, front, test, dependencies, ...)
- use Razor for the UI, but keep it simple since I am not a dotnet expert
- ask me questions to validate the architecture (that must be simple)
- make sure you implement all the screens and features, and consider the things done when you have tested all the features e2e in the old app (Java) and the new one (dotnet): navigation, pagination, search, insert, ...
- do not copy the same CSS since the Java one uses Spring colors and names, use Boostrap default for example
```


### Original Application
- **Repository**: [spring-projects/spring-petclinic](https://github.com/spring-projects/spring-petclinic)
- **Stack**: Spring Boot 4.0.1, Spring MVC, Thymeleaf, Spring Data JPA

### Migrated Application
- **Framework**: ASP.NET Core 10.0 MVC
- **ORM**: Entity Framework Core 10.0
- **Template Engine**: Razor Views
- **CSS Framework**: Bootstrap 5.3.x + Font Awesome 4.7.0
- **Database**: SQLite (default), with support for MySQL and PostgreSQL

## Migration Documentation

The entire migration was performed using GitHub Copilot CLI. You can review the complete process:

| Document | Description |
|----------|-------------|
| [Session Log](copilot-session/session-java-dotnet.md) | Complete Copilot CLI session transcript showing the step-by-step migration process |
| [Migration Plan](copilot-session/plan.md) | Detailed migration plan created by Copilot, including entity mappings, controller mappings, and view mappings between Java/Spring and .NET |

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Running the Application

```bash
cd dotnet-petclinic/PetClinic.Web
dotnet run
```

The application will be available at `http://localhost:5000` (or the port specified in `launchSettings.json`).

### Running Tests

```bash
cd dotnet-petclinic/PetClinic.Tests
dotnet test
```

## Project Structure

```
dotnet-petclinic/
├── PetClinic.Web/          # Main ASP.NET Core MVC application
│   ├── Controllers/        # MVC Controllers
│   ├── Models/             # Entity models and ViewModels
│   ├── Data/               # DbContext and data initialization
│   ├── Views/              # Razor views
│   └── wwwroot/            # Static files (CSS, JS, images)
├── PetClinic.Tests/        # Test project with Playwright tests
└── PetClinic.slnx          # Solution file
```

## License

This project is licensed under the Apache License 2.0 - see the original [Spring PetClinic license](https://github.com/spring-projects/spring-petclinic/blob/main/LICENSE.txt) for details.
