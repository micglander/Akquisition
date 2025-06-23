using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using efAkquisition;
using Akquisition.MyStuff;

namespace Akquisition.Controllers
{
    [LoggedOrAuthorized(Roles = "DataWriter")]
    public class ProjektPersonRelController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        public async Task<ActionResult> PreAdd(int EntryNo)
        {
            List<MyStuff.PrePerson> liste = new List<MyStuff.PrePerson>();

            // Beteiligter, Produzent etc.
            int funktion = 1;

            // Einträge erzeugen
            // Beteiligter
            for (int i = -3; i < 15; i++)
            {
                if (i < 1)
                {
                    funktion = 1;
                }
                else if (i < 5)
                {
                    funktion = 2;
                }
                else if (i < 9)
                {
                    funktion = 3;
                }
                else
                {
                    funktion = 4;
                }

                MyStuff.PrePerson prePerson = new MyStuff.PrePerson(EntryNo, funktion);
                liste.Add(prePerson);
            }

            // ViewBags
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;

            return View(liste);
        }


        [HttpPost]
        public async Task<ActionResult> PreAdd(List<MyStuff.PrePerson> liste)
        {
            List<MyStuff.PrePerson> input = liste.Where(x => !String.IsNullOrEmpty(x.Name)).ToList();

            
            System.Web.HttpContext.Current.Session["Personenliste"] = input;
            return RedirectToAction("Add");
        }

        public ActionResult Add()
        {
            List<MyStuff.PrePerson> input;
            input = System.Web.HttpContext.Current.Session["Personenliste"] as List<MyStuff.PrePerson>;
            if (input == null)
            {
                return HttpNotFound();
            }

            List<MyStuff.PrePerson> output = new List<MyStuff.PrePerson>();

            // Die Auswahllisten für die ComboBoxen initialisieren
            ViewBag.Auswahl = new List<tbl_Personen>[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                MyStuff.PrePerson prePerson = input[i];
                ViewBag.Auswahl[i] = db.GetAuswahl(prePerson.FunktionNr, prePerson.Name);
                output.Add((MyStuff.PrePerson)prePerson);
            }

            return View(output);
        }


        [HttpPost]
        public ActionResult Add(List<PrePerson> liste)
        {
            bool dirty = false;

            // zum Redirect braucht man die EntryNo
            int entryNo = 0;

            foreach (PrePerson entity in liste)
            {
                if (entryNo == 0)
                    entryNo = entity.EntryNo;

                // Hinzufügen anzulegender Personen
                if (entity.CreatePerson)
                {
                    tbl_Personen person = new tbl_Personen();
                    person.Name = entity.Name; ;
                    person.JobOne = entity.FunktionNr;
                    db.tbl_Personen.Add(person);
                    // Speichern, zum Erstellen einer ID
                    //db.SaveChanges();

                    tbl_ProjektPersonRel rel = new tbl_ProjektPersonRel();
                    rel.EntryNo = entryNo;
                    rel.tbl_Personen = person;
                    rel.FunktionNr = entity.FunktionNr;
                    rel.Prio = entity.Prio;
                    db.tbl_ProjektPersonRel.Add(rel);

                    dirty = true;
                }

                // Hinzufügen ausgewählter Personen
                else if (entity.PersonNr != 0)
                {
                    // eigentlich, aber es kommt zu einem unerklärlichen Fehler
                    //db.tbl_ProjektPersonRel.Add(entity as tbl_ProjektPersonRel);

                    tbl_ProjektPersonRel rel = new tbl_ProjektPersonRel();
                    rel.EntryNo = entryNo;
                    rel.PersonNr = entity.PersonNr;
                    rel.FunktionNr = entity.FunktionNr;
                    rel.Prio = entity.Prio;
                    db.tbl_ProjektPersonRel.Add(rel);
                    
                    dirty = true;
                }
            }
            if (dirty)
                db.SaveChanges();

            return Redirect(Url.RouteUrl(new { Controller = "Projekte", Action = "Edit", id = entryNo }) + "#rels");
        }

        public async Task<ActionResult> Edit(int EntryNo)
        {
            List<tbl_ProjektPersonRel> liste = db.tbl_ProjektPersonRel.Where(x => x.EntryNo == EntryNo).OrderBy(y => y.FunktionNr).OrderBy(z => z.Prio).ToList();
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(EntryNo);
            ViewBag.Projekt = projekt.TITEL_PROJEKT_FILM;
                
            return View(liste);
        }

        public async Task<ActionResult> Delete(int ID)
        {
            tbl_ProjektPersonRel rel = await db.tbl_ProjektPersonRel.FindAsync(ID);
            db.tbl_ProjektPersonRel.Remove(rel);
            await db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

    }
}