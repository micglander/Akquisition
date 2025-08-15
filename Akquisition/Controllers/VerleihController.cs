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
    public class VerleihController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        // GET: Verleih
        public async Task<ActionResult> Index()
        {
            return View(await db.tbl_Verleih.OrderBy(x => x.Verleih).ToListAsync());
        }

        // GET: Verleih/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Verleih tbl_Verleih = await db.tbl_Verleih.FindAsync(id);
            if (tbl_Verleih == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Verleih);
        }

        // GET: Verleih/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Verleih/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "VerleihNr,Verleih")] tbl_Verleih tbl_Verleih)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Verleih.Add(tbl_Verleih);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbl_Verleih);
        }

        // GET: Verleih/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Verleih tbl_Verleih = await db.tbl_Verleih.FindAsync(id);
            if (tbl_Verleih == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Verleih);
        }

        // POST: Verleih/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "VerleihNr,Verleih")] tbl_Verleih tbl_Verleih)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Verleih).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tbl_Verleih);
        }

        // GET: Verleih/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Verleih tbl_Verleih = await db.tbl_Verleih.FindAsync(id);
            if (tbl_Verleih == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Verleih);
        }

        // POST: Verleih/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Verleih tbl_Verleih = await db.tbl_Verleih.FindAsync(id);
            db.tbl_Verleih.Remove(tbl_Verleih);
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
