using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcMusicStore.Models;
using Microsoft.ApplicationInsights.DataContracts;

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private MusicStoreEntities storeDB = new MusicStoreEntities();
        //
        // GET: /Home/

        public async Task<ActionResult> Index()
        {
            // Get most popular albums
            var albums = await GetTopSellingAlbums(6);
            //var albums = GetTopSellingAlbums(6);

            // Trigger some good old ADO code 
            var albumCount = GetTotalAlbumns(); 
            Trace.Write(string.Format("Total number of Albums = {0} and Albums with 'The' = {1}", albumCount.Item1, albumCount.Item2));

            var telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();

            //Sample Trace telemetry
            TraceTelemetry traceSample = new TraceTelemetry();
            traceSample.Message = "Slow response - database";
            traceSample.SeverityLevel = SeverityLevel.Warning;
            telemetryClient.TrackTrace(traceSample);

            //Sample event telemetry
            var properties = new Dictionary<string, string> { { "Property 1",string.Format("Album Count {0}" ,albumCount.Item1) } };
            var measurements = new Dictionary<string, double> { { "Sample Meassurement", albumCount.Item1 } };
            telemetryClient.TrackEvent("Top Selling Albums", properties, measurements);

            //Sample exception telemetry
            try
            {
                albumCount = null;
                int count=albumCount.Item1;
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex, properties, measurements);
            }

            //Obtains the ip address from the request
            var request = new RequestTelemetry();
            request.Url = HttpContext.Request.Url;
            request.Duration = System.TimeSpan.FromMilliseconds(100);
            request.Success = false;
            request.Name = "TEST REQUEST " + request.Name;
            telemetryClient.TrackRequest(request);

            return View(albums);
        }


        private Task<List<Album>> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            return storeDB.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToListAsync();
        }

        private Tuple<int, int> GetTotalAlbumns()
        {
            var result1 = 0;
            var result2 = 0;

            var connectionString = ConfigurationManager.ConnectionStrings["MusicStoreEntities"];
            var factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString.ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Albums";
                    command.CommandType = CommandType.Text;
                    result1 = (int)command.ExecuteScalar();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Albums WHERE Title LIKE 'The%'";
                    command.CommandType = CommandType.Text;
                    result2 = (int)command.ExecuteScalar();
                }
            }

            return new Tuple<int, int>(result1, result2);
        }

        protected override void Dispose(bool disposing)
        {
            storeDB.Dispose();
            base.Dispose(disposing);
        }
    }
}