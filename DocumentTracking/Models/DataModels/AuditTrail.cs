using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class AuditTrail
    {
        [Key]
        public int Id { get; set; }
        public string UserID { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime TDate { get; set; }
        public eAction Action { get; set; }
        public string MainID { get; set; }
        public string RecordID { get; set; }
        public string Module { get; set; }
    }
}