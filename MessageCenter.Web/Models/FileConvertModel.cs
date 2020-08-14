using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MessageCenter.Web.Models
{
    public class FileConvertModel
    {
        [JsonIgnore]
        public string MachineName { get; set; }
        [Required]
        public string CollectionName { get; set; }
        [Required]
        public string CollectionId { get; set; }
    }
}