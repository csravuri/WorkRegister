using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRegister.Models
{
    public class WorkReport
    {
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? BreakTime { get; set; }
        public TimeSpan? TotalTime { get; set; }

    }
}
