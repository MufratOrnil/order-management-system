# Order Management System

A modern, scalable order management system built with ASP.NET Core, designed for small to mid-sized businesses, wholesalers, and e-commerce platforms. This project streamlines order processing with role-based access, image uploads, and a sleek user interface.

## Features

- **Role-Based Access**:
  - **Admin**: Full CRUD operations (Create, Read, Update, Delete) for orders.
  - **Staff**: View-only access to orders and summaries.
- **Order Management**:
  - Create, edit, delete, and view orders with details like customer name, order date, and payment status.
  - Support for order items with product names, quantities, prices, and image uploads.
- **Modern UI**:
  - Built with Bootstrap 5 and Font Awesome for a responsive, visually appealing interface.
  - Dark mode support for enhanced user experience.
  - Interactive dashboard with order statistics and a Chart.js bar chart for order status distribution.
- **Order Summary ViewComponent**:
  - Displays total amount, item count, average price, and payment status in a reusable component.
- **Image Uploads**:
  - Upload product images for order items, stored in the `wwwroot/Uploads` folder.
- **DataTables Integration**:
  - Responsive tables for recent orders and order lists with sorting and pagination.

## Prerequisites

To run this project, ensure you have the following installed:

- .NET 8 SDK
- SQL Server Express or LocalDB
- Visual Studio 2022 (Community, Professional, or Enterprise) or Visual Studio Code
- Git for cloning the repository
- A modern web browser (e.g., Chrome, Firefox, Edge)

## Installation

Follow these steps to set up and run the project locally:

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/MufratOrnil/order-management-system.git
   cd order-management-system
   ```

2. **Restore Dependencies**: Navigate to the project root and restore NuGet packages:

   ```bash
   dotnet restore
   ```

3. **Set Up the Database**:

   - Update the connection string in `OrderManagement.Web/appsettings.json` to match your SQL Server setup:

     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderDb;Trusted_Connection=True;MultipleActiveResultSets=true"
       }
     }
     ```
   - Apply Entity Framework Core migrations to create the database:
     - **Using Visual Studio Package Manager Console**:
       - Open Tools &gt; NuGet Package Manager &gt; Package Manager Console.
       - Set "Default project" to `OrderManagement.Infrastructure`.
       - Run:

         ```powershell
         Add-Migration InitialCreate -Context OrderManagement.Infrastructure.Data.AppDbContext -StartupProject OrderManagement.Web
         Update-Database -Context OrderManagement.Infrastructure.Data.AppDbContext -StartupProject OrderManagement.Web
         ```

4. **Install Frontend Dependencies**:

   - The project uses CDN-hosted libraries (Bootstrap 5, Font Awesome, Chart.js, DataTables). No additional setup is required.
   - Ensure `OrderManagement.Web/wwwroot/lib` contains Bootstrap and other static files.

5. **Run the Application**:

   - **Using Visual Studio**:
     1. Open `OrderManagement.Web.sln` in Visual Studio 2022.
     2. Press F5 or select Debug &gt; Start Debugging.
   - **Using CLI**:

     ```bash
     cd OrderManagement.Web
     dotnet build
     dotnet run
     ```
   - Access the application at `https://localhost:5001` or `http://localhost:5000`.

## Usage

1. **Login**:

   - Use the seeded accounts to log in at `/Identity/Account/Login`:
     - **Admin**: `admin@example.com` / `Admin@123` (Full CRUD access)
     - **Staff**: `staff@example.com` / `Staff@123` (View-only access)

2. **Dashboard**:

   - **Home Page** (`/`): View order statistics (Total, Paid, Pending), a Chart.js bar chart for order status, and a Recent Orders table with actions (Details, Edit, Delete for Admins).
   - **Orders Page** (`/Orders`): Manage all orders with filtering by customer name or payment status.

3. **Order Management**:

   - **Create**: Admins can create orders at `/Orders/Create`, including items with image uploads.
   - **Edit/Delete**: Admins can edit (`/Orders/Edit/{id}`) or delete orders via modals in the dashboard or Orders page.
   - **Details**: View order details, including items and summaries, at `/Orders/Details/{id}`.
   - The `OrderSummaryViewComponent` shows key metrics (total, item count, average price) in Details, Edit, and Orders pages.

4. **Dark Mode**:

   - Toggle dark mode via the navbar button for a modern UI experience.

## Project Structure

- **OrderManagement.Web**: ASP.NET Core MVC project with controllers, views, and static files.
- **OrderManagement.Core**: Entities and interfaces for business logic.
- **OrderManagement.Infrastructure**: Entity Framework Core data access and repository implementations.
- **Key Files**:
  - `Controllers/OrdersController.cs`: Handles CRUD operations for orders.
  - `Views/Home/Index.cshtml`: Dashboard with stats, chart, and Recent Orders table.
  - `Components/OrderSummaryViewComponent.cs`: Reusable component for order summaries.
  - `wwwroot/css/site.css`: Custom styles for Bootstrap 5 and dark mode.

## Configuration

- **Database**: Uses SQL Server (LocalDB by default). Update `appsettings.json` for other SQL Server instances.
- **Authentication**: ASP.NET Identity with role-based authorization (`AdminOnly`, `StaffOrAdmin` policies).
- **Image Uploads**: Stores images in `wwwroot/Uploads`. Ensure write permissions for the folder.

## Troubleshooting

- **Database Errors**: Verify the connection string and run migrations (`dotnet ef database update`).
- **UI Issues**: Ensure CDN links for Bootstrap, Font Awesome, Chart.js, and DataTables are accessible.
- **Login Issues**: Check `AspNetUsers` and `AspNetUserRoles` tables for default users and roles.
- **Image Uploads**: Confirm `wwwroot/Uploads` exists and has write permissions.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit changes (`git commit -m "Add YourFeature"`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
