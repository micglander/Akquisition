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
    public class ProjektGenreRelController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            // Billiger Trick:
            // Die neuen Genres werden in einer List<int> gespeichert, an die 0te Poition kommt die EntryNo des Projekts

            List<int> liste = new List<int>(4);
            liste.Add(EntryNo);
            for (int i = 0; i < 4; i++)
            {
                liste.Add(0);
            }

            // ViewBags
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;
            ViewBag.Genres = db.tbl_Genre.AsEnumerable<tbl_Genre>().OrderBy(x => x.Genre);

            return View(liste);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(List<int> liste)
        {
            bool dirty = false;

            int entryNo = liste[0];
            tbl_Projekte projekt = db.tbl_Projekte.Find(entryNo);
            for (int i = 1; i < liste.Count; i++ )
            {
                if (liste[i] != 0)
                {
                    tbl_ProjektGenreRel rel = new tbl_ProjektGenreRel();
                    rel.EntryNo = entryNo;
                    rel.GenreNr = liste[i];
                    projekt.tbl_ProjektGenreRel.Add(rel);
                    dirty = true;
                }
            }
            
            if (dirty)
                db.SaveChanges();

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = entryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_ProjektGenreRel> liste = db.tbl_ProjektGenreRel.Where(x => x.EntryNo == EntryNo).ToList();
            if (liste.Count == 0)
            {
                return null;
            }

            // Genres für die Comboboxen
            ViewBag.Auswahl = db.tbl_Genre.OrderBy(x => x.Genre).ToList();

            // Titel
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }
        
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<ActionResult> Edit(List<tbl_ProjektGenreRel> liste)
         {
             foreach (tbl_ProjektGenreRel entity in liste)
             {
                 db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
             }
             await db.SaveChangesAsync();

             return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
         }

         public async Task<ActionResult> Delete(int ID)
         {
             tbl_ProjektGenreRel rel = await db.tbl_ProjektGenreRel.FindAsync(ID);
             db.tbl_ProjektGenreRel.Remove(rel);
             await db.SaveChangesAsync();

             return Redirect(Request.UrlReferrer.AbsoluteUri);
         }
    }
}