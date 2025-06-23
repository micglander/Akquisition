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
    [LoggedOrAuthorized(Roles = "DataReader,DataWriter")]
    public class FirmenController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Index()
        {
            return View(await db.tbl_Firmen.OrderBy(x => x.Firma).ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Firmen tbl_Firmen = await db.tbl_Firmen.FindAsync(id);
            if (tbl_Firmen == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Firmen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public ActionResult Create()
        {
            return View();
        }


        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FirmaNr,Firma,ImdbID,Companymeter,Job")] tbl_Firmen tbl_Firmen)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Firmen.Add(tbl_Firmen);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbl_Firmen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Firmen tbl_Firmen = await db.tbl_Firmen.FindAsync(id);
            if (tbl_Firmen == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Firmen);
        }


        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FirmaNr,Firma,ImdbID,Companymeter,Job")] tbl_Firmen tbl_Firmen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Firmen).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tbl_Firmen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Firmen tbl_Firmen = await db.tbl_Firmen.FindAsync(id);
            if (tbl_Firmen == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Firmen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Firmen tbl_Firmen = await db.tbl_Firmen.FindAsync(id);
            db.tbl_Firmen.Remove(tbl_Firmen);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
