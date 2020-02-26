using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class DocFileCode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Unit")]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public string Description { get; set; }
    }
}