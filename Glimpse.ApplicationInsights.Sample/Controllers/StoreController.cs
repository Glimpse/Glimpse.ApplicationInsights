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
    public class StoreController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        //
        // GET: /Store/

        public async Task<ActionResult> Index()
        {
            //var genres = storeDB.Genres.ToList();
            var genres = await GetTopGenres(6);

            var genresCount = GetTotalGenres();
            Trace.Write(string.Format("Total number of Genres = {0} ", genresCount.Item1));

            var telemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();

            //Sample Trace telemetry
            TraceTelemetry traceSample = new TraceTelemetry();
            traceSample.Message = "Slow response - database";
            traceSample.SeverityLevel = SeverityLevel.Warning;
            telemetryClient.TrackTrace(traceSample);

            //Sample event telemetry
            var NameGenres = new Dictionary<string, string> { { "Property 1", string.Format("Genres Count {0}", genresCount.Item1) } };
            telemetryClient.TrackEvent("All Genres", NameGenres);

            //Sample exception telemetry
            try
            {
                genresCount = null;
                int count = genresCount.Item1;
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex, NameGenres);
            }

            //Obtains the ip address from the request
            var request = new RequestTelemetry();
            request.Url = HttpContext.Request.Url;
            request.Duration = System.TimeSpan.FromMilliseconds(100);
            request.Success = false;
            request.Name = "TEST REQUEST " + request.Name;
            telemetryClient.TrackRequest(request);

            return View(genres);
        }

        private Task<List<Genre>> GetTopGenres(int count)
        {
            // Group the order details by genre and return
            // the genre with the highest count

            return storeDB.Genres
                .OrderByDescending(a => a.Albums.Count())
                .Take(count)
                .ToListAsync();
        }

        private Tuple<int> GetTotalGenres()
        {

            var result1 = 0;

            var connectionString = ConfigurationManager.ConnectionStrings["MusicStoreEntities"];
            var factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString.ConnectionString;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Genres";
                    command.CommandType = CommandType.Text;
                    result1 = (int)command.ExecuteScalar();
                }

            }
            Trace.Write("Genres count " + result1);

            return new Tuple<int>(result1);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = storeDB.Genres.Include("Albums")
                .Single(g => g.Name == genre);
            
            Trace.Write("Album in the store: " + genreModel.Name);
            

            return View(genreModel);
        }

        public ActionResult Details(int id)
        {
            var album = storeDB.Albums.Find(id);

            return View(album);
        }

        [ChildActionOnly]
        public ActionResult GenreMenu()
        {
            var genres = storeDB.Genres
                .OrderByDescending(
                    g => g.Albums.Sum(
                    a => a.OrderDetails.Sum(
                    od => od.Quantity)))
                .Take(9)
                .ToList();
            
            return PartialView(genres);
        }

        protected override void Dispose(bool disposing)
        {
            storeDB.Dispose();
            base.Dispose(disposing);
        }
    }
}