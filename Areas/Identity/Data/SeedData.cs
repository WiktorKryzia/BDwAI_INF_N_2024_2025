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

                var users = new List<(string Email, string FirstName, string LastName, DateTime BirthDate, Gender Gender, string Password, string Role)>
                {
                    ("igor@chessmanager.com", "Igor", "Kłopotowski", new DateTime(2001, 1, 1), Gender.Male, "Admin1234!", "Admin"),
                    ("wiktor@chessmanager.com", "Wiktor", "Kryzia", new DateTime(2001, 1, 1), Gender.Male, "Admin5678!", "Admin"),
                    ("player1@chessmanager.com", "Player", "1", new DateTime(2001, 1, 1), Gender.Male, "qwerty", "Player"),
                    ("player2@chessmanager.com", "Player", "2", new DateTime(2001, 1, 1), Gender.Female, "qwerty", "Player"),
                    ("player3@chessmanager.com", "Player", "3", new DateTime(2001, 1, 1), Gender.Male, "qwerty!", "Player"),
                    ("player4@chessmanager.com", "Player", "4", new DateTime(2001, 1, 1), Gender.Female, "qwerty!", "Player"),
                };

                foreach (var (email, firstName, lastName, birthDate, gender, password, role) in users)
                {
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            BirthDate = birthDate,
                            Gender = gender
                        };

                        var result = await userManager.CreateAsync(user, password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
            }
        }
    }

}
