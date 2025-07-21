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
            Company solflix = await _context.Companies.FirstOrDefaultAsync(o => o.Id == 4);
            await CheckAdminKPAsync("Luis", "Núñez", "luisalbertonu@gmail.com", "351 681 4963", UserType.AdminKP, keypress);
            await CheckAdminKPAsync("Pablo", "Lacuadri", "pablo@yopmail.com", "351 111 1111", UserType.AdminKP, keypress);
            await CheckAdminKPAsync("Mario", "Kempes", "mario@yopmail.com", "351 111 1112", UserType.Admin, keypress);
            await CheckAdminKPAsync("Gonzalo", "Prieto", "gonzalo@yopmail.com", "011 1111 1111", UserType.Admin, rowing);
            await CheckAdminKPAsync("Dario", "Fleet", "dario@yopmail.com", "011 1111 1112", UserType.User, fleet);
            await CheckAdminKPAsync("Andres", "Oberti", "oberti@yopmail.com", "351 111 1114", UserType.User, solflix);
        }

        //--------------------------------------------------------------------------------------------
        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.AdminKP.ToString());
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
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
                    CompanyId=company.Id,
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
                    Email ="keypress@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "1-11111111-1",
                    Name = "Rowing",
                    Address = "Capital Federal",
                    Phone = "011-11111111",
                    Email = "rowing@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "22-22222222-2",
                    Name = "Fleet",
                    Address = "Capital Federal AMBA",
                    Phone = "011 22222222",
                    Email = "fleet@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "33-33333333-3",
                    Name = "Solflix",
                    Address = "Córdoba",
                    Phone = "351 12345678",
                    Email = "solflix@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157729-3",
                    Name = "Luis Núñez",
                    Address = "Espora 2052 B° Rosedal Córdoba",
                    Phone = "3516814963",
                    Email = "luis@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-1",
                    Name = "Talleres",
                    Address = "Colon 101",
                    Phone = "3516000001",
                    Email = "talleres@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-2",
                    Name = "Belgrano",
                    Address = "Colon 102",
                    Phone = "3516000002",
                    Email = "belgrano@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-3",
                    Name = "Instituto",
                    Address = "Colon 103",
                    Phone = "3516000003",
                    Email = "instituto@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-4",
                    Name = "River",
                    Address = "Colon 104",
                    Phone = "3516000004",
                    Email = "river@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-5",
                    Name = "Boca",
                    Address = "Colon 105",
                    Phone = "3516000005",
                    Email = "boca@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-6",
                    Name = "San Lorenzo",
                    Address = "Colon 106",
                    Phone = "3516000006",
                    Email = "sanlorenzo@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-7",
                    Name = "Huiracan",
                    Address = "Colon 107",
                    Phone = "3516000007",
                    Email = "huracan@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-8",
                    Name = "Independiente",
                    Address = "Colon 108",
                    Phone = "3516000008",
                    Email = "independiente@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157001-9",
                    Name = "Racing",
                    Address = "Colon 109",
                    Phone = "3516000009",
                    Email = "racing@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157002-1",
                    Name = "Newells",
                    Address = "Colon 110",
                    Phone = "3516000010",
                    Email = "newells@yopmail.com",
                    Active = true,
                });

                _context.Companies.Add(new Company
                {
                    Cuil = "20-17157002-2",
                    Name = "Rosario Central",
                    Address = "Colon 111",
                    Phone = "3516000011",
                    Email = "rosariocentral@yopmail.com",
                    Active = true,
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}