using SunnyHillStore.Model.Entities.Base;
using System.Collections.Generic;

namespace SunnyHillStore.Model.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
} 