
using Microsoft.AspNet.Identity.EntityFramework;
namespace Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
