using SSO.Util.Client;
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
        public DataBaseType database { get; set; }
        [Required]
        public string table { get; set; }
        [Required]
        public string key { get; set; }
        [Required]
        public OperationType operationType { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string extra { get; set; }
        public DateTime doc_time { get; set; }
        public DateTime create_time = DateTime.Now;
        public string id { get { return (database + table + key).ToLower().ToMD5(); } }
    }
}