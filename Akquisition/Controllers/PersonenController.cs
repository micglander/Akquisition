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
    
    public class PersonenController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Filter()
        {
            return View(new tbl_Personen());
        }

        public async Task<ActionResult> Index(tbl_Personen person)
        {
            string criteria;

            // Ein Kriterium wurde angegeben
            if (person != null && !String.IsNullOrEmpty(person.Name))
            {
                // Personenfilter neu setzen
                System.Web.HttpContext.Current.Session["Personenfilter"] = person;
                
                criteria = person.Name;
                return View(await db.tbl_Personen.Where(x => x.Matchname.Contains(criteria)).OrderBy(x => x.Name).ToListAsync());
            }

            // Falls nicht, gespeicherten Filter auslesen
            tbl_Personen savedPerson = System.Web.HttpContext.Current.Session["Personenfilter"] as tbl_Personen;
            if (savedPerson != null)
            {
                criteria = savedPerson.Name;
                return View(await db.tbl_Personen.Where(x => x.Name.Contains(criteria)).OrderBy(x => x.Name).ToListAsync());
            }

            // Alle Personen anzeigen, wenn weder Filter angegeben noch gespeichert
            return View(await db.tbl_Personen.OrderBy(x => x.Name).ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Personen tbl_Personen = await db.tbl_Personen.FindAsync(id);
            if (tbl_Personen == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Personen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public ActionResult Create()
        {
            ViewBag.JobOne = new SelectList(db.tbl_Jobs, "JobNr", "Job");
            ViewBag.JobTwo = new SelectList(db.tbl_Jobs, "JobNr", "Job");
            ViewBag.JobThree = new SelectList(db.tbl_Jobs, "JobNr", "Job");
            return View();
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(tbl_Personen tbl_Personen)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Personen.Add(tbl_Personen);
                await db.SaveChangesAsync();
                System.Web.HttpContext.Current.Session["Personenfilter"] = tbl_Personen;
                return RedirectToAction("Filter");
            }

            return View(tbl_Personen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Personen person = await db.tbl_Personen.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            //ViewBag.JobOne = new SelectList(db.tbl_Jobs, "JobNr", "Job", person.JobOne);
            //ViewBag.JobTwo = new SelectList(db.tbl_Jobs, "JobNr", "Job", person.JobTwo);
            //ViewBag.JobThree = new SelectList(db.tbl_Jobs, "JobNr", "Job", person.JobThree);
            

            return View(person);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(tbl_Personen tbl_Personen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Personen).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Filter");
            }
            return View(tbl_Personen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Personen tbl_Personen = await db.tbl_Personen.FindAsync(id);
            if (tbl_Personen == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Personen);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Personen tbl_Personen = await db.tbl_Personen.FindAsync(id);
            db.tbl_Personen.Remove(tbl_Personen);
            await db.SaveChangesAsync();
            return RedirectToAction("Filter");
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
