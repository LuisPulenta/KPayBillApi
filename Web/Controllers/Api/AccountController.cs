using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KPayBillApi.Common.Enums;
using KPayBillApi.Web.Data;
using KPayBillApi.Web.Data.Entities;
using KPayBillApi.Web.Helpers;
using KPayBillApi.Web.Models;
using KPayBillApi.Web.Models.Request;

namespace KPayBillApi.Àpi.Controllers.Àpi
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;


        public AccountController(IUserHelper userHelper, IConfiguration configuration, DataContext context, IMailHelper mailHelper, IImageHelper imageHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _context = context;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user2 = await _userHelper.GetUserAsync(model.Username);
                Company company = await _context.Companies.FirstOrDefaultAsync(o => o.Id == user2.CompanyId);
                if (user2 != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user2, model.Password);

                    UserViewModel user = new UserViewModel
                    {
                        Id = user2.Id,
                        FirstName = user2.FirstName,
                        LastName = user2.LastName,
                        UserTypeId = (int)user2.UserType,
                        UserTypeName = user2.UserType.ToString(),
                        Email = user2.Email,
                        EmailConfirm = user2.EmailConfirmed,
                        PhoneNumber = user2.PhoneNumber,
                        CompanyId = company.Id,
                        CompanyName = company.Name,
                        Active = user2.Active,
                    };

                    if (result.Succeeded)
                    {
                        Claim[] claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(99),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            user
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CreateUser")]
        public async Task<ActionResult<User>> PostUser(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = await _userHelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest("Ya existe un usuario registrado con  ese email.");
            }

            DateTime ahora = DateTime.Now;

            Company company = await _context.Companies.FirstOrDefaultAsync(o => o.Id == request.IdCompany);
            User createUser = await _userHelper.GetUserByIdAsync(request.CreateUserId);
            User lastChangeUser = await _userHelper.GetUserByIdAsync(request.LastChangeUserId);

            user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                CompanyId = company.Id,
                Company=company,
                UserName = request.Email,
                UserType = request.IdUserType == 0 ? UserType.AdminKP : request.IdUserType == 1 ? UserType.Admin : UserType.User,
                Active = true,
            };

            await _userHelper.AddUserAsync(user, request.Password);
            await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
            await SendConfirmationEmailAsync(user);

            UserViewModel user2 = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserTypeId = (int)user.UserType,
                UserTypeName = user.UserType.ToString(),
                Email = user.Email,
                EmailConfirm = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                CompanyId = user.Company != null ? user.Company.Id : 1,
                CompanyName = user.Company != null ? user.Company.Name : "KeyPress",
                Active = user.Active,
            };

            return Ok(user2);
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id.ToString() != request.Id)
            {
                return BadRequest();
            }

            User user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest("No existe el usuario.");
            }

            DateTime ahora = DateTime.Now;

            Company company = await _context.Companies.FirstOrDefaultAsync(o => o.Id == request.IdCompany);
            User lastChangeUser = await _userHelper.GetUserByIdAsync(request.LastChangeUserId);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Company = company;
            user.CompanyId = company.Id;
            user.Active = request.Active;
            user.UserType = request.IdUserType == 0 ? UserType.AdminKP : request.IdUserType == 1 ? UserType.Admin : UserType.User;

            await _userHelper.UpdateUserAsync(user);
            return NoContent();
        }

        //-------------------------------------------------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                User user = await _userHelper.GetUserAsync(email);
                if (user != null)
                {
                    IdentityResult result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest(result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    return BadRequest("Usuario no encontrado.");
                }
            }

            return BadRequest(ModelState);
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("El correo ingresado no corresponde a ningún usuario.");
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "", "KPayBill - Reseteo de contraseña", $"<h1>KPayBill - Reseteo de contraseña</h1>" +
                    $"Para establecer una nueva contraseña haga clic en el siguiente enlace:</br></br>" +
                    $"<a href = \"{link}\">Cambio de Contraseña</a>");
                return Ok("Las instrucciones para el cambio de contraseña han sido enviadas a su email.");
            }

            return BadRequest(model);
        }

        //-------------------------------------------------------------------------------------------------
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            List<User> users = await _context.Users
                .Include(x => x.Company)
                 .OrderBy(x => x.Company.Name + x.LastName + x.FirstName)
                .ToListAsync();

            List<UserViewModel> list = new List<UserViewModel>();
            foreach (User user in users)
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserTypeId = (int)user.UserType,
                    UserTypeName = user.UserType.ToString(),
                    Email = user.Email,
                    EmailConfirm = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    CompanyId = user.Company != null ? user.Company.Id : 1,
                    CompanyName = user.Company != null ? user.Company.Name : "KeyPress",
                    Active = user.Active,
                };

                list.Add(userViewModel);
            }
            return Ok(list);
        }

        //-------------------------------------------------------------------------------------------------
        [HttpGet]
        [Route("GetUsuarios")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsuarios()
        {
            List<User> users = await _context.Users
                .Include(x => x.Company)
                .Where(x => x.UserType==UserType.User)
                 .OrderBy(x => x.Company.Name + x.LastName + x.FirstName)
                .ToListAsync();

            List<UserViewModel> list = new List<UserViewModel>();
            foreach (User user in users)
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserTypeId = (int)user.UserType,
                    UserTypeName = user.UserType.ToString(),
                    Email = user.Email,
                    EmailConfirm = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    CompanyId = user.Company != null ? user.Company.Id : 1,
                    CompanyName = user.Company != null ? user.Company.Name : "KeyPress",
                    Active = user.Active,
                };

                list.Add(userViewModel);
            }
            return Ok(list);
        }


        //-------------------------------------------------------------------------------------------------
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(EmailRequest email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _context.Users.FirstOrDefaultAsync(o => o.Email.ToLower() == email.Email.ToLower());

            if (user == null)
            {
                return BadRequest();
            }
            return Ok(user);
        }

        //-------------------------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            User user = await _context.Users
                .FirstOrDefaultAsync(p => p.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            UserViewModel userViewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserTypeId = (int)user.UserType,
                UserTypeName = user.UserType.ToString(),
                Email = user.Email,
                EmailConfirm = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                CompanyId = user.Company != null ? user.Company.Id : 1,
                CompanyName = user.Company != null ? user.Company.Name : "KeyPress",
                Active = user.Active,
            };

            return userViewModel;
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPost("ResendToken")]
        public async Task<IActionResult> ResendTokenAsync([FromBody] EmailRequest model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("El correo ingresado no corresponde a ningún usuario.");
                }

                await SendConfirmationEmailAsync(user);

                return NoContent();
            }

            return BadRequest(model);
        }

        //-------------------------------------------------------------------------------------------------
        private async Task<IActionResult> SendConfirmationEmailAsync(User user)
        {
            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMailSinCc(user.Email, "KPayBill - Confirmación de cuenta", $"<h1>KPayBill - Confirmación de cuenta</h1>" +
                $"Para habilitar el usuario, " +
                $"por favor hacer clic en el siguiente enlace: </br></br><a href = \"{tokenLink}\">Confirmar Email</a>");

            return Ok(user);
        }


        //-------------------------------------------------------------------------------------------------
        [HttpDelete]
        [Route("DeleteUserById/{Id}")]
        public async Task<IActionResult> DeleteUserById(string Id)
        {
            User user = await _userHelper.GetUserAsync(new Guid(Id));
            if (user == null)
            {
                return NotFound();
            }
            await _userHelper.DeleteUserAsync(user);
            return NoContent();
        }

        //-------------------------------------------------------------------------------------------------
        [HttpPost]
        [Route("GetMailsAdmin/{CompanyId}")]
        public async Task<IActionResult> GetMailsAdmin(int CompanyId)

        {
            List<User> users = await _context.Users
                .Where(x => x.Company.Id == CompanyId && x.UserType == UserType.Admin && x.Active)
                .ToListAsync();

            string emailsAdmins = "";
            foreach (User user in users)
            {
                emailsAdmins = emailsAdmins + user.Email + ",";
            }

            if (!string.IsNullOrEmpty(emailsAdmins))
            {
                emailsAdmins = emailsAdmins.Substring(0, emailsAdmins.Length - 1);
            }

            EmailsResponse emailsResponse = new EmailsResponse
            {
                Emails = emailsAdmins
            };
            return Ok(emailsResponse);
        }
    }
        
}