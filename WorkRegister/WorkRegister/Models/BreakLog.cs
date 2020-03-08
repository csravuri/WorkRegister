using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WorkRegister.Models
{
    public class BreakLog : Entity
    {
        [PrimaryKey, AutoIncrement]
        public int BreakLogID { get; set; }
        public DateTime BreakLogDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int WorkLogID { get; set; }        

    }
}
