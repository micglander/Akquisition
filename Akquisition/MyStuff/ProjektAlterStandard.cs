using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akquisition.MyStuff
{
    public class ProjektAlterStandard
    {
        public int ENTRY_No { get; set;  }
        public string TITEL_PROJEKT_FILM { get; set; }
        public bool alter_Standard { get; set; }

        public ProjektAlterStandard() { }

        public ProjektAlterStandard(int id, string titel, bool alter_Standard)
        {
            ENTRY_No = id;
            TITEL_PROJEKT_FILM = titel;
            this.alter_Standard = alter_Standard;
        }
    }
}