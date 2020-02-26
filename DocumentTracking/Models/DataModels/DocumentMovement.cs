using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class DocumentMovement
    {
        [Key]
        public int Id { get; set; }
        public string PrefID { get; set; }
        [Required]
        public string Description { get; set; }
        public string Subject { get; set; }
        public int FileType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateStatus { get; set; }
        public Status Status { get; set; }
        public Movement Type { get; set; }
        public bool Linked { get; set; }
        public string OldId { get; set; }
        public DateTime? LinkDate { get; set; }

    }
}