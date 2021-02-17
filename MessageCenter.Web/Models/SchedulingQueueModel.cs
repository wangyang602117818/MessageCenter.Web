using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MessageCenter.Web.Models
{
    public class SchedulingQueueModel
    {
        [JsonIgnore]
        public string MachineName { get; set; }
        [Required]
        public int SchedulingId { get; set; }
        [Required]
        public int TriggerId { get; set; }
        public SchedulingStateEnum SchedulingState { get; set; }
    }
    public enum SchedulingStateEnum
    {
        Stoped = -1,
        Running = 0,
    }
}