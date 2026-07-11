# Educational Platform — ASP.NET Core MVC Graduation Project

A full-stack educational platform built on **ASP.NET Core MVC (.NET 8)**, converted from the
"Eduleb" HTML/Bootstrap template (MIT-licensed, by ThemeWagon) into a complete, layered,
database-backed application.

## 1. Solution structure (6 projects)

| Project                        | Responsibility                                                        |
|---------------------------------|-------------------------------------------------------------------------|
| `EducationalPlatform.Models`    | Entities, ViewModels, DTOs                                              |
| `EducationalPlatform.Data`      | `ApplicationDbContext`, Fluent API configurations, `DbSeeder`           |
| `EducationalPlatform.Repository`| Generic Repository + Unit of Work                                       |
| `EducationalPlatform.Services`  | Business logic (interfaces + implementations), called by both Web & API|
| `EducationalPlatform.Web`       | MVC app — Razor views, controllers, ASP.NET Core Identity                |

**Layering / dependency direction:** `Web` → `Services` → `Repository` → `Data` → `Models`.
Controllers never talk to EF Core directly — only the Service layer does, through `IUnitOfWork`.

## 2. Prerequisites

- **.NET 8 SDK** — https://dotnet.microsoft.com/download/dotnet/8.0
- **SQL Server** (LocalDB, Developer Edition, or full SQL Server) — you chose full SQL Server
- **Visual Studio 2022** (17.8+) or VS Code with the C# Dev Kit extension

> **Note on how this project was generated:** it was hand-written file-by-file in a sandboxed
> environment without a .NET SDK available, so it has **not** been compiled or run with
> `dotnet build` / `dotnet ef migrations`. Everything has been carefully cross-checked
> (namespaces, model binding names, Fluent API cascade paths, asset paths, etc.), but please
> treat the **first build** as the real verification step, and see Section 6 (Known risk areas)
> for the handful of spots most likely to need a small fix.

## 3. First-time setup

### 3.1 Configure the connection string

Edit `EducationalPlatform.Web/appsettings.json` (and `EducationalPlatform.API/appsettings.json`
if you plan to run the API too) and replace `YOUR_SERVER_NAME`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EducationalPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

If you use SQL Server login (not Windows auth), use this format instead:
```
Server=YOUR_SERVER;Database=EducationalPlatformDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

### 3.2 Restore & build

```bash
cd EducationalPlatform
dotnet restore
dotnet build
```

### 3.3 Create the database migration

Run this from the solution root (the `Data` project holds the `DbContext`, the `Web` project is
the startup project):

```bash
dotnet tool install --global dotnet-ef   # if you don't already have it
cd EducationalPlatform.Web
dotnet ef migrations add InitialCreate --project ../EducationalPlatform.Data --startup-project .
dotnet ef database update --project ../EducationalPlatform.Data --startup-project .
```

This creates the database and all tables (Identity tables + Categories, Courses, Lessons,
Enrollments, Reviews, SubscriptionPlans, UserSubscriptions, ContactMessages).

> You don't strictly need to run `database update` yourself — `Program.cs` calls
> `await DbSeeder.SeedAsync(...)` on every startup, which itself calls
> `context.Database.MigrateAsync()` first. But creating the migration (the `migrations add` step)
> must be done once before the first run, since migrations can't be generated without the SDK.

### 3.4 Run the Web app

```bash
cd EducationalPlatform.Web
dotnet run
```

Navigate to the URL shown in the console (typically `https://localhost:5001` or similar). On
first run, `DbSeeder` will populate:

- The 3 roles: `Admin`, `Instructor`, `Student`
- An **Admin** account: `admin@eduplatform.com` / `Admin@12345`
- 3 **Instructor** accounts (`manwella.emad@eduplatform.com`, `radwa.abdelmonem@eduplatform.com`,
  `shaza.ashraf@eduplatform.com`, all password `Instructor@123`)
- A demo **Student** account: `student@eduplatform.com` / `Student@123`
- 6 categories, 6 sample courses (each with 3 lessons), 3 subscription plans, and one sample
  enrollment for the demo student.

**Change these seeded passwords before any real deployment** — they exist purely so the app is
demoable immediately after `dotnet run`.

