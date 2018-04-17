using System;
using System.IO;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using LumenWorks.Framework.IO.Csv;
using System.Security.Claims;
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
using sopman.Models.AccountViewModels;
using sopman.Models.SetupViewModels;
using sopman.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using CsvHelper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
namespace sopman.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SetupController : Controller
    {

        CloudStorageAccount storageAccount = new CloudStorageAccount(
        new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                "sopman",
                "RCmOo1xCGu7FIx0wZ3H4wNK4Y0MNtcj5chzAMWlU2GQjC/ehnsiSD9MTuHFGCUDf2sPguMByyX7VrjlQpq4/FA=="), true);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public SetupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [TempData]
        public string ErrorMessage { get; set; }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult SetupIndex()
        {
            var getuser = _userManager.GetUserId(User);

            if (_context.CompanyClaim.Any(o => o.UserId == getuser))
            {
                ViewBag.Class = "true";
                var comp = (from i in _context.CompanyClaim
                            where i.UserId == getuser
                            select i.CompanyId).Single();
                ViewBag.comp = comp;
                var logostring = (from i in _context.TheCompanyInfo
                                  where i.CompanyId == comp
                                  select i.Logo).Single();
                ViewBag.logostring = logostring;
                var theid = (from m in _context.CompanyClaim
                             where m.UserId == getuser
                             select m.CompanyId).Single();
                if (_context.SOPTopTemplates.Any(o => o.CompanyId == theid))
                {
                    ViewBag.ClassThree = "true";
                }
                if (_context.Departments.Any(o => o.CompanyId == theid))
                {
                    ViewBag.Classtwo = "true";
                }
            }
            else { }
            return View();
        }


        public async Task<IActionResult> SetupIndex(SetupIndexViewModel model)
        {

            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;


            var modules = (from i in _context.CompanyClaim
                           where i.UserId == user_id
                           select i.CompanyId).Single();
            
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == modules
                              select i.Logo).Single();
            ViewBag.logostring = logostring;


            return View();

            /*

             var companies = from m in _context.TheCompanyInfo
                          select m;

             return View(model); */

        }

        [HttpGet]
        public PartialViewResult AddNumber()
        {
            return PartialView("_AddNumber", new SOPCodeBuilder());
        }

        [HttpGet]
        public PartialViewResult AddCharacter()
        {
            return PartialView("_AddCharacter", new SOPCodeBuilder());
        }

        [HttpGet]
        public PartialViewResult AddDash()
        {
            return PartialView("_AddDash", new SOPCodeBuilder());
        }

        [HttpGet]
        public PartialViewResult AddSpace()
        {
            return PartialView("_AddSpace", new SOPCodeBuilder());
        }

        [HttpGet]
        public PartialViewResult AddComma()
        {
            return PartialView("_AddComma", new SOPCodeBuilder());
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CompanySetup()
        {
            var ordervm = new CompanySetupViewModel();
            var getuser = _userManager.GetUserId(User);



            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> CompanySetup([Bind("Id,Name,Logo,SOPNumberFormat,SOPStartNumber,UserId")] ApplicationDbContext.CompanyInfo company, IFormFile file,  [Bind("ClaimId,CompanyId,UserId,FirstName,SecondName")] ApplicationDbContext.ClaimComp claim, [Bind(Prefix = "SOPCode")]IEnumerable<SOPCodeBuilder> sopcodebuild)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                var getname = Request.Form["Name"];
                var getfile = Request.Form["file"];

                if(file != null){
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("logos");

                    await container.CreateIfNotExistsAsync();

                    await container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });

                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(getname + "logo" + file.FileName);

                    await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
                    _context.TheCompanyInfo.Select(u => u.Logo);
                    company.Logo = getname + "logo" + file.FileName;
                }


                _context.TheCompanyInfo.Select(u => u.UserId);
                var user_id = user.Id;
                company.UserId = user_id;
                claim.UserId = user_id;

                _context.CompanyClaim.Select(u => u.CompanyId);
                var comp_id = company.CompanyId;
                claim.CompanyId = comp_id;

                _context.CompanyClaim.Select(u => u.UserId);
                claim.UserId = user_id;

                _context.TheCompanyInfo.Select(u => u.SOPNumberFormat);
                var sopfrom = Request.Form["SOPNumberFormat"];
                company.SOPNumberFormat = sopfrom;
               
                _context.Add(company);
                await _context.SaveChangesAsync();
                var getid = (from i in _context.TheCompanyInfo
                             where i.UserId == user_id
                             select i.CompanyId).Single();

                _context.CompanyClaim.Select(u => u.CompanyId);
                claim.CompanyId = getid;

                _context.Add(claim);
                await _context.SaveChangesAsync();

                foreach(var item in sopcodebuild){

                    ApplicationDbContext.SOPNumbering number = new ApplicationDbContext.SOPNumbering();

                    var getcompid = (from i in _context.TheCompanyInfo
                                     where i.UserId == user_id
                                     select i.CompanyId).Single();

                    number.CompanyId = getcompid;

                    if(item.InputValue == "0"){
                        var numbervalue = "0";
                        number.InputValue = numbervalue;
                        Console.WriteLine("Number:");
                        Console.WriteLine(number.InputValue);
                        _context.Add(number);
                    }
                    if (item.InputValue == "A")
                    {
                        var charvalue = "A";
                        number.InputValue = charvalue;
                        Console.WriteLine("Character:");
                        Console.WriteLine(number.InputValue);
                        _context.Add(number);
                    }
                    if (item.InputValue == "-")
                    {
                        var dashvalue = "-";
                        number.InputValue = dashvalue;
                        Console.WriteLine("Dash:");
                        Console.WriteLine(number.InputValue);
                        _context.Add(number);
                    }
                    if (item.InputValue == "space")
                    {
                        var spacevalue = "space";
                        number.InputValue = spacevalue;
                        Console.WriteLine("Space:");
                        Console.WriteLine(number.InputValue);
                        _context.Add(number);
                    }
                    if (item.InputValue == ",")
                    {
                        var commavalue = ",";
                        number.InputValue = commavalue;
                        Console.WriteLine("Comma:");
                        Console.WriteLine(number.InputValue);
                        _context.Add(number);
                    }

                    await _context.SaveChangesAsync();
                }


                return RedirectToAction("SetupIndex", "Setup");
            }
            return View(company);
        }

        public IActionResult SOPTopTemplate()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SOPAdmin")]
        public async Task<IActionResult> SOPTopTemplate([Bind("TopTempId,SOPNameLimit,SOPCodePrefix,SOPCodeSuffix,SOPAllowCodeLimit,ClientId,UserId")] ApplicationDbContext.SOPTopTemplate toptemp)
        {
            var user = await _userManager.GetUserAsync(User);
            var user_id = user.Id;

            var modules = (from i in _context.CompanyClaim
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == modules
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var usermod = (from i in _context.CompanyClaim
                           where i.UserId == user_id
                           select i.UserId).Single();


            if (ModelState.IsValid)
            {
                _context.SOPTopTemplates.Select(u => u.CompanyId);
                toptemp.CompanyId = modules;

                _context.SOPTopTemplates.Select(u => u.UserId);
                toptemp.UserId = usermod;

                _context.Add(toptemp);
                await _context.SaveChangesAsync();
                return RedirectToAction("SOPTopTemplateBuilder", "Setup");
            }
            return View(toptemp);
        }

        [HttpPost]
        public PartialViewResult AddSection()
        {
            return PartialView("_SectionCreator", new Section());
        }

        public ActionResult GetNewLine(string containerPrefix, string containerJustNumber)
        {
            List<SingleLineViewModel> model = new List<SingleLineViewModel>();
            ViewData["ContainerPrefix"] = containerPrefix;
            ViewData["ContainerNoNum"] = containerJustNumber;
            return PartialView("_SingleTextLine", model);
        }

        [ResponseCache(NoStore = true, Duration = 0)]
        public ActionResult GetNewSingleLine(string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            return PartialView("SingleLineViewModel", new SingleLineViewModel());
        }

        public ActionResult GetNewMulti(string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            return PartialView("MultiLineViewModel", new MultilineViewModel());
        }

        public ActionResult GetTable(string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            return PartialView("TableViewModel", new TableViewModel());
        }


        [HttpGet]
        public ActionResult SOPTopTemplateBuilder()
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

            List<Section> model = new List<Section>();
            return View(model);
        }

        [HttpPost]
        public ActionResult SOPTopTemplateBuilder(SOPTopTemplateBuilderViewModel builder, [Bind(Prefix = "Section")]IEnumerable<Section> soptop, [Bind(Prefix = "SingleLine")]IEnumerable<SingleLineViewModel> singleline, [Bind(Prefix = "MultilineTextBlock")]IEnumerable<MultilineViewModel> multiline, [Bind(Prefix = "Table")]IEnumerable<TableViewModel> table, string containerPrefix)
        {
            var theuser = _userManager.GetUserId(User);
            var top = (from i in _context.SOPTopTemplates
                       where i.UserId == theuser
                       select i.TopTempId).Single();

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == theuser
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            Console.WriteLine(top);
            if (ModelState.IsValid)
            {
                Console.WriteLine(soptop.Count());
                if (soptop != null)
                {
                    foreach (var item in soptop)
                    {
                        ApplicationDbContext.SOPSectionCreator sopdb = new ApplicationDbContext.SOPSectionCreator();

                        Console.WriteLine(item.SectionText);

                        var getvalue = item.valuematch;
                        sopdb.valuematch = getvalue;

                        sopdb.TopTempId = top;

                        var text = item.SectionText;
                        sopdb.SectionText = text;

                        _context.Add(sopdb);
                    }
                }
                _context.SaveChanges();
                foreach (var sub in singleline)
                {
                    ApplicationDbContext.SingleLinkTextSec sinline = new ApplicationDbContext.SingleLinkTextSec();

                    var textline = sub.SingleLinkTextBlock;
                    sinline.SingleLinkTextBlock = textline;

                    sinline.UserId = theuser;

                    var getvalue = sub.valuematch;
                    sinline.valuematch = getvalue;

                    _context.Add(sinline);
                }
                _context.SaveChanges();
                foreach (var sub in multiline)
                {
                    ApplicationDbContext.MultilinkTextSec mulline = new ApplicationDbContext.MultilinkTextSec();
                    var textblock = sub.MultilineTextBlock;
                    mulline.MultilineTextBlock = textblock;

                    var getvalue = sub.valuematch;
                    mulline.valuematch = getvalue;

                    _context.Add(mulline);
                }
                _context.SaveChanges();
                foreach (var sub in table)
                {
                    ApplicationDbContext.TableSec newtable = new ApplicationDbContext.TableSec();
                    var tablename = sub.TableHTML;
                    newtable.TableHTML = tablename;

                    Console.WriteLine(tablename);

                    var getvalue = sub.valuematch;
                    newtable.valuematch = getvalue;

                    _context.Add(newtable);
                }
                _context.SaveChanges();
            }
            string url1 = Url.Content("/Setup/SetupIndex");
            string newurl = url1.Replace("%3F%3D", "?=");

            Console.WriteLine(newurl);

            return new RedirectResult(newurl);
        }

        public ActionResult TemplateForm()
        {

            return PartialView("_TemplateForm");
        }

        [HttpGet]
        public ActionResult SectionOutput()
        {
            var getu = _userManager.GetUserId(User);

            var newsec = new List<Section>();

            Console.WriteLine("User:");
            Console.WriteLine(getu);

            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == comp
                       select i.TopTempId).Single();

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == top
                           select new Section { SectionText = i.SectionText }).ToList();

            ViewBag.getsecs = getsecs;

            return PartialView("_TemplateForm");
        }

        //The rows
        public ActionResult AddTable(string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            return PartialView("_AddTable", new AddTableRow());
        }

        //The columns
        public ActionResult AddColumns(string containerPrefix)
        {
            ViewData["ContainerPrefix"] = containerPrefix;
            return PartialView("_AddColumns", new AddTable());
        }

        public ActionResult AddTemp()
        {
            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var getsopcode = (from i in _context.SOPNumberProcess
                              where i.CompanyId == comp
                              select new SOPCodeBuilder { InputValue = i.InputValue, SOPNumberingId = i.SOPNumberingId }).ToList();
            ViewBag.getsopcode = getsopcode;
            return PartialView("_SectionOutput", new SopTemplate());
        }

        public ActionResult AddApprover()
        {
            var orderVM = new SopTemplate();

            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            

            orderVM.Approver = new List<AddApprover>();

            var allusers = (from u in _context.CompanyClaim
                            where u.CompanyId == comp
                            select new AddApprover { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.allusers = allusers;
            foreach (var items in allusers)
            {
                var itemname = @items.FirstName;
                var itemId = @items.UserId.ToString();
                orderVM.Approver.Add(new AddApprover { Value = @itemId, FirstName = @itemId, SecondName = @itemname });
            }

           
            ViewBag.selectlist = new SelectList(allusers, "ClaimId", "FirstName");

            return PartialView("AddApprover", new SopTemplate());
        }

        [HttpGet]
        public ActionResult NewTemplate()
        {
            var getu = _userManager.GetUserId(User);
            var newsec = new SopTemplate();
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

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == top
                           select new SopTemplate{SectionText = i.SectionText, valuematch = i.valuematch}).ToList();
            
            var getid = (from i in _context.SOPSectionCreate
                         where i.TopTempId == top
                         select new { i.valuematch, i.SectionText }).ToList();
            
            var getsin = (from i in _context.SingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch }).ToList();

            ViewBag.GetSin = getsin;

            var getmuls = (from i in _context.MultilineText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch }).ToList();

            ViewBag.GetMul = getmuls;

            var gettabs = (from i in _context.Table
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch }).ToList();

            ViewBag.GetTabs = gettabs;

            return View(getsecs);
        }


        [HttpPost]
        public ActionResult NewTemplate([Bind("TempName,SOPCode,TopTempId,ExpireDate,LiveStatus,TheCreateDae")]ApplicationDbContext.SOPTemplate soptemp, [Bind("SingleLinkTextBlock,valuematch")]ApplicationDbContext.EntSingleLinkTextSec GetSingle, [Bind(Prefix = "Approver")]IEnumerable<AddApprover> approver, [Bind(Prefix = "NewColumn")]IEnumerable<AddTable> tablecol,[Bind(Prefix = "AddTableRow")]IEnumerable<AddTableRow> tablerow)
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

            var topid = (from i in _context.SOPTopTemplates
                         where i.CompanyId == compid
                         select i.TopTempId).Single();
            
            var getu = _userManager.GetUserId(User);
            var newsec = new SopTemplate();
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            
            var allusers = (from u in _context.CompanyClaim
                            where u.CompanyId == comp
                            select new SopTemplate { FirstName = u.FirstName, SecondName = u.SecondName, ClaimId = u.ClaimId }).ToList();
            ViewBag.allusers = allusers;

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var top = (from i in _context.SOPTopTemplates
                       where i.CompanyId == comp
                       select i.TopTempId).Single();

            var getsecs = (from i in _context.SOPSectionCreate
                           where i.TopTempId == top
                           select new SopTemplate { SectionText = i.SectionText, valuematch = i.valuematch }).ToList();

            ViewBag.Secs = getsecs;

            var getid = (from i in _context.SOPSectionCreate
                         where i.TopTempId == top
                         select new { i.valuematch, i.SectionText }).ToList();


            var getsin = (from i in _context.SingleLinkText
                          join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                          select new LineChild { SingleLinkTextBlock = i.SingleLinkTextBlock, valuematch = p.valuematch }).ToList();

            ViewBag.GetSin = getsin;

            var getmuls = (from i in _context.MultilineText
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { MultilineTextBlock = i.MultilineTextBlock, valuematch = p.valuematch }).ToList();

            ViewBag.GetMul = getmuls;

            var gettabs = (from i in _context.Table
                           join p in _context.SOPSectionCreate on i.valuematch equals p.valuematch
                           select new LineChild { TableHTML = i.TableHTML, valuematch = p.valuematch }).ToList();

            ViewBag.GetTabs = gettabs;

            var getsopcode = (from i in _context.SOPNumberProcess
                              where i.CompanyId == comp
                              select new SOPCodeBuilder { InputValue = i.InputValue, SOPNumberingId = i.SOPNumberingId }).ToList();
            ViewBag.getsopcode = getsopcode;
            var orderVM = new SopTemplate();
            orderVM.Approver = new List<AddApprover>();

            foreach (var items in allusers)
            {
                var itemname = @items.FirstName;
                var itemId = @items.UserId.ToString();
                orderVM.Approver.Add(new AddApprover { Value = @itemId, FirstName = @itemId, SecondName = @itemname });
            }

            ViewBag.selectlist = new SelectList(allusers, "ClaimId", "FirstName");

            if (ModelState.IsValid)
            {
                    soptemp.TopTempId = topid;
                    
                    List<string> code = new List<string>();

                    soptemp.TheCreateDae = DateTime.Now;

                    soptemp.LiveStatus = "Draft";
                    foreach (var item in getsopcode)
                    {
                        var getcodes = Request.Form["InputValue-" + item.SOPNumberingId];
                        Console.WriteLine(getcodes);
                        code.Add(getcodes);
                    }

                    string templatename = Request.Form["TempName"];

                    if(String.IsNullOrEmpty(templatename)){
                        soptemp.TempName = " ";
                        soptemp.LiveStatus = "Draft";
                    }if(!String.IsNullOrEmpty(templatename)){
                        soptemp.TempName = templatename; 
                    }

                    string expiredate = Request.Form["ExpireDate"];

                    if (String.IsNullOrEmpty(expiredate))
                    {
                        soptemp.TempName = " ";
                        soptemp.LiveStatus = "Draft";
                    }
                    if (!String.IsNullOrEmpty(expiredate))
                    {
                        soptemp.TempName = templatename;
                    }

                    var sopcodelist = string.Join("", code);
                    Console.WriteLine("Code:");
                    Console.WriteLine(sopcodelist);
                    soptemp.SOPCode = sopcodelist;

                    _context.Add(soptemp);
                    _context.SaveChanges();

                foreach(var item in approver){
                    ApplicationDbContext.Approvers approve = new ApplicationDbContext.Approvers();

                    approve.ApproverStatus = "Pending";

                    var thedrop = item.UserId;

                    approve.UserId = thedrop;
                    Console.WriteLine("Dropdown value:");
                    Console.WriteLine(thedrop);

                    var getname = Request.Form["TempName"];
                    var getexeid = (from y in _context.SOPNewTemplate
                                    where y.TempName == getname
                                    select y.SOPTemplateID).Single();
                    approve.InstanceId = getexeid;
                    _context.Add(approve);
                    _context.SaveChanges();
                }
                    

                    foreach (var item in (ViewBag.Secs))
                    {
                        foreach (var subitem in (ViewBag.GetSin))
                        {
                            if (item.valuematch == subitem.valuematch)
                            {
                                ApplicationDbContext.EntSingleLinkTextSec sinline = new ApplicationDbContext.EntSingleLinkTextSec();

                                var getform = Request.Form["TempName"];
                                var gettopid = (from y in _context.SOPNewTemplate
                                                where y.TempName == getform
                                                select y.SOPTemplateID).Single();

                                sinline.NewTempId = gettopid;

                                _context.UsedSingleLinkText.Select(u => u.SingleLinkTextBlock);
                                _context.UsedSingleLinkText.Select(u => u.valuematch);

                                var textval = subitem.SingleLinkTextBlock;

                                var nospace = subitem.SingleLinkTextBlock.Replace(" ", "");
                                var newval = Request.Form["SingleLinkTextBlock-" + nospace];

                                Console.WriteLine(newval);
                                sinline.SingleLinkTextBlock = newval;

                                var valuem = subitem.valuematch;
                                sinline.valuematch = valuem;

                                _context.Add(sinline);
                            }
                        }
                        foreach (var subitem in (ViewBag.GetMul))
                        {
                            if (item.valuematch == subitem.valuematch)
                            {
                                ApplicationDbContext.EntMultilinkTextSec mulline = new ApplicationDbContext.EntMultilinkTextSec();

                                var getform = Request.Form["TempName"];
                                var gettopid = (from y in _context.SOPNewTemplate
                                                where y.TempName == getform
                                                select y.SOPTemplateID).Single();

                                mulline.NewTempId = gettopid;

                                _context.UsedMultilineText.Select(u => u.MultilineTextBlock);
                                _context.UsedMultilineText.Select(u => u.valuematch);

                                var textval = subitem.MultilineTextBlock;

                                var nospace = subitem.MultilineTextBlock.Replace(" ", "");
                                var newval = Request.Form["MultilineTextBlock-" + nospace];

                                mulline.MultilineTextBlock = newval;

                                var valuem = subitem.valuematch;
                                mulline.valuematch = valuem;

                                _context.Add(mulline);
                            }
                        }
                        foreach (var subitem in (ViewBag.GetTabs))
                        {
                            if (item.valuematch == subitem.valuematch)
                            {
                                ApplicationDbContext.EntTableSec table = new ApplicationDbContext.EntTableSec();

                                var getform = Request.Form["TempName"];
                                var gettopid = (from y in _context.SOPNewTemplate
                                                where y.TempName == getform
                                                select y.SOPTemplateID).Single();

                                table.NewTempId = gettopid;

                                _context.UsedMultilineText.Select(u => u.MultilineTextBlock);
                                _context.UsedMultilineText.Select(u => u.valuematch);

                                var textval = subitem.TableHTML;

                                var nospace = subitem.TableHTML.Replace(" ", "");
                                var newval = Request.Form["TableHTML-" + nospace];

                                table.TableHTML = newval;

                                var valuem = subitem.valuematch;
                                table.valuematch = valuem;

                                foreach (var tab in tablecol)
                                {
                                    ApplicationDbContext.EntTableSecCols cols = new ApplicationDbContext.EntTableSecCols();

                                    var subgetform = Request.Form["TempName"];
                                    var subgettopid = (from y in _context.SOPNewTemplate
                                                       where y.TempName == subgetform
                                                       select y.SOPTemplateID).Single();

                                    cols.NewTempId = subgettopid;

                                    var coltext = tab.RowText;
                                    cols.ColText = coltext;

                                    var theval = subitem.valuematch;
                                    cols.tableval = theval;

                                    _context.Add(cols);
                                }
                                foreach (var tabrows in tablerow)
                                {
                                    Console.WriteLine(tabrows.RowText);
                                    ApplicationDbContext.EntTableSecRows therows = new ApplicationDbContext.EntTableSecRows();

                                    var subgetform = Request.Form["TempName"];
                                    var subgettopid = (from y in _context.SOPNewTemplate
                                                       where y.TempName == subgetform
                                                       select y.SOPTemplateID).Single();

                                    therows.NewTempId = subgettopid;

                                    var gettext = tabrows.RowText;
                                    therows.RowText = gettext;

                                    var theval = subitem.valuematch;
                                    therows.tableval = theval;

                                    _context.Add(therows);
                                }
                                _context.Add(table);
                            }
                        }
                }
                  
            }
            _context.SaveChanges();
            var recname = Request.Form["TempName"];
            var getexe = (from y in _context.SOPNewTemplate
                          where y.TempName == recname
                          select y.SOPTemplateID).Single();

            string url1 = Url.Content("ProcessSteps" + Uri.EscapeUriString("?=") + getexe);
            string newurl = url1.Replace("%3F%3D", "?=");
            Console.WriteLine(newurl);
            return new RedirectResult(newurl);
        }

        [HttpPost]
        public PartialViewResult AddProcess()
        {
            return PartialView("_NewProcess", new Process());
        }

        [HttpPost]
        public PartialViewResult AddSubProcess()
        {
            return PartialView("_SubStep", new SubProcess());
        }

        [HttpPost]
        public PartialViewResult AddRACIResponsible()
        {
            var orderVM = new RACIResponsible();
            var getuser = _userManager.GetUserId(User);
            var compid = (from i in _context.CompanyClaim
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
            };

            return PartialView("RACIResponsible", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIAccountable()
        {
            var orderVM = new RACIAccountable();
            var getuser = _userManager.GetUserId(User);
            var compid = (from i in _context.CompanyClaim
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
            };

            return PartialView("RACIAccountable", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIInformed()
        {
            var orderVM = new RACIInformed();
            var getuser = _userManager.GetUserId(User);
            var compid = (from i in _context.CompanyClaim
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
            };

            return PartialView("RACIInformed", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIConsulted()
        {
            var orderVM = new RACIConsulted();
            var getuser = _userManager.GetUserId(User);
            var compid = (from i in _context.CompanyClaim
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
            };

            return PartialView("RACIConsulted", orderVM);
        }

        [HttpGet]
        public ActionResult ProcessSteps(string SOPTemplateID)
        {
            List<Process> model = new List<Process>();

            ViewBag.sopid = SOPTemplateID;

            var getu = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            return View(model);
        }

        [HttpPost]
        public ActionResult ProcessSteps(IList<IFormFile> files, string SOPTemplateID, [Bind(Prefix = "Process")]IEnumerable<Process> process, [Bind(Prefix = "SubProcess")]IEnumerable<SubProcess> SubProcess, [Bind(Prefix = "RACIResponsible")]IEnumerable<RACIResponsible> resp, [Bind(Prefix = "RACIAccountable")]IEnumerable<RACIAccountable> acc, [Bind(Prefix = "RACIInformed")]IEnumerable<RACIInformed> inf, [Bind(Prefix = "RACIConsulted")]IEnumerable<RACIConsulted> con)
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

            var getsop = SOPTemplateID;
            Console.WriteLine("SOPID");
            Console.WriteLine(getsop);

            foreach (var sub in resp)
            {
                ApplicationDbContext.RACIResposible racires = new ApplicationDbContext.RACIResposible();

                var subid = sub.valuematch;
                racires.valuematch = subid;

                var subidtwo = 123;
                racires.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                racires.DepartmentId = department;

                var sopid = getsop;
                racires.soptoptempid = getsop;

                var job = sub.JobTitleId;
                racires.JobTitleId = job;
                _context.Add(racires);
            }
            _context.SaveChanges();
            foreach (var sub in acc)
            {
                ApplicationDbContext.RACIAccount raciacc = new ApplicationDbContext.RACIAccount();

                var subid = sub.valuematch;
                raciacc.valuematch = subid;

                var subidtwo = 123;
                raciacc.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                raciacc.DepartmentId = department;

                raciacc.soptoptempid = getsop;

                var job = sub.JobTitleId;
                raciacc.JobTitleId = job;
                _context.Add(raciacc);
            }
            _context.SaveChanges();
            foreach (var sub in con)
            {
                ApplicationDbContext.RACIConsulted racicon = new ApplicationDbContext.RACIConsulted();

                var subid = sub.valuematch;
                racicon.valuematch = subid;

                var subidtwo = 123;
                racicon.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                racicon.DepartmentId = department;

                racicon.soptoptempid = getsop;

                var job = sub.JobTitleId;
                racicon.JobTitleId = job;
                _context.Add(racicon);
            }
            _context.SaveChanges();
            foreach (var sub in inf)
            {
                ApplicationDbContext.RACIInformed raciinfo = new ApplicationDbContext.RACIInformed();

                var subid = sub.valuematch;
                raciinfo.valuematch = subid;

                var subidtwo = 123;
                raciinfo.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                raciinfo.DepartmentId = department;

                raciinfo.soptoptempid = getsop;

                var job = sub.JobTitleId;
                raciinfo.JobTitleId = job;
                _context.Add(raciinfo);
            }
            _context.SaveChanges();
            foreach (var item in process) {
                ApplicationDbContext.SOPTemplateProcesses soppro = new ApplicationDbContext.SOPTemplateProcesses();

                soppro.ProcessType = "MainProcess";

                var name = item.ProcessName;
                soppro.ProcessName = name;

                var desc = item.ProcessDesc;
                soppro.ProcessDesc = desc;

                soppro.processStatus = "Pending";

                var getid = item.valuematch;
                soppro.valuematch = getid;

                foreach (var file in files)
                {

                }

                soppro.SOPTemplateID = getsop;
                _context.Add(soppro);
            }
            foreach (var item in SubProcess)
            {
                ApplicationDbContext.SOPTemplateProcesses soppro = new ApplicationDbContext.SOPTemplateProcesses();

                soppro.ProcessType = "SubProcess";
                soppro.processStatus = "Pending";
                var name = item.ProcessName;
                soppro.ProcessName = name;

                var desc = item.ProcessDesc;
                soppro.ProcessDesc = desc;

                var getid = item.valuematch;
                soppro.valuematch = getid;

                soppro.SOPTemplateID = getsop;

                Console.WriteLine(name);
                Console.WriteLine(desc);

                _context.Add(soppro);
            }
            _context.SaveChanges();

            string url1 = Url.Content("/Manage/SOPTemplates");
            string newurl = url1.Replace("%3F%3D", "?=");
            Console.WriteLine(newurl);
            return new RedirectResult(newurl);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult People(List<RegisterSOPUserViewModel> list)
        {
            var orderVM = new RegisterSOPUserViewModel();
            var getuser = _userManager.GetUserId(User);
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getuser
                        select i.CompanyId).Single();
            ViewBag.comp = comp;
            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == comp
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var compid = (from i in _context.CompanyClaim
                          where i.UserId == getuser
                          select i.CompanyId).Single();

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

            var thepeople = (from m in _context.CompanyClaim
                             join p in _context.Users on m.UserId equals p.Id
                             join d in _context.Departments on m.DepartmentId equals d.DepartmentId
                             join j in _context.JobTitles on m.JobTitleId equals j.JobTitleId
                             where p.Id == m.UserId
                             select new RegisterSOPUserViewModel { UserId = p.Id, Email = p.Email, FirstName = m.FirstName, SecondName = m.SecondName, DepartmentName = d.DepartmentName, JobTitle = j.JobTitle }).ToList();
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
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var getu = _userManager.GetUserId(User);



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

            EditUser edituser = new EditUser(){
                Id = user.Id,
                Email = user.Email,
                FirstName = firstName,
                SecondName = SecondName,
            };

            return View(edituser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUser model, [Bind("FirstName,SecondName")]ApplicationDbContext.ClaimComp extrau)
        {
            if (ModelState.IsValid)
            {
               

                _context.SaveChanges();
            }
            return View();
        }


        public ActionResult PeopleList(){

            return View();
        }

        public IActionResult DepartmentList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OrgStructure()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrgStructure(IFormFile file)
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

            if (file == null || file.Length == 0)
            {
                return View();
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                /* var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads/CSV",
                    userId + "-" + file.FileName); */
                var path = Path.Combine(
                    "D:/home/site/wwwroot/uploads/CSV",
                    userId + "-" + file.FileName);
                Console.WriteLine("CHRIS");
                Console.WriteLine(path, file.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return RedirectToAction("CSVMap", new {file = userId + "-" + file.FileName});
            }
        }

        [HttpGet]
        public ActionResult CSVMap()
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

            var filePath = Request.Query["file"];
            /* var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads/CSV",
                    filePath); */
            var path = Path.Combine(
                "~/uploads/CSV",
                "D:/home/site/wwwroot/uploads/CSV",
                filePath);

            int failed = 0;
            List<CSVMapModel> rows = new List<CSVMapModel>();
            using (StreamReader sr = new StreamReader(path))
            {
                CsvHelper.CsvReader csv = new CsvHelper.CsvReader(sr);
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {   
                    try
                    {
                        var record = csv.GetRecord<CSVMapModel>();
                        if(String.IsNullOrEmpty(record.Department) || String.IsNullOrEmpty(record.Email) || String.IsNullOrEmpty(record.JobTitle) || String.IsNullOrEmpty(record.FirstName) || String.IsNullOrEmpty(record.SecondName))
                        {
                            failed++;
                        }
                        else
                        {
                            rows.Add(record);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            ViewBag.Rows = rows;
            ViewBag.Failed = failed;
            ViewBag.File = filePath;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CSVMap(IFormFile file, RegisterSopCreatorViewModel model, [Bind("ClaimId,FirstName,SecondName,CompanyId")] ApplicationDbContext.ClaimComp compclaim)
        {
            
            var currentuser = await _userManager.GetUserAsync(User);
            var user_id = currentuser.Id;
            var modules = (from i in _context.CompanyClaim
                           where i.UserId == user_id
                           select i.CompanyId).Single();


            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == modules
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var filePath = Request.Query["file"];
            var path = Path.Combine(
                "D:/home/site/wwwroot/uploads/CSV",
                   filePath);

            int failed = 0;
            List<CSVMapModel> rows = new List<CSVMapModel>();
            using (StreamReader sr = new StreamReader(path))
            {
                CsvHelper.CsvReader csv = new CsvHelper.CsvReader(sr);
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    try
                    {
                        var record = csv.GetRecord<CSVMapModel>();
                        if (String.IsNullOrEmpty(record.Department) || String.IsNullOrEmpty(record.Email) || String.IsNullOrEmpty(record.JobTitle) || String.IsNullOrEmpty(record.FirstName) || String.IsNullOrEmpty(record.SecondName))
                        {
                            failed++;
                        }
                        else
                        {
                            rows.Add(record);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            ViewBag.Rows = rows;
            ViewBag.Failed = failed;

            foreach(var row in rows)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                await _userManager.AddToRoleAsync(user, "SOPCreator");
                var result = await _userManager.CreateAsync(user, model.Password);

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

                // If we got this far, the process succeeded
                var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                Console.WriteLine(apiKey);

                var client = new SendGridClient(apiKey);


                var loggedInEmail = _userManager.GetUserName(User);
                var newUserEmail = model.Email;
                var newUserPassword = model.Password;
                var firstName = Request.Form["FirstName"];
                var secondName = Request.Form["SecondName"];

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(loggedInEmail, "SOPMan"),
                    Subject = "You have been registered as a SOPMan Creator",
                    PlainTextContent = "Hello, " + firstName + " " + secondName,
                    HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>Your username is: " + newUserEmail + "<br>Your password is: " + newUserPassword
                };
                Console.WriteLine(msg);
                msg.AddTo(new EmailAddress(newUserEmail, firstName + " " + secondName));
                var response = await client.SendEmailAsync(msg);
                Console.WriteLine(response);

                await _context.SaveChangesAsync();

            }
            return RedirectToAction("CSVMap", path);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitCSV(RegisterSopCreatorViewModel model, [Bind("ClaimId,FirstName,SecondName,CompanyId")] ApplicationDbContext.ClaimComp compclaim, [Bind("DepartmentName,CompanyId")] ApplicationDbContext.DepartmentTable departments, [Bind("JobTitleId,JobTitle,DepartmentId,CompanyId")] ApplicationDbContext.JobTitlesTable jobtitle )
        {
            ViewBag.File = Request.Query["file"];



            var currentuser = await _userManager.GetUserAsync(User);
            var user_id = currentuser.Id;
            var modules = (from i in _context.CompanyClaim
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            var logostring = (from i in _context.TheCompanyInfo
                              where i.CompanyId == modules
                              select i.Logo).Single();
            ViewBag.logostring = logostring;

            var filePath = Request.Query["file"];
            var path = Path.Combine(
                   Directory.GetCurrentDirectory(),
                "D:/home/site/wwwroot/uploads/CSV",
                   filePath);

            int failed = 0;
            List<CSVMapModel> rows = new List<CSVMapModel>();
            using (StreamReader sr = new StreamReader(path))
            {
                CsvHelper.CsvReader csv = new CsvHelper.CsvReader(sr);
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    try
                    {
                        var record = csv.GetRecord<CSVMapModel>();
                        if (String.IsNullOrEmpty(record.Department) || String.IsNullOrEmpty(record.Email) || String.IsNullOrEmpty(record.JobTitle) || String.IsNullOrEmpty(record.FirstName) || String.IsNullOrEmpty(record.SecondName))
                        {
                            failed++;
                        }
                        else
                        {
                            rows.Add(record);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            ViewBag.Rows = rows;
            ViewBag.Failed = failed;

            foreach (var row in rows)
            {
                var user = new ApplicationUser { UserName = row.Email, Email = row.Email };
                // await _userManager.AddToRoleAsync(user, "SOPCreator");

                var pw = Extensions.Password.Generate(32, 12);
                var result = await _userManager.CreateAsync(user, pw);

                await _context.SaveChangesAsync();

                ApplicationDbContext.ClaimComp claims = new ApplicationDbContext.ClaimComp();

                _context.CompanyClaim.Select(u => u.UserId);
                var getnewuser = (from u in _context.Users
                                  where u.Email == row.Email
                                  select u.Id).FirstOrDefault();
                var newuserid = user.Id;
                claims.UserId = getnewuser;


                var checkdeps = (from x in _context.Departments
                                 where x.DepartmentName == row.Department
                                 select x.DepartmentName).FirstOrDefault();

                var checkdepscomp = (from x in _context.Departments
                                 where x.DepartmentName == row.Department
                                 select x.CompanyId).FirstOrDefault();

                var writedep = row.Department;
                ViewBag.writedep = writedep;

                if(writedep != checkdeps){
                    if(modules != checkdepscomp){
                        ApplicationDbContext.DepartmentTable deps = new ApplicationDbContext.DepartmentTable();
                        _context.Departments.Select(u => u.DepartmentName);
                        deps.DepartmentName = row.Department;
                        deps.CompanyId = modules;

                        _context.Add(deps);
                        await _context.SaveChangesAsync();
                    }
                }if (writedep == checkdeps)
                {
                    if (modules != checkdepscomp)
                    {
                        ApplicationDbContext.DepartmentTable deps = new ApplicationDbContext.DepartmentTable();
                        _context.Departments.Select(u => u.DepartmentName);
                        deps.DepartmentName = row.Department;
                        deps.CompanyId = modules;

                        _context.Add(deps);
                        await _context.SaveChangesAsync();
                    }
                }


                var getdepid = (from i in _context.Departments
                                where i.DepartmentName == row.Department
                                select i.DepartmentId).FirstOrDefault();

                claims.DepartmentId = getdepid;

                var checkjobs = (from x in _context.JobTitles
                                 where x.JobTitle == row.JobTitle
                                 select x.JobTitle).FirstOrDefault();
                @ViewBag.checkjobs = row.JobTitle;

                var checkjobscomp = (from x in _context.JobTitles
                                 where x.JobTitle == row.JobTitle
                                 select x.CompanyId).FirstOrDefault();

                var writejob = row.JobTitle;

                if(writejob != checkjobs){
                    if(modules != checkjobscomp){
                        ApplicationDbContext.JobTitlesTable jobs = new ApplicationDbContext.JobTitlesTable();
                        _context.CompanyClaim.Select(u => u.JobTitleId);
                        jobs.JobTitle = row.JobTitle;
                        jobs.DepartmentId = getdepid;
                        jobs.CompanyId = modules;

                        _context.Add(jobs);
                        await _context.SaveChangesAsync();
                    }
                }if (writejob == checkjobs)
                {
                    if (modules != checkjobscomp)
                    {
                        ApplicationDbContext.JobTitlesTable jobs = new ApplicationDbContext.JobTitlesTable();
                        _context.CompanyClaim.Select(u => u.JobTitleId);
                        jobs.JobTitle = row.JobTitle;
                        jobs.DepartmentId = getdepid;
                        jobs.CompanyId = modules;

                        _context.Add(jobs);
                        await _context.SaveChangesAsync();
                    }
                }

                var getjobid = (from j in _context.JobTitles
                                where j.DepartmentId == getdepid
                                select j.JobTitleId).FirstOrDefault();

                claims.JobTitleId = getjobid;

                claims.FirstName = row.FirstName;
                claims.SecondName = row.SecondName;

                _context.CompanyClaim.Select(u => u.CompanyId);
                claims.CompanyId = modules;

                _context.Add(claims);

                // If we got this far, the process succeeded
                var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
                Console.WriteLine(apiKey);

                var client = new SendGridClient(apiKey);

                var loggedInEmail = _userManager.GetUserName(User);
                var newUserEmail = row.Email;
                var newUserPassword = pw;
                var firstName = row.FirstName;
                var secondName = row.SecondName;

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(loggedInEmail, "SOPMan"),
                    Subject = "You have been registered as a SOPMan Creator",
                    PlainTextContent = "Hello, " + firstName + " " + secondName,
                    HtmlContent = "Hello, " + firstName + " " + secondName + ",<br>Your username is: " + newUserEmail + "<br>Your password is: " + newUserPassword
                };
                Console.WriteLine(msg);
                msg.AddTo(new EmailAddress(newUserEmail, firstName + " " + secondName));
                var response = await client.SendEmailAsync(msg);
                Console.WriteLine(response);

                await _context.SaveChangesAsync();

            }
            string url1 = Url.Content("/Setup/SetupIndex");
            return new RedirectResult(url1);
        }
    }  
}
