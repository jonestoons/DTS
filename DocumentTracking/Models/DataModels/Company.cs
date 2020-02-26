using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
}