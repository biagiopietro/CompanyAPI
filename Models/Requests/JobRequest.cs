using System;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class JobRequest
    {
        [Required]
        public long Id { get; set; }
        
        [Required]
        public string Name {get; set;}
    }
}