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
using KPayBillApi.Web.Models;
using System.IO;
using KPayBillApi.Common.Enums;

namespace KPayBillApi.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BillsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;
        private readonly IUserHelper _userHelper;

        public BillsController(IUserHelper userHelper,DataContext context, IFilesHelper filesHelper )
        {
            _context = context;
            _filesHelper = filesHelper;
            _userHelper = userHelper;
        }

        //-----------------------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
            List<Bill> bills = await _context.Bills
                .Include(x => x.User)
              .OrderBy(x => x.Id)
              .ToListAsync();

            List<BillViewModel> list = new List<BillViewModel>();

            foreach (Bill bill in bills)
            {
                BillViewModel billViewModel = new BillViewModel
                {
                    Id = bill.Id,
                    EmitterCompanyId=bill.EmitterCompanyId,
                    EmitterCompanyName=bill.EmitterCompanyName,
                    ReceiverCompanyId=bill.ReceiverCompanyId,   
                    ReceiverCompanyName =bill.ReceiverCompanyName,
                    UserId  =bill.UserId,
                    UserName    =bill.User.FullName,
                    Cuil    =bill.Cuil,
                    CreateDate =bill.CreateDate,
                    BillDate    =bill.BillDate,
                    Tipo =bill.Tipo,
                    Letra   =bill.Letra,
                    PV =bill.PV,
                    Numero  =bill.Numero,
                    StrComprobante  =bill.StrComprobante,
                    ImporteNeto =bill.ImporteNeto,
                    ImporteIVA  =bill.ImporteIVA,
                    ImporteTotal    =bill.ImporteTotal,
                    Archivo =bill.Archivo,
                    OC  =bill.OC,
                    DocContable =bill.DocContable,
                    Estado=bill.Estado,
                    Motivo  =bill.Motivo,                   
                };

                list.Add(billViewModel);
            }
            return Ok(list);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        [Route("GetBillsByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBillsByUser(string userId)
        {
            List<Bill> bills = await _context.Bills
                .Include(x => x.User)
                .Where(x => x.User.Id== userId && x.Estado==BillState.Enviado)
              .OrderBy(x => x.Id)
              .ToListAsync();

            List<BillViewModel> list = new List<BillViewModel>();

            foreach (Bill bill in bills)
            {
                BillViewModel billViewModel = new BillViewModel
                {
                    Id = bill.Id,
                    EmitterCompanyId = bill.EmitterCompanyId,
                    EmitterCompanyName = bill.EmitterCompanyName,
                    ReceiverCompanyId = bill.ReceiverCompanyId,
                    ReceiverCompanyName = bill.ReceiverCompanyName,
                    UserId = bill.UserId,
                    UserName = bill.User.FullName,
                    Cuil = bill.Cuil,
                    CreateDate = bill.CreateDate,
                    BillDate = bill.BillDate,
                    Tipo = bill.Tipo,
                    Letra = bill.Letra,
                    PV = bill.PV,
                    Numero = bill.Numero,
                    StrComprobante = bill.StrComprobante,
                    ImporteNeto = bill.ImporteNeto,
                    ImporteIVA = bill.ImporteIVA,
                    ImporteTotal = bill.ImporteTotal,
                    Archivo = bill.Archivo,
                    OC = bill.OC,
                    DocContable = bill.DocContable,
                    Estado = bill.Estado,
                    Motivo = bill.Motivo,
                };

                list.Add(billViewModel);
            }
            return Ok(list);
        }

        //-----------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<BillViewModel>> GetBill(int id)
        {

            Bill bill = await _context.Bills
                .Include(u => u.User)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (bill == null)
            {
                return NotFound();
            }

            return new BillViewModel
            {
                Id = bill.Id,
                EmitterCompanyId = bill.EmitterCompanyId,
                EmitterCompanyName = bill.EmitterCompanyName,
                ReceiverCompanyId = bill.ReceiverCompanyId,
                ReceiverCompanyName = bill.ReceiverCompanyName,
                UserId = bill.UserId,
                UserName = bill.User.FullName,
                Cuil = bill.Cuil,
                CreateDate = bill.CreateDate,
                BillDate = bill.BillDate,
                Tipo = bill.Tipo,
                Letra = bill.Letra,
                PV = bill.PV,
                Numero = bill.Numero,
                StrComprobante = bill.StrComprobante,
                ImporteNeto = bill.ImporteNeto,
                ImporteIVA = bill.ImporteIVA,
                ImporteTotal = bill.ImporteTotal,
                Archivo = bill.Archivo,
                OC = bill.OC,
                DocContable = bill.DocContable,
                Estado = bill.Estado,
                Motivo = bill.Motivo,
            };
        }

        //-----------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(int id, BillRequest billRequest)
        {
            if (id != billRequest.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Bill oldBill = await _context.Bills.FirstOrDefaultAsync(o => o.Id == billRequest.Id);

            //Foto
            string imageUrl = string.Empty;
            if (billRequest.ImageArray != null && billRequest.ImageArray.Length > 0)
            {
                imageUrl = string.Empty;
                var stream = new MemoryStream(billRequest.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.pdf";
                var folder = "wwwroot\\images\\Documents";
                var fullPath = $"~/images/Documents/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;                    
                }
            }

            DateTime ahora = DateTime.Now;

            oldBill!.EmitterCompanyId = billRequest.EmitterCompanyId;
            oldBill!.EmitterCompanyName = billRequest.EmitterCompanyName;
            oldBill!.ReceiverCompanyId = billRequest.ReceiverCompanyId;
            oldBill!.ReceiverCompanyName = billRequest.ReceiverCompanyName;
            oldBill!.UserId = billRequest.UserId;
            oldBill!.Cuil = billRequest.Cuil;
            oldBill!.CreateDate = ahora;
            oldBill!.BillDate = billRequest.BillDate;
            oldBill!.Tipo = billRequest.Tipo;
            oldBill!.Letra = billRequest.Letra;
            oldBill!.PV = billRequest.PV;
            oldBill!.Numero = billRequest.Numero;
            oldBill!.StrComprobante = billRequest.StrComprobante;
            oldBill!.ImporteNeto = billRequest.ImporteNeto;
            oldBill!.ImporteIVA = billRequest.ImporteIVA;
            oldBill!.ImporteTotal = billRequest.ImporteTotal;
            oldBill!.OC = billRequest.OC;
            oldBill!.DocContable = billRequest.DocContable;
            oldBill!.Estado = Common.Enums.BillState.Enviado;
            oldBill!.Motivo = billRequest.Motivo;
            oldBill!.Archivo = imageUrl;


            _context.Update(oldBill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe el documento.");
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

            return Ok(oldBill);
        }

        //-----------------------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(BillRequest billRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime ahora = DateTime.Now;
            User createUser = await _userHelper.GetUserByIdAsync(billRequest.UserId);

            BillState state = BillState.Enviado;

            if (billRequest.Estado == 1)
            {
                state = BillState.Rechazado;
            }

            if (billRequest.Estado == 2)
            {
                state = BillState.Pagado;
            }

            Bill newBill = new Bill
            {
                Id = 0,
                EmitterCompanyId = billRequest.EmitterCompanyId,
                EmitterCompanyName = billRequest.EmitterCompanyName,
                ReceiverCompanyId = billRequest.ReceiverCompanyId,
                ReceiverCompanyName = billRequest.ReceiverCompanyName,
                UserId = billRequest.UserId,
                User = createUser,
                Cuil = billRequest.Cuil,
                CreateDate = ahora,
                BillDate = billRequest.BillDate,
                Tipo = billRequest.Tipo,
                Letra = billRequest.Letra,
                PV = billRequest.PV,
                Numero = billRequest.Numero,
                StrComprobante = billRequest.StrComprobante,
                ImporteNeto = billRequest.ImporteNeto,
                ImporteIVA = billRequest.ImporteIVA,
                ImporteTotal = billRequest.ImporteTotal,
                OC = billRequest.OC,
                DocContable = billRequest.DocContable,
                Estado = state,
                Motivo = billRequest.Motivo,
                Archivo ="",
            };


            //Foto


            if (billRequest.ImageArray != null)
            {
                var stream = new MemoryStream(billRequest.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.pdf";
                var folder = "wwwroot\\images\\Documents";
                var fullPath = $"~/images/Documents/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    newBill.Archivo = fullPath;
                }
            }
            _context.Bills.Add(newBill);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(newBill);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicada"))
                {
                    return BadRequest("Ya existe este Documento.");
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
            Bill bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
