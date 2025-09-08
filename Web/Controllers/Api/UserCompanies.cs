using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KPayBillApi.Web.Data;
using KPayBillApi.Web.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace KPayBillApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserCompaniesController : ControllerBase
    {
        private readonly DataContext _context;

        public UserCompaniesController(DataContext context)
        {
            _context = context;
        }

        //-----------------------------------------------------------------------------------
        [HttpDelete("{userId}/{companyId}")]
        public async Task<IActionResult> DeleteCompany(string userId, int companyId)
        {
            UserCompany userCompany = await _context.UserCompanies
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId);

            if (userCompany == null)
            {
                return NotFound();
            }

            _context.UserCompanies.Remove(userCompany);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}