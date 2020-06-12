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
                    Console.WriteLine("No Employee seeds added");
                    return;   // DB has been seeded
                }

                context.Employees.AddRange(
                    new Employee
                    {
                        Name = "Phil",
                        Surname = "Rossi",
                        Age = 18,
                        Gender = Gender.Male,
                        SerialNumber = "QAWSEDRFT12"
                    },
                    new Employee
                    {
                        Name = "Jonathan",
                        Surname = "Markov",
                        Age = 25,
                        Gender = Gender.Male,
                        SerialNumber = "LAKSM15WRS"
                    },
                    new Employee
                    {
                        Name = "Lucas",
                        Surname = "Buffalo",
                        Age = 16,
                        Gender = Gender.Male,
                        SerialNumber = "UHASCB25SG"
                    },
                    new Employee
                    {
                        Name = "Jessica",
                        Surname = "Apple",
                        Age = 35,
                        Gender = Gender.Female,
                        SerialNumber = "PLMAVSC1D5"
                    },
                    new Employee
                    {
                        Name = "Francesca",
                        Surname = "Mc Grey",
                        Age = 16,
                        Gender = Gender.Female,
                        SerialNumber = "PLASTEFBN9"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}