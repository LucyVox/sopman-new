using System;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Security.Claims;
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
using sopman.Models.AccountViewModels;
using sopman.Models.SetupViewModels;
using sopman.Services;

namespace sopman.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SetupController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;


        public SetupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [TempData]
        public string ErrorMessage { get; set; }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult SetupIndex()
        {
            var getuser = _userManager.GetUserId(User);

            if (_context.TheCompanyInfo.Any(o => o.UserId == getuser))
            {
                ViewBag.Class = "true";

                var theid = (from m in _context.TheCompanyInfo
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

            var modules = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();



            return View();

            /*

             var companies = from m in _context.TheCompanyInfo
                          select m;

             return View(model); */

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
        public async Task<IActionResult> CompanySetup([Bind("Id,Name,Logo,SOPNumberFormat,SOPStartNumber,UserId")] ApplicationDbContext.CompanyInfo company, [Bind("ClaimId,CompanyId,UserId,FirstName,SecondName")] ApplicationDbContext.ClaimComp claim)
        {
            var user = await _userManager.GetUserAsync(User);


            if (ModelState.IsValid)
            {
                _context.TheCompanyInfo.Select(u => u.UserId);
                var user_id = user.Id;
                company.UserId = user_id;
                claim.UserId = user_id;

                _context.TheCompanyInfo.Select(u => u.Logo);
                var file = company.Logo;

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

            var modules = (from i in _context.TheCompanyInfo
                           where i.UserId == user_id
                           select i.CompanyId).Single();

            var usermod = (from i in _context.TheCompanyInfo
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

                    var getvalue = sub.valuematch;
                    newtable.valuematch = getvalue;

                    _context.Add(newtable);
                }
                _context.SaveChanges();
                return RedirectToAction("SetupIndex", "Setup");
            }
            return View(soptop);
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
            return PartialView("_SectionOutput", new SopTemplate());
        }

        [HttpGet]
        public ActionResult NewTemplate()
        {
            var getu = _userManager.GetUserId(User);
            var newsec = new SopTemplate();
            var comp = (from i in _context.CompanyClaim
                        where i.UserId == getu
                        select i.CompanyId).Single();

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
        public ActionResult NewTemplate([Bind("TempName,SOPCode,TopTempId,ExpireDate")]ApplicationDbContext.SOPTemplate soptemp, [Bind("SingleLinkTextBlock,valuematch")]ApplicationDbContext.EntSingleLinkTextSec GetSingle,[Bind(Prefix = "NewColumn")]IEnumerable<AddTable> tablecol,[Bind(Prefix = "AddTableRow")]IEnumerable<AddTableRow> tablerow)
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
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

            if (ModelState.IsValid)
            {
                soptemp.TopTempId = topid;


                _context.Add(soptemp);
                _context.SaveChanges();
                foreach(var item in (ViewBag.Secs)){
                    foreach (var subitem in (ViewBag.GetSin))
                    {
                        if(item.valuematch == subitem.valuematch){
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
                            var newval = Request.Form["SingleLinkTextBlock-"+ nospace];

                            Console.WriteLine(newval);
                            sinline.SingleLinkTextBlock = newval;

                            var valuem = subitem.valuematch;
                            sinline.valuematch = valuem;

                            _context.Add(sinline);
                        }
                    }
                    foreach(var subitem in (ViewBag.GetMul)){
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

                            foreach(var tab in tablecol){
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
                            foreach(var tabrows in tablerow){
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
                _context.SaveChanges();
            }
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
            };

            return PartialView("RACIResponsible", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIAccountable()
        {
            var orderVM = new RACIAccountable();
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
            };

            return PartialView("RACIAccountable", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIInformed()
        {
            var orderVM = new RACIInformed();
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
            };

            return PartialView("RACIInformed", orderVM);
        }

        [HttpPost]
        public PartialViewResult AddRACIConsulted()
        {
            var orderVM = new RACIConsulted();
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
            };

            return PartialView("RACIConsulted", orderVM);
        }

        [HttpGet]
        public ActionResult ProcessSteps(string SOPTemplateID)
        {
            List<Process> model = new List<Process>();

            ViewBag.sopid = SOPTemplateID;

            return View(model);
        }

        [HttpPost]
        public ActionResult ProcessSteps(IList<IFormFile> files, string SOPTemplateID, [Bind(Prefix = "Process")]IEnumerable<Process> process, [Bind(Prefix = "SubProcess")]IEnumerable<SubProcess> SubProcess, [Bind(Prefix = "RACIResponsible")]IEnumerable<RACIResponsible> resp, [Bind(Prefix = "RACIAccountable")]IEnumerable<RACIAccountable> acc, [Bind(Prefix = "RACIInformed")]IEnumerable<RACIInformed> inf, [Bind(Prefix = "RACIConsulted")]IEnumerable<RACIConsulted> con)
        {
            var getuser = _userManager.GetUserId(User);


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

                var job = sub.JobTitleId;
                racires.JobTitleId = job;
                _context.Add(racires);
            }
            _context.SaveChanges();
            foreach (var sub in acc)
            {
                ApplicationDbContext.RACIAccount racires = new ApplicationDbContext.RACIAccount();

                var subid = sub.valuematch;
                racires.valuematch = subid;

                var subidtwo = 123;
                racires.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                racires.DepartmentId = department;

                var job = sub.JobTitleId;
                racires.JobTitleId = job;
                _context.Add(racires);
            }
            _context.SaveChanges();
            foreach (var sub in con)
            {
                ApplicationDbContext.RACIConsulted racires = new ApplicationDbContext.RACIConsulted();

                var subid = sub.valuematch;
                racires.valuematch = subid;

                var subidtwo = 123;
                racires.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                racires.DepartmentId = department;

                var job = sub.JobTitleId;
                racires.JobTitleId = job;
                _context.Add(racires);
            }
            _context.SaveChanges();
            foreach (var sub in inf)
            {
                ApplicationDbContext.RACIInformed racires = new ApplicationDbContext.RACIInformed();

                var subid = sub.valuematch;
                racires.valuematch = subid;

                var subidtwo = 123;
                racires.SOPTemplateProcessID = subidtwo;

                var department = sub.DepartmentId;
                racires.DepartmentId = department;

                var job = sub.JobTitleId;
                racires.JobTitleId = job;
                _context.Add(racires);
            }
            _context.SaveChanges();
            foreach (var item in process) {
                ApplicationDbContext.SOPTemplateProcesses soppro = new ApplicationDbContext.SOPTemplateProcesses();

                soppro.ProcessType = "MainProcess";

                var name = item.ProcessName;
                soppro.ProcessName = name;

                var desc = item.ProcessDesc;
                soppro.ProcessDesc = desc;

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

            var compid = (from i in _context.TheCompanyInfo
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

        public IActionResult OrgStructure()
        {
            return View();
        }


    }  
}
