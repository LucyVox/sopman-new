using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sopman.Data;
using sopman.Models;
using sopman.Models.AccountViewModels;
using sopman.Services;

namespace sopman.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public PartialViewResult LoggedInLogo()
        {
            var getuser = _userManager.GetUserId(User);

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();

            var theimg = (from i in _context.TheCompanyInfo
                          where i.CompanyId == comp
                          select i.Logo).ToString();

            if (theimg != null)
            {

            }
            ViewBag.Logged = "hello";
            return PartialView();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Manage");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "SOPAdmin");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    await _context.SaveChangesAsync();
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterMainUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterMainUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "SOPAdmin");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    await _context.SaveChangesAsync();
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUser(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                }
                return RedirectToAction("SetupIndex", "Setup");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterSOPCreator(string returnUrl = null)
        {
            var orderVM = new RegisterSopCreatorViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            var getjobs = (from m in _context.JobTitles
                           where m.CompanyId == compid
                           select new { m.JobTitleId, m.JobTitle }).ToList();

            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };

            orderVM.JobTitles = new List<RegisterJobTitleViewModel>();
            foreach (var items in getjobs)
            {
                var jobname = @items.JobTitle;
                var jobid = @items.JobTitleId;
                orderVM.JobTitles.Add(new RegisterJobTitleViewModel { Value = @jobid, JobTitleId = @jobid, JobTitle = @jobname });
            }

            return View(orderVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> RegisterSOPCreator(RegisterSopCreatorViewModel model,[Bind("ClaimId,FirstName,SecondName,CompanyId")] ApplicationDbContext.ClaimComp compclaim)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            var orderVM = new RegisterSopCreatorViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            var getjobs = (from m in _context.JobTitles
                           where m.CompanyId == compid
                           select new { m.JobTitleId, m.JobTitle }).ToList();


            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };


            orderVM.JobTitles = new List<RegisterJobTitleViewModel>();
            foreach (var items in getjobs)
            {
                var jobname = @items.JobTitle;
                var jobid = @items.JobTitleId;
                orderVM.JobTitles.Add(new RegisterJobTitleViewModel { Value = @jobid, JobTitleId = @jobid, JobTitle = @jobname });
            }
            var currentuser = await _userManager.GetUserAsync(User);
            var user_id = currentuser.Id;

            var modules = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            if (ModelState.IsValid)
            {
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SOPCreator");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    _context.CompanyClaim.Select(u => u.UserId);
                    var newuserid = user.Id;
                    compclaim.UserId = newuserid;

                    _context.CompanyClaim.Select(u => u.DepartmentId);
                    var selected = Request.Form["DepartmentId"];
                    compclaim.DepartmentId = selected;

                    _context.CompanyClaim.Select(u => u.JobTitleId);
                    var seljob = Request.Form["JobTitleId"];
                    compclaim.JobTitleId = seljob;

                    _context.CompanyClaim.Select(u => u.CompanyId);
                    compclaim.CompanyId = modules;

                    _context.Add(compclaim);

                    _logger.LogInformation("User created a new account with password.");
                    await _context.SaveChangesAsync();
                    AddErrors(result);
                    return RedirectToAction("People", "Setup");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(orderVM);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterSOPUser()
        {
            var orderVM = new RegisterSOPUserViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            var getjobs = (from m in _context.JobTitles
                           where m.CompanyId == compid
                           select new { m.JobTitleId, m.JobTitle }).ToList();

            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };

            orderVM.JobTitles = new List<RegisterJobTitleViewModel>();
            foreach(var items in getjobs){
                var jobname = @items.JobTitle;
                var jobid = @items.JobTitleId;
                orderVM.JobTitles.Add(new RegisterJobTitleViewModel { Value = @jobid, JobTitleId = @jobid, JobTitle = @jobname});
            }

            return View(orderVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> RegisterSOPUser(RegisterSOPUserViewModel model, [Bind("ClaimId,FirstName,SecondName,CompanyId")] ApplicationDbContext.ClaimComp compclaim)
        {
            var currentuser = await _userManager.GetUserAsync(User);
            var user_id = currentuser.Id;

            var modules = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            var orderVM = new RegisterSOPUserViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            var getjobs = (from m in _context.JobTitles
                           where m.CompanyId == compid
                           select new { m.JobTitleId, m.JobTitle }).ToList();

            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };

            orderVM.JobTitles = new List<RegisterJobTitleViewModel>();
            foreach (var items in getjobs)
            {
                var jobname = @items.JobTitle;
                var jobid = @items.JobTitleId;
                orderVM.JobTitles.Add(new RegisterJobTitleViewModel { Value = @jobid, JobTitleId = @jobid, JobTitle = @jobname });
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SOPUser");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    _context.CompanyClaim.Select(u => u.UserId);
                    var newuserid = user.Id;
                    compclaim.UserId = newuserid;

                    _context.CompanyClaim.Select(u => u.DepartmentId);
                    var selected = Request.Form["DepartmentId"];
                    compclaim.DepartmentId = selected;

                    _context.CompanyClaim.Select(u => u.JobTitleId);
                    var seljob = Request.Form["JobTitleId"];
                    compclaim.JobTitleId = seljob;                  

                    _context.CompanyClaim.Select(u => u.CompanyId);
                    compclaim.CompanyId = modules;

                    _context.Add(compclaim);

                    _logger.LogInformation("User created a new account with password.");
                    await _context.SaveChangesAsync();
                    AddErrors(result);
                    return RedirectToAction("People", "Setup");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterDepartment()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> RegisterDepartment([Bind("DepartmentName,CompanyId")] ApplicationDbContext.DepartmentTable departments)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;

            var modules = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            if (ModelState.IsValid)
            {
                _context.Departments.Select(u => u.CompanyId);
                departments.CompanyId = modules; 

                _context.Add(departments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(RegisterDepartment));
            }
            // If we got this far, something failed, redisplay form
            return View(departments);

        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterJobTitle()
        {
            var orderVM = new RegisterJobTitleViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };

            return View(orderVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> RegisterJobTitle(RegisterJobTitleViewModel regjob, [Bind("JobTitleId,JobTitle,DepartmentId,CompanyId")] ApplicationDbContext.JobTitlesTable jobtitle)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;
            var getcomp = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();
            var orderVM = new RegisterJobTitleViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getdep = (from m in _context.Departments
                          where m.CompanyId == compid
                          select new { m.DepartmentId, m.DepartmentName }).ToList();

            orderVM.Departments = new List<RegisterDepartmentViewModel>();
            foreach (var items in getdep)
            {
                var itemname = @items.DepartmentName;
                var itemId = @items.DepartmentId;
                orderVM.Departments.Add(new RegisterDepartmentViewModel { Value = @itemId, DepartmentId = @itemId, DepartmentName = @itemname });
            };


            if (ModelState.IsValid)
            {
                _context.JobTitles.Select(u => u.CompanyId);
                jobtitle.CompanyId = getcomp;

                _context.JobTitles.Select(u => u.DepartmentId);
                var selected = Request.Form["DepartmentId"];
                jobtitle.DepartmentId = selected;

                _context.JobTitles.Select(u => u.JobTitle);
                var selectname = Request.Form["JobTitle"];
                jobtitle.JobTitle = selectname;

                _context.Add(jobtitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(RegisterJobTitle));
            }

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterCompanyInfo(string returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUserRole()
        {
            return View();
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
