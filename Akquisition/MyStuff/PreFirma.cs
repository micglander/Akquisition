using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akquisition.MyStuff
{
    public class PreFirma : efAkquisition.tbl_ProjektFirmaRel
    {
        public string Name { get; set; }

        public bool CreatePerson { get; set; }

        public PreFirma() : base() { }

        public PreFirma(int EntryNo)
            : base()
        {
            this.EntryNo = EntryNo;
        }
    }
}