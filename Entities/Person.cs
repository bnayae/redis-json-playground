using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Redis.OM.Modeling;

namespace RedisJsonPlayground.Entities
{
    [DebuggerDisplay("{FirstName} {LastName}: {Email}, {Age}")]
    [Document(IndexName = "people", StorageType = StorageType.Json, Prefixes = new[] { "person" }, Language = "en")]
    public class Person
    {
        [Indexed(Separator = ',')] 
        public string Tags { get; set; } = string.Empty;
        [Indexed(Sortable = true, CaseSensitive = false, Normalize = true)] 
        [Searchable(Weight = 10)]
        public string FirstName { get; set; } = string.Empty;
        [Indexed(Aggregatable = true)] 
        [Searchable(Weight = 2)]
        public string LastName { get; set; } = string.Empty;
        [RedisIdField]
        public string Email { get; set; }=string.Empty;
        [Indexed(Sortable = true)]
        public int Age { get; set; }= 20;
        [Indexed]
        public Address? Address { get; set; }


    }
}
