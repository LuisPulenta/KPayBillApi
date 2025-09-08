using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KPayBillApi.Web.Data;
using KPayBillApi.Web.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        //-----------------------------------------------------------------------------------
        [HttpGet("{userId}")]
        public async Task<ActionResult<bool>> GetAssignedCompanies(string userId)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(p => p.Id == userId);

            List<UserCompany> userCompanies = [];

            List<UserCompany> userCompaniesTemp = await _context.UserCompanies
                .Where(x => x.UserId == userId)
              .ToListAsync();

            foreach (var userCompanyTemp in userCompaniesTemp)
            {
                Company company = await _context.Companies
                .FirstOrDefaultAsync(p => p.Id == userCompanyTemp.CompanyId);

                Supplier supplier = await _context.Suppliers
                .FirstOrDefaultAsync(p => p.ForCompanyId == company.Id && p.FromCompanyId == user.CompanyId);

                if (company.Active && supplier.Active)
                {
                    userCompanies.Add(userCompanyTemp);
                }
            }

            if (userCompanies.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}