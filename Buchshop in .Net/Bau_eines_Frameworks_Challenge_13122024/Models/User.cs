using System.Collections.Generic;

namespace BuchShop.Models
{
    public class User : BaseUser
    {
        public int UserId { get; set; } // Primärschlüssel
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();
    }
}
