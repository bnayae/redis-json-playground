using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedisJsonPlayground
{
    [DebuggerDisplay("{Zip}: {City} {Street} {House}")]
    public class Address
    {
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string House { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
    }
}
