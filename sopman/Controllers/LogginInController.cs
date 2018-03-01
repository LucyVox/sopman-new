using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public class LoggedInViewModel : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LoggedInViewModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [TempData]
        public string ErrorMessage { get; set; }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult _LoggedInLogo()
        {
            var getuser = _userManager.GetUserId(User);

            var compid = (from i in _context.TheCompanyInfo
                          where i.UserId == getuser
                          select i.Name).Single();

            if(compid != null){
               
            }

            return View(compid);

        }


    }
}
