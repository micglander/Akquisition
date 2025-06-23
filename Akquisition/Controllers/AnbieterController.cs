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
    [LoggedOrAuthorized(Roles ="DataWriter,DataReader")]
    public class AnbieterController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        // GET: Anbieter
        public async Task<ActionResult> Index()
        {
            var tbl_Anbieter = db.tbl_Anbieter.Include(t => t.tbl_Bearbeiter).OrderBy(x => x.Kürzel);
            return View(await tbl_Anbieter.ToListAsync());
        }

        // GET: Anbieter/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Anbieter tbl_Anbieter = await db.tbl_Anbieter.FindAsync(id);
            if (tbl_Anbieter == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Anbieter);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public ActionResult Create()
        {
            ViewBag.zuständig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");
            return View();
        }

        // POST: Anbieter/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AnbieterNr,Kürzel,Kontaktperson,zuständig,AnbieterAF")] tbl_Anbieter tbl_Anbieter)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Anbieter.Add(tbl_Anbieter);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.zuständig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", tbl_Anbieter.zuständig);
            return View(tbl_Anbieter);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Anbieter tbl_Anbieter = await db.tbl_Anbieter.FindAsync(id);
            if (tbl_Anbieter == null)
            {
                return HttpNotFound();
            }
            ViewBag.zuständig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", tbl_Anbieter.zuständig);
            return View(tbl_Anbieter);
        }

        // POST: Anbieter/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AnbieterNr,Kürzel,Kontaktperson,zuständig,AnbieterAF")] tbl_Anbieter tbl_Anbieter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Anbieter).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception exc)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            ViewBag.zuständig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", tbl_Anbieter.zuständig);
            return View(tbl_Anbieter);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Anbieter tbl_Anbieter = await db.tbl_Anbieter.FindAsync(id);
            if (tbl_Anbieter == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Anbieter);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Anbieter tbl_Anbieter = await db.tbl_Anbieter.FindAsync(id);
            db.tbl_Anbieter.Remove(tbl_Anbieter);
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
