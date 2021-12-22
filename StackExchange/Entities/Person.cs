using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedisJsonPlayground
{
    [DebuggerDisplay("{FirstName} {LastName}: {Email}, {Age}")]
    public class Person
    {
        public string Tags { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string Email { get; set; }=string.Empty;
        public int Age { get; set; }= 20;
        public Address? Address { get; set; }


    }
}
