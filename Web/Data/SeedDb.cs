using System.Threading.Tasks;
using KPayBillApi.Web.Data.Entities;
using KPayBillApi.Web.Helpers;
using KPayBillApi.Common.Enums;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            await CheckCompaniesAsync();
            Company keypress = await _context.Companies.FirstOrDefaultAsync(o => o.Id == 1);
            Company rowing = await _context.Companies.FirstOrDefaultAsync(o => o.Id == 2);
            Company fleet = await _context.Companies.FirstOrDefaultAsync(o => o.Id == 3);
            await CheckAdminKPAsync("Luis", "Núñez", "luisalbertonu@gmail.com", "351 681 4963", UserType.AdminKP, keypress);
            await CheckAdminKPAsync("Pablo", "Lacuadri", "pablo@yopmail.com", "351 111 1111", UserType.AdminKP, keypress);
            await CheckAdminKPAsync("Gonzalo", "Prieto", "gonzalo@yopmail.com", "011 1111 1111", UserType.Admin, rowing);
            await CheckAdminKPAsync("Dario", "Fleet", "dario@yopmail.com", "011 1111 1112", UserType.Admin, fleet);
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.AdminKP.ToString());
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Contable.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckAdminKPAsync(string firstName, string lastName, string email, string phoneNumber, UserType userType, Company? company)
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
                    Company = company,
                    CompanyId = company.Id,
                    Active = true,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckCompaniesAsync()
        {
            if (!_context.Companies.Any())

            {
                _context.Companies.Add(new Company
                {
                    Cuil = "20-12345678-9",
                    Name = "Keypress",
                    Address = "Villa Santa Ana",
                    Phone = "351 11223344",
                    Email = "keypress@yopmail.com",
                    Active = true,
                    Type = "Empresa",
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "1-11111111-1",
                    Name = "Rowing",
                    Address = "Capital Federal",
                    Phone = "011-11111111",
                    Email = "rowing@yopmail.com",
                    Active = true,
                    Type = "Empresa",
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "22-22222222-2",
                    Name = "Fleet",
                    Address = "Capital Federal AMBA",
                    Phone = "011 22222222",
                    Email = "fleet@yopmail.com",
                    Active = true,
                    Type = "Empresa",
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}