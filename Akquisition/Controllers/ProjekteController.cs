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
    public class ProjekteController : Controller
    {
        private AkquiseEntities db = new AkquiseEntities();

        // GET: Projekte
        public async Task<ActionResult> Index(MyStuff.ProjektFilter filter)
        {
            var projekte = db.qry_ProjekteIndex as IQueryable<qry_ProjekteIndex>; 

            // Der vorher benutzte Filter
            MyStuff.ProjektFilter storedFilter = System.Web.HttpContext.Current.Session["Filter"] as MyStuff.ProjektFilter;

            // Neuen Filter anwenden
            if (filter != null && filter.IsReal)
            {
                filter.ApplyFilter(ref projekte);

                // Filter Cookie setzen
                System.Web.HttpContext.Current.Session["Filter"] = filter;
            }
            // gespeicherten Filter anwenden
            else
            {
                if (storedFilter != null && storedFilter.IsReal)
                {
                    storedFilter.ApplyFilter(ref projekte);
                }
            }

            //projekte = projekte.OrderByDescending(x => x.DATUM_ANGEBOT);
            return View(await projekte.OrderByDescending(x => x.aufgenommen_am).ToListAsync());
        }

        // Der Index als Printversion, wird nicht mit neuem Filter aufgerufen
        public ActionResult IndexPrintVersion()
        {
            var projekte = db.qry_ProjekteIndex as IQueryable<qry_ProjekteIndex>;

            // Der vorher benutzte Filter
            MyStuff.ProjektFilter storedFilter = System.Web.HttpContext.Current.Session["Filter"] as MyStuff.ProjektFilter;

            if (storedFilter != null && storedFilter.IsReal)
            {
                storedFilter.ApplyFilter(ref projekte);
            }

            return View("IndexForPrint", "~/Views/Shared/_LayoutForPrint.cshtml", projekte);
        }

        /// <summary>
        /// Kreiert eien einfachen Filter und ruft die Indexseite auf
        /// </summary>
        /// <param name="was">
        /// Welcher Filter wird erzeugt
        /// </param>
        /// <returns></returns>
        public ActionResult FilterVordefiniert(string was)
        {
            MyStuff.ProjektFilter filter = new MyStuff.ProjektFilter();
            filter.IsReal = true;

            switch (was)
            {
                case "Marktliste":
                    filter.InListe = MyStuff.ProjektFilter.Liste.Marktliste;
                    break;
                case "Produktionsliste":
                    filter.InListe = MyStuff.ProjektFilter.Liste.Produktionsliste;
                    break;
                case "Prioliste":
                    filter.InListe = MyStuff.ProjektFilter.Liste.Prioliste;
                    break;
                default:
                    return HttpNotFound("Die angeforderte Liste ist nicht bekannt");
            }

            return RedirectToAction("Index", filter);
        }

        public ActionResult AktuelleMarktliste()
        {
            // in Version 2.5 abgeschafft
            return HttpNotFound("Die Funktion steht nicht mehr zur Verügung");

            var projekte = db.qry_ProjekteIndex as IQueryable<qry_ProjekteIndex>;
            MyStuff.ProjektFilter filter = new MyStuff.ProjektFilter(true);
            filter.IsReal = true;
            filter.ApplyFilter(ref projekte);

            // Filter Cookie setzen
            System.Web.HttpContext.Current.Session["Filter"] = filter;

           // List<qry_ProjekteIndex> liste = projekte.ToList();
            return View("Index", projekte.OrderByDescending(x => x.aufgenommen_am).ToList());
        }


        public ActionResult Warenkorb()
        {
            // aktueller Benutzer
            string login = User.Identity.Name.ToLower();

            // Projekte seines Warenkorbs
            var projekte = db.sp_Warenkorbprojekte(login);

            return View("Index", projekte.ToList());
        }

        // Spezielle Statusliste mit einem hinzugefügten Status 'nicht verkauft'
        private List<SelectListItem> SpecialStatus()
        {
            SelectList stati = new SelectList(db.tbl_Status, "StatusNr", "Status");
            List<SelectListItem> selectListItems = stati.ToList();
            selectListItems.Insert(selectListItems.Count, new SelectListItem { Value = "100", Text = "nicht verkauft" });
            return selectListItems;
        }


        public async Task<ActionResult> Filter()
        {
            MyStuff.ProjektFilter filter = new MyStuff.ProjektFilter();
            filter.IsReal = true;

            ViewBag.zustaendig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");
            ViewBag.Land = new SelectList(db.Laender, "Land", "Land");
            ViewBag.Status = SpecialStatus();
            ViewBag.Archiv = new SelectList(new string[] { "aktuell", "Archiv" });
            ViewBag.Lektorat = new SelectList(db.tbl_Lektoratseinschaetzung, "LENr", "Lektoratseinschaetzung");
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating");
            
            return View(filter);
        }

        [HttpPost]
        public ActionResult Filter(MyStuff.ProjektFilter filter)
        {
            if (ModelState.IsValid) 
            {
                return RedirectToAction("Index", filter);
            }

            // Falls Fehler auf der Seite bleiben
            ViewBag.zustaendig = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");
            ViewBag.Land = new SelectList(db.Laender, "Land", "Land");
            ViewBag.Status = SpecialStatus();
            ViewBag.Archiv = new SelectList(new string[] { "aktuell", "Archiv" });
            ViewBag.Lektorat = new SelectList(db.tbl_Lektoratseinschaetzung, "LENr", "Lektoratseinschaetzung");
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating");

            return View(filter);
        }

        // GET: Projekte/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Projekte projekt = db.tbl_Projekte.Find(id);
            if (projekt == null)
            {
                return HttpNotFound();
            }

            if (projekt.alter_Standard)
            {
                qry_ProjekteASP qry = await db.qry_ProjekteASP.FindAsync(id);
                if (qry == null)
                {
                    return HttpNotFound();
                }
                return View("DetailsAltesMuster", qry);
            }
                

            return View(projekt);
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public ActionResult Create()
        {
            ViewBag.AnbieterNr = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL");
            ViewBag.BearbeiterNr = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating");
            ViewBag.StatusNr = new SelectList(db.tbl_Status, "StatusNr", "Status");

            // Version 3.0
            ViewBag.AnbieterNr2 = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL");
            ViewBag.BearbeiterNr2 = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");

            return View();
        }

        // POST: Projekte/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(tbl_Projekte tbl_Projekte)
        {
            if (ModelState.IsValid)
            {
                // Ein paar Felder initialisieren
                tbl_Projekte.aufgenommen_am = DateTime.Today;
                tbl_Projekte.geaendert_von = User.Identity.Name.ToLower();
                tbl_Anbieter anbieter = db.tbl_Anbieter.Find(tbl_Projekte.AnbieterNr);
                tbl_Projekte.Kontakt = anbieter.Kontaktperson;

                db.tbl_Projekte.Add(tbl_Projekte);
                await db.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = tbl_Projekte.ENTRY_No });
            }

            ViewBag.AnbieterNr = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", tbl_Projekte.AnbieterNr);
            ViewBag.BearbeiterNr = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating", tbl_Projekte.Rating);
            ViewBag.StatusNr = new SelectList(db.tbl_Status, "StatusNr", "Status", tbl_Projekte.StatusNr);

            // Version 3.0
            ViewBag.AnbieterNr2 = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", tbl_Projekte.AnbieterNr2);
            ViewBag.BearbeiterNr2 = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter");

            return View(tbl_Projekte);
        }

        
        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(id); 
            if (projekt == null)
            {
                return HttpNotFound();
            }
            else if (projekt.alter_Standard)
            {
                return RedirectToAction("EditAlterStandard", new { id = id });
            }

            ViewBag.AnbieterNr = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", projekt.AnbieterNr);
            ViewBag.BearbeiterNr = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", projekt.BearbeiterNr);
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating", projekt.Rating);
            ViewBag.StatusNr = new SelectList(db.tbl_Status, "StatusNr", "Status", projekt.StatusNr);
            ViewBag.LENr = new SelectList(db.tbl_Lektoratseinschaetzung, "LENr", "Lektoratseinschaetzung", projekt.LENr);
            ViewBag.ProjektartNr = new SelectList(db.tbl_Projektart, "ProjektartNr", "Projektart", projekt.ProjektartNr);
            ViewBag.FormatNr = new SelectList(db.tbl_Filmformate, "FormatNr", "Format", projekt.FormatNr);
            ViewBag.BackingWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.BackingWE);
            ViewBag.AngebotWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.AngebotWE);
            ViewBag.AskingWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.AskingWE);
            ViewBag.KaufpreisWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.KaufpreisWE);
            ViewBag.BudgetWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.BudgetWE);
            ViewBag.verkauft_an = new SelectList(db.tbl_Verleih.OrderBy(x => x.Verleih), "VerleihNr", "Verleih", projekt.verkauft_an);
            ViewBag.Age_Rating = new SelectList(projekt.Age_Rating_List, projekt.Age_Rating);
            ViewBag.MPAA = new SelectList(projekt.MPAA_List, projekt.MPAA);

            // version 3.0
            ViewBag.AnbieterNr2 = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", projekt.AnbieterNr2);
            ViewBag.BearbeiterNr2 = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", projekt.BearbeiterNr2);

            return View(projekt);
        }

        // POST: Projekte/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(tbl_Projekte projekt)
            //Edit([Bind(Include = "ENTRY_No,Land,AnbieterNr,Kontakt,DATUM_Angebot,TITEL_PROJEKT_FILM,StatusNr,Land,kURZBEMERKUNG,RegisseurNr,Produzenten,Produktionsfirma,Script_Buch,Drehbeginn,Lieferbar,Geschichte,Asking,Angebot,Sonstiges,Besetzung,Budget,US_Release,Beurteilung,aktualisiert,Festival_akt,xLektorat,Lektorat")] projekt projekt)
        {
            if (ModelState.IsValid)
            {
                projekt.aktualisiert = DateTime.Today;
                projekt.geaendert_von = User.Identity.Name.ToLower();
                setBlobData(projekt);
                db.Entry(projekt).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AnbieterNr = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", projekt.AnbieterNr);
            ViewBag.Rating = new SelectList(db.tbl_Rating, "Rating", "Rating", projekt.Rating);
            ViewBag.StatusNr = new SelectList(db.tbl_Status, "StatusNr", "Status", projekt.StatusNr);
            ViewBag.BearbeiterNr = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", projekt.BearbeiterNr);
            ViewBag.LENr = new SelectList(db.tbl_Lektoratseinschaetzung, "LENr", "Lektoratseinschaetzung", projekt.LENr);
            ViewBag.ProjektartNr = new SelectList(db.tbl_Projektart, "ProjektartNr", "Projektart", projekt.ProjektartNr);
            ViewBag.FormatNr = new SelectList(db.tbl_Filmformate, "FormatNr", "Format", projekt.FormatNr);
            ViewBag.BackingWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.BackingWE);
            ViewBag.AngebotWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.AngebotWE);
            ViewBag.AskingWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.AskingWE);
            ViewBag.KaufpreisWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.KaufpreisWE);
            ViewBag.BudgetWE = new SelectList(db.tbl_WE, "WENr", "Waehrung", projekt.BudgetWE);
            ViewBag.verkauft_an = new SelectList(db.tbl_Verleih.OrderBy(x => x.Verleih), "VerleihNr", "Verleih", projekt.verkauft_an);
            ViewBag.Age_Rating = new SelectList(projekt.Age_Rating_List, projekt.Age_Rating);
            ViewBag.MPAA = new SelectList(projekt.MPAA_List, projekt.MPAA);

            // version 3.0
            ViewBag.AnbieterNr2 = new SelectList(db.tbl_Anbieter.OrderBy(x => x.Kürzel), "AnbieterNr", "KÜRZEL", projekt.AnbieterNr2);
            ViewBag.BearbeiterNr2 = new SelectList(db.tbl_Bearbeiter, "BearbeiterNr", "Bearbeiter", projekt.BearbeiterNr2);

            return View(projekt);
        }

        /// <summary>
        /// Setzt Lektorat und Vergleichsfilm in Tabelle tbl_BlobProjekte
        /// </summary>
        /// <param name="projekt">
        /// Das Projekt</param>
        /// <param name="was">
        /// 1=Lektorat, 2=Vergleichsfilm</param>
        private void setBlobData(tbl_Projekte projekt)
        {
            // Abbrechen, falls weder Lektorat noch Vergleichsfilm angegeben wurde
            if (projekt.xLektorat == null && projekt.xVergleichsfilm == null)
                return;

            // Falls noch kein Eintrag in tbl_BlobProjekte, diesen erzeugen
            // und EntityState auf Added oder Modified setzen
            tbl_BlobProjekte blob = db.tbl_BlobProjekte.Find(projekt.ENTRY_No);
            EntityState blobState;
            if (blob != null)
            {
                blobState = EntityState.Modified;
            }
            else
            {
                blob = new tbl_BlobProjekte();
                blob.EntryNo = projekt.ENTRY_No;
                blobState = EntityState.Added;
            }

            System.Web.HttpPostedFileBase fileBase;
            byte[] data;

            // Lektorat
            if (projekt.xLektorat != null)
            {
                fileBase = projekt.xLektorat;

                // zu grosse Dateien abfangen
                if (fileBase.InputStream.Length > 10000000)
                    throw new ArgumentException("Es sind nur bis zu 10 MB an Dateigröße erlaubt");

                System.IO.BinaryReader reader = new System.IO.BinaryReader(fileBase.InputStream);
                data = reader.ReadBytes((int)fileBase.InputStream.Length);
                reader.Close();
                blob.Lektorat = data;
                blob.LektoratFileName = createFilename(fileBase);
            }

            // Vergleichsfilm
            if (projekt.xVergleichsfilm != null)
            {
                fileBase = projekt.xVergleichsfilm;

                // zu grosse Dateien abfangen
                if (fileBase.InputStream.Length > 10000000)
                    throw new ArgumentException("Es sind nur bis zu 10 MB an Dateigröße erlaubt");

                System.IO.BinaryReader reader = new System.IO.BinaryReader(fileBase.InputStream);
                data = reader.ReadBytes((int)fileBase.InputStream.Length);
                reader.Close();
                blob.Vergleichsfilm = data;
                blob.VergleichsfilmFileName = createFilename(fileBase);
            }

            // Den Entity State setzen
            db.Entry(blob).State = blobState;
        }

        private string createFilename(HttpPostedFileBase fileBase)
        {
            // Maximale Länge des Titels
            int maxLength = 100;

            // Der Titel ist kurz genug
            if (fileBase.FileName.Length <= maxLength)
                return fileBase.FileName;

            // Titel zu lang -> Kürzen
            int pos = fileBase.FileName.LastIndexOf('.');
            if (pos == -1)
            {
                return fileBase.FileName.Substring(0, maxLength);
            }
            else
            {
                string extension = fileBase.FileName.Substring(pos);
                return fileBase.FileName.Substring(0, maxLength - extension.Length) + extension;
            }
        }

        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> EditAlterStandard(int? id)
        {
            // Das Projekt finden
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(id);

            if (projekt == null)
            {
                return HttpNotFound();
            }
            else if (projekt.alter_Standard == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Objekt vom Typ ProjektAlterStandard erzeugen und der View übergeben
            MyStuff.ProjektAlterStandard oldStandard = new MyStuff.ProjektAlterStandard(projekt.ENTRY_No, projekt.TITEL_PROJEKT_FILM, projekt.alter_Standard);
            return View(oldStandard);
        }

        [HttpPost]
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAlterStandard(MyStuff.ProjektAlterStandard projekt)
        {
            tbl_Projekte clone = await db.tbl_Projekte.FindAsync(projekt.ENTRY_No);
            clone.alter_Standard = projekt.alter_Standard;
            await db.SaveChangesAsync();

            // Zum Editieren im neuen Muster, falls alter_Standard=false
            if (!clone.alter_Standard)
            {
                return RedirectToAction("Edit", new { id = projekt.ENTRY_No });
            }
            return RedirectToAction("Index");
        }

        // GET: Projekte/Delete/5
        [LoggedOrAuthorized(Roles = "DataWriter")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Projekte tbl_Projekte = await db.tbl_Projekte.FindAsync(id);
            if (tbl_Projekte == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Projekte);
        }

        // POST: Projekte/Delete/5
        [HttpPost, ActionName("Delete")]
        [LoggedOrAuthorized(Roles = "DataWriter")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_Projekte tbl_Projekte = await db.tbl_Projekte.FindAsync(id);
            db.tbl_Projekte.Remove(tbl_Projekte);
            await db.SaveChangesAsync();
            return RedirectToAction("Filter");
        }

        public async Task<ActionResult> GetLektorat(int id)
        {
            tbl_BlobProjekte projekt = await db.tbl_BlobProjekte.FindAsync(id);
            if (projekt == null || projekt.Lektorat == null)
            {
                return HttpNotFound("Für dieses Projekt kann kein Lektorat gefunden weden!");
            }
            byte[] data = projekt.Lektorat;
            if (data.Length > 0)
            {
                Response.Buffer = true;
                Response.ContentType = "application/msword";
                Response.BinaryWrite(data);
                Response.AddHeader("Content-Disposition", String.Format(@"filename=""{0}""", projekt.LektoratFileName));
            }
            return null;
        }

        public async Task<ActionResult> GetVergleichsfilm(int id)
        {
            tbl_BlobProjekte projekt = await db.tbl_BlobProjekte.FindAsync(id);
            if (projekt == null || projekt.Vergleichsfilm == null)
            {
                return HttpNotFound("Für dieses Projekt kann keine Vergleichsfilmliste gefunden weden!");
            }
            byte[] data = projekt.Vergleichsfilm;
            if (data.Length > 0)
            {
                Response.Buffer = true;
                Response.ContentType = "application/msexcel";
                Response.BinaryWrite(data);
                Response.AddHeader("Content-Disposition", String.Format(@"filename=""{0}""", projekt.VergleichsfilmFileName));
            }
            return null;
        }

        public async Task<ActionResult> InvertBool(int id, int was)
        {
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(id);
            switch (was)
            {
                case 1:
                    projekt.Festival_akt = !projekt.Festival_akt;
                    break;
                case 2:
                    projekt.Leseliste = !projekt.Leseliste;
                    break;
                case 3:
                    projekt.Prioliste = !projekt.Prioliste;
                    break;
                case 4:
                    projekt.Sichtung = !projekt.Sichtung;
                    break;
                case 5:
                    projekt.Produktionsliste = !projekt.Produktionsliste;
                    break;
            }
            await db.SaveChangesAsync();
            return null;
        }

        
        public async Task<ActionResult> DetailsPrintVersion(int id)
        {
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(id);
            return View("Details", "~/Views/Shared/_LayoutForPrint.cshtml", projekt);
        }

        public async Task<ActionResult> DetailsAltesMusterPrintVersion(int id)
        {
            tbl_Projekte projekt = await db.tbl_Projekte.FindAsync(id);
            return View("DetailsAltesMuster", "~/Views/Shared/_LayoutForPrint.cshtml", projekt);
        }

        public string ToHtml(string viewToRender, ViewDataDictionary viewData, ControllerContext controllerContext)
        {
            var result = ViewEngines.Engines.FindView(controllerContext, viewToRender, null);

            System.IO.StringWriter output;
            using (output = new System.IO.StringWriter())
            {
                var viewContext = new ViewContext(controllerContext, result.View, viewData, controllerContext.Controller.TempData, output);
                result.View.Render(viewContext, output);
                result.ViewEngine.ReleaseView(controllerContext, result.View);
            }

            return output.ToString();
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
