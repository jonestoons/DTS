using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }
        [Required]
        [Display(Name ="Unit Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name ="Department")]
        public int DeptID { get; set; }
        public Departments Departments { get; set; }
        public ICollection<Document> Document { get; set; }
    }
}