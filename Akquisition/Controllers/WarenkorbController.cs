using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using efAkquisition;

namespace Akquisition.Controllers
{
    public class WarenkorbController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Index()
        {
            // aktueller Benutzer
            string login = User.Identity.Name.ToLower();

            // Sein Warenkorb
            tbl_Warenkorb warenkorb = db.tbl_Warenkorb.Where(x => x.Benutzer.Equals(login)).FirstOrDefault();
            List<tbl_WarenkorbProjektRel> mylist = new List<tbl_WarenkorbProjektRel>();
            if (warenkorb != null)
            {
                mylist = warenkorb.tbl_WarenkorbProjektRel.OrderByDescending(x => x.Prio).ToList();
            }
            return View(mylist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(List<tbl_WarenkorbProjektRel> liste)
        {
            if (ModelState.IsValid)
            {
                foreach (tbl_WarenkorbProjektRel entity in liste)
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(liste);
        }

        public async Task<ActionResult> Delete(int id)
        {
            tbl_WarenkorbProjektRel tbl_WarenkorbProjektRel = await db.tbl_WarenkorbProjektRel.FindAsync(id);
            db.tbl_WarenkorbProjektRel.Remove(tbl_WarenkorbProjektRel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> AddToWarenkorb(int id)
        {
            // aktueller Benutzer
            string login = User.Identity.Name.ToLower();

            // Sein Warenkorb
            tbl_Warenkorb warenkorb = db.tbl_Warenkorb.Where(x => x.Benutzer.Equals(login)).FirstOrDefault();

            if (warenkorb == null)
                return HttpNotFound("Für diesen Benutzer können keine Favoriten gefunden werden gefunden weden!");

            tbl_WarenkorbProjektRel rel = new tbl_WarenkorbProjektRel();
            rel.EntryNo = id;
            warenkorb.tbl_WarenkorbProjektRel.Add(rel);

            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
