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
    public class SuppliersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;
        private readonly IUserHelper _userHelper;

        public SuppliersController(IUserHelper userHelper, DataContext context, IFilesHelper filesHelper)
        {
            _context = context;
            _filesHelper = filesHelper;
            _userHelper = userHelper;
        }

        //-----------------------------------------------------------------------------------
        [HttpGet]
        [Route("GetSuppliers/{companyId}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers(int companyId)
        {
            List<Supplier> suppliers = await _context.Suppliers
               .Where(x => x.CompanyId == companyId)
              .OrderBy(x => x.Name)
              .ToListAsync();

            return Ok(suppliers);
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            Supplier supplier = await _context.Suppliers
                .FirstOrDefaultAsync(p => p.Id == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        //-----------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierRequest supplierRequest)
        {
            if (id != supplierRequest.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier oldSupplier = await _context.Suppliers.FirstOrDefaultAsync(o => o.Id == supplierRequest.Id);

            oldSupplier!.Active = supplierRequest.Active;
            oldSupplier!.Cuil = supplierRequest.Cuil;
            oldSupplier!.Name = supplierRequest.Name;
            oldSupplier!.Address = supplierRequest.Address;
            oldSupplier!.Phone = supplierRequest.Phone;
            oldSupplier!.Email = supplierRequest.Email;

            _context.Update(oldSupplier);
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

            return Ok(oldSupplier);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(SupplierRequest supplierRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier newSupplier = new Supplier
            {
                Id = 0,
                Cuil = supplierRequest.Cuil,
                Name = supplierRequest.Name,
                Active = true,
                Address = supplierRequest.Address,
                Phone = supplierRequest.Phone,
                Email = supplierRequest.Email,
                CompanyId = supplierRequest.CompanyId,
                CompanyName = supplierRequest.CompanyName,
            };

            _context.Suppliers.Add(newSupplier);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newSupplier);
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
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            Supplier supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            List<Supplier> suppliers = await _context.Suppliers
             .OrderBy(x => x.Name)
             .Where(c => c.Active)
             .ToListAsync();

            return Ok(suppliers);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        [Route("AddUserCompany/{UserId}/{SupplierId}")]
        public async Task<ActionResult> AddUserSupplier(string userId, int supplierId)
        {
            UserCompany newUserCompany = new UserCompany
            {
                Id = 0,
                UserId = userId,
                CompanyId = supplierId,
            };

            _context.UserCompanies.Add(newUserCompany);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newUserCompany);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe esta Empresa en este Usuario.");
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
        [HttpPost]
        [Route("DeleteUserCompany/{UserId}/{CompanyId}")]
        public async Task<ActionResult> DeleteUserCompany(string userId, int companyId)
        {
            UserCompany userCompany = await _context.UserCompanies.FirstOrDefaultAsync(t => t.UserId == userId && t.CompanyId == companyId);
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