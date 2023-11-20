using Microsoft.AspNetCore.Identity;
using Testing.Auth.model;

namespace Testing.Auth
{
    public class AuthDbSeeder
    {
        private readonly UserManager<RentUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthDbSeeder(UserManager<RentUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            await AddDefaultRoles();
            await AddAminUser();
        }
        private async Task AddDefaultRoles()
        {
            foreach (var role in RentRoles.All)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists) {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        private async Task AddAminUser()
        {
            var newAdminUser = new RentUser
            {
                UserName = "admin",
                Email = "admin@admin.com"
            };


            var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
            if (existingAdminUser == null)
            {
                var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "SafeP@ss1?");
                if (createAdminUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newAdminUser, RentRoles.All);
                }
            }
        }






    }
}
