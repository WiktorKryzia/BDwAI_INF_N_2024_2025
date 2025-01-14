using Microsoft.AspNetCore.Identity;

namespace ChessManager.Areas.Identity.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "Arbiter", "Player" };

                foreach (var role in roles)
                {
                    if (await roleManager.FindByNameAsync(role) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminUsers = new List<(string Email, string FirstName, string LastName, DateTime BirthDate, Gender Gender, string Password)>
                {
                    ("igor@chessmanager.com", "Igor", "Kłopotowski", new DateTime(2001, 1, 1), Gender.Male, "Admin1234!"),
                    ("wiktor@chessmanager.com", "Wiktor", "Kryzia", new DateTime(2001, 1, 1), Gender.Male, "Admin5678!")
                };

                foreach (var (email, firstName, lastName, birthDate, gender, password) in adminUsers)
                {
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var adminUser = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            BirthDate = birthDate,
                            Gender = gender
                        };

                        var result = await userManager.CreateAsync(adminUser, password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                        }
                    }
                }
            }
        }
    }

}
