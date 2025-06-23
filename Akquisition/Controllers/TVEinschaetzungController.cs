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
    public class TVEinschaetzungController : Controller
    {

        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> Add(int EntryNo)
        {
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            // schon vorhandene Sender
            IEnumerable<int> vorhanden = projekt.tbl_TV_Einschaetzung.Select(x => x.SenderNr);

            List<tbl_TV_Einschaetzung> liste = new List<tbl_TV_Einschaetzung>();
            foreach (tbl_Sender sender in db.tbl_Sender.ToList())
            {
                if (!vorhanden.Contains(sender.SenderNr))
                {
                    tbl_TV_Einschaetzung rel = new tbl_TV_Einschaetzung();
                    rel.EntryNo = EntryNo;
                    rel.SenderNr = sender.SenderNr;
                    rel.tbl_Sender = sender;
                    liste.Add(rel);
                }
            }

            // Länder für die Comboboxen
            ViewBag.Auswahl = db.tbl_Sender.ToList();

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Add(List<tbl_TV_Einschaetzung> liste)
        {
            IEnumerable<tbl_TV_Einschaetzung> newItems = liste.Where(x => !String.IsNullOrEmpty(x.Kommentar));
            if (newItems.Count() > 0)
            {
                db.tbl_TV_Einschaetzung.AddRange(newItems);
                await db.SaveChangesAsync();
            }

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_TV_Einschaetzung> liste = db.tbl_TV_Einschaetzung.Where(x => x.EntryNo == EntryNo).ToList();
            if (liste.Count == 0)
            {
                return null;
            }

            // Genres für die Comboboxen
            //ViewBag.Auswahl = db.tbl_Sender.ToList();

            // Titel
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(List<tbl_TV_Einschaetzung> liste)
        {
            foreach (tbl_TV_Einschaetzung entity in liste)
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            await db.SaveChangesAsync();
            
            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = liste.First().EntryNo }) + "#rels");
        }

        public async Task<ActionResult> Delete(int EntryNo, int SenderNr)
        {
            tbl_TV_Einschaetzung rel = await db.tbl_TV_Einschaetzung.FindAsync(EntryNo, SenderNr);
            db.tbl_TV_Einschaetzung.Remove(rel);
            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}