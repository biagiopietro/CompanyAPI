using System;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EmployeeRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public int Age {get; set;}
        [Required]
        public string Gender {get; set;}
        [Required]
        public string Surname {get; set;}
        [Required]
        public string Name {get; set;}
    }
}