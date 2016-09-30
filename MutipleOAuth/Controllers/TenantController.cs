using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MutipleOAuth.Controllers
{
    [Authorize]
    public class TenantController : Controller
    {
        // GET: Tenant
        public ActionResult Index()
        {
            return View();
        }
    }
}