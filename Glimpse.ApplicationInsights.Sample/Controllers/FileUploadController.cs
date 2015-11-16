using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class FileUploadController : Controller
    {
        public ActionResult Index(string fileName = "")
        {
            ViewBag.FileName = fileName;

            var telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();

            //Sample Trace telemetry
            TraceTelemetry traceSample = new TraceTelemetry();
            traceSample.Message = "Database status: Normal";
            traceSample.SeverityLevel = SeverityLevel.Information;
            telemetryClient.TrackTrace(traceSample);

            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            try
            {
                byte[] input = new byte[file.ContentLength];
                file.InputStream.Read(input, 0, file.ContentLength);

                Trace.Write("File '" + file.FileName + " created", "Information");

                return RedirectToAction("Index", new { fileName = file.FileName });
            }
            catch
            {
                Trace.Write("File '" + file.FileName + " wasn´t created", "Error");

                return View();
            }
        }
    }
}