### 3.5 (Optional) Run the API

```bash
cd EducationalPlatform.API
dotnet run
```

Swagger UI is available at `/swagger` in Development mode. The API shares the same database as
the Web app (same connection string), so anything seeded by the Web app's `DbSeeder` is visible
through the API too.

## 4. Default login credentials (seeded)

| Role       | Email                            | Password         |
|------------|----------------------------------|------------------|
| Admin      | admin@eduplatform.com            | Admin@12345      |
| Instructor | manwella.emad@eduplatform.com    | Instructor@123   |
| Instructor | radwa.abdelmonem@eduplatform.com | Instructor@123   |
| Instructor | shaza.ashraf@eduplatform.com     | Instructor@123   |
| Student    | sara.yasser@eduplatform.com      | Student@123      |

## 5. Feature map (what's where)

| Original template page | Converted to |
|---|---|
| `index.html` / `index2.html` | `HomeController.Index` → dynamic featured courses |
| `about.html` | `HomeController.About` |
| `course.html` | `CourseController.Index` — real search/filter/sort/paging |
| `course_details.html` | `CourseController.Details` — tabs, lessons, reviews, enroll |
| `instructor.html` / `ins_details.html` | `InstructorController.Index` / `Details` |
| `pricing.html` | `MembershipController.Pricing` + real `Subscribe` flow |
| `faq.html` | `HomeController.Faq` |
| `contact.html` + `contact.php` | `HomeController.Contact` — saved to `ContactMessages` table |
| Sign In / Sign Up links | `AccountController` (Identity: Register/Login/Logout/Profile/ChangePassword/ForgotPassword/ResetPassword) |
| *(new, not in template)* | Admin/Instructor/Student dashboards (`DashboardController`), Category CRUD, Lesson CRUD, Enrollment management, Review submission |

## 6. Known risk areas (read before your first build)

Since this couldn't be compiled in the sandbox it was built in, these are the spots most likely
to need a one-line fix if something doesn't compile immediately:

1. **NuGet package versions** — pinned to `8.0.10` / `6.6.2` / `8.0.2`. If those exact patch
   versions aren't available on nuget.org by the time you build, bump the patch version (e.g.
   `8.0.11`) in the relevant `.csproj`.
2. **`FrameworkReference` in class libraries** — `EducationalPlatform.Models` and
   `EducationalPlatform.Services` use `<FrameworkReference Include="Microsoft.AspNetCore.App" />`
   so `IFormFile` resolves outside a Web SDK project. This is a supported, common pattern, but if
   your SDK version handles it differently you may see a warning (safe to ignore) rather than an
   error.
3. **Cascade delete paths** — SQL Server forbids certain multi-path cascade chains. We've set
   `Review.Student` and `Course.Category`/`Course.Instructor` to `DeleteBehavior.Restrict`
   specifically to avoid "may cause cycles or multiple cascade paths" errors during
   `migrations add`. If you still hit that error, the fix is almost always changing one more
   `OnDelete(...)` call from `Cascade` to `Restrict` in `EducationalPlatform.Data/Configurations`.
4. **First migration** — must be created manually (`dotnet ef migrations add InitialCreate`)
   since no SDK was available to generate it for you.

## 7. Suggested defense talking points

- **Layered architecture**: trace a request from `CourseController.Details` → `ICourseService` →
  `IUnitOfWork.CourseRepository` → `ApplicationDbContext` to show separation of concerns.
- **Generic Repository + Unit of Work**: open `IGenericRepository<T>` and show how every entity
  gets full CRUD without rewriting data-access code per entity.
- **Identity & role-based authorization**: show `[Authorize(Roles = ...)]` on `CourseController`
  and `LessonController`, and the ownership check (`IsOwnerOrAdminAsync`) that stops one
  instructor from editing another's course.
- **ViewModels vs Entities**: explain why `CourseCreateViewModel` ≠ `Course` (keeps EF entities
  out of the UI, lets you validate/shape exactly what a form needs).
- **REST API**: open Swagger, demonstrate `GET /api/courses`, then show the same data reflected
  in the MVC site, proving both layers share one Service/Repository/Data stack.
