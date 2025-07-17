using System.Collections.Generic;
using System.Threading.Tasks;
using KPayBillApi.Web.Data.Entities;
using KPayBillApi.Web.Models;

namespace KPayBillApi.Web.Helpers
{
    public interface IConverterHelper
    {
        Task<User> ToUserAsync(UserViewModel model);

        UserViewModel ToUserViewModel(User user);

        List<UserViewModel> ToUserResponse(List<User> users);
    }
}
