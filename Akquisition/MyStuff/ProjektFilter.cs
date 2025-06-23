using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using efAkquisition;


namespace Akquisition.MyStuff
{
    public class ProjektFilter : System.ComponentModel.DataAnnotations.IValidatableObject
    {
        public enum Liste { Undef, Marktliste, Produktionsliste, Prioliste };

        /// <summary>
        /// Wird gebraucht, da beim Routing immer ein ProjektFilter erzeugt wird und nicht auf IsNull getestet werden kann
        /// </summary>
        public bool IsReal { get; set; }

        /// <summary>
        /// Spezialfilter, der die anderen Filterkriterien ausser Kraft setzt
        /// </summary>
        private bool _aktuelleMarktliste;

        public string Titel { get; set; }

        [System.ComponentModel.DisplayName("Zuständig")]
        public int? zustaendig { get; set; }

        public string Anbieter { get; set; }

        public string Archiv { get; set; }

        public int? Status { get; set; }

        public string Land { get; set; }

        public string Beteiligter { get; set; }

        public string Produktionsfirma { get; set; }

        public int? Lektorat { get; set; }

        public string Rating { get; set; }

        public DateTime? ab { get; set; }

        public string Volltext { get; set; }

        /// <summary>
        /// Ist das Projekt in der erfragten Liste
        /// </summary>
        public Liste InListe { get; set; }


        public ProjektFilter()
        { }

        public ProjektFilter(bool AktuelleMarktliste)
        {
            _aktuelleMarktliste = AktuelleMarktliste;
        }


        /// <summary>
        /// Verändert eine übergebene Liste, indem sie die Filterkriterien anwendet
        /// </summary>
        /// <param name="projekte"></param>
        public void ApplyFilter(ref IQueryable<qry_ProjekteIndex> projekte)
        {
            // exklusiver Filter
            if (_aktuelleMarktliste)
            {
                DateTime stichtag = DateTime.Today.AddYears(-2);

                // In Version 2.2 geändert
                //projekte = projekte.Where(x => x.AnbieterAF == true).Where(y => y.StatusNr.Equals(1) || y.StatusNr.Equals(2) || y.StatusNr.Equals(12) || (y.StatusNr.Equals(10) && y.aufgenommen_am.Value > stichtag));
                projekte = projekte.Where(x => x.AnbieterAF == true).Where(y => y.Status2.Equals("aktuell"));

                return;
            }

            if (zustaendig != null)
                projekte = projekte.Where(x => x.BearbeiterNr == zustaendig);

            if (!String.IsNullOrEmpty(Titel))
                projekte = projekte.Where(x => (x.Titel.Contains(Titel) || x.Originaltitel.Contains(Titel)));

            if (!String.IsNullOrEmpty(Anbieter))
                projekte = projekte.Where(x => x.Anbieter.Contains(Anbieter));

            if (!String.IsNullOrEmpty(Archiv))
                projekte = projekte.Where(x => x.Status2 == Archiv);

            if (Status.HasValue)
                projekte = projekte.Where(x => x.StatusNr == Status.Value);

            if (!String.IsNullOrEmpty(Land))
            {
                string landsuche = String.Format(@"/{0}/", Land);
                projekte = projekte.Where(x => x.Landsuche.Contains(landsuche));
            }

            if (!String.IsNullOrEmpty(Rating))
            {
                projekte = projekte.Where(x => x.Rating.CompareTo(Rating) <= 0);
            }

            if (!String.IsNullOrEmpty(Beteiligter))
            {
                // Liste von Projekten in denen der Gesuchte als Person vorkommt
                efAkquisition.AkquiseEntities entities = new AkquiseEntities();
                List<int> beteiligtAn = entities.qry_Personmatch.Where(x => x.Matchname.Contains(Beteiligter)).Select(x => x.EntryNo).ToList();

                // Die Liste um Projekte erweitern, in denen der Gesuchte als Firma vorkommt
                beteiligtAn.AddRange(entities.qry_Firmamatch.Where(x => x.Matchname.Contains(Beteiligter)).Select(x => x.EntryNo).ToList());
                //beteiligtAn = beteiligtAn.Union(entities.qry_Firmamatch.Where(x => x.Matchname.Contains(Beteiligter)).Select(x => x.EntryNo).ToList() as IEnumerable<int>);
                projekte = projekte.Where(x => beteiligtAn.Contains(x.ENTRY_No));
                //projekte = projekte.Where(x => x.RegieMatchname.Contains(Beteiligter));
            }

            if (!String.IsNullOrEmpty(Volltext))
            {
                efAkquisition.AkquiseEntities entities = new AkquiseEntities();
                List<int?> treffer = entities.fn_Volltext(Volltext).ToList();
                List<int> treffer2 = new List<int>();
                foreach (int? i in treffer)
                {
                    treffer2.Add(i.Value);
                }

                projekte = projekte.Where(x => treffer.Contains(x.ENTRY_No));
            }

            if (ab != null)
            {
                projekte = projekte.Where(x => x.aufgenommen_am >= ab);
            }

            if (Lektorat.HasValue)
                projekte = projekte.Where(x => x.LENr == Lektorat.Value);

            if (this.InListe > 0)
            {
                // nach einem Listenprädikat filtern
                switch (InListe)
                {
                    case Liste.Marktliste:
                        projekte = projekte.Where(x => x.Festival_akt);
                        break;
                    case Liste.Prioliste:
                        projekte = projekte.Where(x => x.Prioliste);
                        break;
                    case Liste.Produktionsliste:
                        projekte = projekte.Where(x => x.Produktionsliste);
                        break;
                }
            }

        }


        public IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            // Fehler, falls Blank in Volltextsuche
            if (!String.IsNullOrEmpty(Volltext) && Volltext.Contains(" "))
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Keine Leerzeichen in der Volltextsuche", new[] { "Volltext" });
        }
    }
}