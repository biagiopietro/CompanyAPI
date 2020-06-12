using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Models.Database.Seed
{
    public static class JobSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {   
            using (var context = new CompanyContext(
                    serviceProvider.GetRequiredService<
                        DbContextOptions<CompanyContext>>()))
            {
                if (context.Jobs.Any())
                {
                    Console.WriteLine("No Job seeds added");
                    return;   // DB has been seeded
                }

                context.Jobs.AddRange(
                    new Job { Name = "Engineer" },
                    new Job { Name = "Police man/woman" },
                    new Job { Name = "Teacher" },
                    new Job { Name = "Waiter" },
                    new Job { Name = "Recruiter" },
                    new Job { Name = "Bus Driver" }

                );
                context.SaveChanges();
            }
        }
    }
}