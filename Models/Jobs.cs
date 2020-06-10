using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Job
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name {get; set;}
    }
}
