using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.Reporting.WebForms;

namespace Akquisition.Controllers
{
    [LoggedOrAuthorized(Roles = "DataReader,DataWriter")]
    public class ReportsController : Controller
    {
         
        public ActionResult FestivalReport()
        {
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ServerReport.ReportPath = "/Akquisition/AktuellerMarkt"; 
            viewer.SizeToReportContent = true;

            //viewer.ShowParameterPrompts = false;
            //ReportParameter param = new ReportParameter("MinDatum");
            //param.Values.Add(null);
            //viewer.ServerReport.SetParameters(param);
            ViewBag.ReportViewer = viewer;

            try
            {
                return View();
            }
            catch (Exception exc)
            {
                return HttpNotFound(exc.Message);
            }
        }

        public ActionResult Prioliste()
        {
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ServerReport.ReportPath = "/Akquisition/Prioliste"; 
            viewer.SizeToReportContent = true;

            ViewBag.ReportViewer = viewer;

            try
            {
                return View();
            }
            catch (Exception exc)
            {
                return HttpNotFound(exc.Message);
            }
        }

        /// <summary>
        /// Ruft die parametrisierte Produktionsliste auf
        /// </summary>
        /// <param name="was">
        /// 0 = alles
        /// 1 = Serien
        /// 2 = Eigenproduktionen
        /// 3 = Akquisition / Co-Produktion
        /// </param>
        /// <returns></returns>
        public ActionResult Produktionsliste(int was)
        {
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ServerReport.ReportPath = "/Akquisition/Produktionsliste";
            viewer.SizeToReportContent = true;

            // Parameter setzen
            viewer.ShowParameterPrompts = false;

            // Die Parameter so setzen, dass alles angezeigt wird
            ReportParameter param1 = new ReportParameter("parameter1", "0");
            ReportParameter param2 = new ReportParameter("parameter2", "3");

            switch(was)
            {
                case 1:
                    param2.Values[0] = "0";
                    break;
                case 2:
                    param1.Values[0] = "3";
                    break;
                case 3:
                    param1.Values[0] = "1";
                    param2.Values[0] = "2";;
                    break;
                case 4:
                    param1.Values[0] = "1";
                    break;
            }

            viewer.ServerReport.SetParameters(param1);
            viewer.ServerReport.SetParameters(param2);

            ViewBag.ReportViewer = viewer;

            try
            {
                return View();
            }
            catch (Exception exc)
            {
                return HttpNotFound(exc.Message);
            }
        }

        public ActionResult Favoriten()
        {
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ServerReport.ReportPath = "/Akquisition/Favoriten";
            string login = User.Identity.Name.ToLower();
            ReportParameter param = new ReportParameter("Benutzer", login);
            viewer.ServerReport.SetParameters(param);
            viewer.SizeToReportContent = true;

            ViewBag.ReportViewer = viewer;

            try
            {
                return View();
            }
            catch (Exception exc)
            {
                return HttpNotFound(exc.Message);
            }
        }

        public ActionResult FavoritenMarktStyle()
        {
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ServerReport.ReportPath = "/Akquisition/FavoritenWieAktuellerMarkt";
            string login = User.Identity.Name.ToLower();
            ReportParameter param = new ReportParameter("Benutzer", login);
            viewer.ServerReport.SetParameters(param);
            viewer.SizeToReportContent = true;

            ViewBag.ReportViewer = viewer;

            try
            {
                return View();
            }
            catch (Exception exc)
            {
                return HttpNotFound(exc.Message);
            }
        }
    }
}