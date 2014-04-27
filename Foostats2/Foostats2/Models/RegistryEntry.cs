using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foostats2.Models
{
    public class RegistryEntry
    {
        [Key, Column(Order = 0)]
        public string ParentKey { get; set; }
        [Key, Column(Order = 1)]
        public string ChildKey { get; set; }
        public string Value { get; set; }
    }
}