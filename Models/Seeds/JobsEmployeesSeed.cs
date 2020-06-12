using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model;

namespace Models.Database.Seed
{
    public static class JobEmployeeSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CompanyContext(
                    serviceProvider.GetRequiredService<
                        DbContextOptions<CompanyContext>>()))
            {
                // Look for any movies.
                if (context.JobEmployees.Any())
                {
                    Console.WriteLine("No seed added");
                    return;   // DB has been seeded
                }

                List<Employee> employees = context.Employees.ToList();
                if (employees.Count <= 0)
                {
                    return;
                }
                List<Job> jobs = context.Jobs.ToList();
                if (jobs.Count <= 0)
                {
                    return;
                }

                context.JobEmployees.AddRange(
                    new JobEmployee
                    {
                        Employee = employees[0],
                        Job = jobs[0],
                        Start = new DateTime(2012, 02, 13),
                        End = new DateTime(2013, 12, 01),
                        MonthlySalaryGross = 1350
                    },
                    new JobEmployee
                    {
                        Employee = employees[0],
                        Job = jobs[1],
                        Start = new DateTime(2014, 07, 17),
                        MonthlySalaryGross = 2100
                    }
                );
                context.SaveChanges();
            }
        }
    }
}