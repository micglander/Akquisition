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
    public class LandController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        // GET: Land
        public async Task<ActionResult> Index()
        {
            List<tbl_Land> laender = db.Laender;
            return View(laender);
        }

        // GET: Land/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Land tbl_Land = await db.tbl_Land.FindAsync(id);
            if (tbl_Land == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Land);
        }

        // GET: Land/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Land/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(tbl_Land tbl_Land)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Land.Add(tbl_Land);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbl_Land);
        }

        // GET: Land/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Land tbl_Land = await db.tbl_Land.FindAsync(id);
            if (tbl_Land == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Land);
        }

        // POST: Land/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LandNr,Land,Sortierung")] tbl_Land tbl_Land)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Land).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tbl_Land);
        }

        // GET: Land/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Land tbl_Land = await db.tbl_Land.FindAsync(id);
            if (tbl_Land == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Land);
        }

        // POST: Land/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Land tbl_Land = await db.tbl_Land.FindAsync(id);
            db.tbl_Land.Remove(tbl_Land);
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
