using System;
using System.ComponentModel.DataAnnotations;
using Models;

namespace Model
{
    public class JobEmployee
    {
        public JobEmployee(long Id, Employee Employee, Job Job, DateTime Start, DateTime End, double MonthlySalaryGross)
        {
            this.Id = Id;
            this.Employee = Employee;
            this.Job = Job;
            this.Start = Start;
            this.End = End;
            this.MonthlySalaryGross = MonthlySalaryGross;
        }

        public JobEmployee() {}

        [Key]
        public long Id {get; set;}
        [Required]
        public virtual Employee Employee {get; set;}
        [Required]
        public virtual Job Job {get; set;}
        [Required]
        public DateTime Start {get; set;}
        public DateTime End {get; set;}
        [Required]
        public double MonthlySalaryGross {get; set;}
        public double YearlySalaryGross 
        {
            get => MonthlySalaryGross * 12;
        }
        public bool isWorkingNow() => (End != DateTime.MinValue);

    }   
}