using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MessageCenter.Web.Models
{
    public class SearchDataModel
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }
        public DateTime doc_time { get; set; }
        public DateTime create_time = DateTime.Now;
    }
}