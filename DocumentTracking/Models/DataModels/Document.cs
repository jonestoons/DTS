using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="File ID")]
        //[RegularExpression("/(.*[a-z]){3}/i")]
        public string FileId { get; set; }
        //[Required]
        public string Subject { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }
        [Display(Name ="Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        [Display(Name ="File Type")]
        public int FileTypeId { get; set; }
        public FileType FileType { get; set; }
        public string UserID { get; set; }
        public string OldFileId { get; set; }


    }
}