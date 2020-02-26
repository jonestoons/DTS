using DocumentTracking.Models;
using System.Web.Mvc;
using System.Linq;
using System;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Net;
using DocumentTracking.Models.DataModels;

namespace DocumentTracking.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            int unit = MiscClass.getUserUnit();
            HomeAnalytics data = new HomeAnalytics();

            //Analytics for Today
            data._TDC = db.Documents.Where(x => (DbFunctions.TruncateTime(x.DateCreated) == DateTime.Today.Date) && (x.UnitId == unit)).Count();
            data._TID = db.DocumentMovements.Where(x => (DbFunctions.TruncateTime(x.DateCreated) == DateTime.Today.Date) && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString())).Count();
            data._TOD = db.DocumentMovements.Where(x => (DbFunctions.TruncateTime(x.DateCreated) == DateTime.Today.Date) && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString())).Count(); 


            //Analytics for 7 Days
            var currDate = DateTime.Today.Date;
            var prev7Date = DateTime.Today.AddDays(-7);

            data._7DC = db.Documents
                .Where(x =>  (DbFunctions.TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.UnitId == unit))
                .Count();

            data._7ID = db.DocumentMovements
                .Where(x =>
                    (DbFunctions
                    .TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                .Count();

            data._7OD = db.DocumentMovements
                .Where(x =>
                    (DbFunctions
                    .TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                .Count(); ;


            //Analytics for 30 days
            prev7Date = DateTime.Today.AddDays(-30);

            data._30DC = db.Documents
                .Where(x =>
                    (DbFunctions
                    .TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.UnitId == unit))
                .Count();

            data._30ID = db.DocumentMovements
                .Where(x =>
                    (DbFunctions
                    .TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                .Count(); 

            data._30OD = db.DocumentMovements
                .Where(x =>
                    (DbFunctions
                    .TruncateTime(x.DateCreated) <= currDate &&
                    DbFunctions
                    .TruncateTime(x.DateCreated) >= prev7Date) &&
                    (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                .Count();

            //ViewBag.Analytics = data;

            return View(data);
        }

        // GET: MsgSetups/Edit/5
        public ActionResult MsgSettings(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MsgSetup msgSetup = db.MsgSetups.Find(id);
            if (msgSetup == null)
            {
                return HttpNotFound();
            }
            return View(msgSetup);
        }

        // POST: MsgSetups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MsgSettings([Bind(Include = "Id,Email,Password,Port,Host,Backlog")] MsgSetup msgSetup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(msgSetup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(msgSetup);
        }

        //[Authorize(Roles = "Super Admin")]
        //[HttpGet]
        //public ActionResult MsgSettings()
        //{
        //    MsgSetup setup = new MsgSetup();
        //    String line;
        //    string path = Server.MapPath("/") + "Content/themes/mns.txt";
        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        //Pass the file path and file name to the StreamReader constructor
        //        using (StreamReader sr = new StreamReader(path))
        //        {

        //            //Read the first line of text
        //            line = sr.ReadLine();

        //            var t = line.Split(';').ToList();

        //            for (int i = 0; i < t.Count; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    setup.Email = t[i].ToString();
        //                }
        //                else if (i == 1)
        //                {
        //                    setup.Password = t[i].ToString();
        //                }
        //                else if (i == 2)
        //                {
        //                    setup.Port = t[i];
        //                }
        //                else if (i == 3)
        //                {
        //                    setup.Host = t[i];
        //                }
        //                else if (i == 4)
        //                {
        //                    bool d;
        //                    if (t[i].ToString() == "False")
        //                    {
        //                        d = false;
        //                    }
        //                    else
        //                    {
        //                        d = true;
        //                    }
        //                    setup.Backlog = d;
        //                }
        //            }
        //            sr.Close();
        //            sr.Dispose();
        //            //Continue to read until you reach end of file
        //            //while (line != null)
        //            //{
        //            //    //Read the next line
        //            //    line = sr.ReadLine();


        //            //}

        //        }
        //    }
        //    return View(setup);

        //}

        //[Authorize(Roles = "Super Admin")]
        //[HttpPost]
        //public ActionResult MsgSettings(MsgSetup setup)
        //{
        //    string path = Server.MapPath("/") + "Content/themes/mns.txt";

        //    StringBuilder line = new StringBuilder();
        //    //System.IO.File.WriteAllText(path, String.Empty);
        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        //Pass the file path and file name to the StreamReader constructor
        //        using (StreamWriter sr = new StreamWriter(path))
        //        {
        //            line.Append(setup.Email + ";");
        //            line.Append(setup.Password + ";");
        //            line.Append(setup.Port + ";");
        //            line.Append(setup.Host + ";");
        //            line.Append(setup.Backlog);


        //            //Write the first line of text
        //            sr.WriteLine(line.ToString());


        //            sr.Close();
        //            sr.Dispose();
        //        }
        //    }


        //    //StreamWriter sr = new StreamWriter(System.IO.File.OpenWrite(path));           


        //    return View(setup);
        //}

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
