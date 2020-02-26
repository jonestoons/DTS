using DocumentTracking.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DocumentTracking.Models
{
    public class Audit
    {
        public static void Trail(string MainID, eAction action, string RecordID)
        {

            string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            ApplicationDbContext db = new ApplicationDbContext();
            AuditTrail audit = new AuditTrail();
            audit.Action = action;
            audit.MainID = MainID;
            audit.RecordID = RecordID;
            audit.TDate = DateTime.Now;
            audit.UserID = HttpContext.Current.User.Identity.Name;
            audit.Module = controller;
            db.AuditTrails.Add(audit);
            db.SaveChanges();
        }

    }

    public class DisplayData
    {
        public string PrefID { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string Destination { get; set; }
        public string DateCreated { get; set; }
    }

    public class CreateSend
    {
        [Key]
        public int Id { get; set; }
        public string FileId { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateCreated { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public int FileTypeId { get; set; }
        public FileType FileType { get; set; }
        public string UserID { get; set; }
        public string Destination { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateSend { get; set; }



    }

    public class Link
    {
        [Required] 
        [Display(Name ="New ID")]
        public string NewID { get; set; }
        [Required]
        [Display(Name ="Old ID")]
        public string OldID { get; set; }
    }

    public class DisplaySearch
    {
        public int CompanyID { get; set; }
        public string Description { get; set; }
    }

    public class HomeAnalytics
    {
        public int _TDC { get; set; }
        public int _TID { get; set; }
        public int _TOD { get; set; }
        public int _7DC { get; set; }
        public int _7ID { get; set; }
        public int _7OD { get; set; }
        public int _30DC { get; set; }
        public int _30ID { get; set; }
        public int _30OD { get; set; }

    }

   public class DocDetails
    {
        public string FileId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
        public string FileType { get; set; }
        public string UserID { get; set; }
    }

    public class Report
    {
        public ReportOption Option { get; set; }
        public string Role { get; set; }
        public string Unit { get; set; }
        public string Day { get; set; }
        public string DateRange { get; set; }
        public string Year { get; set; }
    }
    public class ReportDisplay
    {
        public int docCreated { get; set; }
        public int docIncoming { get; set; }
        public int docOutgoing { get; set; }
        public string Unit { get; set; }
        public List<DocDetails> docCreatedList { get; set; }
        public List<RD> docIncomingList { get; set; }
        public List<RD> docOutgoingList { get; set; }
    }

    public class RD: DocDetails 
    {
        public string Destination { get; set; }
        public string Source { get; set; }
    }

    public class Merge
    {
        [Display(Name ="Main Company")]
        [Required]
        public string MainCompany { get; set; }
        [Required]
        [Display(Name ="To be Merged")]
        public string OldCompany { get; set; }
        [Display(Name = "Delete after Merging")]
        public bool Del { get; set; }
    }


    public enum ReportOption
    {
        Day,DateRange,Year
    }

    public class MiscClass
    {
        public static string DateConversion(DateTime val)
        {
            string result = "";
            DateTime dateFrom = val;

            val = DateTime.Parse(val.TimeOfDay.ToString());
            DateTime to = DateTime.Parse(DateTime.Now.TimeOfDay.ToString());

            //dateFrom = DateTime.Parse(dateFrom.Date.ToString());
            DateTime DateTo = DateTime.Today;

            double DayDiff = (DateTo.Date - dateFrom.Date).TotalDays; 
            //int HourDiff = val.Subtract(to).Hours;
            double HourDiff = to.Subtract(val).Hours;
            //int MinuteDiffs = val.Subtract(to).Minutes;
            double MinuteDiff = (to - val).Minutes;
            string hour = "";

            if (DayDiff >= 1)
            {
                if (DayDiff == 1)
                {
                  hour= HourDiff.ToString();
                    if (hour.StartsWith("-"))
                    {
                        hour = hour.TrimStart('-');
                    }
                    result = DayDiff.ToString() + "day " + hour + "hrs ago";

                }
                else
                {
                    result = DayDiff.ToString() + "days " + HourDiff.ToString() + "hrs ago";

                }
            }
            else if(DayDiff < 1 && HourDiff >= 1)
            {
                hour = HourDiff.ToString();
                if (hour.StartsWith("-"))
                {
                    hour.TrimStart('-');
                }
                result = hour  + "hrs "+ MinuteDiff.ToString() +"mins ago";
            }
            else if(DayDiff < 1 && HourDiff < 1)
            {
                result = MinuteDiff.ToString() + "mins ago";
            }

            return result;
        }
        public static int getUserUnit()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string username = HttpContext.Current.User.Identity.Name;
            var user = db.Users.Where(s => s.UserName == username).FirstOrDefault();
            return Convert.ToInt32(user.Unit);
        }
        public static string getUnit(int Id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string unit = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().Description;
            int d = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().DeptID;
            string dept = db.Departments.Where(m => m.DeptId == d).FirstOrDefault().Description;
            return "Department: " + dept + ",  Unit: " + unit;
        }
        public static string getUnitReports(int Id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string unit = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().Description;
            int d = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().DeptID;
            string dept = db.Departments.Where(m => m.DeptId == d).FirstOrDefault().Description;
            return unit;
        }
        public static string getUnitSend(int Id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string unit = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().Description;
            int d = db.Units.Where(m => m.UnitId == Id).FirstOrDefault().DeptID;
            string dept = db.Departments.Where(m => m.DeptId == d).FirstOrDefault().Description;
            return "<span><b>Department: </b></span>"+ dept + "<b>|</b> <span><b> Unit: </b></span>"+ unit;
        }
        public static string getFileType(int Id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var f = db.FileTypes.Where(r => r.FileTypeId == Id).FirstOrDefault();
            string filetype = "";
            if (f != null)
            {
                filetype = f.Description;

            }
            return filetype;
        }
        public static string GetUniqueKey(int unit)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var code = db.DocFileCodes.Where(x => x.UnitId == unit).FirstOrDefault();
            if (code != null)
            {
                char[] chars = new char[62];
                chars = "123456789".ToCharArray();
                byte[] data = new byte[1];
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                crypto.GetNonZeroBytes(data);
                data = new byte[5];
                crypto.GetNonZeroBytes(data);
                StringBuilder result = new StringBuilder(5);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }


                string prefix = code.Description;
                return prefix + "-" + result.ToString();
            }
            else
            {
                return "";
                //return "Set System Codes";
            }


        }
        public static string GenerateNumber(int unit)
        {
            char[] chars = new char[62];
            chars = "123456789".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[5];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(5);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();

        }
        public static string GetFullName(string userID)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var s = db.Users.Where(x => x.Email == userID).FirstOrDefault();

            return s.FullName;
        }
        public static void updateDocuments(Document doc)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Entry(doc).State = EntityState.Modified;
            db.SaveChanges();
        }

    }

    public enum Status
    {
        Confirmed,
        Pending
    }

    public enum Movement
    {
        Incoming,
        Outgoing
    }

    public  enum eAction
    {
        Create,
        Confirmed,
        Update,
        Delete,
        Link,
        Merge
    }

}