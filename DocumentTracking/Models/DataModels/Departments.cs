using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class Departments
    {
        [Key]
        public int DeptId { get; set; }
        [Required]
        [Display(Name ="Department Name")]
        public string Description { get; set; }
        public ICollection<Unit> Unit { get; set; }

    }
}