using iRentCronJob.Models;
using iRentCronJob.Views.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class PostVacanciesController : Controller
    {
        public MyiRentEntities db = new MyiRentEntities();
        public const string apartmentsComURL = "http://irentfeeddata20180130104047.azurewebsites.net/api/sendingData";
        public const string rentJungleURl = "http://rentjunglefeed20180404122020.azurewebsites.net/RentJungle/sendData";
        public const string oodleURL = "http://oodlefeed20180409034534.azurewebsites.net/Oodle/sendData";
        public const string subletURL = "http://subletfeed20180504034333.azurewebsites.net/Sublet/sendData";
        public const string zumperURL = "http://zumperfeed20181017112857.azurewebsites.net/Zumper/sendData";

        // GET: PostVacancies
        public ActionResult Index()
        {
            return View(); 
        }

        public ActionResult PostVacancies()
        {
            try
            {
                var getPostVacancies = db.postvacancies.Where(x => x.UnitsUpdated == 1).ToList();

                foreach (var post in getPostVacancies)
                {
                    // Post to apartments.com (CoStar)
                    PostData dataPost = new PostData();
                    dataPost.PropertyID = post.PropertyID;
                    var getUnits = db.units.Where(x => x.PropertyID == post.PropertyID && x.PostToVacancies == 1).ToList();
                    List<int> units = new List<int>();
                    foreach (var unit in getUnits)
                    {
                        units.Add(unit.UnitID);
                    }
                    dataPost.Units = units;

                    // Send to apartments.com | apartmentFinder | apartamentos.com (Spanish) | apartmenthomeliving
                    var json = JsonConvert.SerializeObject(dataPost);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(apartmentsComURL);
                    var result = client.PostAsync("", content).Result;

                    // Send to RentJungle
                    var jsonRentJungle = JsonConvert.SerializeObject(dataPost);
                    var contentRentJungle = new StringContent(jsonRentJungle, Encoding.UTF8, "application/json");
                    HttpClient clientRentJungle = new HttpClient();
                    clientRentJungle.BaseAddress = new Uri(rentJungleURl);
                    var resultRentJungle = clientRentJungle.PostAsync("", contentRentJungle).Result;

                    // Send to Oodle
                    var jsonOodle = JsonConvert.SerializeObject(dataPost);
                    var contentOodle = new StringContent(jsonOodle, Encoding.UTF8, "application/json");
                    HttpClient clientOodle = new HttpClient();
                    clientOodle.BaseAddress = new Uri(oodleURL);
                    var resultOodle = clientOodle.PostAsync("", contentOodle).Result;

                    // Send to Zumper
                    var jsonZumper = JsonConvert.SerializeObject(dataPost);
                    var contentZumper = new StringContent(jsonZumper, Encoding.UTF8, "application/json");
                    HttpClient clientZumper = new HttpClient();
                    clientZumper.BaseAddress = new Uri(zumperURL);
                    var resultZumper = clientZumper.PostAsync("", contentZumper).Result;

                    // Send to Sublet
                    /*
                    var jsonSublet = JsonConvert.SerializeObject(dataPost);
                    var contentSublet = new StringContent(jsonSublet, Encoding.UTF8, "application/json");
                    HttpClient clientSublet = new HttpClient();
                    clientSublet.BaseAddress = new Uri(subletURL);
                    var resultSublet = clientSublet.PostAsync("", contentSublet).Result;
                    */

                    post.UnitsUpdated = 0;
                    db.SaveChanges();
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            } catch(Exception any)
            {
                SendUsEmail email = new SendUsEmail();
                email.sendError(any.ToString(), "Error Daily Post Vacancies");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult UpdateVacantUnits()
        {
            try
            {
                var getPostVacancies = db.postvacancies.Where(x => x.AutoPostVacancies == 1 || x.AutoRemoveNonVacancies == 1).ToList();
                bool sendData = true;

                foreach(var post in getPostVacancies)
                {
                    PostData dataPost = new PostData();
                    dataPost.PropertyID = post.PropertyID;
                    if (post.AutoPostVacancies == 1 && post.AutoRemoveNonVacancies == 1)
                    {
                        // Get only vacant units
                        var getUnits = db.units.Where(x => x.PropertyID == post.PropertyID && x.Occupied == 0).ToList();
                        List<int> units = new List<int>();
                        foreach (var unit in getUnits)
                        {
                            units.Add(unit.UnitID);
                        }
                        dataPost.Units = units;
                    } else if(post.AutoPostVacancies == 1 && post.AutoRemoveNonVacancies == 0)
                    {
                        // Update vacant units
                        var getUnits = (from u in db.units
                                        where u.PostToVacancies == 1 || u.Occupied == 0
                                        select u).ToList();
                        List<int> units = new List<int>();
                        foreach (var unit in getUnits)
                        {
                            units.Add(unit.UnitID);
                        }
                        dataPost.Units = units;
                    } else if(post.AutoPostVacancies == 0 && post.AutoRemoveNonVacancies == 1)
                    {
                        // Update vacant units
                        var getUnits = (from u in db.units
                                        where u.PostToVacancies == 1 && u.Occupied == 0
                                        select u).ToList();
                        List<int> units = new List<int>();
                        foreach (var unit in getUnits)
                        {
                            units.Add(unit.UnitID);
                        }
                        dataPost.Units = units;
                    } else
                    {
                        sendData = false;
                    }

                    if (sendData)
                    {
                        // Send to apartments.com | apartmentFinder | apartamentos.com (Spanish) | apartmenthomeliving
                        var json = JsonConvert.SerializeObject(dataPost);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(apartmentsComURL);
                        var result = client.PostAsync("", content).Result;

                        // Send to RentJungle
                        var jsonRentJungle = JsonConvert.SerializeObject(dataPost);
                        var contentRentJungle = new StringContent(jsonRentJungle, Encoding.UTF8, "application/json");
                        HttpClient clientRentJungle = new HttpClient();
                        clientRentJungle.BaseAddress = new Uri(rentJungleURl);
                        var resultRentJungle = clientRentJungle.PostAsync("", contentRentJungle).Result;
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            } catch(Exception any)
            {
                SendUsEmail email = new SendUsEmail();
                email.sendError(any.ToString(), "Error Weekly Post Vacancies");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}