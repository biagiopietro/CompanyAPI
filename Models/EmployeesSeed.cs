using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Models.Database.Seed
{
    public static class EmployeeSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CompanyContext(
                    serviceProvider.GetRequiredService<
                        DbContextOptions<CompanyContext>>()))
            {
                if (context.Employees.Any())
                {
                    Console.WriteLine("No seed added");
                    return;   // DB has been seeded
                }

                context.Employees.AddRange(
                    new Employee
                    {
                        Name = "Phil",
                        Surname = "Rossi",
                        Age = 18,
                        Gender = Gender.Male
                    },
                    new Employee
                    {
                        Name = "Jonathan",
                        Surname = "Markov",
                        Age = 25,
                        Gender = Gender.Male
                    },
                    new Employee
                    {
                        Name = "Lucas",
                        Surname = "Buffalo",
                        Age = 16,
                        Gender = Gender.Male
                    },
                    new Employee
                    {
                        Name = "Jessica",
                        Surname = "Apple",
                        Age = 35,
                        Gender = Gender.Female
                    },
                    new Employee
                    {
                        Name = "Francesca",
                        Surname = "Mc Grey",
                        Age = 16,
                        Gender = Gender.Female
                    }
                );
                context.SaveChanges();
            }
        }
    }
}