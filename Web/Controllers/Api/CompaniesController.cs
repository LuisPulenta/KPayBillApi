using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using KPayBillApi.Web.Data;
using KPayBillApi.Web.Data.Entities;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using KPayBillApi.Common.Helpers;
using KPayBillApi.Web.Models.Request;
using KPayBillApi.Web.Helpers;

namespace KPayBillApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompaniesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;
        private readonly IUserHelper _userHelper;

        public CompaniesController(IUserHelper userHelper, DataContext context, IFilesHelper filesHelper)
        {
            _context = context;
            _filesHelper = filesHelper;
            _userHelper = userHelper;
        }

        //-----------------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            List<Company> companies = await _context.Companies
                .Where(x => x.Type == "Empresa")
              .OrderBy(x => x.Name)
              .ToListAsync();

            return Ok(companies);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        [Route("GetCompaniesAssigned/{userId}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompaniesAssigned(string userId)
        {
            List<UserCompany> userCompanies = await _context.UserCompanies
                .Where(x => x.UserId == userId)
              .ToListAsync();

            List<Company> companies = [];

            foreach (var userCompany in userCompanies)
            {
                Company company = await _context.Companies
                .FirstOrDefaultAsync(p => p.Id == userCompany.CompanyId && p.Active);
                companies.Add(company);
            }

            return Ok(companies);
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            Company company = await _context.Companies
                .FirstOrDefaultAsync(p => p.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        //-----------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, CompanyRequest companyRequest)
        {
            if (id != companyRequest.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Company oldCompany = await _context.Companies.FirstOrDefaultAsync(o => o.Id == companyRequest.Id);

            oldCompany!.Active = companyRequest.Active;
            oldCompany!.Cuil = companyRequest.Cuil;
            oldCompany!.Name = companyRequest.Name;
            oldCompany!.Address = companyRequest.Address;
            oldCompany!.Phone = companyRequest.Phone;
            oldCompany!.Email = companyRequest.Email;

            _context.Update(oldCompany);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe una empresa con el mismo nombre.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(oldCompany);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(CompanyRequest companyRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Company newCompany = new Company
            {
                Id = 0,
                Cuil = companyRequest.Cuil,
                Name = companyRequest.Name,
                Active = true,
                Address = companyRequest.Address,
                Phone = companyRequest.Phone,
                Email = companyRequest.Email,
                Type = companyRequest.Type
            };

            _context.Companies.Add(newCompany);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newCompany);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe esta Empresa.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        //-----------------------------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            Company company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            List<Company> companies = await _context.Companies
             .OrderBy(x => x.Name)
             .Where(c => c.Active && c.Type == "Empresa")
             .ToListAsync();

            return Ok(companies);
        }
    }
}