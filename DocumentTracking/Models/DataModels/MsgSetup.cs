using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentTracking.Models.DataModels
{
    public class MsgSetup
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public bool Backlog { get; set; }

    }
}