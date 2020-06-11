using System.ComponentModel.DataAnnotations;

namespace Models
{
    public enum Gender
    {
        Male = 'M',
        Female = 'F'
    }

    public class Employee
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [MaxLength(150)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string SerialNumber { get; set; }
        public string FullName
        {
            get => Surname + " " + Name;
        }
        public bool isAdult
        {
            get => Age >= 18;
        }

    }

}
