using System.Collections.Generic;
using System.Threading.Tasks;
using KPayBillApi.Common.Enums;
using KPayBillApi.Web.Data;
using KPayBillApi.Web.Data.Entities;
using KPayBillApi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace KPayBillApi.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly UserHelper _userHelper;

        public ConverterHelper(DataContext context, UserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        //-------------------------------------------------------------------------------------------------
        public async Task<User> ToUserAsync(UserViewModel model)
        {

            UserType userType = model.UserTypeId == 0 ? UserType.AdminKP : model.UserTypeId == 1 ? UserType.Admin : UserType.User;

            Company company = await _context.Companies.FirstOrDefaultAsync(o => o.Id == model.CompanyId);

            return new User
            {
                Id=model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserType = userType,
                Email = model.Email,
                EmailConfirmed=model.EmailConfirm,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Company = company,
                Active =model.Active,
            };
        }

        //-------------------------------------------------------------------------------------------------
        public UserViewModel ToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserTypeId = (int) user.UserType,
                UserTypeName = user.UserType.ToString(),
                Email = user.Email,
                EmailConfirm=user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                CompanyId = user.Company.Id,
                CompanyName = user.Company.Name,
                Active =user.Active,
            };
        }

        
       
        
        //-------------------------------------------------------------------------------------------------
        public List<UserViewModel> ToUserResponse(List<User> users)
        {
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (User user in users)
            {
                list.Add(ToUserViewModel(user));
            }

            return list;
        }
    }
}
