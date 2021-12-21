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
    [Document]
    public class Customer
    {
        [Indexed(Sortable = true, CaseSensitive = false, Normalize = true)] 
        public string FirstName { get; set; } = string.Empty;
        [Indexed(Aggregatable = true)] 
        public string LastName { get; set; } = string.Empty;
        [RedisIdField]
        public string Email { get; set; }=string.Empty;
        [Indexed(Sortable = true)]
        public int Age { get; set; }= 20;
        [Indexed(Separator = ',')]
        public string Tags { get; set; } = string.Empty;
    }
}
