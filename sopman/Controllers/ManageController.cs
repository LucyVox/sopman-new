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
using Microsoft.Extensions.Configuration;
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
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
namespace sopman.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                "sopman",
                "RCmOo1xCGu7FIx0wZ3H4wNK4Y0MNtcj5chzAMWlU2GQjC/ehnsiSD9MTuHFGCUDf2sPguMByyX7VrjlQpq4/FA=="), true);



        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
            UrlEncoder urlEncoder,
            IHostingEnvironment hostingEnvironment,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public PartialViewResult CompanyLogo()
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            Console.WriteLine("Logo:");
            Console.WriteLine(logostring);
            return PartialView("companyLogo", new CompanyLogo());
        }

        [HttpGet]
        public PartialViewResult ProjectTable(string sortOrder)
        {
            var getu = _userManager.GetUserId(User);

            ViewBag.loggedinuser = getu;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
            var getcompid = (from i in _context.CompanyClaim
                             where i.UserId == getu
                             select i.CompanyId).Single();
            var theprojects = (from x in _context.Projects
                               where x.CompId == comp
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
            var getuserid = (from i in _context.CompanyClaim
                             where i.UserId == getu
                             select i.ClaimId).Single();

            ViewBag.getuserid = getuserid;
            //Task Section
            var gettoptemp = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompid
                              select i.TopTempId).Single();
            var getsopnewsname = (from i in _context.SOPNewTemplate
                                  where i.TopTempId == gettoptemp
                                  select new TasksViewModel { TempName = i.TempName, SOPTemplateID = i.SOPTemplateID, }).ToList();

            ViewBag.getsopnewsname = getsopnewsname;

            var getinstances = (from i in _context.NewInstance
                                select new TasksViewModel { InstanceId = i.InstanceId, SOPTemplateID = i.SOPTemplateID, ProjectId = i.ProjectId, ExpireDate = i.InstanceExpire, InstanceRef = i.InstanceRef }).ToList();
            ViewBag.getinstances = getinstances;

            var processSteps = (from p in _context.SOPProcessTempls
                                select new TasksViewModel { SOPTemplateID = p.SOPTemplateID, ProcessName = p.ProcessName, valuematch = p.valuematch }).ToList();

            ViewBag.processSteps = processSteps;

            var pojects = (from o in _context.Projects
                           select new TasksViewModel { ProjectId = o.ProjectId, Projectname = o.ProjectName }).ToList();

            ViewBag.projects = pojects;

            var racirespro = (from p in _context.SOPRACIRes
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIResID = p.RACIResID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.racirespro = racirespro;

            var raciaccpro = (from p in _context.SOPRACIAcc
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIAccID = p.RACIAccID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciaccpro = raciaccpro;

            var raciconpro = (from p in _context.SOPRACICon
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIConID = p.RACIConID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciconpro = raciconpro;

            var raciinfpro = (from p in _context.SOPRACIInf
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIInfID = p.RACIInfID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciinfpro = raciinfpro;

            var exesop = (from p in _context.ExecutedSop
                          select new TasksViewModel { ExecuteSopID = p.ExecuteSopID, SectionId = p.SectionId }).ToList();

            ViewBag.exesop = exesop;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var theprojectslist = (from x in _context.Projects
                                   where x.CompId == comp
                                   select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName, UserId = x.UserId });
            ViewBag.theprojectslist = theprojectslist;
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
           
            var getuserid = (from i in _context.CompanyClaim
                             where i.UserId == getu
                             select i.ClaimId).Single();

            ViewBag.getuserid = getuserid;

            var getcompid = (from i in _context.CompanyClaim
                             where i.UserId == getu
                             select i.CompanyId).Single();
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == getcompid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var gettoptemp = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompid
                              select i.TopTempId).Single();


            var getsopnewsname = (from i in _context.SOPNewTemplate
                                  where i.TopTempId == gettoptemp
                                  select new TasksViewModel { TempName = i.TempName, SOPTemplateID = i.SOPTemplateID, }).ToList();

            ViewBag.getsopnewsname = getsopnewsname;

            var getinstances = (from i in _context.NewInstance
                                select new TasksViewModel { InstanceId = i.InstanceId, SOPTemplateID = i.SOPTemplateID, ProjectId = i.ProjectId, ExpireDate = i.InstanceExpire, InstanceRef = i.InstanceRef }).ToList();
            ViewBag.getinstances = getinstances;

            var processSteps = (from p in _context.SOPProcessTempls
                                select new TasksViewModel { SOPTemplateID = p.SOPTemplateID, ProcessName = p.ProcessName, valuematch = p.valuematch }).ToList();

            ViewBag.processSteps = processSteps;

            var pojects = (from o in _context.Projects
                           select new TasksViewModel { ProjectId = o.ProjectId, Projectname = o.ProjectName }).ToList();

            ViewBag.projects = pojects;

            var racirespro = (from p in _context.SOPRACIRes
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIResID = p.RACIResID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.racirespro = racirespro;

            var raciaccpro = (from p in _context.SOPRACIAcc
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIAccID = p.RACIAccID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciaccpro = raciaccpro;

            var raciconpro = (from p in _context.SOPRACICon
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIConID = p.RACIConID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciconpro = raciconpro;

            var raciinfpro = (from p in _context.SOPRACIInf
                              select new TasksViewModel { InstanceId = p.InstanceId, RACIInfID = p.RACIInfID, soptoptempid = p.soptoptempid, Status = p.Status, UserId = p.UserId, valuematch = p.valuematch }).ToList();
            ViewBag.raciinfpro = raciinfpro;

            var exesop = (from p in _context.ExecutedSop
                          select new TasksViewModel { ExecuteSopID = p.ExecuteSopID, SectionId = p.SectionId }).ToList();

            ViewBag.exesop = exesop;


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
        [HttpGet]
        public ActionResult ArchiveSOP(string SOPTemplateID)
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            return View();
        }

        [HttpPost]
        public ActionResult ArchiveSOP(string SOPTemplateID, [Bind("SOPTemplateId")]ApplicationDbContext.ArchivedSOP archive)
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            if (ModelState.IsValid)
            {
                archive.SOPTemplateId = SOPTemplateID;
                Console.WriteLine(archive.SOPTemplateId);
                _context.Add(archive);
                _context.SaveChanges();
            }
            string url1 = Url.Content("/Manage/SOPTemplates");
            return new RedirectResult(url1);
        }

        [HttpGet]
        public ActionResult ArchiveInstance(string ExecuteSopID)
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            return View();
        }

        [HttpPost]
        public ActionResult ArchiveInstance(string ExecuteSopID, [Bind("SOPTemplateId")]ApplicationDbContext.Archived archive)
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            if (ModelState.IsValid)
            {

                archive.InstancedId = ExecuteSopID;
                _context.Add(archive);
                _context.SaveChanges();
            }
            string url1 = Url.Content("/Manage/SOPInstances");
            return new RedirectResult(url1);
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

            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
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

            var getarchive = (from a in _context.ArchivedInstacned
                              select new SOPTemplateList { InstancedId = a.InstancedId }).ToList();
            ViewBag.getarchive = getarchive;

            var getinst = (from y in _context.NewInstance
                           select new SOPTemplateList { SOPTemplateID = y.SOPTemplateID, InstanceExpire = y.InstanceExpire, ProjectId = y.ProjectId, InstanceRef = y.InstanceRef, InstanceId = y.InstanceId }).ToList();

            var getexe = (from y in _context.ExecutedSop
                          select new SOPTemplateList { ExecuteSopID = y.ExecuteSopID, SectionId = y.SectionId, UserId = y.UserId }).ToList();

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

            ViewBag.User = top;
            ViewBag.getinst = getinst;
            ViewBag.getexe = getexe;

            var theprojects = (from x in _context.Projects
                               where x.CompId == comp
                               select new SOPTemplateList { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();

            ViewBag.theprojects = theprojects;

            double completeProcesses = 0;

            var processtmps = (from i in _context.SOPProcessTempls
                               select new SOPOverView { SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();

            ViewBag.processtmps = processtmps;
            foreach (var item in processtmps)
            {
                bool rescomplete = false, acccomplete = false, concomplete = false, infcomplete = false;
                foreach (var sub in acc)
                {
                    if (sub.valuematch == item.valuematch)
                    {

                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            acccomplete = true;
                            Console.WriteLine(sub.RACIAccID);
                        }
                    }
                }
                foreach (var sub in res)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            rescomplete = true;
                        }
                    }
                }
                foreach (var sub in cons)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            concomplete = true;
                        }
                    }
                }
                foreach (var sub in infi)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            infcomplete = true;
                        }
                    }
                }

                if (rescomplete && acccomplete && concomplete && infcomplete)
                {
                    completeProcesses++;
                    Console.WriteLine(completeProcesses);
                }
            }
            Console.WriteLine(processtmps.Count());
            double processcount = processtmps.Count();
            double percentageComplete = (completeProcesses / processcount) * 100;
            ViewBag.percentageComplete = Math.Round(percentageComplete, 2);
            Console.WriteLine(ViewBag.percentageComplete);

            return View();
        }

        [HttpGet]
        public ActionResult SOPTemplates()
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var top = (from y in _context.SOPTopTemplates
                       where y.CompanyId == comp
                       select y.TopTempId).Single();

            var newtemps = (from t in _context.SOPNewTemplate
                            where t.TopTempId == top
                            select new SOPTemplateList { SOPTemplateID = t.SOPTemplateID, TempName = t.TempName, SOPCode = t.SOPCode, ExpireDate = t.ExpireDate, LiveStatus = t.LiveStatus, TheCreateDae = t.TheCreateDae }).ToList();

            var getarchive = (from a in _context.ArchiveASOP
                              select new SOPTemplateList { SOPTemplateId = a.SOPTemplateId }).ToList();
            ViewBag.getarchive = getarchive;

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
            
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            ordervm.Projects = new List<ProjectsList>();
            if (getprojects != null)
            {
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

            if (SOPTemplateID == null)
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
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var projects = (from y in _context.Projects
                            where y.CompId == comp
                            select new CreateInstanceViewModel { ProjectName = y.ProjectName, ProjectId = y.ProjectId }).ToList();

            ViewBag.projects = projects;
            ViewBag.createname = createname;
            ViewBag.selectlist = new SelectList(projects, "ProjectId", "ProjectName");

            var orderdm = new CreateInstanceViewModel();

            CreateInstanceViewModel create = new CreateInstanceViewModel()
            {
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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
                if (String.IsNullOrEmpty(valuestr))
                {
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
                        select new ProcessOutput { DepartmentId = p.DepartmentId, valuematch = i.valuematch, JobTitleId = p.JobTitleId, DepartmentName = m.DepartmentName, JobTitle = j.JobTitle }).ToList();

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


            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
                        select new ProcessOutput { RACIInfID = i.RACIInfID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId }).ToList();

            ViewBag.infi = infi;

            var userlist = (from i in _context.CompanyClaim
                            where i.CompanyId == theuser
                            select new ProcessOutput { ClaimId = i.ClaimId, FirstName = i.FirstName, SecondName = i.SecondName, JobTitleId = i.JobTitleId }).ToList();
            ViewBag.userlist = userlist;

            ViewBag.selectlist = new SelectList(userlist, "ClaimId", "FirstName");


            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> InstanceProcess(List<IFormFile> files, [Bind("RACIResChosenID,RACIResID,UserId")]ApplicationDbContext.RACIResChosenUser resp, string InstanceId, [Bind("DueDate,valuematch,ExternalDocument,SOPTemplateID,InstanceId")]ApplicationDbContext.SOPInstanceProcess pro, [Bind(Prefix = "SectionInfo")]IEnumerable<ProcessOutput> secinf)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;

            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
                           select new ProcessOutput { SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, ProcessType = i.ProcessType, valuematch = i.valuematch }).ToList();

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
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(getid);
                await container.CreateIfNotExistsAsync();
                await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine(file.ContentDisposition);
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                        await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
                    }
                }
                _context.SaveChanges();

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
                    Console.WriteLine("Due date:");
                    Console.WriteLine(date);
                    Console.WriteLine("External Docs");
                    Console.WriteLine(externdocs);

                    Console.WriteLine("The value:");
                    Console.WriteLine(valuematch);

                    _context.Add(sopproc);
                    _context.SaveChanges();

                    var dbprocess = _context.SOPInstanceProcesses.FirstOrDefault(x => x.valuematch == item.valuematch && x.SOPTemplateID == getid);
                    Console.WriteLine(dbprocess.SOPTemplateID);
                    Console.WriteLine(dbprocess.DueDate);

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

                                if(dbcon.UserId > 1){
                                    ApplicationDbContext.RACIResposible respnew = new ApplicationDbContext.RACIResposible();
                                    var status = "Pending";
                                    int resid = sub.RACIResID;
                                    Console.WriteLine("Resid" + resid);
                                    var sel = Request.Form[sub.valuematch + "-RES-" + resid];
                                    Console.WriteLine("Value from Res dropdown");
                                    Console.WriteLine(sel);
                                    int onevalue = int.Parse(sel);

                                    respnew.DepartmentId = dbcon.DepartmentId;
                                    respnew.JobTitleId = dbcon.JobTitleId;
                                    respnew.valuematch = dbcon.valuematch;
                                    respnew.SOPTemplateProcessID = dbcon.SOPTemplateProcessID;
                                    respnew.Status = status;
                                    respnew.UserId = onevalue;
                                    respnew.InstanceId = InstanceId;
                                    _context.Add(respnew);
                                    _context.SaveChanges();
                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalue
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalue
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalue
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as a responsible user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
                                else {
                                    var status = "Pending";
                                    int resid = sub.RACIResID;
                                    Console.WriteLine("Resid" + resid);
                                    var sel = Request.Form[sub.valuematch + "-RES-" + resid];
                                    Console.WriteLine("Value from Res dropdown");
                                    Console.WriteLine(sel);
                                    int onevalue = int.Parse(sel);

                                    dbcon.Status = status;
                                    dbcon.UserId = onevalue;
                                    dbcon.InstanceId = InstanceId;
                                    _context.SOPRACIRes.Update(dbcon);
                                    _context.SaveChanges(); 
                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalue
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalue
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalue
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as a responsible user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
                            }

                        }
                    }
                    foreach (var sub in (ViewBag.cons))
                    {

                        if (sub.valuematch == item.valuematch)
                        {
                            int conedid = sub.RACIConID;
                            var getidofcon = Request.Form["conid-hidden" + conedid];

                            string changeid = conedid.ToString();
                            Console.WriteLine("Hidden value");
                            Console.WriteLine(changeid);

                            if (getidofcon == changeid)
                            {
                                var dbconcon = _context.SOPRACICon.FirstOrDefault(x => x.RACIConID == conedid);

                                if (dbconcon.UserId > 1)
                                {
                                    ApplicationDbContext.RACIConsulted respccon = new ApplicationDbContext.RACIConsulted();
                                    var statuscon = "Pending";
                                    int conid = sub.RACIConID;

                                    Console.WriteLine(dbconcon.valuematch);

                                    var selcon = Request.Form[sub.valuematch + "-CON-" + conid];
                                    Console.WriteLine("Dropdown value:");
                                    Console.WriteLine(selcon);
                                    int onevalueacc = int.Parse(selcon);

                                    respccon.DepartmentId = dbconcon.DepartmentId;
                                    respccon.JobTitleId = dbconcon.JobTitleId;
                                    respccon.valuematch = dbconcon.valuematch;
                                    respccon.SOPTemplateProcessID = dbconcon.SOPTemplateProcessID;
                                    respccon.Status = statuscon;
                                    respccon.UserId = onevalueacc;
                                    respccon.InstanceId = getid;
                                    _context.Add(respccon);
                                    _context.SaveChanges();

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalueacc
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalueacc
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalueacc
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as a Consulted user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
                                else
                                {
                                    var statuscon = "Pending";
                                    int conid = sub.RACIConID;

                                    Console.WriteLine(dbconcon.valuematch);

                                    var selcon = Request.Form[sub.valuematch + "-CON-" + conid];
                                    Console.WriteLine("Dropdown value:");
                                    Console.WriteLine(selcon);
                                    int onevalueacc = int.Parse(selcon);

                                    dbconcon.Status = statuscon;
                                    dbconcon.UserId = onevalueacc;
                                    dbconcon.InstanceId = getid;
                                    _context.SOPRACICon.Update(dbconcon);
                                    _context.SaveChanges();

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalueacc
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalueacc
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalueacc
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as a Consulted user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
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

                            if (getidofacc == changeid)
                            {
                                var dbconacc = _context.SOPRACIAcc.FirstOrDefault(x => x.RACIAccID == accid);

                                if (dbconacc.UserId > 1)
                                {
                                    ApplicationDbContext.RACIAccount respacc = new ApplicationDbContext.RACIAccount();
                                    var statusacc = "Pending";
                                    var selacc = Request.Form[sub.valuematch + "-ACC-" + accid];
                                    Console.WriteLine("Value from dropdown:");
                                    Console.WriteLine(selacc);
                                    int onevalueacc = int.Parse(selacc);

                                    respacc.DepartmentId = dbconacc.DepartmentId;
                                    respacc.JobTitleId = dbconacc.JobTitleId;
                                    respacc.valuematch = dbconacc.valuematch;
                                    respacc.SOPTemplateProcessID = dbconacc.SOPTemplateProcessID;
                                    respacc.Status = statusacc;
                                    respacc.UserId = onevalueacc;
                                    respacc.InstanceId = getid;
                                    _context.Add(respacc);
                                    _context.SaveChanges();

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalueacc
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalueacc
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalueacc
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as an Accountable user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
                                else {
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

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevalueacc
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevalueacc
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevalueacc
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as an Accountable user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
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

                                if (dbconinf.UserId > 1)
                                {
                                    ApplicationDbContext.RACIInformed resinf = new ApplicationDbContext.RACIInformed();
                                    var statuscon = "Pending";
                                    int infid = sub.RACIInfID;

                                    var selcon = Request.Form[sub.valuematch + "-INF-" + infid];
                                    Console.WriteLine(selcon);
                                    int onevaluecon = int.Parse(selcon);

                                    resinf.DepartmentId = dbconinf.DepartmentId;
                                    resinf.JobTitleId = dbconinf.JobTitleId;
                                    resinf.valuematch = dbconinf.valuematch;
                                    resinf.SOPTemplateProcessID = dbconinf.SOPTemplateProcessID;
                                    resinf.Status = statuscon;
                                    resinf.UserId = onevaluecon;
                                    resinf.InstanceId = getid;
                                    _context.Add(resinf);
                                    _context.SaveChanges();

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevaluecon
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevaluecon
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevaluecon
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as an Informed user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
                                else {
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

                                    // If we got this far, the process succeeded
                                    var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                                    Console.WriteLine(apiKey);

                                    var client = new SendGridClient(apiKey);

                                    var loggedInEmail = _userManager.GetUserName(User);
                                    var getuserclaimid = (from i in _context.CompanyClaim
                                                          where i.ClaimId == onevaluecon
                                                          select i.UserId).Single();
                                    var getuseremail = (from i in _context.Users
                                                        where i.Id == getuserclaimid
                                                        select i.Email).Single();
                                    var getfirstname = (from i in _context.CompanyClaim
                                                        where i.ClaimId == onevaluecon
                                                        select i.FirstName).Single();
                                    var getsecondname = (from i in _context.CompanyClaim
                                                         where i.ClaimId == onevaluecon
                                                         select i.SecondName).Single();
                                    var newUserEmail = getuseremail;
                                    var firstName = getfirstname;
                                    var secondName = getsecondname;


                                    var msg = new SendGridMessage()
                                    {
                                        From = new EmailAddress(loggedInEmail, "SOPMan"),
                                        Subject = "You have been added to " + intref,
                                        PlainTextContent = "Hello, " + firstName + " " + secondName,
                                        HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>You have been added as an Informed user to: " + intref,
                                    };
                                    Console.WriteLine(msg);
                                    msg.AddTo(new EmailAddress(getuseremail, firstName + " " + secondName));
                                    var response = await client.SendEmailAsync(msg);
                                    Console.WriteLine(response);
                                }
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
        public ActionResult Settings()
        {

            var getuser = _userManager.GetUserId(User);

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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

            var filenames = getcompid + "logo";

            var path = Path.Combine(
                                    "/images/", filenames);

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
                                where i.CompanyId == getcompiduser
                                select i.SOPAllowCodeLimit).Single();

            var codeprefix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompiduser
                              select i.SOPCodePrefix).Single();

            var codesuffix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompiduser
                              select i.SOPCodeSuffix).Single();

            var namelimit = (from i in _context.SOPTopTemplates
                             where i.CompanyId == getcompiduser
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
        public async Task<IActionResult> Settings(IFormFile file, [Bind("SOPStartNumber,SOPNumberFormat,Logo")]ApplicationDbContext.CompanyInfo comp, [Bind("TopTempId,SOPNameLimit,SOPCodePrefix,SOPCodeSuffix,SOPAllowCodeLimit,ClientId,UserId")] ApplicationDbContext.SOPTopTemplate toptemp)
        {
            var getuser = _userManager.GetUserId(User);

            var getcompiduser = (from y in _context.CompanyClaim
                                 where y.UserId == getuser
                                 select y.CompanyId).Single();

            var getcompid = (from i in _context.TheCompanyInfo
                             where i.CompanyId == getcompiduser
                             select i.Name).Single();

            ViewBag.getcompid = getcompid;

            var compid = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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

            var filenames = getcompid + "logo";

            var path = Path.Combine(
                "/images/", filenames);

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

            var getthecompid = _context.TheCompanyInfo.FirstOrDefault(x => x.CompanyId == getcompiduser);
            var getthesettingsid = _context.SOPTopTemplates.FirstOrDefault(x => x.CompanyId == getcompiduser);

            Console.WriteLine(getthecompid);
            var getsopnum = Request.Form["SOPStartNumber"];
            var getdigits = Request.Form["SOPNumberFormat"];
            var getsuffix = Request.Form["SOPCodeSuffix"];
            var getprefix = Request.Form["SOPCodePrefix"];
            var getcharlimit = Request.Form["SOPNameLimit"];
            var userlimit = Request.Form["SOPAllowCodeLimit"];

            int charint = 200;
            if (string.IsNullOrEmpty(getcharlimit))
            {

            }
            else
            {
                charint = int.Parse(getcharlimit);
            }


            Console.WriteLine(getsopnum);

            //SOP Template
            var allowcodelim = (from i in _context.SOPTopTemplates
                                where i.CompanyId == getcompiduser
                                select i.SOPAllowCodeLimit).Single();

            var codeprefix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompiduser
                              select i.SOPCodePrefix).Single();

            var codesuffix = (from i in _context.SOPTopTemplates
                              where i.CompanyId == getcompiduser
                              select i.SOPCodeSuffix).Single();

            var namelimit = (from i in _context.SOPTopTemplates
                             where i.CompanyId == getcompiduser
                             select i.SOPNameLimit).Single();

            if (ModelState.IsValid)
            {
               
                if(file == null){
                    
                }else{
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("logos");

                    await container.CreateIfNotExistsAsync();

                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                    var getname = Request.Form["Name"];
                    var getfile = Request.Form["file"];

                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(getname + "logo" + file.FileName);

                    await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

                    getthecompid.Logo = getname + "logo" + file.FileName;
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

            return View(savesettings);
        }


        [HttpGet]
        public async Task<IActionResult> TheSOP(string InstanceId)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;
            var getid = InstanceId;

            ViewBag.getid = getid;
            Console.WriteLine("InstanceId");
            Console.WriteLine(getid);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var SOPTemplateID = (from y in _context.NewInstance
                                 where y.InstanceId == getid
                                 select y.SOPTemplateID).Single();
            
            ViewBag.getinstanceid = SOPTemplateID;
            Console.WriteLine("SOPTemplateID");
            Console.WriteLine(SOPTemplateID);

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
                            select new SOPOverView { RACIResID = u.RACIResID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();
            var resiaccuser = (from u in _context.RACIAccUser
                               select new SOPOverView { RACIAccID = u.RACIAccID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiconuser = (from u in _context.RACIConUser
                               select new SOPOverView { RACIConID = u.RACIConID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

            var resiinfuser = (from u in _context.RACIInfUser
                               select new SOPOverView { RACIInfID = u.RACIInfID, UserId = u.UserId, soptoptempid = u.soptoptempid }).ToList();

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
                               select new SOPOverView { SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();
            ViewBag.processtmps = processtmps;


            var instancepro = (from i in _context.SOPInstanceProcesses
                               select new SOPOverView { SOPTemplateProcessID = i.SOPInstancesProcessId, SOPTemplateID = i.SOPTemplateID, valuematch = i.valuematch }).ToList();
            var res = (from i in _context.SOPRACIRes
                       select new ProcessOutput { InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput { InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput { InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var getinstprocess = (from i in _context.SOPInstanceProcesses
                                  select new ProcessOutput { valuematch = i.valuematch, SOPTemplateID = i.SOPTemplateID, DueDate = i.DueDate, ExternalDocument = i.ExternalDocument }).ToList();

            var thetempid = SOPTemplateID;

            List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();

            ViewBag.getinstprocess = getinstprocess;

            var gethistory = (from i in _context.EditSOP
                              where i.SOPID == SOPTemplateID
                              select new versionHistory { SOPID = i.SOPID, ValuematchEdited = i.ValuematchEdited, TextEdited = i.TextEdited, EditDate = i.EditDate, UserId = i.UserId }).ToList();

            ViewBag.gethistory = gethistory;

            foreach (var subitem in instancepro)
            {
                if (subitem.SOPTemplateID == InstanceId)
                {
                    var blobname = InstanceId;
                    Console.WriteLine("Blobname:");
                    Console.WriteLine(blobname);

                    string cloudStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sopman;AccountKey=RCmOo1xCGu7FIx0wZ3H4wNK4Y0MNtcj5chzAMWlU2GQjC/ehnsiSD9MTuHFGCUDf2sPguMByyX7VrjlQpq4/FA==;EndpointSuffix=core.windows.net";

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference(blobname);

                    //List blobs to the console window, with paging.
                    Console.WriteLine("List blobs in pages:");

                    int i = 0;
                    BlobContinuationToken continuationToken = null;
                    BlobResultSegment resultSegment = null;

                    ProcessOutput.aList filesList = new ProcessOutput.aList();

                    //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
                    //When the continuation token is null, the last page has been returned and execution can exit the loop.

                    do
                    {
                        //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                        //or by calling a different overload.
                        resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, null, continuationToken, null, null);
                        if (resultSegment.Results.Count<IListBlobItem>() > 0) { Console.WriteLine("Page {0}:", ++i); }
                        filesList.ProcessFiles = new List<string>();
                        foreach (var blobItem in resultSegment.Results)
                        {
                            var st = DateTime.UtcNow.AddMinutes(-5);
                            var prev = DateTime.Now.AddMinutes(-5).ToString("HH:mm:ss");
                            Console.WriteLine(prev);
                            var se = DateTime.UtcNow.AddMinutes(10);
                            var next = DateTime.Now.AddMinutes(10).ToString("HH:mm:ss");
                            var sasToday = DateTime.Now.ToString("yyyy-MM-dd");
                            var sasVersion = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");

                            var endurl = $"?comp=list&restype=container?sv=2017-07-29&ss=b&srt=sco&sp=rwdlac&se={sasToday}T{next}Z&st={sasToday}T{prev}Z&spr=https,http&sig=TsgXeNMoLjKpy99As6U3w0jSl1jXGBsjD%2FllSurA3K8%3D";

                            Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                            var addline = blobItem.StorageUri.PrimaryUri.ToString();


                            Console.WriteLine(sasVersion);

                            Console.WriteLine(endurl);

                            filesList.ProcessFiles.Add(addline);
                        }
                        anotherlist.Add(filesList);

                        ViewBag.files = anotherlist;
                        Console.WriteLine();

                        //Get the continuation token.
                        continuationToken = resultSegment.ContinuationToken;
                    }
                    while (continuationToken != null);

                }
            }
        

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
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
                       select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

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
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
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
                       select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput { InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

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
                        "D:/home/site/wwwroot/uploads/" + firstid + secondid);

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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = ExecuteSopID;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();
            var getinstanceid = (from y in _context.NewInstance
                                 where y.InstanceId == getexe
                                 select y.InstanceId).Single();
            ViewBag.getinstanceid = getinstanceid;

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
                       select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput {InstanceId = i.InstanceId, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status }).ToList();

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
                            "D:/home/site/wwwroot/uploads/" + firstid + secondid);

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
        public ActionResult ViewSOP(string SOPTemplateID)
        {
            
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            var gettopid = SOPTemplateID;
            ViewBag.gettopid = gettopid;
            var gettempname = (from y in _context.SOPNewTemplate
                               where y.SOPTemplateID == SOPTemplateID
                               select y.TempName).Single();

            ViewBag.gettempname = gettempname;

            var gettempstatus = (from y in _context.SOPNewTemplate
                               where y.SOPTemplateID == SOPTemplateID
                                 select y.LiveStatus).Single();
            ViewBag.gettempstatus = gettempstatus;

            var allusers = (from u in _context.CompanyClaim
                            where u.CompanyId == comp
                            select new AddApprover { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.allusers = allusers;

            var theapprovers = (from i in _context.InstanceApprovers
                                where i.InstanceId == SOPTemplateID
                                select new SOPOverView { UserId = i.UserId, ApproverStatus = i.ApproverStatus }).ToList();

            ViewBag.theapprovers = theapprovers;
            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var deppartments = (from y in _context.Departments
                                select new RegisterSOPUserViewModel { DepartmentId = y.DepartmentId, DepartmentName = y.DepartmentName }).ToList();
            ViewBag.deppartments = deppartments;

            var jobtitle = (from x in _context.JobTitles
                            select new RegisterSOPUserViewModel { CompanyId = x.CompanyId, JobTitleId = x.JobTitleId, JobTitle = x.JobTitle }).ToList();
            ViewBag.jobtitle = jobtitle;

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = SOPTemplateID;
            ViewBag.exeid = exeid;

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SubSecId = i.SubSecId, SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SubSecId = i.SubSecId, MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { SubSecId = i.SubSecId, TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.gettopid = SOPTemplateID;

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView { processStatus = i.processStatus, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var gethistory = (from i in _context.EditSOP
                              where i.SOPID == SOPTemplateID
                              select new versionHistory { SOPID = i.SOPID, ValuematchEdited = i.ValuematchEdited, TextEdited = i.TextEdited, EditDate = i.EditDate, UserId = i.UserId }).ToList();

            ViewBag.gethistory = gethistory;
            var alltheusers = (from u in _context.CompanyClaim
                               select new versionHistory { UserId = u.UserId, FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = alltheusers;

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

            var getjobs = (from j in _context.JobTitles
                           select new ProcessOutput { JobTitle = j.JobTitle, JobTitleId = j.JobTitleId }).ToList();

            ViewBag.getjobs = getjobs;


            return View();
        }

        [HttpPost]
        public ActionResult ViewSOP(string SOPTemplateID, string notusedstring)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            var gettopid = SOPTemplateID;

            var gettempname = (from y in _context.SOPNewTemplate
                               where y.SOPTemplateID == SOPTemplateID
                               select y.TempName).Single();

            ViewBag.gettempname = gettempname;

            var gettempstatus = (from y in _context.SOPNewTemplate
                                 where y.SOPTemplateID == SOPTemplateID
                                 select y.LiveStatus).Single();
            ViewBag.gettempstatus = gettempstatus;

            var allusers = (from u in _context.CompanyClaim
                            where u.CompanyId == comp
                            select new AddApprover { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.allusers = allusers;

            var theapprovers = (from i in _context.InstanceApprovers
                                where i.InstanceId == SOPTemplateID
                                select new SOPOverView { UserId = i.UserId, ApproverStatus = i.ApproverStatus }).ToList();

            ViewBag.theapprovers = theapprovers;
            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var deppartments = (from y in _context.Departments
                                select new RegisterSOPUserViewModel { DepartmentId = y.DepartmentId, DepartmentName = y.DepartmentName }).ToList();
            ViewBag.deppartments = deppartments;

            var jobtitle = (from x in _context.JobTitles
                            select new RegisterSOPUserViewModel { CompanyId = x.CompanyId, JobTitleId = x.JobTitleId, JobTitle = x.JobTitle }).ToList();
            ViewBag.jobtitle = jobtitle;

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = SOPTemplateID;
            ViewBag.exeid = exeid;

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SubSecId = i.SubSecId, SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SubSecId = i.SubSecId, MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { SubSecId = i.SubSecId, TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.gettopid = SOPTemplateID;

            ViewBag.thesecs = getsecs;
            ViewBag.thesin = getsin;
            ViewBag.getmul = getmul;
            ViewBag.gettabs = gettabs;
            ViewBag.gettabscols = gettabscols;
            ViewBag.gettabsrows = gettabsrows;

            var processtmps = (from i in _context.SOPProcessTempls
                               where i.SOPTemplateID == gettopid
                               select new SOPOverView { processStatus = i.processStatus, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


            ViewBag.processtmps = processtmps;

            var gethistory = (from i in _context.EditSOP
                              where i.SOPID == SOPTemplateID
                              select new versionHistory { SOPID = i.SOPID, ValuematchEdited = i.ValuematchEdited, TextEdited = i.TextEdited, EditDate = i.EditDate, UserId = i.UserId }).ToList();

            ViewBag.gethistory = gethistory;
            var alltheusers = (from u in _context.CompanyClaim
                               select new versionHistory { UserId = u.UserId, FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = alltheusers;

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

            var getjobs = (from j in _context.JobTitles
                           select new ProcessOutput { JobTitle = j.JobTitle, JobTitleId = j.JobTitleId }).ToList();

            ViewBag.getjobs = getjobs;

            if (ModelState.IsValid)
            {   
                foreach(var item in theapprovers){
                    bool appcomplete = false;
                    foreach (var user in allusers){
                        if (item.UserId == user.ClaimId)
                        {
                            var updateapprove = _context.InstanceApprovers.FirstOrDefault(x => x.InstanceId == SOPTemplateID && item.UserId == getuserid);

                            var namenospace = "approve" + user.FirstName + user.SecondName + item.UserId;
                            var getname = Request.Form["signature-" + namenospace];
                            if (!String.IsNullOrEmpty(getname))
                            {
                                updateapprove.ApproverStatus = "Complete";
                            }
                            _context.InstanceApprovers.Update(updateapprove);
                            _context.SaveChanges();

                            var status = "Complete";
                            var getstatus = item.ApproverStatus;
                            if(getstatus == status){
                                appcomplete = true;
                            }
                            Console.WriteLine(updateapprove.ApproverStatus);
                        }
                    }
                    Console.WriteLine(appcomplete);
                    var getappstatus = (from i in _context.InstanceApprovers
                                        where i.InstanceId == SOPTemplateID
                                        select i.ApproverStatus).ToList();

                    bool allcomplete = true;
                    foreach(var subitem in getappstatus){
                        if (subitem != "Complete")
                        {
                            allcomplete = false;
                        }
                    }
                    if(allcomplete){
                        var gettemps = _context.SOPNewTemplate.FirstOrDefault(x => x.SOPTemplateID == SOPTemplateID);

                        gettemps.LiveStatus = "Active";
                        Console.WriteLine(gettemps.TempName);
                        Console.WriteLine(gettemps.LiveStatus);
                        _context.SOPNewTemplate.Update(gettemps);
                        _context.SaveChanges();

                        Console.WriteLine(gettemps.LiveStatus);
                    }
                }
            }
            string url1 = Url.Content("/Manage/SOPTemplates");
            return new RedirectResult(url1);
        }

        [HttpGet]
        public ActionResult EditMainSOP(string SOPTemplateID)
        {
            var getuser = _userManager.GetUserId(User);
            var getuserid = (from y in _context.CompanyClaim
                             where y.UserId == getuser
                             select y.ClaimId).Single();

            ViewBag.loggedin = getuserid;

            var gettempname = (from y in _context.SOPNewTemplate
                               where y.SOPTemplateID == SOPTemplateID
                               select y.TempName).Single();

            ViewBag.gettempname = gettempname;

            var getuserinfo = (from i in _context.CompanyClaim
                               where i.UserId == getuser
                               select new SOPOverView { FirstName = i.FirstName, SecondName = i.SecondName }).ToList();
            ViewBag.userinfo = getuserinfo;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = SOPTemplateID;
            ViewBag.exeid = exeid;

            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();
            
            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild {SubSecId = i.SubSecId, SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild {SubSecId = i.SubSecId, MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild {SubSecId = i.SubSecId, TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabscols = (from i in _context.UsedTableCols
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { ColText = i.ColText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabsrows = (from i in _context.UsedTableRows
                               join p in _context.SOPSectionCreate on i.tableval equals p.valuematch
                               select new SOPOverView { RowText = i.RowText, valuematch = p.valuematch, tableval = p.valuematch, NewTempId = i.NewTempId }).ToList();

            ViewBag.gettopid = SOPTemplateID;

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

            var gethistory = (from i in _context.EditSOP
                              where i.SOPID == SOPTemplateID
                              select new versionHistory { SOPID = i.SOPID, ValuematchEdited = i.ValuematchEdited, TextEdited = i.TextEdited, EditDate = i.EditDate, UserId = i.UserId }).ToList();

            ViewBag.gethistory = gethistory;
            var alltheusers = (from u in _context.CompanyClaim
                            select new versionHistory {UserId = u.UserId, FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.resiuser = alltheusers;
            return View();
        }

        [HttpPost]
        public ActionResult EditMainSOP(string SOPTemplateID, [Bind("ProcessDesc")]ApplicationDbContext.SOPTemplateProcesses sopprocess)
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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = SOPTemplateID;


            var toptemp = (from w in _context.SOPTopTemplates
                           where w.CompanyId == compid
                           select w.TopTempId).Single();

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == toptemp
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            var getsin = (from i in _context.UsedSingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild {SubSecId = i.SubSecId,  SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId, }).ToList();

            var getmul = (from i in _context.UsedMultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild {SubSecId = i.SubSecId,  MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

            var gettabs = (from i in _context.UsedTable
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild {SubSecId = i.SubSecId,  TableHTML = i.TableHTML, valuematch = p.valuematch, NewTempId = i.NewTempId }).ToList();

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

            var getprocessstatus = (from i in _context.InstanceApprovers
                                    where i.InstanceId == SOPTemplateID
                                    select new SOPOverView { UserId = i.UserId, ApproverStatus = i.ApproverStatus }).ToList();


            if (ModelState.IsValid)
            {
                var dbrequest = _context.SOPNewTemplate.FirstOrDefault(x => x.SOPTemplateID == SOPTemplateID);
                var getname = Request.Form["the-templatename"];

                dbrequest.TempName = getname;
                _context.SOPNewTemplate.Update(dbrequest);
                _context.SaveChanges();
                foreach (var subitem in getsecs)
                {
                    foreach (var item in getsin)
                    {
                        if (item.valuematch == subitem.valuematch)
                        {
                            if (item.NewTempId == SOPTemplateID)
                            {
                                var gethidden = Request.Form["hiiden-line-" + item.SubSecId];
                                Console.WriteLine(gethidden);
                                int gettableid = item.SubSecId;
                                int changeto = int.Parse(gethidden);
                                if (gettableid == changeto)
                                {
                                    var getline = Request.Form["line-" + item.SubSecId];

                                    var getcurrentinput = (from p in _context.UsedSingleLinkText
                                                           where p.SubSecId == changeto
                                                           select p.SingleLinkTextBlock).Single();

                                    Console.WriteLine(getline);

                                    if(getcurrentinput != getline){
                                        ApplicationDbContext.EditedSop edited = new ApplicationDbContext.EditedSop();
                                        edited.TextEdited = getcurrentinput;
                                        edited.SOPID = SOPTemplateID;
                                        edited.ValuematchEdited = item.valuematch;
                                        edited.EditDate = DateTime.Now;
                                        edited.UserId = _userManager.GetUserId(User);
                                        _context.Add(edited);
                                        _context.SaveChanges();

                                        var dbconnection = _context.UsedSingleLinkText.FirstOrDefault(x => x.SubSecId == changeto);
                                        dbconnection.SingleLinkTextBlock = getline;
                                        _context.UsedSingleLinkText.Update(dbconnection);
                                        _context.SaveChanges();
                                    }
                                    else { }
                                }

                            }
                        }
                    }
                    foreach (var item in getmul)
                    {
                        if (item.valuematch == subitem.valuematch)
                        {
                            if (item.NewTempId == SOPTemplateID)
                            {
                                var gethidden = Request.Form["hiiden-mult-" + item.SubSecId];
                                Console.WriteLine(gethidden);
                                int gettableid = item.SubSecId;
                                int changeto = int.Parse(gethidden);
                                if (gettableid == changeto)
                                {
                                    var getline = Request.Form["text-" + item.SubSecId];

                                    var getcurrentinput = (from p in _context.UsedMultilineText
                                                           where p.SubSecId == changeto
                                                           select p.MultilineTextBlock).Single();
                                    if (getcurrentinput != getline)
                                    {
                                        ApplicationDbContext.EditedSop edited = new ApplicationDbContext.EditedSop();
                                        edited.TextEdited = getcurrentinput;
                                        edited.SOPID = SOPTemplateID;
                                        edited.ValuematchEdited = item.valuematch;
                                        edited.EditDate = DateTime.Now;
                                        edited.UserId = _userManager.GetUserId(User);
                                        _context.Add(edited);
                                        _context.SaveChanges();


                                        var dbconnection = _context.UsedMultilineText.FirstOrDefault(x => x.SubSecId == changeto);
                                        dbconnection.MultilineTextBlock = getline;
                                        _context.UsedMultilineText.Update(dbconnection);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }   

                }
                foreach (var item in getprocessstatus)
                {

                }


            }
            string url1 = Url.Content("/Manage/SOPTemplates");
            string newurl = url1.Replace("%3F%3D", "?=");

            return new RedirectResult(newurl);
        }
        [HttpGet]
        public async Task<IActionResult> SOPs(string ExecuteSopID)
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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var exeid = ExecuteSopID;
            ViewBag.exeid = exeid;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();

            var getinstanceid = (from y in _context.NewInstance
                                 where y.InstanceId == getexe
                                 select y.InstanceId).Single();
            ViewBag.getinstanceid = getinstanceid;



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
                               select new SOPOverView {processStatus = i.processStatus, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();


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
                       select new ProcessOutput {InstanceId = i.InstanceId, RACIResID = i.RACIResID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput {InstanceId = i.InstanceId, RACIAccID = i.RACIAccID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput {InstanceId = i.InstanceId, RACIConID = i.RACIConID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput {InstanceId = i.InstanceId, RACIInfID = i.RACIInfID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();

            ViewBag.infi = infi;

            var instpocess = (from i in _context.SOPInstanceProcesses
                              select new ProcessOutput { DueDate = i.DueDate, SOPTemplateID = i.SOPTemplateID, valuematch = i.valuematch }).ToList();

            ViewBag.instpocess = instpocess;
            double completeProcesses = 0;

            foreach (var item in processtmps)
            {
                bool rescomplete = false, acccomplete = false, concomplete = false, infcomplete = false; 
                foreach (var sub in acc)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        
                        var status = "Complete";
                         var thecount = sub.Status;

                        if (thecount == status){
                            acccomplete = true;
                            Console.WriteLine(sub.RACIAccID);
                        }
                    }
                }
                foreach (var sub in res)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            rescomplete = true;
                        }
                    }
                }
                foreach (var sub in cons)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            concomplete = true;
                        }
                    }
                }
                foreach (var sub in infi)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            infcomplete = true;
                        }
                    }
                }

                if(rescomplete && acccomplete && concomplete && infcomplete)
                {
                    completeProcesses++;
                    Console.WriteLine(completeProcesses);
                }
            }
            Console.WriteLine(processtmps.Count());
            double processcount = processtmps.Count();
            double percentageComplete = (completeProcesses / processcount) * 100;
            ViewBag.percentageComplete = Math.Round(percentageComplete,2);
            Console.WriteLine(ViewBag.percentageComplete);
			var getoverstatus = (from i in _context.SOPProcessTempls
                                 where i.SOPTemplateID == gettopid
                                 select i.processStatus).ToList();

            bool complete = true;

            foreach (var item in getoverstatus)
            {
                if (item != "Complete")
                {
                    complete = false;
                }
            }
            if (complete)
            {
                var getexesop = _context.ExecutedSop.FirstOrDefault(x => x.ExecuteSopID == exeid);
                getexesop.SOPStatus = "Complete";
                ViewBag.sopstatus = getexesop.SOPStatus;
                _context.ExecutedSop.Update(getexesop);
                _context.SaveChanges();
                //Sendgrid - send to main user. 
            }	
            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var comments = (from i in _context.Comments
                            where i.ExecuteSopID == exeid
                            select new CommentsView { CommentId = i.CommentId, TheComment = i.TheComment, PostTime = i.PostTime , UserId = i.UserId}).ToList();

            ViewBag.comments = comments;


            var instancepro = (from i in _context.SOPInstanceProcesses
                               select new SOPOverView { SOPTemplateProcessID = i.SOPInstancesProcessId, SOPTemplateID = i.SOPTemplateID, valuematch = i.valuematch }).ToList();
            
            List<ProcessOutput.aList> anotherlist = new List<ProcessOutput.aList>();
            var thetempid = gettopid;
            Console.WriteLine("InstanceID:" + getexe);
            foreach (var subitem in instancepro)
            {
                if (subitem.SOPTemplateID == getexe)
                {
                    var blobname = getexe;
                    Console.WriteLine("Blobname:");
                    Console.WriteLine(blobname);

                    string cloudStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sopman;AccountKey=RCmOo1xCGu7FIx0wZ3H4wNK4Y0MNtcj5chzAMWlU2GQjC/ehnsiSD9MTuHFGCUDf2sPguMByyX7VrjlQpq4/FA==;EndpointSuffix=core.windows.net";

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference(blobname);

                    //List blobs to the console window, with paging.
                    Console.WriteLine("List blobs in pages:");

                    int i = 0;
                    BlobContinuationToken continuationToken = null;
                    BlobResultSegment resultSegment = null;

                    ProcessOutput.aList filesList = new ProcessOutput.aList();

                    //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
                    //When the continuation token is null, the last page has been returned and execution can exit the loop.
                    do
                    {
                        //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                        //or by calling a different overload.
                        resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, null, continuationToken, null, null);
                        if (resultSegment.Results.Count<IListBlobItem>() > 0) { Console.WriteLine("Page {0}:", ++i); }
                        filesList.ProcessFiles = new List<string>();
                        foreach (var blobItem in resultSegment.Results)
                        {
                            var st = DateTime.UtcNow.AddMinutes(-5);
                            var prev = DateTime.Now.AddMinutes(-5).ToString("HH:mm:ss");
                            Console.WriteLine(prev);
                            var se = DateTime.UtcNow.AddMinutes(10);
                            var next = DateTime.Now.AddMinutes(10).ToString("HH:mm:ss");
                            var sasToday = DateTime.Now.ToString("yyyy-MM-dd");
                            var sasVersion = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");

                            var endurl = $"?comp=list&restype=container?sv=2017-07-29&ss=b&srt=sco&sp=rwdlac&se={sasToday}T{next}Z&st={sasToday}T{prev}Z&spr=https,http&sig=TsgXeNMoLjKpy99As6U3w0jSl1jXGBsjD%2FllSurA3K8%3D";

                            Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                            var addline = blobItem.StorageUri.PrimaryUri.ToString();


                            Console.WriteLine(sasVersion);

                            Console.WriteLine(endurl);

                            filesList.ProcessFiles.Add(addline);
                        }
                        anotherlist.Add(filesList);

                        ViewBag.files = anotherlist;
                        Console.WriteLine();

                        //Get the continuation token.
                        continuationToken = resultSegment.ContinuationToken;
                    }
                    while (continuationToken != null);

                }
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> SOPs(List<IFormFile> files, string ExecuteSopID, [Bind("SectionId,ExecuteSopID,UserId")]ApplicationDbContext.ExecuteSop exe )
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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;
            var exeid = ExecuteSopID;

            var getexe = (from i in _context.ExecutedSop
                          where i.ExecuteSopID == exeid
                          select i.SectionId).Single();
            var getinstanceid = (from y in _context.NewInstance
                                 where y.InstanceId == getexe
                                 select y.InstanceId).Single();
            ViewBag.getinstanceid = getinstanceid;
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
                               select new SOPOverView {processStatus = i.processStatus, completedDate = i.completedDate, SOPTemplateProcessID = i.SOPTemplateProcessID, SOPTemplateID = i.SOPTemplateID, ProcessName = i.ProcessName, ProcessDesc = i.ProcessDesc, valuematch = i.valuematch, ProcessType = i.ProcessType }).ToList();
            

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
                       select new ProcessOutput {InstanceId = i.InstanceId, RACIResID = i.RACIResID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.res = res;

            var acc = (from i in _context.SOPRACIAcc
                       select new ProcessOutput {InstanceId = i.InstanceId, RACIAccID = i.RACIAccID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.acc = acc;

            var cons = (from i in _context.SOPRACICon
                        select new ProcessOutput {InstanceId = i.InstanceId, RACIConID = i.RACIConID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();
            ViewBag.cons = cons;
            var infi = (from i in _context.SOPRACIInf
                        select new ProcessOutput {InstanceId = i.InstanceId, RACIInfID = i.RACIInfID, SOPTemplateID = i.soptoptempid, valuematch = i.valuematch, JobTitleId = i.JobTitleId, DepartmentId = i.DepartmentId, UserId = i.UserId, Status = i.Status, editDate = i.editDate }).ToList();

            ViewBag.infi = infi;

            var deps = (from i in _context.Departments
                        select new ProcessOutput { DepartmentId = i.DepartmentId, DepartmentName = i.DepartmentName }).ToList();

            ViewBag.deps = deps;

            var comments = (from i in _context.Comments
                            where i.ExecuteSopID == exeid
                            select new CommentsView { CommentId = i.CommentId, TheComment = i.TheComment, PostTime = i.PostTime , UserId = i.UserId}).ToList();

            ViewBag.comments = comments;

            double completeProcesses = 0;

            foreach (var item in processtmps)
            {
                bool rescomplete = false, acccomplete = false, concomplete = false, infcomplete = false;
                foreach (var sub in acc)
                {
                    if (sub.valuematch == item.valuematch)
                    {

                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            acccomplete = true;
                            Console.WriteLine(sub.RACIAccID);
                        }
                    }
                }
                foreach (var sub in res)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            rescomplete = true;
                        }
                    }
                }
                foreach (var sub in cons)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            concomplete = true;
                        }
                    }
                }
                foreach (var sub in infi)
                {
                    if (sub.valuematch == item.valuematch)
                    {
                        var status = "Complete";
                        var thecount = sub.Status;

                        if (thecount == status)
                        {
                            infcomplete = true;
                        }
                    }
                }

                if (rescomplete && acccomplete && concomplete && infcomplete)
                {
                    completeProcesses++;
                    Console.WriteLine(completeProcesses);
                    var gettempstatus = _context.SOPProcessTempls.FirstOrDefault(x => x.valuematch == item.valuematch && x.SOPTemplateProcessID == item.SOPTemplateProcessID);

                    Console.WriteLine("Process Status");
                    Console.WriteLine(gettempstatus.processStatus);

                    gettempstatus.processStatus = "Complete";
                    gettempstatus.completedDate = System.DateTime.Now;

                    _context.SOPProcessTempls.Update(gettempstatus);
                    _context.SaveChanges();
                }
            }
            Console.WriteLine(processtmps.Count());
            double processcount = processtmps.Count();
            double percentageComplete = (completeProcesses / processcount) * 100;
            ViewBag.percentageComplete = Math.Round(percentageComplete, 2);
            Console.WriteLine(ViewBag.percentageComplete);
				
			var getoverstatus = (from i in _context.SOPProcessTempls
                                where i.SOPTemplateID == gettopid
                                 select i.processStatus).ToList();

            bool complete = true;

            foreach (var item in getoverstatus){
                if(item != "Complete"){
                    complete = false;
                }
            }
            if (complete)
            {
                var getexesop = _context.ExecutedSop.FirstOrDefault(x => x.ExecuteSopID == exeid);
                getexesop.SOPStatus = "Complete";

                _context.ExecutedSop.Update(getexesop);
                _context.SaveChanges();
                //Sendgrid - send to main user. 
            }   

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
                                    var updateres = _context.SOPRACIRes.FirstOrDefault(x => x.valuematch == getvalue && x.UserId == item.UserId && x.InstanceId == getinstanceid);
                                    var namenospace = "resp" + name.FirstName + name.SecondName + item.RACIResID + item.valuematch;
                                    Console.WriteLine("namenospace");
                                    Console.WriteLine(namenospace);
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updateres.Status = "Complete";
                                        updateres.editDate = DateTime.Now;
                                        _context.SOPRACIRes.Update(updateres);
                                        _context.SaveChanges();
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateres.Status = "Recused";
                                        updateres.editDate = DateTime.Now;
                                        _context.SOPRACIRes.Update(updateres);
                                        _context.SaveChanges();
                                    }

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
                                    var updateacc = _context.SOPRACIAcc.FirstOrDefault(x => x.valuematch == getvalue && x.UserId == item.UserId && x.InstanceId == getinstanceid);
                                    var namenospace = "acc" + name.FirstName + name.SecondName + item.RACIAccID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updateacc.Status = "Complete";
                                        updateacc.editDate = DateTime.Now;
                                        _context.SOPRACIAcc.Update(updateacc);
                                        _context.SaveChanges();
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateacc.Status = "Recused";
                                        updateacc.editDate = DateTime.Now;
                                        _context.SOPRACIAcc.Update(updateacc);
                                        _context.SaveChanges();
                                    }

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
                                    var updatecon = _context.SOPRACICon.FirstOrDefault(x => x.valuematch == getvalue && x.UserId == item.UserId && x.InstanceId == getinstanceid);
                                    var namenospace = "cons" + name.FirstName + name.SecondName + item.RACIConID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        updatecon.Status = "Complete";
                                        updatecon.editDate = DateTime.Now;
                                        _context.SOPRACICon.Update(updatecon);
                                        _context.SaveChanges();
                                    }
                                    else if(!String.IsNullOrEmpty(getrecuse)){
                                        updatecon.Status = "Recused";
                                        updatecon.editDate = DateTime.Now;
                                        _context.SOPRACICon.Update(updatecon);
                                        _context.SaveChanges();
                                    }

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
                                    var updateinf = _context.SOPRACIInf.FirstOrDefault(x => x.valuematch == getvalue && x.UserId == item.UserId && x.InstanceId == getinstanceid);
                                    var namenospace = "infi" + name.FirstName + name.SecondName + item.RACIInfID + item.valuematch;
                                    string firstname = name.FirstName;
                                    var getname = Request.Form["signature-" + namenospace];
                                    var getrecuse = Request.Form["recuse-" + namenospace];
                                    if (!String.IsNullOrEmpty(getname))
                                    {
                                        Console.WriteLine(getname + "Complete");
                                        updateinf.Status = "Complete";
                                        updateinf.editDate = DateTime.Now;
                                        _context.SOPRACIInf.Update(updateinf);
                                        _context.SaveChanges();
                                    }
                                    else if (!String.IsNullOrEmpty(getrecuse))
                                    {
                                        updateinf.Status = "Recused";
                                        updateinf.editDate = DateTime.Now;
                                        _context.SOPRACIInf.Update(updateinf);
                                        _context.SaveChanges();
                                    }
                                }
                            }

                        }
                    }
                }
                var instancepro = (from i in _context.SOPInstanceProcesses
                               select new SOPOverView { SOPTemplateProcessID = i.SOPInstancesProcessId, SOPTemplateID = i.SOPTemplateID, valuematch = i.valuematch }).ToList();

                Console.WriteLine("New uploads: " + files.Count());



                if (files.Count() > 0)
                {
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(getexe);

                    Console.WriteLine(getexe);

                    await container.CreateIfNotExistsAsync();
                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                    foreach (var file in files)
                    {
                        Console.WriteLine(file.ContentDisposition);
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                        await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
                    }
                }
                _context.SaveChanges();
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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;


            return View();
        }

        [HttpPost]
        public ActionResult CreateNewProject([Bind("ProjectId,CompId,ProjectName,CreationDate,UserId")] ApplicationDbContext.Project project)
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();
            
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == compid
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

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
