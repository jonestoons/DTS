using DocumentTracking.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models
{
    public class FileType
    {
        [Key]
        public int FileTypeId { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<Document> Document { get; set; }
    }
}