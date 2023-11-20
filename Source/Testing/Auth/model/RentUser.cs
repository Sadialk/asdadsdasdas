using Microsoft.AspNetCore.Identity;

namespace Testing.Auth.model
{
    public class RentUser : IdentityUser
    {

        public bool ForceRelogin { get; set; }
    }
}
