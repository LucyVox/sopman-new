using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sopman.Models;
using sopman.Data;
using sopman.Models.ManageViewModels;
using sopman.Models.SetupViewModels;
using sopman.Models.AccountViewModels;
using sopman.Services;

namespace sopman.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHostingEnvironment _hostingEnvironment;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
            UrlEncoder urlEncoder,
            IHostingEnvironment hostingEnvironment,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _hostingEnvironment = hostingEnvironment;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public PartialViewResult ProjectTable(string sortOrder)
        {
            var getu = _userManager.GetUserId(User);

            ViewBag.loggedinuser = getu;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();


            var theprojects = (from x in _context.Projects
                               where x.CompId == comp
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
            


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var theprojectslist = (from x in _context.Projects
                                   where x.CompId == comp
                                   select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName });

            switch (sortOrder)
            {
                case "name_desc":
                    theprojectslist = theprojectslist.OrderByDescending(x => x.ProjectName);
                    break;
                default:
                    theprojectslist = theprojectslist.OrderBy(x => x.ProjectName);
                    break;
            }

            ViewBag.theprojects = theprojectslist.ToList();

            return PartialView("projectTable");
        }


        [HttpGet]
        public ActionResult Index(string sortOrder)
        {
            var getu = _userManager.GetUserId(User);

            Console.WriteLine("User:");
            Console.WriteLine(getu);
            ViewBag.loggedinuser = getu;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == comp
                       select i.TopTempId).Single();

            Console.WriteLine("top:");
            Console.WriteLine(top);

            var claimid = (from t in _context.CompanyClaim
                           where t.UserId == getu
                           select t.ClaimId).Single();
            ViewBag.claimid = claimid;

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate }).ToList();

            ViewBag.newtemps = newtemps;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId  }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();

            ViewBag.User = top;
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;

            var theprojects = (from x in _context.Projects
                              where x.CompId == comp
                              select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();



            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var theprojectslist = (from x in _context.Projects
                                   where x.CompId == comp
                                   select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName });

            switch (sortOrder)
            {
                case "name_desc":
                    theprojectslist = theprojectslist.OrderByDescending(x => x.ProjectName);
                    break;
                default:
                    theprojectslist = theprojectslist.OrderBy(x => x.ProjectName);
                    break;
            }

            ViewBag.theprojects = theprojectslist.ToList();

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid, RACIResChosenID = u.RACIResChosenID }).ToList();
            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();

            var procdate = (from p in _context.SOPInstanceProcesses
                            select new SOPOverView { DueDate = p.DueDate, valuematch = p.valuematch, SOPTemplateID = p.SOPTemplateID }).ToList();

            var process = (from t in _context.SOPProcessTempls
                           select new SOPOverView { ProcessName = t.ProcessName, valuematch = t.valuematch }).ToList();

            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;
            ViewBag.procdate = procdate;
            ViewBag.process = process;
            if (getu == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var rescomp = (from i in _context.RACIResComplete
                           select new SOPOverView { RACIResChosenID = i.RACIResChosenID, InstanceID = i.InstanceID, StatusComplete = i.StatusComplete }).ToList();

            var resres = (from i in _context.RACIResRecusal
                          select new SOPOverView { RACIResChosenID = i.RACIResChosenID, InstanceID = i.InstanceID, StatusRecusal = i.StatusRecusal }).ToList();

            var acccomp = (from i in _context.RACIAccComplete
                           select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, InstanceID = i.InstanceID, StatusComplete = i.StatusComplete }).ToList();

            var accres = (from i in _context.RACIAccRecusal
                          select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, InstanceID = i.InstanceID, StatusRecusal = i.StatusRecusal }).ToList();

            var concomp = (from i in _context.RACIConComplete
                           select new SOPOverView { RACIConChosenID = i.RACIConChosenID, InstanceID = i.InstanceID, StatusComplete = i.StatusComplete }).ToList();

            var conres = (from i in _context.RACIConRecusal
                          select new SOPOverView { RACIConChosenID = i.RACIConChosenID, InstanceID = i.InstanceID, StatusRecusal = i.StatusRecusal }).ToList();
            var infcomp = (from i in _context.RACIInfComplete
                           select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, InstanceID = i.InstanceID, StatusComplete = i.StatusComplete }).ToList();

            var infres = (from i in _context.RACIInfRecusal
                          select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, InstanceID = i.InstanceID, StatusRecusal = i.StatusRecusal }).ToList();


            ViewBag.rescomp = rescomp;
            ViewBag.resres = resres;
            ViewBag.acccomp = acccomp;
            ViewBag.accres = accres;
            ViewBag.concomp = concomp;
            ViewBag.conres = conres;
            ViewBag.infcomp = infcomp;
            ViewBag.infres = infres;

            return View();
        }

        [HttpGet]
        public ActionResult Tasks()
        {
            var getu = _userManager.GetUserId(User);

            Console.WriteLine("User:");
            Console.WriteLine(getu);
            ViewBag.loggedinuser = getu;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == comp
                       select i.TopTempId).Single();

            Console.WriteLine("top:");
            Console.WriteLine(top);

            var claimid = (from t in _context.CompanyClaim
                           where t.UserId == getu
                           select t.ClaimId).Single();
            ViewBag.claimid = claimid;

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate }).ToList();

            ViewBag.newtemps = newtemps;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;
         
            var processtmps = (from i in _context.SOPProcessTempls
                               select new SOPOverView { SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();

            ViewBag.processtmps = processtmps;

            ViewBag.User = top;
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;

            var theprojects = (from x in _context.Projects
                               where x.CompId == comp
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();

            ViewBag.theprojects = theprojects;

            var getprocess = (from x in _context.SOPInstanceProcesses
                              select new ProcessOutput { DueDate = x.DueDate, valuematch = x.valuematch, SOPTemplateID = x.SOPTemplateID }).ToList();

            ViewBag.getprocess = getprocess;

            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, InstanceId = i.InstanceId }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, InstanceId = i.InstanceId }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, InstanceId = i.InstanceId }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, InstanceId = i.InstanceId, }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();


            if (getu == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult People(List<RegisterSOPUserViewModel> list)
        {
            var orderVM = new RegisterSOPUserViewModel();
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            ViewBag.compid = compid;
            var getpeople = (from m in _context.CompanyClaim
                             where m.CompanyId == compid
                             select new RegisterSOPUserViewModel { FirstName = m.FirstName, SecondName = m.SecondName, UserId = m.UserId }).ToList();

            var claimemails = (from m in _context.CompanyClaim
                               where m.CompanyId == compid
                               select m.UserId).ToList();

            foreach (var item in claimemails)
            {
                var theusers = (from m in _context.Users
                                where m.Id == item
                                select new RegisterSOPUserViewModel { Email = m.Email, UserId = m.Id }).ToList();
            }

            var getthepeople = (from m in _context.CompanyClaim
                                where m.CompanyId == compid
                                select new RegisterSOPUserViewModel { UserId = m.UserId, CompanyId = m.CompanyId, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentId = m.DepartmentId, JobTitleId = m.JobTitleId }).ToList();

            ViewBag.getthepeople = getthepeople;

            var userinfo = (from i in _context.Users
                            select new RegisterSOPUserViewModel { Email = i.Email, Id = i.Id }).ToList();
            ViewBag.userinfo = userinfo;

            var deppartments = (from y in _context.Departments
                                select new RegisterSOPUserViewModel { DepartmentId = y.DepartmentId, DepartmentName = y.DepartmentName }).ToList();
            ViewBag.deppartments = deppartments;

            var jobtitle = (from x in _context.JobTitles
                            select new RegisterSOPUserViewModel { CompanyId = x.CompanyId, JobTitleId = x.JobTitleId, JobTitle = x.JobTitle }).ToList();
            ViewBag.jobtitle = jobtitle;

            var thepeople = (from m in _context.CompanyClaim
                             join p in _context.Users on m.UserId equals p.Id
                             join b in _context.CompanyClaim on m.CompanyId equals compid
                             join d in _context.Departments on m.DepartmentId equals d.DepartmentId
                             join j in _context.JobTitles on m.JobTitleId equals j.JobTitleId
                             where p.Id == m.UserId
                             select new RegisterSOPUserViewModel { CompanyId = m.CompanyId, UserId = p.Id, Email = p.Email, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentName = d.DepartmentName, JobTitle = j.JobTitle }).ToList();

            return View(thepeople);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public ActionResult People()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SOPInstances()
        {
            var getu = _userManager.GetUserId(User);

            Console.WriteLine("User:");
            Console.WriteLine(getu);
            ViewBag.loggedinuser = getu;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == comp
                       select i.TopTempId).Single();

            Console.WriteLine("top:");
            Console.WriteLine(top);

            var claimid = (from t in _context.CompanyClaim
                           where t.UserId == getu
                           select t.ClaimId).Single();
            ViewBag.claimid = claimid;

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate }).ToList();

            ViewBag.newtemps = newtemps;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();

            ViewBag.User = top;
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;

            var theprojects = (from x in _context.Projects
                               where x.CompId == comp
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();

            ViewBag.theprojects = theprojects;

            return View();
        }

        [HttpGet]
        public ActionResult SOPTemplates()
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var top = (from y in _context.SOPTopTemplates
                       where y.CompanyId == comp
                       select y.TopTempId).Single();

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate }).ToList();

            return View(newtemps);
        }

        [HttpGet]
        public PartialViewResult GetProjects()
        {
            var getuser = _userManager.GetUserId(User);

            var ordervm = new CreateInstanceViewModel();
            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var getprojects = (from i in _context.Projects
                               where i.CompId == compid
                               select new { i.ProjectName, i.ProjectId }).ToList();

            ordervm.Projects = new List<ProjectsList>();
            if(getprojects != null){
                foreach (var item in getprojects)
                {
                    var itemname = item.ProjectName;
                    var itemval = item.ProjectId;
                    ordervm.Projects.Add(new ProjectsList { Value = @itemval, ProjectName = @itemname, ProjectId = @itemval });
                }   
            }

            return PartialView("_GetProjects", ordervm);
        }

        [HttpGet]
        public ActionResult CreateInstance(string SOPTemplateID)
        {

            if (SOPTemplateID== null)
            {
                return NotFound();
            }
            var getuser = _userManager.GetUserId(User);

            var sopid = SOPTemplateID;

            ViewBag.theid = sopid;

            var createname = (from y in _context.SOPNewTemplate
                              where y.SOPTemplateID == SOPTemplateID
                              select y.TempName).Single();

            var sopcode = (from y in _context.SOPNewTemplate
                              where y.SOPTemplateID == SOPTemplateID
                           select y.SOPCode).Single();
            
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();

            var projects = (from y in _context.Projects
                            where y.CompId == comp
                            select new CreateInstanceViewModel{ ProjectName = y.ProjectName, ProjectId = y.ProjectId}).ToList();

            ViewBag.projects = projects;
            ViewBag.createname = createname;
            ViewBag.selectlist = new SelectList(projects, "ProjectId", "ProjectName");

            var orderdm = new CreateInstanceViewModel();

            CreateInstanceViewModel create = new CreateInstanceViewModel(){
                TempName = createname,
                SOPCode = sopcode, 
            };

            return View(create);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInstance(string SOPTemplateID, [Bind("SOPTemplateID,InstanceExpire,ProjectId,InstanceRef")]ApplicationDbContext.SOPNewInstance inst, [Bind("ProjectId,CompId,ProjectName")] ApplicationDbContext.Project project)
        {
            var getuser = _userManager.GetUserId(User);

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            
            var projects = (from y in _context.Projects
                            where y.CompId == comp
                            select new CreateInstanceViewModel { ProjectName = y.ProjectName, ProjectId = y.ProjectId }).ToList();
            ViewBag.selectlist = new SelectList(projects, "ProjectId", "ProjectName");
            if (ModelState.IsValid)
            {
                var newpro = Request.Form["ProjectName"];
                string valuestr = Request.Form["ProjectName"];

                var secval = Request.Form["SOPTemplateID"];

                inst.SOPTemplateID = SOPTemplateID;

                if (!String.IsNullOrEmpty(valuestr))
                {
                    project.ProjectName = valuestr;
                    project.CompId = comp;

                    Console.WriteLine(valuestr);
                    _context.Add(project);
                }
                if(String.IsNullOrEmpty(valuestr)) {
                    var preqproj2 = Request.Form["ProjectDropdown"];
                    Console.WriteLine("Project drop:");


                    Console.WriteLine(preqproj2);

                    var getprojs = (from y in _context.Projects
                                    where y.ProjectId == preqproj2
                                    select y.ProjectId).Single();
                    inst.ProjectId = getprojs;
                }
                _context.SaveChanges();

                if (!String.IsNullOrEmpty(valuestr))
                {
                    
                    var getprojs2 = (from y in _context.Projects
                                     where y.ProjectName == valuestr
                                    select y.ProjectId).Single();

                    inst.ProjectId = getprojs2;
                }
                inst.SOPTemplateID = secval;
                _context.Add(inst);
                _context.SaveChanges();


            }
            var getref = Request.Form["InstanceRef"];
            var getnewid = (from y in _context.NewInstance
                            where y.InstanceRef == getref
                            select y.InstanceId).Single();
            object getnewidtwo = (from y in _context.NewInstance
                               where y.InstanceRef == getref
                               select y.InstanceId).Single();

            string url1 = Url.Content("InstanceProcess" + Uri.EscapeUriString("?=") + getnewid);
            string newurl = url1.Replace("%3F%3D", "?=");

            Console.WriteLine(newurl);

            return new RedirectResult(newurl);
        }


        [HttpGet]
        public PartialViewResult TheProcesses(string SOPTemplateID)
        {

            ViewBag.theid = SOPTemplateID;

            var newinstance = (from i in _context.SOPNewTemplate
                               where i.SOPTemplateID == SOPTemplateID
                               select i.TempName).Single();

            var refnum = (from i in _context.NewInstance
                          where i.SOPTemplateID == SOPTemplateID
                          select i.InstanceRef).Single();

            var proid = (from i in _context.NewInstance
                         where i.SOPTemplateID == SOPTemplateID
                         select i.ProjectId).Single();

            var theproj = (from i in _context.Projects
                           where i.ProjectId == proid
                           select i.ProjectName).Single();

            InstanceProcessViewModel inst = new InstanceProcessViewModel()
            {
                TempName = newinstance,
                InstanceRef = refnum,
                ProjectName = theproj
            };
            return PartialView("_TheProcesses", inst);
        }

        [HttpGet]
        public PartialViewResult DueDate(string SOPTemplateID, string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;


            return PartialView("DueDate", new ProcessOutput());
        }

        [HttpGet]
        public PartialViewResult RACIInf(string SOPTemplateID, string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            var getuser = _userManager.GetUserId(User);
            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var model = new ProcessOutput();

            var temp = (from i in _context.SOPProcessTempls
                        join p in _context.SOPRACIRes on i.valuematch equals p.valuematch
                        join m in _context.Departments on p.DepartmentId equals m.DepartmentId
                        join j in _context.JobTitles on p.DepartmentId equals j.DepartmentId
                        select new ProcessOutput{ DepartmentId = p.DepartmentId, valuematch = i.valuematch, JobTitleId = p.JobTitleId, DepartmentName = m.DepartmentName, JobTitle = j.JobTitle }).ToList();

            ViewBag.tempdata = temp;


            return PartialView("ProcessRACIInfo", temp);
        }

        [HttpGet]
        public async Task<IActionResult> InstanceProcess(string InstanceId)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;

            Console.WriteLine("User:");
            Console.WriteLine(user_id);
            var getid = InstanceId;
            Console.WriteLine("InstanceId:");
            Console.WriteLine(getid);

            ViewBag.theid = getid;
            ViewBag.user = user_id;

            var getsop = (from y in _context.NewInstance
                          where y.InstanceId == getid
                          select y.SOPTemplateID).Single();

            ViewBag.getsop = getsop;
            Console.WriteLine("SOPId:");
            Console.WriteLine(getsop);

            var theuser = (from y in _context.CompanyClaim
                           where y.UserId == user_id
                           select y.CompanyId).Single();
            
            ViewBag.getsop = theuser;
            Console.WriteLine("CompId:");
            Console.WriteLine(theuser);

            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == getsop
                        select i.TempName).Single();
            
            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == getsop
                           select i.SOPCode).Single();
            
            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getid
                          select y.InstanceRef).Single();

            var projectId = (from y in _context.NewInstance
                             where y.InstanceId == getid
                             select y.ProjectId).Single();

            var projname = (from y in _context.Projects
                            where y.ProjectId == projectId
                            select y.ProjectName).Single();
            
            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;
            ViewBag.projname = projname;

            var getdeparts = (from d in _context.Departments
                              where d.CompanyId == theuser
                              select new ProcessOutput { DepartmentId = d.DepartmentId, DepartmentName = d.DepartmentName }).ToList();
            ViewBag.getdeparts = getdeparts;

            var getjobs = (from j in _context.JobTitles
                           where j.CompanyId == theuser
                           select new ProcessOutput { JobTitle = j.JobTitle, JobTitleId = j.JobTitleId }).ToList();

            ViewBag.getjobs = getjobs;
            var ordervm = new ProcessOutput();

            var process = (from i in _context.SOPProcessTempls
                           where i.SOPTemplateID == getsop
                           select new ProcessOutput { SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, ProcessType = i.ProcessType, valuematch = i.valuematch }).ToList();


            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { RACIResID = i.RACIResID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { RACIAccID = i.RACIAccID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon                       
                        select new ProcessOutput { RACIConID = i.RACIConID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.cons = cons;
             var infi = (from i in _context.SOPRACIInf
                         select new ProcessOutput {RACIInfID = i.RACIInfID,  SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();

            ViewBag.infi = infi;

            var userlist = (from i in _context.CompanyClaim
                            where i.CompanyId == theuser
                            select new ProcessOutput { ClaimId = i.ClaimId, FirstName = i.FirstName, SecondName = i.SecondName, JobTitleId = i.JobTitleId }).ToList();
            ViewBag.userlist = userlist;

            ViewBag.selectlist = new SelectList(userlist, "ClaimId", "FirstName");


            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> InstanceProcess(List<IFormFile> files, [Bind("RACIResChosenID,RACIResID,UserId")]ApplicationDbContext.RACIResChosenUser resp, string InstanceId, [Bind("DueDate,valuematch,ExternalDocument,SOPTemplateID")]ApplicationDbContext.SOPInstanceProcess pro, [Bind(Prefix="SectionInfo")]IEnumerable<ProcessOutput> secinf)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;

            Console.WriteLine("User:");
            Console.WriteLine(user_id);
            var getid = InstanceId;
            Console.WriteLine("InstanceId:");
            Console.WriteLine(getid);

            ViewBag.theid = getid;
            ViewBag.user = user_id;

            var getsop = (from y in _context.NewInstance
                          where y.InstanceId == getid
                          select y.SOPTemplateID).Single();

            ViewBag.getsop = getsop;
            Console.WriteLine("SOPId:");
            Console.WriteLine(getsop);

            var theuser = (from y in _context.CompanyClaim
                           where y.UserId == user_id
                           select y.CompanyId).Single();

            ViewBag.getsop = theuser;
            Console.WriteLine("CompId:");
            Console.WriteLine(theuser);

            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == getsop
                        select i.TempName).Single();

            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == getsop
                           select i.SOPCode).Single();

            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getid
                          select y.InstanceRef).Single();

            var projectId = (from y in _context.NewInstance
                             where y.InstanceId == getid
                             select y.ProjectId).Single();

            var projname = (from y in _context.Projects
                            where y.ProjectId == projectId
                            select y.ProjectName).Single();

            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;
            ViewBag.projname = projname;

          
            var getdeparts = (from d in _context.Departments
                              where d.CompanyId == theuser
                              select new ProcessOutput { DepartmentId = d.DepartmentId, DepartmentName = d.DepartmentName }).ToList();
            ViewBag.getdeparts = getdeparts;

            var getjobs = (from j in _context.JobTitles
                           where j.CompanyId == theuser
                           select new ProcessOutput { JobTitle = j.JobTitle, JobTitleId = j.JobTitleId }).ToList();

            ViewBag.getjobs = getjobs;
            var ordervm = new ProcessOutput();

            var process = (from i in _context.SOPProcessTempls
                           where i.SOPTemplateID == getsop
                           select new ProcessOutput { SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, ProcessType = i.ProcessType, valuematch = i.valuematch }).ToList();

            ViewBag.process = process;
            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { RACIResID = i.RACIResID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { RACIAccID = i.RACIAccID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { RACIConID = i.RACIConID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { RACIInfID = i.RACIInfID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();

            ViewBag.infi = infi;

            var userlist = (from i in _context.CompanyClaim
                            where i.CompanyId == theuser
                            select new ProcessOutput { ClaimId = i.ClaimId, FirstName = i.FirstName, SecondName = i.SecondName, JobTitleId = i.JobTitleId }).ToList();
            ViewBag.userlist = userlist;
            ViewBag.selectlist = new SelectList(userlist, "ClaimId", "FirstName");


            if (ModelState.IsValid)
            {
                Console.WriteLine(secinf.Count());
                foreach (var item in secinf)
                {
                    ApplicationDbContext.SOPInstanceProcess sopproc = new ApplicationDbContext.SOPInstanceProcess();

                    var date = item.DueDate;
                    var externdocs = item.ExternalDocument;
                    var valuematch = item.valuematch;

                    //this is the instance id NOT SOPTemplateID 
                    sopproc.SOPTemplateID = getid;
                    sopproc.DueDate = date;
                    sopproc.valuematch = valuematch;
                    sopproc.ExternalDocument = externdocs;

                    Console.WriteLine(date);
                    Console.WriteLine(externdocs);
                    Console.WriteLine(valuematch);

                    Console.WriteLine(files.Count());

                    _context.Add(sopproc);
                    _context.SaveChanges();

                    var dbprocess = _context.SOPInstanceProcesses.FirstOrDefault(x => x.valuematch == item.valuematch && x.SOPTemplateID == getid);
                    Console.WriteLine(dbprocess.SOPTemplateID);

                    dbprocess.DueDate = date;
                    dbprocess.ExternalDocument = externdocs;

                    _context.SOPInstanceProcesses.Update(dbprocess);
                    _context.SaveChanges();
                    foreach (var sub in (ViewBag.res))
                    {
                        if (sub.valuematch == item.valuematch)
                        {
                            int resedid = sub.RACIResID;
                            var getidofcon = Request.Form["resid-hidden" + resedid];

                            string changeid = resedid.ToString();

                            if (getidofcon == changeid)
                            {
                                var dbcon = _context.SOPRACIRes.FirstOrDefault(x => x.RACIResID == resedid);

                                var status = "Pending";
                                int resid = sub.RACIResID;
                                Console.WriteLine("Resid" + resid);
                                var sel = Request.Form[sub.valuematch + "-RES-" + resid];
                                Console.WriteLine("Value from Res dropdown");
                                Console.WriteLine(sel);
                                int onevalue = int.Parse(sel);

                                dbcon.Status = status;
                                dbcon.UserId = onevalue;
                                dbcon.InstanceId = getid;
                                _context.SOPRACIRes.Update(dbcon);
                                _context.SaveChanges();
                            }

                        }
                    }
                    foreach (var sub in (ViewBag.cons))
                    {
                        
                        if (sub.valuematch == item.valuematch)
                        {
                            int conedid = sub.RACIInfID;
                            var getidofcon = Request.Form["conid-hidden" + conedid];

                            string changeid = conedid.ToString();

                            if (getidofcon == changeid)
                            {
                                var dbconcon = _context.SOPRACICon.FirstOrDefault(x => x.RACIConID == conedid);
                                var statuscon = "Pending";
                                int conid = sub.RACIConID;

                                var selcon = Request.Form[sub.valuematch + "-CON-" + conid];
                                Console.WriteLine(selcon);
                                int onevalueacc = int.Parse(selcon);

                                dbconcon.Status = statuscon;
                                dbconcon.UserId = onevalueacc;
                                dbconcon.InstanceId = getid;
                                _context.SOPRACICon.Update(dbconcon);
                                _context.SaveChanges();
                            }

                        }
                    }
                    foreach (var sub in (ViewBag.acc))
                    {
                        if (sub.valuematch == item.valuematch)
                        {
                            int accid = sub.RACIAccID;
                            var getidofacc = Request.Form["accid-hidden" + accid];
                            Console.WriteLine("Value of Hidden");
                            Console.WriteLine(getidofacc);
                            Console.WriteLine(sub.RACIAccID);

                            string changeid = accid.ToString();

                            if(getidofacc == changeid){
                                var dbconacc = _context.SOPRACIAcc.FirstOrDefault(x => x.RACIAccID == accid);
                                var statusacc = "Pending";
                                var selacc = Request.Form[sub.valuematch + "-ACC-" + accid];
                                Console.WriteLine("Value from dropdown:");
                                Console.WriteLine(selacc);
                                int onevalueacc = int.Parse(selacc);

                                dbconacc.Status = statusacc;
                                dbconacc.UserId = onevalueacc;
                                dbconacc.InstanceId = getid;
                                _context.SOPRACIAcc.Update(dbconacc);
                                _context.SaveChanges();
                            }
                        }
                    }
                    foreach (var sub in (ViewBag.infi))
                    {
                        if (sub.valuematch == item.valuematch)
                        {
                            int infiID = sub.RACIInfID;
                            var getidofcon = Request.Form["infiid-hidden" + infiID];

                            string changeid = infiID.ToString();

                            if (getidofcon == changeid)
                            {
                                var dbconinf = _context.SOPRACIInf.FirstOrDefault(x => x.RACIInfID == infiID);
                                var statuscon = "Pending";
                                int infid = sub.RACIInfID;

                                var selcon = Request.Form[sub.valuematch + "-INF-" + infid];
                                Console.WriteLine(selcon);
                                int onevaluecon = int.Parse(selcon);

                                dbconinf.Status = statuscon;
                                dbconinf.UserId = onevaluecon;
                                dbconinf.InstanceId = getid;
                                _context.SOPRACIInf.Update(dbconinf);
                                _context.SaveChanges();
                            }
                        }
                    }
                    _context.SaveChanges();

                    if (files.Count() > 0)
                    {
                        var folderpath = Path.Combine(Directory.GetCurrentDirectory(),
                              "Uploads/" + getid + valuematch);
                        Console.WriteLine(folderpath);

                        Directory.CreateDirectory(folderpath);

                        foreach (var file in files)
                        {
                            using (FileStream stream = new FileStream(Path.Combine(folderpath, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                    _context.SaveChanges();
                }
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CHRISERROR");
                    Console.WriteLine(ex.Message);
                }

            }
            string url1 = Url.Content("TheSOP" + Uri.EscapeUriString("?=") + getid);
            string newurl = url1.Replace("%3F%3D", "?=");

            Console.WriteLine(newurl);

            return new RedirectResult(newurl);
        }

        [HttpGet]
        public PartialViewResult SOPOverview(string SOPTemplateID, string containerPrefix)
        {

            return PartialView("ProcessRACIInfo");
        }

        [HttpGet]
        public ActionResult Settings(int CompanyId)
        {

            var getuser = _userManager.GetUserId(User);

            var compid = CompanyId;
            ViewBag.compid = compid;
            var getcompiduser = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.CompanyId).Single();

            var getcompid = (from i in _context.TheCompanyInfo
                             where i.CompanyId == getcompiduser
                             select i.Name).Single();
            
            ViewBag.getcompid = getcompid;


            //Generabl Tab
            var selsopformat = (from y in _context.TheCompanyInfo
                                where y.CompanyId == getcompiduser
                                select y.SOPNumberFormat).Single();
            var sopstartnum = (from y in _context.TheCompanyInfo
                               where y.CompanyId == getcompiduser
                               select y.SOPStartNumber).Single();

            var filenamefirstpart = (from y in _context.TheCompanyInfo
                                     where y.CompanyId == getcompiduser
                                     select y.Logo).Single();
            
            var fileuserid = (from y in _context.TheCompanyInfo
                                     where y.CompanyId == getcompiduser
                              select y.UserId).Single();

            var filenames = getcompid + filenamefirstpart;

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),         
                "Uploads/CompanyLogos", filenames);
            
            ViewBag.path = path;
            ViewBag.filenamefirstpart = filenamefirstpart;
            ViewBag.fileuserid = fileuserid;


            // Users tab
            var getthepeople = (from m in _context.CompanyClaim
                                where m.CompanyId == getcompiduser
                                select new RegisterSOPUserViewModel { UserId = m.UserId, CompanyId = m.CompanyId, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentId = m.DepartmentId, JobTitleId = m.JobTitleId }).ToList();

            ViewBag.getthepeople = getthepeople;

            var userinfo = (from i in _context.Users
                            select new RegisterSOPUserViewModel { Email = i.Email, Id = i.Id }).ToList();
            ViewBag.userinfo = userinfo;

            var deppartments = (from y in _context.Departments
                                select new RegisterSOPUserViewModel { DepartmentId = y.DepartmentId, DepartmentName = y.DepartmentName }).ToList();
            ViewBag.deppartments = deppartments;

            var jobtitle = (from x in _context.JobTitles
                            select new RegisterSOPUserViewModel { CompanyId = x.CompanyId, JobTitleId = x.JobTitleId, JobTitle = x.JobTitle }).ToList();
            ViewBag.jobtitle = jobtitle;


            //SOP Template
            var allowcodelim = (from i in _context.SOPTopTemplates
                               where i.CompanyId == compid
                                select i.SOPAllowCodeLimit).Single();

            var codeprefix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == compid
                              select i.SOPCodePrefix).Single();

            var codesuffix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == compid
                              select i.SOPCodeSuffix).Single();

            var namelimit = (from i in _context.SOPTopTemplates
                              where i.CompanyId == compid
                             select i.SOPNameLimit).Single();

            SettingsViewModel savesettings = new SettingsViewModel()
            {
                SOPStartNumber = sopstartnum,
                SOPNumberFormat = selsopformat,
                SOPAllowCodeLimit = allowcodelim,
                SOPCodePrefix = codeprefix,
                SOPCodeSuffix = codesuffix,
                SOPNameLimit = namelimit
            };

            //Form Forward
            string companyid = "Settings" + Uri.EscapeUriString("?=") + ViewBag.compid;
            string newurl = companyid.Replace("%3F%3D", "?=");
            ViewBag.newurl = newurl;


            return View(savesettings);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(int CompanyId, IFormFile file, [Bind("SOPStartNumber,SOPNumberFormat,Logo")]ApplicationDbContext.CompanyInfo comp,[Bind("TopTempId,SOPNameLimit,SOPCodePrefix,SOPCodeSuffix,SOPAllowCodeLimit,ClientId,UserId")] ApplicationDbContext.SOPTopTemplate toptemp)
        {var getuser = _userManager.GetUserId(User);

            var compid = CompanyId;
            ViewBag.compid = compid;
            var getcompiduser = (from y in _context.CompanyClaim
                                 where y.UserId == getuser
                                 select y.CompanyId).Single();

            var getcompid = (from i in _context.TheCompanyInfo
                             where i.CompanyId == getcompiduser
                             select i.Name).Single();

            ViewBag.getcompid = getcompid;


            //Generabl Tab
            var selsopformat = (from y in _context.TheCompanyInfo
                                where y.CompanyId == getcompiduser
                                select y.SOPNumberFormat).Single();
            var sopstartnum = (from y in _context.TheCompanyInfo
                               where y.CompanyId == getcompiduser
                               select y.SOPStartNumber).Single();

            var filenamefirstpart = (from y in _context.TheCompanyInfo
                                     where y.CompanyId == getcompiduser
                                     select y.Logo).Single();

            var fileuserid = (from y in _context.TheCompanyInfo
                              where y.CompanyId == getcompiduser
                              select y.UserId).Single();

            var filenames = getcompid + filenamefirstpart;

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads/CompanyLogos", filenames);

            ViewBag.path = path;
            ViewBag.filenamefirstpart = filenamefirstpart;
            ViewBag.fileuserid = fileuserid;

            // Users tab
            var getthepeople = (from m in _context.CompanyClaim
                                where m.CompanyId == getcompiduser
                                select new RegisterSOPUserViewModel { UserId = m.UserId, CompanyId = m.CompanyId, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentId = m.DepartmentId, JobTitleId = m.JobTitleId }).ToList();

            ViewBag.getthepeople = getthepeople;

            var userinfo = (from i in _context.Users
                            select new RegisterSOPUserViewModel { Email = i.Email, Id = i.Id }).ToList();
            ViewBag.userinfo = userinfo;

            var deppartments = (from y in _context.Departments
                                select new RegisterSOPUserViewModel { DepartmentId = y.DepartmentId, DepartmentName = y.DepartmentName }).ToList();
            ViewBag.deppartments = deppartments;

            var jobtitle = (from x in _context.JobTitles
                            select new RegisterSOPUserViewModel { CompanyId = x.CompanyId, JobTitleId = x.JobTitleId, JobTitle = x.JobTitle }).ToList();
            ViewBag.jobtitle = jobtitle;

            SettingsViewModel savesettings = new SettingsViewModel()
            {
                SOPStartNumber = sopstartnum,
                SOPNumberFormat = selsopformat,
            };


            var getthecompid = _context.TheCompanyInfo.FirstOrDefault(x => x.CompanyId == CompanyId);
            var getthesettingsid = _context.SOPTopTemplates.FirstOrDefault(x => x.CompanyId == CompanyId);

            Console.WriteLine(getthecompid);
            var getsopnum = Request.Form["SOPStartNumber"];
            var getdigits = Request.Form["SOPNumberFormat"];
            var getsuffix = Request.Form["SOPCodeSuffix"];
            var getprefix = Request.Form["SOPCodePrefix"];
            var getcharlimit = Request.Form["SOPNameLimit"];
            var userlimit = Request.Form["SOPAllowCodeLimit"];

            int charint = int.Parse(getcharlimit);

            Console.WriteLine(getsopnum);

            //SOP Template
            var allowcodelim = (from i in _context.SOPTopTemplates
                                where i.CompanyId == compid
                                select i.SOPAllowCodeLimit).Single();

            var codeprefix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == compid
                              select i.SOPCodePrefix).Single();

            var codesuffix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == compid
                              select i.SOPCodeSuffix).Single();

            var namelimit = (from i in _context.SOPTopTemplates
                             where i.CompanyId == compid
                             select i.SOPNameLimit).Single();
            
            if(ModelState.IsValid){

                // General
                if (file == null || file.Length == 0) {}else{
                    var logopath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads/CompanyLogos",
                        file.FileName);
                    Console.WriteLine(logopath);

                    using (var stream = new FileStream(logopath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var logopathform = file.FileName;
                    getthecompid.Logo = logopathform;
                }

                getthecompid.SOPStartNumber = getsopnum;
                getthecompid.SOPNumberFormat = getdigits;
                _context.TheCompanyInfo.Update(getthecompid);

                // SOP Template
                getthesettingsid.SOPNameLimit = charint;
                getthesettingsid.SOPCodeSuffix = getsuffix;
                getthesettingsid.SOPCodePrefix = getprefix;
                getthesettingsid.SOPAllowCodeLimit = userlimit;
                _context.SOPTopTemplates.Update(getthesettingsid);

                _context.SaveChanges();
            }

            string companyid = "Settings" + Uri.EscapeUriString("?=") + ViewBag.compid.ToString();
            string newurl = companyid.Replace("%3F%3D", "?=");

            ViewBag.newurl = newurl;

            return View(savesettings);
        }

        [HttpGet]
        public ActionResult TheSOP(string InstanceId)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;
            var getid = InstanceId;

            ViewBag.getid = getid;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            
            var SOPTemplateID = (from y in _context.NewInstance
                                 where y.InstanceId == getid
                                 select y.SOPTemplateID).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == SOPTemplateID
                        select i.TempName).Single();
            
            var code = (from i in _context.NewInstance
                        where i.InstanceId == getid
                        select i.InstanceRef).Single();

            var proid = (from i in _context.NewInstance
                         where i.InstanceId == getid
                         select i.ProjectId).Single();

            var theproj = (from i in _context.Projects
                           where i.ProjectId == proid
                           select i.ProjectName).Single();

            ViewBag.temp = temp;
            ViewBag.code = code;
            ViewBag.proj = theproj;

            var process = (from i in _context.SOPProcessTempls
                           where i.SOPTemplateID == SOPTemplateID
                           select new SOPOverView { }).ToList();
            
            ViewBag.instId = SOPTemplateID;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();
            
            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResID = u.RACIResID, UserId = u.UserId , soptoptempid = u.soptoptempid}).ToList();
            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccID = u.RACIAccID, UserId = u.UserId , soptoptempid = u.soptoptempid }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConID = u.RACIConID, UserId = u.UserId , soptoptempid = u.soptoptempid}).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfID = u.RACIInfID, UserId = u.UserId , soptoptempid = u.soptoptempid}).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            
            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;
            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == SOPTemplateID
                               select new SOPOverView { SOPTemplateID = i.SOPTemplateID , ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();
            ViewBag.processtmps = processtmps;


            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId , Status = i.Status}).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var getinstprocess = (from i in _context.SOPInstanceProcesses
                                  select new ProcessOutput { valuematch = i.valuematch, SOPTemplateID = i.SOPTemplateID, DueDate = i.DueDate, ExternalDocument = i.ExternalDocument }).ToList();

            var thetempid = SOPTemplateID;

            List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();

            ViewBag.getinstprocess = getinstprocess;
            foreach (var item in processtmps)
            {
                if(item.SOPTemplateID == thetempid){


                    ViewBag.Date = item.DueDate;
                    ViewBag.externDoc = item.ExternalDocument;


                    var firstid = InstanceId;
                    var secondid = item.valuematch;
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Uploads/" + firstid + secondid);

                    if(Directory.Exists(path)){
                        ProcessOutput.aList filesList = new ProcessOutput.aList();
                        filesList.ProcessName = item.ProcessName;

                        filesList.ProcessFiles = new List<string>();
                        foreach (string file in Directory.EnumerateFiles(path, "*"))
                        {
                            filesList.ProcessFiles.Add(file);
                        }

                        anotherlist.Add(filesList);
                    }
                }
            }
            ViewBag.files = anotherlist;
            return View();
        }

        [HttpPost]
        public ActionResult TheSOP(string InstanceId,[Bind("SectionId,ExecuteSopID,UserId")]ApplicationDbContext.ExecuteSop execute)
        {
            var getuser = _userManager.GetUserId(User);
            var getid = InstanceId;

            ViewBag.getid = getid;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var SOPTemplateID = (from y in _context.NewInstance
                                 where y.InstanceId == getid
                                 select y.SOPTemplateID).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == SOPTemplateID
                        select i.TempName).Single();

            var code = (from i in _context.NewInstance
                        where i.InstanceId == getid
                        select i.InstanceRef).Single();

            var proid = (from i in _context.NewInstance
                         where i.InstanceId == getid
                         select i.ProjectId).Single();

            var theproj = (from i in _context.Projects
                           where i.ProjectId == proid
                           select i.ProjectName).Single();

            ViewBag.temp = temp;
            ViewBag.code = code;
            ViewBag.proj = theproj;

            var process = (from i in _context.SOPProcessTempls
                           where i.SOPTemplateID == SOPTemplateID
                           select new SOPOverView { }).ToList();

            ViewBag.instId = SOPTemplateID;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch }).ToList();

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();
            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;
            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;
            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == SOPTemplateID
                               select new SOPOverView { ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();
            ViewBag.processtmps = processtmps;




            if (ModelState.IsValid)
            {

                execute.SectionId = getid;
                execute.UserId = getuser;
                _context.Add(execute);
                _context.SaveChanges();
            }
            var getexe = (from y in _context.ExecutedSop
                          where y.SectionId == getid
                          select y.ExecuteSopID).Single();

            string url1 = Url.Content("SOPs" + Uri.EscapeUriString("?=") + getexe);
            string newurl = url1.Replace("%3F%3D", "?=");
            Console.WriteLine(newurl);
            return new RedirectResult(newurl);
        }

        [HttpGet]
        public ActionResult EditSOP(string ExecuteSopID)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            var exeid = ExecuteSopID;
            ViewBag.exeid = exeid;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();


            var gettopid = (from y in _context.NewInstance
                            where y.InstanceId == getexe
                            select y.SOPTemplateID).Single();


            var instref = (from y in _context.NewInstance
                           where y.InstanceId == getexe
                           select y.InstanceRef).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var expire = (from w in _context.SOPNewTemplate
                          where w.SOPTemplateID == gettopid
                          select w.ExpireDate).SingleOrDefault();

            var projectId = (from p in _context.NewInstance
                             where p.InstanceId == getexe
                             select p.ProjectId).Single();

            var proj = (from p in _context.Projects
                        where p.ProjectId == projectId
                        select p.ProjectName).Single();

            ViewBag.expire = expire;
            ViewBag.instref = instref;
            ViewBag.toptemp = toptemp;
            ViewBag.getid = getexe;
            ViewBag.gettopid = gettopid;
            ViewBag.proj = proj;
            Console.WriteLine("TopID");
            Console.WriteLine(ViewBag.gettopid);
            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == gettopid
                        select i.TempName).Single();

            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == gettopid
                           select i.SOPCode).Single();

            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getexe
                          select y.InstanceRef).Single();

            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResChosenID = u.RACIResChosenID, RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccChosenID = u.RACIAccChosenID, RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConChosenID = u.RACIConChosenID, RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfChosenID = u.RACIInfChosenID, RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView {SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var ifrescomp = (from i in _context.RACIResComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifrescomp = ifrescomp;

            var ifresres = (from i in _context.RACIResRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifresres = ifresres;

            var ifacccomp = (from i in _context.RACIAccComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifacccomp = ifacccomp;

            var ifaccres = (from i in _context.RACIAccRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifaccres = ifaccres;

            var ifconcomp = (from i in _context.RACIConComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconcomp = ifconcomp;

            var ifconres = (from i in _context.RACIConRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconres = ifconres;

            var ifinfcomp = (from i in _context.RACIInfComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfcomp = ifinfcomp;

            var ifinfres = (from i in _context.RACIInfRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIInfChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfres = ifinfres;


            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var comments = (from i in _context.Comments
                            where i.ExecuteSopID == exeid
                            select new CommentsView { CommentId = i.CommentId, TheComment = i.TheComment, PostTime = i.PostTime }).ToList();

            ViewBag.comments = comments;

            List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();
            var thetempid = gettopid;
            foreach (var item in processtmps)
            {
                if (item.SOPTemplateID == thetempid)
                {
                    Console.WriteLine(item.SOPTemplateID + thetempid);
                    ViewBag.Date = item.DueDate;
                    ViewBag.externDoc = item.ExternalDocument;

                    var firstid = getexe;
                    var secondid = item.valuematch;
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Uploads/" + firstid + secondid);

                    if (Directory.Exists(path))
                    {
                        ProcessOutput.aList filesList = new ProcessOutput.aList();
                        filesList.ProcessName = item.ProcessName;

                        filesList.ProcessFiles = new List<string>();
                        foreach (string file in Directory.EnumerateFiles(path, "*"))
                        {
                            filesList.ProcessFiles.Add(file);
                        }

                        anotherlist.Add(filesList);
                    }

                }
            }
            ViewBag.files = anotherlist;
            return View();
        }

        [HttpPost]
        public ActionResult EditSOP(string ExecuteSopID, [Bind("ProcessDesc")]ApplicationDbContext.SOPTemplateProcesses sopprocess)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            var exeid = ExecuteSopID;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();


            var gettopid = (from y in _context.NewInstance
                            where y.InstanceId == getexe
                            select y.SOPTemplateID).Single();

            var instref = (from y in _context.NewInstance
                           where y.InstanceId == getexe
                           select y.InstanceRef).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var expire = (from w in _context.SOPNewTemplate
                          where w.SOPTemplateID == gettopid
                          select w.ExpireDate).SingleOrDefault();

            var projectId = (from p in _context.NewInstance
                             where p.InstanceId == getexe
                             select p.ProjectId).Single();

            var proj = (from p in _context.Projects
                        where p.ProjectId == projectId
                        select p.ProjectName).Single();

            ViewBag.expire = expire;
            ViewBag.instref = instref;
            ViewBag.toptemp = toptemp;
            ViewBag.getid = getexe;
            ViewBag.gettopid = gettopid;
            ViewBag.proj = proj;
            Console.WriteLine("TopID");
            Console.WriteLine(ViewBag.gettopid);
            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == gettopid
                        select i.TempName).Single();

            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == gettopid
                           select i.SOPCode).Single();

            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getexe
                          select y.InstanceRef).Single();

            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResChosenID = u.RACIResChosenID, RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccChosenID = u.RACIAccChosenID, RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConChosenID = u.RACIConChosenID, RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfChosenID = u.RACIInfChosenID, RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView {SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var ifrescomp = (from i in _context.RACIResComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifrescomp = ifrescomp;

            var ifresres = (from i in _context.RACIResRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifresres = ifresres;

            var ifacccomp = (from i in _context.RACIAccComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifacccomp = ifacccomp;

            var ifaccres = (from i in _context.RACIAccRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifaccres = ifaccres;

            var ifconcomp = (from i in _context.RACIConComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconcomp = ifconcomp;

            var ifconres = (from i in _context.RACIConRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconres = ifconres;

            var ifinfcomp = (from i in _context.RACIInfComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfcomp = ifinfcomp;

            var ifinfres = (from i in _context.RACIInfRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIInfChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfres = ifinfres;


            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            if (ModelState.IsValid)
            {
                List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();
                var thetempid = gettopid;
                foreach (var item in processtmps)
                {
                   

                    if (item.SOPTemplateID == thetempid)
                    {
                        ViewBag.Date = item.DueDate;
                        ViewBag.externDoc = item.ExternalDocument;

                        var firstid = getexe;
                        var secondid = item.valuematch;
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "Uploads/" + firstid + secondid);

                        if(Directory.Exists(path)){
                            ProcessOutput.aList filesList = new ProcessOutput.aList();
                            filesList.ProcessName = item.ProcessName;

                            filesList.ProcessFiles = new List<string>();
                            foreach (string file in Directory.EnumerateFiles(path, "*"))
                            {
                                filesList.ProcessFiles.Add(file);
                            }

                            anotherlist.Add(filesList);
                        }

                        int sopprocid = item.SOPTemplateProcessID;
                        Console.WriteLine("item SOPTemplateProcessID:" + sopprocid);
                        var hiddenname = Request.Form["hidden" + sopprocid]; 
                        Console.WriteLine("hidden value:" + hiddenname);
                        int hiddenint = int.Parse(hiddenname);

                        if(hiddenint == item.SOPTemplateProcessID){

                            int showid = item.SOPTemplateProcessID;
                            Console.WriteLine("show id:" + showid);

                            var getnameofdiv = item.valuematch;
                            var getdiv = Request.Form["desc-" + getnameofdiv];
                            Console.WriteLine("item valuematch:" + item.valuematch);
                            Console.WriteLine("New text:" + getdiv);

                            var dbprocesstemps = _context.SOPProcessTempls.FirstOrDefault(x => x.SOPTemplateProcessID == hiddenint);
                            dbprocesstemps.ProcessDesc = getdiv;
                            _context.SOPProcessTempls.Update(dbprocesstemps);
                            _context.SaveChanges();
                        }

                    }

                }
                ViewBag.files = anotherlist;
            }
            string url1 = Url.Content("SOPs" + Uri.EscapeUriString("?=") + ExecuteSopID);
            string newurl = url1.Replace("%3F%3D", "?=");

            return new RedirectResult(newurl);
        }

        [HttpGet]
        public ActionResult SOPs(string ExecuteSopID)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var getuserinfo = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                               select new SOPOverView{ FirstName = i.FirstName, SecondName = i.SecondName}).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var exeid = ExecuteSopID;
            ViewBag.exeid = exeid;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();


            var gettopid = (from y in _context.NewInstance
                            where y.InstanceId == getexe
                            select y.SOPTemplateID).Single();

            var instref = (from y in _context.NewInstance
                           where y.InstanceId == getexe
                           select y.InstanceRef).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var expire = (from w in _context.SOPNewTemplate
                          where w.SOPTemplateID == gettopid
                          select w.ExpireDate).SingleOrDefault();

            var projectId = (from p in _context.NewInstance
                             where p.InstanceId == getexe
                             select p.ProjectId).Single();

            var proj = (from p in _context.Projects
                        where p.ProjectId == projectId
                        select p.ProjectName).Single();

            ViewBag.expire = expire;
            ViewBag.instref = instref;
            ViewBag.toptemp = toptemp;
            ViewBag.getid = getexe;
            ViewBag.gettopid = gettopid;
            ViewBag.proj = proj;
            Console.WriteLine("TopID");
            Console.WriteLine(ViewBag.gettopid);
            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == gettopid
                        select i.TempName).Single();

            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == gettopid
                           select i.SOPCode).Single();

            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getexe
                          select y.InstanceRef).Single();

            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId,  }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView {RACIResChosenID = u.RACIResChosenID, RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status}).ToList();

            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView {RACIAccChosenID = u.RACIAccChosenID, RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConChosenID = u.RACIConChosenID, RACIConID = u.RACIConID, UserId = u.UserId , soptoptempid = u.soptoptempid, Status = u.Status}).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfChosenID = u.RACIInfChosenID, RACIInfID = u.RACIInfID, UserId = u.UserId , soptoptempid = u.soptoptempid, Status = u.Status}).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView {SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var ifrescomp = (from i in _context.RACIResComplete
                          where i.InstanceID == exeid
                             select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifrescomp = ifrescomp;

            var ifresres = (from i in _context.RACIResRecusal                             
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifresres = ifresres;

            var ifacccomp = (from i in _context.RACIAccComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifacccomp = ifacccomp;

            var ifaccres = (from i in _context.RACIAccRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifaccres = ifaccres;

            var ifconcomp = (from i in _context.RACIConComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconcomp = ifconcomp;

            var ifconres = (from i in _context.RACIConRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconres = ifconres;

            var ifinfcomp = (from i in _context.RACIInfComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfcomp = ifinfcomp;

            var ifinfres = (from i in _context.RACIInfRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIInfChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfres = ifinfres;


            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var comments = (from i in _context.Comments
                            where i.ExecuteSopID == exeid
                            select new CommentsView { CommentId = i.CommentId, TheComment = i.TheComment, PostTime = i.PostTime , UserId = i.UserId}).ToList();

            ViewBag.comments = comments;

            List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();
            var thetempid = gettopid;
            foreach (var item in processtmps)
            {
                if (item.SOPTemplateID == thetempid)
                {
                    Console.WriteLine(item.SOPTemplateID + thetempid);
                    ViewBag.Date = item.DueDate;
                    ViewBag.externDoc = item.ExternalDocument;

                    var firstid = getexe;
                    var secondid = item.valuematch;
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Uploads/" + firstid + secondid);

                    if (Directory.Exists(path))
                    {
                        ProcessOutput.aList filesList = new ProcessOutput.aList();
                        filesList.ProcessName = item.ProcessName;

                        filesList.ProcessFiles = new List<string>();
                        foreach (string file in Directory.EnumerateFiles(path, "*"))
                        {
                            filesList.ProcessFiles.Add(file);
                        }

                        anotherlist.Add(filesList);
                    }

                }
            }
            ViewBag.files = anotherlist;

            return View();
        }



        [HttpPost]
        public ActionResult SOPs(string ExecuteSopID, [Bind("SectionId,ExecuteSopID,UserId")]ApplicationDbContext.ExecuteSop exe)
        {
            var getuser = _userManager.GetUserId(User);

            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            var exeid = ExecuteSopID;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();

            var gettopid = (from y in _context.NewInstance
                            where y.InstanceId == getexe
                            select y.SOPTemplateID).Single();
            
            var instref = (from y in _context.NewInstance
                           where y.InstanceId == getexe
                           select y.InstanceRef).Single();

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var expire = (from w in _context.SOPNewTemplate
                          where w.SOPTemplateID == gettopid
                          select w.ExpireDate).SingleOrDefault();

            ViewBag.expire = expire;
            ViewBag.instref = instref;
            ViewBag.getid = getexe;
            ViewBag.gettopid = gettopid;
            Console.WriteLine("TopID");
            Console.WriteLine(ViewBag.gettopid);
            var temp = (from i in _context.SOPNewTemplate
                        where i.SOPTemplateID == gettopid
                        select i.TempName).Single();

            var thecode = (from i in _context.SOPNewTemplate
                           where i.SOPTemplateID == gettopid
                           select i.SOPCode).Single();

            var intref = (from y in _context.NewInstance
                          where y.InstanceId == getexe
                          select y.InstanceRef).Single();

            ViewBag.temp = temp;
            ViewBag.thecode = thecode;
            ViewBag.intref = intref;

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch }).ToList();

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var resiuser = (from u in _context.RACIResUser
                            select new SOPOverView { RACIResChosenID = u.RACIResChosenID, RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccChosenID = u.RACIAccChosenID, RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConChosenID = u.RACIConChosenID, RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfChosenID = u.RACIInfChosenID, RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid, Status = u.Status }).ToList();

            var allusers = (from u in _context.CompanyClaim
                            select new SOPOverView { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = resiuser;
            ViewBag.resiaccuser = resiaccuser;
            ViewBag.resiconuser = resiconuser;
            ViewBag.resiinfuser = resiinfuser;
            ViewBag.allusers = allusers;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView { ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var ifrescomp = (from i in _context.RACIResComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifrescomp = ifrescomp;

            var ifresres = (from i in _context.RACIResRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIResChosenID = i.RACIResChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifresres = ifresres;

            var ifacccomp = (from i in _context.RACIAccComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifacccomp = ifacccomp;

            var ifaccres = (from i in _context.RACIAccRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIAccChosenID = i.RACIAccChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifaccres = ifaccres;

            var ifconcomp = (from i in _context.RACIConComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconcomp = ifconcomp;

            var ifconres = (from i in _context.RACIConRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIConChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifconres = ifconres;

            var ifinfcomp = (from i in _context.RACIInfComplete
                             where i.InstanceID == exeid
                             select new SOPOverView { RACIInfChosenID = i.RACIInfChosenID, StatusComplete = i.StatusComplete, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfcomp = ifinfcomp;

            var ifinfres = (from i in _context.RACIInfRecusal
                            where i.InstanceID == exeid
                            select new SOPOverView { RACIConChosenID = i.RACIInfChosenID, StatusRecusal = i.StatusRecusal, InstanceID = i.InstanceID }).ToList();
            ViewBag.ifinfres = ifinfres;

            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var comments = (from i in _context.Comments
                            where i.ExecuteSopID == exeid
                            select new CommentsView { CommentId = i.CommentId, TheComment = i.TheComment, PostTime = i.PostTime , UserId = i.UserId}).ToList();

            ViewBag.comments = comments;


            if (ModelState.IsValid)
            {
                var newcomment = Request.Form["newcomment"];
                if (!String.IsNullOrEmpty(newcomment))
                {
                    ApplicationDbContext.Comment comment = new ApplicationDbContext.Comment();

                    comment.TheComment = newcomment;
                    comment.PostTime = DateTime.Now;
                    comment.ExecuteSopID = exeid;
                    comment.UserId = getuserid;

                    _context.Add(comment);
                    _context.SaveChanges();
                }


                foreach (var data in (ViewBag.processtmps))
                {
                    foreach (var item in res)
                    {
                        if (item.valuematch == data.valuematch)
                        {
                            var getvalue = (from i in _context.SOPRACIRes
                                            where i.valuematch == item.valuematch
                                            select i.valuematch).First();


                            foreach (var name in allusers)
                            {
                                if (item.UserId == name.ClaimId)
                                {
                                    var updateres = _context.SOPRACIRes.FirstOrDefault(x => x.soptoptempid == gettopid && x.valuematch == getvalue && x.UserId == item.UserId);
                                    var namenospace = "resp" + name.FirstName + name.SecondName + item.RACIResChosenID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updateres.Status = "Complete";
                                        updateres.editDate = DateTime.Now;
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateres.Status = "Recused";
                                        updateres.editDate = DateTime.Now;
                                    }
                                    _context.SOPRACIRes.Update(updateres);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    foreach (var item in acc)
                    {
                        if (item.valuematch == data.valuematch)
                        {
                            var getvalue = (from i in _context.SOPRACIAcc
                                            where i.valuematch == item.valuematch
                                            select i.valuematch).First();

                            foreach (var name in allusers)
                            {
                                if (item.UserId == name.ClaimId)
                                {
                                    var updateacc = _context.SOPRACIAcc.FirstOrDefault(x => x.soptoptempid == gettopid && x.valuematch == getvalue && x.UserId == item.UserId);
                                    var namenospace = "resp" + name.FirstName + name.SecondName + item.RACIAccChosenID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updateacc.Status = "Complete";
                                        updateacc.editDate = DateTime.Now;
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateacc.Status = "Recused";
                                        updateacc.editDate = DateTime.Now;
                                    }

                                    _context.SOPRACIAcc.Update(updateacc);
                                    _context.SaveChanges();
                                    Console.WriteLine(getname);
                                }
                            }

                        }
                    }
                    foreach (var item in cons)
                    {
                        if (item.valuematch == data.valuematch)
                        {
                            var getvalue = (from i in _context.SOPRACICon
                                            where i.valuematch == item.valuematch
                                            select i.valuematch).First();

                            foreach (var name in allusers)
                            {
                                if (item.UserId == name.ClaimId)
                                {
                                    var updatecon = _context.SOPRACICon.FirstOrDefault(x => x.soptoptempid == gettopid && x.valuematch == getvalue && x.UserId == item.UserId);
                                    var namenospace = "resp" + name.FirstName + name.SecondName + item.RACIConChosenID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updatecon.Status = "Complete";
                                        updatecon.editDate = DateTime.Now;
                                    }
                                    else if(!String.IsNullOrEmpty(getrecuse)){
                                        updatecon.Status = "Recused";
                                        updatecon.editDate = DateTime.Now;
                                    }

                                    _context.SOPRACICon.Update(updatecon);
                                    _context.SaveChanges();

                                    Console.WriteLine(getname);
                                }
                            }

                        }
                    }
                    foreach (var item in infi)
                    {
                        if (item.valuematch == data.valuematch)
                        {
                            var getvalue = (from i in _context.SOPRACIInf
                                            where i.valuematch == item.valuematch
                                            select i.valuematch).First();
                            foreach (var name in allusers)
                            {
                                if (item.UserId == name.ClaimId)
                                {
                                    var updateinf = _context.SOPRACIInf.FirstOrDefault(x => x.soptoptempid == gettopid && x.valuematch == getvalue && x.UserId == item.UserId);
                                    var namenospace = "resp" + name.FirstName + name.SecondName + item.RACIInfChosenID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updateinf.Status = "Complete";
                                        updateinf.editDate = DateTime.Now;
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateinf.Status = "Recused";
                                        updateinf.editDate = DateTime.Now;
                                    }
                                    _context.SOPRACIInf.Update(updateinf);
                                    _context.SaveChanges();

                                }
                            }

                        }
                    }
                }

            }
            string url1 = Url.Content("SOPs" + Uri.EscapeUriString("?=") + exeid);
            string newurl = url1.Replace("%3F%3D", "?=");
            return new RedirectResult(newurl);
        }


        [HttpGet]
        public ActionResult Projects()
        {
            var getuser = _userManager.GetUserId(User);

            ViewBag.Userid = getuser;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            
            var theprojects = (from x in _context.Projects
                               where x.CompId == compid
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();

            ViewBag.theprojects = theprojects;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();

            ViewBag.getexe = getexe;
            ViewBag.getinst = getinst;


            foreach(var proj in theprojects){
                var id = proj.ProjectId;

            }

            var projlist = (from i in _context.Projects
                            where i.CompId == compid
                            select new SOPTemplateList { 
                                    ProjectName = i.ProjectName,
                                    ProjectId = i.ProjectId,
                                    UserId = i.UserId,  
                                    creationdate = i.creationdate,
                                     countnum =  _context.NewInstance.Where(x => x.ProjectId == i.ProjectId).Count()    
                            }).ToList();


            foreach(var item in projlist){
                Console.WriteLine(item.ProjectName );
                Console.WriteLine(item.countnum);

               
            }


            return View(projlist);
        }

        [HttpGet]
        public ActionResult CreateNewProject()
        {



            return View();
        }

        [HttpPost]
        public ActionResult CreateNewProject([Bind("ProjectId,CompId,ProjectName,CreationDate,UserId")] ApplicationDbContext.Project project)
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();


            if (ModelState.IsValid)
            {
                project.CompId = compid;

                project.creationdate = DateTime.Now;
                project.UserId = getuser;
                _context.Add(project);
                _context.SaveChanges();
            }

            var pro = Request.Form["ProjectName"];
            var getprojid = (from i in _context.Projects
                             where i.ProjectName == pro
                             select i.ProjectId).Single();

            string url1 = Url.Content("ViewProject" + Uri.EscapeUriString("?=") + getprojid);
            string newurl = url1.Replace("%3F%3D", "?=");
            Console.WriteLine(newurl);
            return new RedirectResult(newurl);
        }

        [HttpGet]
        public ActionResult ViewProject(string ProjectId)
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            ViewBag.ProjectId = ProjectId;

            var projname = (from i in _context.Projects
                            where i.ProjectId == ProjectId
                            select i.ProjectName).Single();

            ViewBag.projname = projname;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();

            ViewBag.User = getuser;
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == compid
                       select i.TopTempId).Single();

            Console.WriteLine("top:");
            Console.WriteLine(top);

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate }).ToList();

            ViewBag.newtemps = newtemps;

            var thepeople = (from m in _context.CompanyClaim
                             join p in _context.Users on m.UserId equals p.Id
                             join d in _context.Departments on m.DepartmentId equals d.DepartmentId
                             join j in _context.JobTitles on m.JobTitleId equals j.JobTitleId
                             where p.Id == m.UserId
                             select new RegisterSOPUserViewModel { UserId = p.Id, Email = p.Email, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentName = d.DepartmentName, JobTitle = j.JobTitle }).ToList();



            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var firstName = (from i in _context.CompanyClaim
                             where i.UserId == id
                             select i.FirstName).Single();

            var SecondName = (from i in _context.CompanyClaim
                              where i.UserId == id
                              select i.SecondName).Single();

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == id
                        select i.CompanyId).Single();

            var department = (from i in _context.Departments
                              where i.CompanyId == comp
                              select i.DepartmentName).ToList();

            EditUser edituser = new EditUser()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = firstName,
                SecondName = SecondName,
            };

            return View(edituser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUser model, [Bind("FirstName,SecondName")]ApplicationDbContext.ClaimComp extrau)
        {
            if (ModelState.IsValid)
            {


                _context.SaveChanges();
            }
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

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("sopman"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
