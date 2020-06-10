using System;
using System.Diagnostics.CodeAnalysis;

namespace Requests
{
    public class JobEmployeeRequest
    {       
         public long JobId {get; set;}
        public DateTime Start {get; set;}
        [MaybeNull]
        public DateTime End {get; set;}
        public double MonthlySalaryGross {get; set;}
    }
}