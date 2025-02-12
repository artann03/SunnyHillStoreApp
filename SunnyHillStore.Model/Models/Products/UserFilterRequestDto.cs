using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyHillStore.Model.Models.Products
{
    public class UserFilterRequestDto : BaseFilterRequestDto
    {
        public int Page { get; set; }
        public int Count { get; set; }  
    }
}
