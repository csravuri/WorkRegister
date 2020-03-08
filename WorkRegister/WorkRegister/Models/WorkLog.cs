using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WorkRegister.Models
{
    public class WorkLog : Entity
    {
        [PrimaryKey, AutoIncrement]
        public int WorkLogID { get; set; }
        public DateTime WorkLogDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        
    }
}
