# DoctorateDrive

DoctorateDrive is a web application developed using ASP.NET Core MVC to simplify the management of doctoral research activities within a university. The system provides separate portals for students, guides, and administrators, allowing them to manage research progress, documents, approvals, and other academic activities from a single platform.

## Project Overview

Managing doctoral research through paper-based records or spreadsheets is time-consuming and difficult to track. DoctorateDrive provides a centralized system where users can securely access information according to their roles, reducing manual work and improving transparency in the research process.

## Technologies Used

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server
* HTML
* CSS
* Bootstrap
* JavaScript

## Main Modules

### Student

* Login
* Dashboard
* Update profile
* Submit research progress
* Upload required documents
* View guide feedback
* Track approval status

### Guide

* Login
* Dashboard
* View assigned students
* Review submissions
* Approve or request changes
* Provide feedback

### Administrator / HOD

* Manage students
* Manage guides
* Assign guides
* Monitor research progress
* Approve final submissions
* Generate reports

## Project Structure

```
DoctorateDrive
│
├── Controllers
├── Models
├── Views
├── Data
├── Repository
├── Services
├── ViewModels
├── wwwroot
├── Program.cs
├── appsettings.json
└── README.md
```

## How to Run the Project

1. Clone the repository.

```
git clone https://github.com/your-username/DoctorateDrive.git
```

2. Open the solution in Visual Studio 2022.

3. Update the SQL Server connection string in `appsettings.json`.

4. Apply the database migrations.

```
Update-Database
```

or

```
dotnet ef database update
```

5. Run the application.

## Database

The project uses SQL Server with Entity Framework Core (Code First).

If required, create a migration using:

```
dotnet ef migrations add InitialCreate
```

Then update the database:

```
dotnet ef database update
```



Government Engineering College, Modasa
