using System;
using System.ComponentModel.DataAnnotations;
using Models;

namespace Model
{
    public class JobEmployeeResponse
    {
        public JobEmployeeResponse(long id, string jobName, DateTime start, DateTime end, double monthlySalaryGross, double yearlySalaryGross)
        {
            this.Id = id;
            this.JobName = jobName;
            this.Start = start;
            this.End = end;
            this.MonthlySalaryGross = monthlySalaryGross;
            this.YearlySalaryGross = yearlySalaryGross;
        }

        public long Id {get; set;}
        public string JobName {get; set;}
        public DateTime Start {get; set;}
        public DateTime End {get; set;}
        public double MonthlySalaryGross {get; set;}
        public double YearlySalaryGross {get; set;}

    }   
}