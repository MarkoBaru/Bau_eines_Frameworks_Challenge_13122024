using BuchShop.Framework.Data;

namespace BuchShop.Models
{
    public abstract class BaseUser
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
    }
}
