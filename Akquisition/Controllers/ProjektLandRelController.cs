using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using efAkquisition;

namespace Akquisition.Controllers
{
    [LoggedOrAuthorized(Roles = "DataWriter")]
    public class ProjektLandRelController : Controller
    {
        // Anzuzeigende Elemente
        private int elemente = 4;

        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            List<tbl_ProjektLandRel> liste = new List<tbl_ProjektLandRel>();
            for (int i = 0; i < elemente; i++ )
            {
                tbl_ProjektLandRel rel = new tbl_ProjektLandRel();
                rel.EntryNo = EntryNo;
                rel.LandNr = 0;
                liste.Add(rel);
            }

            // Länder für die Comboboxen
            ViewBag.Auswahl = db.Laender;

            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Add(List<tbl_ProjektLandRel> liste)
        {
            IEnumerable<tbl_ProjektLandRel> newItems = liste.Where(x => x.LandNr != 0);
            if (newItems.Count() > 0)
            {
                db.tbl_ProjektLandRel.AddRange(newItems);
                await db.SaveChangesAsync();
            }

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_ProjektLandRel> liste = db.tbl_ProjektLandRel.Where(x => x.EntryNo == EntryNo).ToList();
            if (liste.Count == 0)
            {
                return null;
            }

            // Genres für die Comboboxen
            ViewBag.Auswahl = db.Laender;

            // Titel
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(List<tbl_ProjektLandRel> liste)
        {
            foreach (tbl_ProjektLandRel entity in liste)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Delete(int ID)
        {
            tbl_ProjektLandRel rel = await db.tbl_ProjektLandRel.FindAsync(ID);
            db.tbl_ProjektLandRel.Remove(rel);
            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}