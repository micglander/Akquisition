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
    public class LinksController : Controller
    {
        // Anzuzeigende Elemente
        private int elemente = 4;

        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            List<tbl_Links> liste = new List<tbl_Links>();
            for (int i = 0; i < elemente; i++)
            {
                tbl_Links rel = new tbl_Links();
                rel.EntryNo = EntryNo;
                liste.Add(rel);
            }

            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(List<tbl_Links> liste)
        {
            IEnumerable<tbl_Links> newItems = liste.Where(x => !String.IsNullOrEmpty(x.URL));
            if (newItems.Count() > 0)
            {
                db.tbl_Links.AddRange(newItems);
                await db.SaveChangesAsync();
            }

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }


        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_Links> liste = db.tbl_Links.Where(x => x.EntryNo == EntryNo).ToList();
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(List<tbl_Links> liste)
        {
            foreach (tbl_Links entity in liste)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }


        public async Task<ActionResult> Delete(int ID)
        {
             tbl_Links link = await db.tbl_Links.FindAsync(ID);
             db.tbl_Links.Remove(link);
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
