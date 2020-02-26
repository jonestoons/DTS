using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using DocumentTracking.Models;
using Microsoft.AspNet.Identity;

namespace DocumentTracking.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reports
        public ActionResult Index()
        {
            ViewBag.Unit = MiscClass.getUserUnit();
            ViewBag.UnitId = new SelectList(db.Units.OrderBy(c => c.Description), "UnitId", "Description");
            return View();
        }

        [HttpPost]
        public JsonResult Index(Report report)
        {
            ReportDisplay rd = new ReportDisplay();
            report.Role = HttpContext.User.Identity.GetUserName();
            DateTime dateFrom;
            DateTime dateTo;
            List<string> dt = new List<string>();
            string dFrom = "";
            string dTo = "";
            dt = report.DateRange.Split('-').ToList();

            for (int i = 0; i < dt.Count; i++)
            {
                if (i == 0)
                {
                    dFrom = dt[0].ToString().Trim();
                }
                else
                {
                    dTo = dt[1].ToString().Trim();
                }
            }

            dateFrom = DateTime.Parse(dFrom);
            dateTo = DateTime.Parse(dTo);

            if (User.IsInRole("Admin") || User.IsInRole("Super Admin"))
            {
                if (report.Unit == null)
                {
                    if (report.Option == ReportOption.Day)
                    {
                        dt.Clear();
                        dt = report.Day.Split('/').ToList();
                        string newDate = dt[1].ToString() + "/" + dt[0].ToString() + "/" + dt[2].ToString();
                        
                        DateTime currDate = DateTime.Parse(report.Day);
                        rd.docCreated = db.Documents.Where(x => x.DateCreated == currDate.Date).Count();
                        rd.docIncoming =db.DocumentMovements.Where(x => x.DateCreated == currDate.Date && (x.Type == Movement.Incoming)).Count();
                        rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated == currDate.Date && (x.Type == Movement.Outgoing)).Count();

                        rd.docCreatedList = db.Documents.Where(x => x.DateCreated == currDate.Date)
                            .Select(x => new DocDetails {
                                Description = x.Description,
                                FileId = x.FileId,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                UserID = x.UserID
                            }).ToList();

                        rd.docIncomingList = db.DocumentMovements.Where(x => x.DateCreated == currDate.Date && (x.Type == Movement.Incoming))
                             .Select(x => new RD
                             {
                                 Description = x.Description,
                                 FileId = x.PrefID,
                                 Subject = x.Subject,
                                 DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                 FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                 Destination = db.Units.Where(g => g.UnitId.ToString() == (x.Destination)).FirstOrDefault().Description,
                                 Source = db.Units.Where(g => g.UnitId.ToString() == (x.Source)).FirstOrDefault().Description
                             }).ToList();

                        rd.docOutgoingList = db.DocumentMovements.Where(x => x.DateCreated == currDate.Date && (x.Type == Movement.Outgoing))
                             .Select(x => new RD
                             {
                                 Description = x.Description,
                                 FileId = x.PrefID,
                                 Subject = x.Subject,
                                 DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                 FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                 Destination = db.Units.Where(g => g.UnitId.ToString() == (x.Destination)).FirstOrDefault().Description,
                                 Source = db.Units.Where(g => g.UnitId.ToString() == (x.Source)).FirstOrDefault().Description

                             }).ToList();

                    }
                    else if (report.Option == ReportOption.DateRange)
                    {
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date)).Count();

                        rd.docIncoming = db.DocumentMovements
                            .Where(x => x.Type == Movement.Incoming && 
                            (x.DateCreated >= dateFrom.Date) &&
                            (x.DateCreated <= dateTo.Date)).Count();

                        rd.docOutgoing = db.DocumentMovements
                             .Where(x => x.Type == Movement.Outgoing &&
                             (x.DateCreated >= dateFrom.Date) &&
                             (x.DateCreated <= dateTo.Date)).Count();


                        rd.docCreatedList = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date))
                              .Select(x => new DocDetails
                              {
                                  Description = x.Description,
                                  FileId = x.FileId,
                                  Subject = x.Subject,
                                  DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                  FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                  UserID = x.UserID
                              }).ToList();

                        // FILE
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date) && x.FileTypeId == 1)
                              .Count();

                        // Letters & Correspondences
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date) && x.FileTypeId == 2)
                              .Count();
                        // Proposals
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date) && x.FileTypeId == 3)
                              .Count();
                        // Memo
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date) && x.FileTypeId == 4)
                              .Count();
                        // Requests
                        rd.docCreated = db.Documents
                              .Where(x => x.DateCreated >= dateFrom.Date && (x.DateCreated <= dateTo.Date) && x.FileTypeId == 5)
                              .Count();


                        //Incoming
                        // FILE
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Incoming &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 1 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Letters & Correspondences
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Incoming &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 2 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Proposals
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Incoming &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 3 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Memo
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Incoming &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 4 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Requests
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Incoming &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 5 &&
                          (x.DateCreated <= dateTo.Date)).Count();



                        //Outgoing
                        // FILE
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Outgoing &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 1 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Letters & Correspondences
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Outgoing &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 2 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Proposals
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Outgoing &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 3 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Memo
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Outgoing &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 4 &&
                          (x.DateCreated <= dateTo.Date)).Count();
                        // Requests
                        rd.docIncoming = db.DocumentMovements
                          .Where(x => x.Type == Movement.Outgoing &&
                          (x.DateCreated >= dateFrom.Date) &&
                          x.FileType == 5 &&
                          (x.DateCreated <= dateTo.Date)).Count();

                        try
                        {
                            //rd.docIncomingList = db.DocumentMovements
                            //    .Where(c => c.Type == Movement.Incoming && (c.DateCreated >= dateFrom) && (c.).ToList();
                            //rd.docIncomingList = db.DocumentMovements
                            //    .Where(x => x.Type == Movement.Incoming &&
                            //    (x.DateCreated >= dateFrom.Date) &&
                            //    (x.DateCreated <= dateTo.Date))
                            //    .Select(x => new RD
                            //    {
                            //        Description = x.Description,
                            //        FileId = x.PrefID,
                            //        Subject = x.Subject,
                            //        DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            //        FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                            //        Destination = db.Units.Where(g => g.UnitId == Convert.ToInt16(x.Destination)).FirstOrDefault().Description,
                            //        Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                            //    }).ToList();

                            rd.docOutgoingList = db.DocumentMovements
                                 .Where(x => x.Type == Movement.Outgoing &&
                                 (x.DateCreated >= dateFrom.Date) &&
                                 (x.DateCreated <= dateTo.Date))
                                 .Select(x => new RD
                                 {
                                     Description = x.Description,
                                     FileId = x.PrefID,
                                     Subject = x.Subject,
                                     DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                     FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                     Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                                     Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                                 }).ToList();
                        }
                        catch (Exception ex)
                         {

                            throw;
                        }
                       
                    }
                    else if (report.Option == ReportOption.Year)
                    {
                        rd.docCreated = db.Documents.Where(x => x.DateCreated.Value.Year.ToString() == report.Year).Count();
                        rd.docIncoming = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming)).Count();
                        rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing)).Count();

                        rd.docCreatedList = db.Documents.Where(x => x.DateCreated.Value.Year.ToString() == report.Year)
                            .Select(x => new DocDetails
                            {
                                Description = x.Description,
                                FileId = x.FileId,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                UserID = x.UserID
                            }).ToList();

                        rd.docIncomingList = db.DocumentMovements
                            .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming))
                             .Select(x => new RD
                             {
                                 Description = x.Description,
                                 FileId = x.PrefID,
                                 Subject = x.Subject,
                                 DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                 FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                 Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                                 Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                             }).ToList();

                        rd.docOutgoingList = db.DocumentMovements
                            .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing))
                            .Select(x => new RD
                            {
                                Description = x.Description,
                                FileId = x.PrefID,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                                Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                            }).ToList();
                    }
                }
                else
                {
                    int unit = Convert.ToInt32(report.Unit);
                    if (report.Option == ReportOption.Day)
                    {
                        dt.Clear();
                        dt = report.Day.Split('/').ToList();
                        string newDate = dt[1].ToString() + "/" + dt[0].ToString() + "/" + dt[2].ToString();

                        DateTime currDate = DateTime.Parse(report.Day);

                        rd.docCreated = db.Documents.Where(x => x.DateCreated == currDate && (x.UnitId == unit)).Count();
                        rd.docIncoming = db.DocumentMovements.Where(x => x.DateCreated == currDate && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString())).Count();
                        rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated == currDate && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString())).Count();

                        rd.docCreatedList = db.Documents.Where(x => x.DateCreated == currDate && (x.UnitId == unit))
                            .Select(x => new DocDetails
                            {
                                Description = x.Description,
                                FileId = x.FileId,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                UserID = x.UserID
                            }).ToList();

                        rd.docIncomingList = db.DocumentMovements
                            .Where(x => x.DateCreated == currDate && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                            .Select(x => new RD
                            {
                                Description = x.Description,
                                FileId = x.PrefID,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                                Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                            }).ToList();

                        rd.docOutgoingList = db.DocumentMovements
                            .Where(x => x.DateCreated == currDate && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                            .Select(x => new RD
                            {
                                Description = x.Description,
                                FileId = x.PrefID,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                                Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                            }).ToList();

                    }
                    else if (report.Option == ReportOption.DateRange)
                    {
                        //rd.docCreated = db.Documents
                        //    .Where(x => x.DateCreated >= dateFrom && x.DateCreated <= dateTo).Count();

                        rd.docCreated = db.Documents
                            .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.UnitId == unit))
                            .Count();

                        rd.docIncoming = db.DocumentMovements
                             .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                             .Count();

                        rd.docOutgoing = db.DocumentMovements
                            .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) &&(x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                            .Count();

                        rd.docCreatedList = db.Documents
                            .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.UnitId == unit))
                            .Select(x => new DocDetails
                            {
                                Description = x.Description,
                                FileId = x.FileId,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                UserID = x.UserID
                            }).ToList();

                        rd.docIncomingList = db.DocumentMovements
                             .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                            .Select(x => new RD
                            {
                                Description = x.Description,
                                FileId = x.PrefID,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                                Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                            }).ToList();

                        rd.docOutgoingList = db.DocumentMovements
                            .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                            .Select(x => new RD
                            {
                                Description = x.Description,
                                FileId = x.PrefID,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                                Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                            }).ToList();
                    }
                    else if (report.Option == ReportOption.Year)
                    {
                        rd.docCreated = db.Documents.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.UnitId == unit)).Count();
                        rd.docIncoming = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString())).Count();
                        rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString())).Count();

                        rd.docCreatedList = db.Documents
                            .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.UnitId == unit))
                            .Select(x => new DocDetails
                            {
                                Description = x.Description,
                                FileId = x.FileId,
                                Subject = x.Subject,
                                DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                                UserID = x.UserID
                            }).ToList();

                        rd.docIncomingList = db.DocumentMovements
                            .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                             .Select(x => new RD
                             {
                                 Description = x.Description,
                                 FileId = x.PrefID,
                                 Subject = x.Subject,
                                 DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                 FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                 Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                                 Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                             }).ToList();

                        rd.docOutgoingList = db.DocumentMovements
                            .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                             .Select(x => new RD
                             {
                                 Description = x.Description,
                                 FileId = x.PrefID,
                                 Subject = x.Subject,
                                 DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                                 FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                                 Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                                 Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                             }).ToList();
                    }
                }

            }
            else
            {
                int unit = Convert.ToInt32(report.Unit);
                if (report.Option == ReportOption.Day)
                {
                    dt.Clear();
                    dt = report.Day.Split('/').ToList();
                    string newDate = dt[1].ToString() + "/" + dt[0].ToString() + "/" + dt[2].ToString();

                    DateTime currDate = DateTime.Parse(report.Day);

                    rd.docCreated = db.Documents.Where(x => x.DateCreated == currDate && (x.UnitId == unit)).Count();
                    rd.docIncoming = db.DocumentMovements.Where(x => x.DateCreated == currDate && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString())).Count();
                    rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated == currDate && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString())).Count();

                    rd.docCreatedList = db.Documents.Where(x => x.DateCreated == currDate && (x.UnitId == unit))
                        .Select(x => new DocDetails
                        {
                            Description = x.Description,
                            FileId = x.FileId,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                            UserID = x.UserID
                        }).ToList();

                    rd.docIncomingList = db.DocumentMovements.Where(x => x.DateCreated == currDate && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                        .Select(x => new RD
                        {
                            Description = x.Description,
                            FileId = x.PrefID,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                            Destination = db.Units.Where(g => g.UnitId.ToString() == (x.Destination)).FirstOrDefault().Description,
                            Source = db.Units.Where(g => g.UnitId.ToString() == (x.Source)).FirstOrDefault().Description
                        }).ToList();

                    rd.docOutgoingList = db.DocumentMovements
                        .Where(x => x.DateCreated == currDate && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                        .Select(x => new RD
                        {
                            Description = x.Description,
                            FileId = x.PrefID,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                            Destination = db.Units.Where(g => g.UnitId.ToString() == (x.Destination)).FirstOrDefault().Description,
                            Source = db.Units.Where(g => g.UnitId.ToString() == (x.Source)).FirstOrDefault().Description

                        }).ToList();

                }
                else if (report.Option == ReportOption.DateRange)
                {
                    //rd.docCreated = db.Documents
                    //    .Where(x => x.DateCreated >= dateFrom && x.DateCreated <= dateTo).Count();

                    rd.docCreated = db.Documents
                        .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.UnitId == unit))
                        .Count();

                    rd.docIncoming = db.DocumentMovements
                         .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                         .Count();

                    rd.docOutgoing = db.DocumentMovements
                        .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                        .Count();

                    rd.docCreatedList = db.Documents
                        .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.UnitId == unit))
                        .Select(x => new DocDetails
                        {
                            Description = x.Description,
                            FileId = x.FileId,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                            UserID = x.UserID
                        }).ToList();

                    rd.docIncomingList = db.DocumentMovements
                         .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                        .Select(x => new RD
                        {
                            Description = x.Description,
                            FileId = x.PrefID,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                            Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                            Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                        }).ToList();

                    rd.docOutgoingList = db.DocumentMovements
                        .Where(x => (DbFunctions.TruncateTime(x.DateCreated) <= dateFrom && DbFunctions.TruncateTime(x.DateCreated) >= dateTo) && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                        .Select(x => new RD
                        {
                            Description = x.Description,
                            FileId = x.PrefID,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                            Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                            Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                        }).ToList();
                }
                else if (report.Option == ReportOption.Year)
                {
                    rd.docCreated = db.Documents.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.UnitId == unit)).Count();
                    rd.docIncoming = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString())).Count();
                    rd.docOutgoing = db.DocumentMovements.Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString())).Count();

                    rd.docCreatedList = db.Documents
                        .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.UnitId == unit))
                        .Select(x => new DocDetails
                        {
                            Description = x.Description,
                            FileId = x.FileId,
                            Subject = x.Subject,
                            DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                            FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileTypeId).FirstOrDefault().Description,
                            UserID = x.UserID
                        }).ToList();

                    rd.docIncomingList = db.DocumentMovements
                        .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Incoming) && (x.Destination == unit.ToString()))
                         .Select(x => new RD
                         {
                             Description = x.Description,
                             FileId = x.PrefID,
                             Subject = x.Subject,
                             DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                             FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                             Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,
                             Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description
                         }).ToList();

                    rd.docOutgoingList = db.DocumentMovements
                        .Where(x => x.DateCreated.Value.Year.ToString() == report.Year && (x.Type == Movement.Outgoing) && (x.Source == unit.ToString()))
                         .Select(x => new RD
                         {
                             Description = x.Description,
                             FileId = x.PrefID,
                             Subject = x.Subject,
                             DateCreated = DbFunctions.TruncateTime(x.DateCreated).ToString(),
                             FileType = db.FileTypes.Where(d => d.FileTypeId == x.FileType).FirstOrDefault().Description,
                             Source = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Source)).FirstOrDefault().Description,
                             Destination = db.Units.Where(g => g.UnitId == Convert.ToInt32(x.Destination)).FirstOrDefault().Description,

                         }).ToList();
                }
            
        }

            if (report.Unit == null)
            {
                rd.Unit = "All";
            }
            else
            {
                rd.Unit = MiscClass.getUnitReports(Convert.ToInt32(report.Unit));
            }
            return Json(rd, JsonRequestBehavior.AllowGet);
        }
    }
}