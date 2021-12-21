using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Redis.OM.Modeling;

namespace RedisJsonPlayground.Entities
{
    [DebuggerDisplay("{Zip}: {City} {Street} {House}")]
    [Document]
    public class Address
    {
        [Indexed(Sortable = true, CaseSensitive = false, Normalize = true)]
        public string City { get; set; } = string.Empty;
        [Indexed]
        public string Street { get; set; } = string.Empty;
        [Indexed(Aggregatable = true)]
        public string House { get; set; } = string.Empty;
        [RedisIdField]
        public string Zip { get; set; } = string.Empty;
    }
}
