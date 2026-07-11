using EducationalPlatform.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EducationalPlatform.Data.Seed
{
    /// <summary>
    /// Populates a fresh database with the minimum data needed to demo every
    /// feature: roles, an admin login, a couple of instructors, categories,
    /// sample courses with lessons, and the membership plans. Called once
    /// from Program.cs on startup - every step checks "does this already
    /// exist?" first, so it's safe to run on every app start.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            var admin = await SeedAdminAsync(userManager);
            var instructors = await SeedInstructorsAsync(userManager);
            var student = await SeedDemoStudentAsync(userManager);
            var categories = await SeedCategoriesAsync(context);
            await SeedCoursesAndLessonsAsync(context, categories, instructors);
            await SeedSubscriptionPlansAsync(context);
            await SeedSampleEnrollmentAsync(context, student);

            logger.LogInformation("Database seeding completed.");
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task<ApplicationUser> SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@eduplatform.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Platform Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                // NOTE for graduation defense: this seeded password is for demo
                // purposes only. In a real deployment it should come from
                // configuration/secrets and be changed on first login.
                var result = await userManager.CreateAsync(admin, "Admin@12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }

            return admin;
        }

        private static async Task<List<ApplicationUser>> SeedInstructorsAsync(UserManager<ApplicationUser> userManager)
        {
            var instructorsData = new (string Email, string FullName, string ProfileImage)[]
            {
        ("manwella.emad@eduplatform.com", "Manwella Emad", "/assets/img/team/manwella.jpg"),
        ("radwa.abdelmonem@eduplatform.com", "Radwa Abd Elmonem", "/assets/img/team/radwa1.jpg"),
        ("shaza.ashraf@eduplatform.com", "Shaza Ashraf", "/assets/img/team/shaza.jpg")
            };

            var instructors = new List<ApplicationUser>();

            foreach (var data in instructorsData)
            {
                var existing = await userManager.FindByEmailAsync(data.Email);

                if (existing == null)
                {
                    var instructor = new ApplicationUser
                    {
                        UserName = data.Email,
                        Email = data.Email,
                        FullName = data.FullName,
                        ProfileImage = data.ProfileImage,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(instructor, "Instructor@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(instructor, Roles.Instructor);
                    }

                    instructors.Add(instructor);
                }
                else
                {
                    // يحدث الصورة والاسم لو اتغيروا في الـ Seed
                    existing.FullName = data.FullName;
                    existing.ProfileImage = data.ProfileImage;

                    await userManager.UpdateAsync(existing);

                    instructors.Add(existing);
                }
            }

            return instructors;
        }

        private static async Task<ApplicationUser> SeedDemoStudentAsync(UserManager<ApplicationUser> userManager)
        {
            const string studentEmail = "sara.yasser@eduplatform.com";
            var student = await userManager.FindByEmailAsync(studentEmail);

            if (student == null)
            {
                student = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Sara Yasser",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(student, "Student@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, Roles.Student);
                }
            }

            return student;
        }

        private static async Task<List<Category>> SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Education", Description = "General and foundational education courses." },
                    new() { Name = "UI/UX", Description = "User interface and user experience design." },
                    new() { Name = "Lifestyle", Description = "Web fundamentals and lifestyle-related skills." },
                    new() { Name = "Science", Description = "Mobile app development and applied science." },
                    new() { Name = "Modern Design", Description = "Graphic design and modern creative tools." },
                    new() { Name = "Marketing", Description = "Data science and digital marketing." }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
                return categories;
            }

            return await context.Categories.ToListAsync();
        }

        private static async Task SeedCoursesAndLessonsAsync(
            ApplicationDbContext context,
            List<Category> categories,
            List<ApplicationUser> instructors)
        {
            if (await context.Courses.AnyAsync())
            {
                return;
            }

            Category Cat(string name) => categories.First(c => c.Name == name);
            ApplicationUser Instr(int index) => instructors[index % instructors.Count];

            var courses = new List<Course>
            {
                new()
                {
                    Title = "Complete User Fundamentals: Beginner to Advanced",
                    Description = "A complete walkthrough of core platform fundamentals, designed for absolute beginners who want a structured path to advanced topics.",
                    Price = 99,
                    ImageUrl = "/assets/img/course/funds.jpeg",
                    DurationMinutes = 152,
                    SeatsAvailable = 30,
                    Category = Cat("Education"),
                    Instructor = Instr(0)
                },
                new()
                {
                    Title = "Advanced Android 12 & Kotlin Development Course",
                    Description = "Learn modern Android development with Kotlin, covering Jetpack libraries, architecture components, and best practices.",
                    Price = 49,
                    ImageUrl = "/assets/img/course/kotlin.webp",
                    DurationMinutes = 212,
                    SeatsAvailable = 25,
                    Category = Cat("UI/UX"),
                    Instructor = Instr(1)
                },
                new()
                {
                    Title = "Complete HTML, CSS and JavaScript Course",
                    Description = "Build a strong front-end foundation by mastering HTML5, CSS3, and modern JavaScript from scratch.",
                    Price = 0,
                    ImageUrl = "/assets/img/course/basics.jpg",
                    DurationMinutes = 152,
                    SeatsAvailable = null,
                    Category = Cat("Lifestyle"),
                    Instructor = Instr(2)
                },
                new()
                {
                    Title = "iOS and Swift: Complete iOS Application Development",
                    Description = "Design and build full iOS applications using Swift and SwiftUI, from your first screen to App Store submission.",
                    Price = 59,
                    ImageUrl = "/assets/img/course/swift.jfif",
                    DurationMinutes = 152,
                    SeatsAvailable = 20,
                    Category = Cat("Science"),
                    Instructor = Instr(0)
                },
                new()
                {
                    Title = "Learn Graphic Design with a Top IT Trainer",
                    Description = "Master the principles of modern graphic design, typography, and color theory using industry-standard tools.",
                    Price = 79,
                    ImageUrl = "/assets/img/course/Design.webp",
                    DurationMinutes = 152,
                    SeatsAvailable = 30,
                    Category = Cat("Modern Design"),
                    Instructor = Instr(1)
                },
                new()
                {
                    Title = "Complete Data Science for Your Next Business",
                    Description = "Apply data science and analytics techniques to real business problems, from data cleaning to actionable insights.",
                    Price = 39,
                    ImageUrl = "/assets/img/course/Data.jfif",
                    DurationMinutes = 152,
                    SeatsAvailable = 30,
                    Category = Cat("Marketing"),
                    Instructor = Instr(2)
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // Add a few lessons to each course so the Curriculum tab has real content.
            foreach (var course in courses)
            {
                context.Lessons.AddRange(
                    new Lesson
                    {
                        CourseId = course.Id,
                        Title = "Course Introduction & Setup",
                        Content = "Overview of what you'll learn in this course and how to set up your environment.",
                        VideoUrl = "https://www.youtube.com/embed/RXv_uIN6e-Y",
                        LessonOrder = 1,
                        DurationMinutes = 12
                    },
                    new Lesson
                    {
                        CourseId = course.Id,
                        Title = "Core Concepts",
                        Content = "A deep dive into the fundamental concepts that the rest of the course builds on.",
                        VideoUrl = "https://www.youtube.com/embed/RXv_uIN6e-Y",
                        LessonOrder = 2,
                        DurationMinutes = 25
                    },
                    new Lesson
                    {
                        CourseId = course.Id,
                        Title = "Hands-on Practice",
                        Content = "Apply what you've learned with a guided, hands-on exercise.",
                        VideoUrl = null,
                        LessonOrder = 3,
                        DurationMinutes = 30
                    }
                );
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSubscriptionPlansAsync(ApplicationDbContext context)
        {
            if (await context.SubscriptionPlans.AnyAsync())
            {
                return;
            }

            context.SubscriptionPlans.AddRange(
                new SubscriptionPlan
                {
                    Name = "Basic",
                    Price = 0,
                    DurationInDays = 30,
                    Features = "Access to free courses\nCommunity support\nCourse completion certificates",
                    IsFeatured = false
                },
                new SubscriptionPlan
                {
                    Name = "Standard",
                    Price = 49,
                    DurationInDays = 30,
                    Features = "Everything in Basic\nAccess to all paid courses\nPriority email support",
                    IsFeatured = false
                },
                new SubscriptionPlan
                {
                    Name = "Premium",
                    Price = 69,
                    DurationInDays = 30,
                    Features = "Everything in Standard\n1-on-1 mentorship sessions\nDownloadable course resources",
                    IsFeatured = true
                }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedSampleEnrollmentAsync(ApplicationDbContext context, ApplicationUser student)
        {
            if (await context.Enrollments.AnyAsync() || student is null)
            {
                return;
            }

            var firstCourse = await context.Courses.FirstOrDefaultAsync();
            if (firstCourse != null)
            {
                context.Enrollments.Add(new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = firstCourse.Id,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = EnrollmentStatus.Active,
                    ProgressPercentage = 35
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
