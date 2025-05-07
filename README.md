# Ward Management System

A web-based application developed as part of a group project at Nelson Mandela University. The system is designed to manage the operations of a hospital ward, including the assignment of doctors and nurses, patient tracking, and bed allocation.

## Features

- Role-based login for doctors, nurses, and administrators
- Dashboard with real-time ward information
- Patient admission and discharge management
- Bed and medication assignment
- Admin panel for managing users and resources

## Tech Stack

- **Frontend:** HTML, CSS, JavaScript (Template-based design)
- **Backend:** C#, ASP.NET MVC
- **Database:** Microsoft SQL Server
- **Tools:** Visual Studio, Git

## My Contribution

As the front-end contributor, I was responsible for:
- Designing and customizing the homepage using an HTML/CSS template
- Ensuring responsive layout and visual consistency
- Collaborating with the team to integrate front-end elements with backend logic
  
## Setup Instructions

1. Clone the repository:
   git clone https://github.com/SoftwareDevFiqi/ward-management-system

2. Open the solution in Visual Studio.

3. Update the database connection string:
   - Open appsettings.json
   - Replace the existing connection string with your local SQL Server connection:
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=WardManagementDB;Trusted_Connection=True;"
     }

4. Apply Entity Framework Migrations:
   - Go to Tools > NuGet Package Manager > Package Manager Console
   - Run the following command:
     Update-Database

5. Run the project:
   - Press F5 or click the Start button in Visual Studio

## License

This project is for educational purposes only.

---

Developed by Shafeeq Agnew and team – Nelson Mandela University (2024)
