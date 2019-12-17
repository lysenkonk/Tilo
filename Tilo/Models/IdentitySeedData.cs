//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Tilo.Models
//{
//    public class IdentitySeedData
//    {
//        private const string adminUser = "Admin";
//        private const string adminPassword = "Secret123$";

//        public static async Task EnsurePopulated(IApplicationBuilder app)
//        {
//            AppIdentityDbContext identity_context = app.ApplicationServices
//                .GetRequiredService<AppIdentityDbContext>();
//            identity_context.Database.Migrate();

//            UserManager<IdentityUser> userManager = app.ApplicationServices
//             .GetRequiredService<UserManager<IdentityUser>>();

//            IdentityUser user = await userManager.FindByIdAsync(adminUser);
//            if (user == null)
//                user = new IdentityUser("Admin");
//            await userManager.CreateAsync(user, adminPassword);
//        }
//}
