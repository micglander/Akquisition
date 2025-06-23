using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using efAkquisition;

namespace Akquisition.Controllers
{
     [LoggedOrAuthorized(Roles = "DataReader,DataWriter")]
    public class ThemenController : Controller
    {

        // Anzuzeigende Elemente
        private int elemente = 4;

        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            List<tbl_Themen> liste = new List<tbl_Themen>();
            for (int i = 0; i < elemente; i++)
            {
                tbl_Themen rel = new tbl_Themen();
                rel.EntryNo = EntryNo;
                rel.ThemaID = 0;
                liste.Add(rel);
            }

            

            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Add(List<tbl_Themen> liste)
        {
            IEnumerable<tbl_Themen> newItems = liste.Where(x => !String.IsNullOrEmpty(x.Thema));
            if (newItems.Count() > 0)
            {
                db.tbl_Themen.AddRange(newItems);
                await db.SaveChangesAsync();
            }

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_Themen> liste = db.tbl_Themen.Where(x => x.EntryNo == EntryNo).ToList();
            if (liste.Count == 0)
            {
                return null;
            }

            // Titel
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(List<tbl_Themen> liste)
        {
            foreach (tbl_Themen entity in liste)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Delete(int ID)
        {
            tbl_Themen rel = await db.tbl_Themen.FindAsync(ID);
            db.tbl_Themen.Remove(rel);
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