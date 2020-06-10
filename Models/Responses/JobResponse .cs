using System;

namespace Responses
{
    public class JobResponse
    {
        public JobResponse(long id, string name)
        {
            Id = id;
            Name = name;
        }
        public long Id { get; set; }
        public string Name {get; set;}
    }
}