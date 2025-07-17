using System.Threading.Tasks;
using KPayBillApi.Web.Data.Entities;
using KPayBillApi.Web.Helpers;
using KPayBillApi.Common.Enums;
using System;

namespace KPayBillApi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsycn();
            await CheckAdminKPAsync("Luis", "Núñez", "luisalbertonu@gmail.com", "351 681 4963", UserType.AdminKP);
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.AdminKP.ToString());
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckAdminKPAsync(string firstName, string lastName, string email, string phoneNumber, UserType userType)
        {
            DateTime ahora = DateTime.Now;

            User user = await _userHelper.GetUserAsync(email);
            
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    UserName = email,
                    UserType = userType,
                    Active = true,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
        }
    }
}