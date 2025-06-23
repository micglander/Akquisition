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
    public class ProjektAuswertungsartRelController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo); 
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            // schon vorhandene Auswertungsarten
            IEnumerable<int> vorhanden = projekt.tbl_ProjektAuswertungsArtRel.Select(x => x.AuswertungsartNr);

            List<tbl_ProjektAuswertungsArtRel> liste = new List<tbl_ProjektAuswertungsArtRel>();
            foreach (tbl_Auswertungsart art in db.tbl_Auswertungsart.ToList())
            {
                if (!vorhanden.Contains(art.AuswertungsartNr))
                {
                    tbl_ProjektAuswertungsArtRel rel = new tbl_ProjektAuswertungsArtRel();
                    rel.EntryNo = EntryNo;
                    rel.AuswertungsartNr = art.AuswertungsartNr;
                    rel.tbl_Auswertungsart = art;
                    liste.Add(rel);
                }
            }

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Add(List<tbl_ProjektAuswertungsArtRel> liste)
        {
            IEnumerable<tbl_ProjektAuswertungsArtRel> newItems = liste.Where(x => x.Potential > 0);
            if (newItems.Count() > 0)
            {
                db.tbl_ProjektAuswertungsArtRel.AddRange(newItems);
                await db.SaveChangesAsync();
            }

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_ProjektAuswertungsArtRel> liste = db.tbl_ProjektAuswertungsArtRel.Where(x => x.EntryNo == EntryNo).ToList();
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
        public async Task<ActionResult> Edit(List<tbl_ProjektAuswertungsArtRel> liste)
        {
            foreach (tbl_ProjektAuswertungsArtRel entity in liste)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            await db.SaveChangesAsync();
            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Delete(int EntryNo, int AuswertungsartNr)
        {
            tbl_ProjektAuswertungsArtRel rel = await db.tbl_ProjektAuswertungsArtRel.FindAsync(EntryNo, AuswertungsartNr);
            db.tbl_ProjektAuswertungsArtRel.Remove(rel);
            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}
