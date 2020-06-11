using System;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EmployeeRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string SerialNumber { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [MaxLength(150)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
    }
}