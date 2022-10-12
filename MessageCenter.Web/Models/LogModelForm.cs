using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MessageCenter.Web.Models
{
    public class LogModelForm
    {
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Route { get; set; }
        public string QueryString { get; set; }
        public string Content { get; set; }
        public string Response { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string UserHost { get; set; }
        [Required]
        public string UserAgent { get; set; }
        [Required]
        public long Time { get; set; }
        public int CountPerMinute { get; set; }
        public bool Exception { get; set; }

    }
}