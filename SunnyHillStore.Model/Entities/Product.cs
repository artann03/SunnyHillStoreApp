using SunnyHillStore.Model.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SunnyHillStore.Model.Entities
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Status { get; set; } 

        [Required]
        public string Category { get; set; }

    }
}
