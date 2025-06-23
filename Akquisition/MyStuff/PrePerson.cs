using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akquisition.MyStuff
{
    public class PrePerson : efAkquisition.tbl_ProjektPersonRel
    {
        public string Name { get; set; }

        public bool CreatePerson { get; set; }

        public PrePerson()
            : base()
        { }

        public PrePerson(int EntryNo, int FunktionNr)
            : base()
        {
            this.EntryNo = EntryNo;
            this.FunktionNr = FunktionNr;
        }
    }
}