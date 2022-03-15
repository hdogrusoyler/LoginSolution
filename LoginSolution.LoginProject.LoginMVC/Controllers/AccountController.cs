using Microsoft.AspNetCore.Mvc;
using LoginSolution.LoginProject.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LoginSolution.LoginProject.LoginMVC.Models;
using LoginSolution.LoginProject.BusinessLogic.Abstract;
using System.Net.Mail;
using System.Net;

namespace LoginSolution.LoginProject.LoginMVC.Controllers
{
    public class AccountController : Controller
    {
        public static int OnlineUserCount = 0;

        private IUserService userService;
        private ILogService logService;
        public IConfiguration Configuration { get; }

        public AccountController(IUserService _userService, ILogService _logService, IConfiguration configuration)
        {
            userService = _userService;
            logService = _logService;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RegisterConfirm()
        {
            return View();
        }

        public IActionResult Login()
        {
            ViewBag.OnlineUserCount = OnlineUserCount;

            return View();
        }

        public IActionResult ResetActivationCode()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.Email = model.Email;
                user.Password = model.Password;
                user.Name = model.Name;
                user.SurName = model.SurName;
                user.ActivationCode = Guid.NewGuid().ToString();
                user.IsActive = false;
                var confirm = new
                {
                    NetworkCredentialMail = Configuration.GetSection("UserConfirm:NetworkCredentialMail").Value,
                    NetworkCredentialPassword = Configuration.GetSection("UserConfirm:NetworkCredentialPassword").Value,
                    MailAddressEmail = Configuration.GetSection("UserConfirm:MailAddressEmail").Value,
                    MailAddressName = Configuration.GetSection("UserConfirm:MailAddressName").Value
                };
                var userres = userService.AddUserAndLog(user, confirm.NetworkCredentialMail, confirm.NetworkCredentialPassword, confirm.MailAddressEmail, confirm.MailAddressName);
                if (userres == "2")
                {
                    return RedirectToAction("RegisterConfirm", "Account");
                }
            }

            return RedirectToAction("Register");
        }


        [HttpPost]
        public async Task<IActionResult> RegisterConfirm(LoginConfirmModel model)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                user = userService.GetByEmail(model.Email);
                if (user != null && user.ActivationCode == model.ActivationCode)
                {
                    user.IsActive = true;
                    var userres = userService.UpdateUserAndAddLog(user);
                    if (userres == "2")
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return RedirectToAction("RegisterConfirm");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            User user = new User();

            if (ModelState.IsValid)
            {
                user = userService.GetByEmail(model.Email);
                if (user != null && user.IsActive && user.Password == model.Password)
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.Name + " " + user.SurName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);

                    OnlineUserCount++;

                    return RedirectToAction("List", "Home");
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            OnlineUserCount--;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ResetActivationCode(string Email)
        {
            var resUser = userService.GetByEmail(Email);
            if (resUser != null)
            {
                resUser.ActivationCode = Guid.NewGuid().ToString();
                var resEnt = userService.Update(resUser);
                if (resEnt == "1")
                {
                    var confirm = new
                    {
                        NetworkCredentialMail = Configuration.GetSection("UserConfirm:NetworkCredentialMail").Value,
                        NetworkCredentialPassword = Configuration.GetSection("UserConfirm:NetworkCredentialPassword").Value,
                        MailAddressEmail = Configuration.GetSection("UserConfirm:MailAddressEmail").Value,
                        MailAddressName = Configuration.GetSection("UserConfirm:MailAddressName").Value
                    };
                    userService.UserConfirm(confirm.NetworkCredentialMail, confirm.NetworkCredentialPassword, confirm.MailAddressEmail, confirm.MailAddressName, Email, resUser.ActivationCode);
                    return RedirectToAction("ResetPassword");
                }
            }
            return RedirectToAction("ResetActivationCode");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(LoginConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var resUser = userService.GetByEmail(model.Email);
                if (resUser != null && resUser.ActivationCode == model.ActivationCode)
                {
                    resUser.Password = model.Password;
                    var resEnt = userService.Update(resUser);
                    if (resEnt == "1")
                    {
                        return RedirectToAction("Login");
                    }
                }
            }
            return RedirectToAction("ResetPassword");
        }

    }
}
