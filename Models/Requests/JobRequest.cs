using System;

namespace Requests
{
    public class JobRequest
    {
        public JobRequest(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; set; }
        public string Name {get; set;}
    }
}