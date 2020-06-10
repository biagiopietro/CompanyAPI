using System;

namespace Responses
{
    public class EmployeeResponse
    {
        public EmployeeResponse(long id, string name, string surname, string fullName, int age, string gender)
        {
            Id = id;
            Name = name;
            Surname = surname;
            FullName = fullName;
            Age = age;
            Gender = gender;
        }

        public long Id { get; set; }
        public int Age {get; set;}
        public string Gender {get; set;}
        public string Surname {get; set;}
        public string Name {get; set;}
        public string FullName {get; set;}
    }
}