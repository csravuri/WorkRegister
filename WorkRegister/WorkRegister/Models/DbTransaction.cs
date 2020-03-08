using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WorkRegister.Models
{
    public class DbTransaction
    {
        private static DbTransaction dbTransaction = null;
        private readonly SQLiteAsyncConnection _connection;
        private DbTransaction()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Database");

            Utils.EnsureDirectoryExists(folderPath);

            string dbPath = Path.Combine(folderPath, "WorkRegister.db3");

            _connection = new SQLiteAsyncConnection(dbPath);

            CreateTables();
        }

        public static DbTransaction GetInstance()
        {
            if(dbTransaction == null)
            {
                dbTransaction = new DbTransaction();
            }

            return dbTransaction;
        }

        public async void CreateTables()
        {
            await _connection.CreateTablesAsync(CreateFlags.None, typeof(WorkLog), typeof(BreakLog));
        }

        public async Task<int> InsertLog(Entity log)
        {
            return await _connection.InsertAsync(log);
        }

        public async void UpdateLog(Entity log)
        {
            await _connection.UpdateAsync(log);
        }

        public async Task<WorkLog> GetLatestWorkLog()
        {
            return await _connection.FindAsync<WorkLog>(x => x.EndTime == null);
        }
        
        public async Task<BreakLog> GetLatestBreakLog()
        {
            return await _connection.FindAsync<BreakLog>(x => x.EndTime == null);
        }

        public async Task<List<WorkLog>> GetWorkLogs(DateTime? fromDate, DateTime? toDate)
        {
            return await _connection.Table<WorkLog>().Where(x => (fromDate == null || x.WorkLogDate >= fromDate) && (toDate == null || x.WorkLogDate <= toDate)).ToListAsync();
        }

        public async Task<List<BreakLog>> GetBreakLogs(DateTime? fromDate, DateTime? toDate)
        {
            return await _connection.Table<BreakLog>().Where(x => (fromDate == null || x.BreakLogDate >= fromDate) && (toDate == null || x.BreakLogDate <= toDate)).ToListAsync();
        }

    }
}
