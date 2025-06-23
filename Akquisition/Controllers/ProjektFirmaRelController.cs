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
    public class ProjektFirmaRelController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> PreAdd(int EntryNo)
        {
            List<MyStuff.PreFirma> liste = new List<MyStuff.PreFirma>();

            // Einträge erzeugen
            // Beteiligter
            for (int i = 0; i < 4; i++)
            {
                MyStuff.PreFirma PreFirma = new MyStuff.PreFirma(EntryNo);
                liste.Add(PreFirma);
            }

            // ViewBags
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }


        [HttpPost]
        public async Task<ActionResult> PreAdd(List<MyStuff.PreFirma> liste)
        {
            List<MyStuff.PreFirma> input = liste.Where(x => !String.IsNullOrEmpty(x.Name)).ToList();

            System.Web.HttpContext.Current.Session["Firmenliste"] = input;
            return RedirectToAction("Add");
        }

        public ActionResult Add()
        {
            List<MyStuff.PreFirma> input;
            input = System.Web.HttpContext.Current.Session["Firmenliste"] as List<MyStuff.PreFirma>;
            if (input == null)
            {
                return HttpNotFound();
            }

            List<MyStuff.PreFirma> output = new List<MyStuff.PreFirma>();

            // Die Auswahllisten für die ComboBoxen initialisieren
            ViewBag.Auswahl = new List<tbl_Firmen>[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                MyStuff.PreFirma PreFirma = input[i];
                ViewBag.Auswahl[i] = db.tbl_Firmen.Where(x => x.Matchname.Contains(PreFirma.Name)).ToList<tbl_Firmen>();
                output.Add(PreFirma);
            }

            return View(output);
        }


        [HttpPost]
        public ActionResult Add(List<MyStuff.PreFirma> liste)
        {
            bool dirty = false;

            // zum Redirect braucht man die EntryNo
            int entryNo = 0;

            foreach (MyStuff.PreFirma entity in liste)
            {
                if (entryNo == 0)
                    entryNo = entity.EntryNo;

                 // Hinzufügen anzulegender Firmen
                if (entity.CreatePerson)
                {
                    tbl_Firmen firma = new tbl_Firmen();
                    firma.Firma = entity.Name;
                    db.tbl_Firmen.Add(firma);
                    // Speichern, zum Erstellen einer ID
                    //db.SaveChanges();

                    tbl_ProjektFirmaRel rel = new tbl_ProjektFirmaRel();
                    rel.EntryNo = entryNo;
                    rel.tbl_Firmen = firma;
                    rel.Prio = entity.Prio;
                    db.tbl_ProjektFirmaRel.Add(rel);

                    dirty = true;
                }
                else if (entity.FirmaNr != 0)
                {
                    tbl_ProjektFirmaRel rel = new tbl_ProjektFirmaRel();
                    rel.EntryNo = entryNo;
                    rel.FirmaNr = entity.FirmaNr;
                    rel.Prio = entity.Prio;
                    db.tbl_ProjektFirmaRel.Add(rel);
                    dirty = true;
                }
               
            }
            if (dirty)
                db.SaveChanges();

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = entryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_ProjektFirmaRel> liste = db.tbl_ProjektFirmaRel.OrderBy(x => x.Prio).Where(x => x.EntryNo == EntryNo).ToList();
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;
                
            return View(liste);
        }

        public async Task<ActionResult> Delete(int ID)
        {
            tbl_ProjektFirmaRel rel = await db.tbl_ProjektFirmaRel.FindAsync(ID);
            db.tbl_ProjektFirmaRel.Remove(rel);
            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }
}