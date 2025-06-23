using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akquisition.Controllers
{
    [LoggedOrAuthorized(Roles = "DataWriter")]
    public class GeneralController : Controller
    {
        
        public ActionResult UpdateFestivalAkt()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateFestivalAkt(int? dummy)
        {
            efAkquisition.AkquiseEntities db = new efAkquisition.AkquiseEntities();
            db.sp_UpdateFestivalAkt();
            return RedirectToAction("FilterVordefiniert", "Projekte", new { was = "Marktliste" }); //"Marktliste", "FilterVordefiniert", "Projekte", new { was = "Marktliste" }, null
        }
    }
}