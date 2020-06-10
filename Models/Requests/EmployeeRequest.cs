using System;

namespace Requests
{
    public class EmployeeRequest
    {
        public EmployeeRequest(long id, string name, string surname, int age, string gender)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Age = age;
            Gender = gender;
        }

        public long Id { get; set; }
        public int Age {get; set;}
        public string Gender {get; set;}
        public string Surname {get; set;}
        public string Name {get; set;}
    }
}