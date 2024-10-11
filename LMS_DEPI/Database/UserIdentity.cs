using Microsoft.AspNetCore.Identity;

namespace LMS_DEPI.APP.Database
{
    public class UserIdentity : IdentityUser
    {
        public string? Initials { get; set; }
    }
}
