using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkRegister.Models;
using Xamarin.Forms;

namespace WorkRegister
{
    public partial class MainPage : TabbedPage
    {
        DbTransaction dbTransaction;
        WorkLog workLog;
        BreakLog breakLog;

        public MainPage()
        {
            InitializeComponent();
                        
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitialScreenSetup();

        }

        private async void InitialScreenSetup()
        {
            dbTransaction = DbTransaction.GetInstance();

            workLog = await dbTransaction.GetLatestWorkLog();

            if(workLog == null)
            {
                workLog = new WorkLog();
                WorkStopState();
            }
            else
            {

                WorkStartState();
            }

            breakLog = await dbTransaction.GetLatestBreakLog();

            if (breakLog == null)
            {
                breakLog = new BreakLog();
                BreakStopState();
            }
            else
            {

                BreakStartState();
            }            
        }

        
        
        private async void OnStartClick(object sender, EventArgs e)
        {
            workLog.WorkLogDate = DateTime.Today;
            workLog.StartTime = DateTime.Now;

            workLog.WorkLogID = await dbTransaction.InsertLog(workLog);

            WorkStartState();

        }

        private void OnStopClick(object sender, EventArgs e)
        {
            workLog.EndTime = DateTime.Now;            
            dbTransaction.UpdateLog(workLog);
            TimeSpan timeDiff = (TimeSpan)(workLog.EndTime - workLog.StartTime);
            lblWorkDuration.Text = $"{timeDiff.Hours}h {timeDiff.Minutes}m {timeDiff.Seconds}s";

            workLog = new WorkLog();

            WorkStopState();

        }

        private async void OnBeginClick(object sender, EventArgs e)
        {
            breakLog.BreakLogDate = DateTime.Today;
            breakLog.WorkLogID = workLog.WorkLogID;
            breakLog.StartTime = DateTime.Now;

            await dbTransaction.InsertLog(breakLog);


            BreakStartState();

        }

        private void OnEndClick(object sender, EventArgs e)
        {
            breakLog.EndTime = DateTime.Now;
            dbTransaction.UpdateLog(breakLog);

            TimeSpan timeDiff = (TimeSpan)(breakLog.EndTime - breakLog.StartTime);
            lblBreakTimer.Text = $"{timeDiff.Hours}h {timeDiff.Minutes}m {timeDiff.Seconds}s";

            breakLog = new BreakLog();

            BreakStopState();


        }

        private async void OnSettingsClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }


        private void WorkStartState()
        {
            btnWorkStart.IsEnabled = false;
            btnWorkStop.IsEnabled = true;

            btnBreakStart.IsEnabled = true;
            btnBreakStop.IsEnabled = false;

        }

        private void WorkStopState()
        {
            btnWorkStart.IsEnabled = true;
            btnWorkStop.IsEnabled = false;

            btnBreakStart.IsEnabled = false;
            btnBreakStop.IsEnabled = false;
        }

        private void BreakStartState()
        {
            btnWorkStart.IsEnabled = false;
            btnWorkStop.IsEnabled = false;

            btnBreakStart.IsEnabled = false;
            btnBreakStop.IsEnabled = true;
        }

        private void BreakStopState()
        {
            if (workLog == null || workLog.StartTime == null)
            {
                WorkStopState();
            }
            else
            {
                btnWorkStart.IsEnabled = false;
                btnWorkStop.IsEnabled = true;

                btnBreakStart.IsEnabled = true;
                btnBreakStop.IsEnabled = false;
            }
        }

        private async void OnReportSearch(object sender, EventArgs e)
        {
            List<WorkLog> workLogs = await dbTransaction.GetWorkLogs(dpFromDate.Date, dpToDate.Date);
            List<BreakLog> breakLogs = await dbTransaction.GetBreakLogs(dpFromDate.Date, dpToDate.Date);

            List<WorkReport> workReports = GetReportGridData(workLogs, breakLogs);

            AddReportGridData(workReports);
            
        }

        private void AddReportGridData(List<WorkReport> workReports)
        {
            gridReportData.Children.Clear();
            if(workReports.Count > 0)
            {
                gridReportData.RowDefinitions = new RowDefinitionCollection();
                for (int i = 0; i < workReports.Count; i++)
                {
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = workReports[i].Date.ToString("dd/MMM/yyyy")}, 0, i);
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = workReports[i].StartTime?.ToString("hh:mm:ss")}, 1, i);
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = workReports[i].EndTime?.ToString("hh:mm:ss")}, 2, i);
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = Utils.GetTimeString(workReports[i].WorkTime)}, 3, i);
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = Utils.GetTimeString(workReports[i].BreakTime)}, 4, i);
                    gridReportData.Children.Add(new Label {BackgroundColor = Color.White, Text = Utils.GetTimeString(workReports[i].TotalTime)}, 5, i);

                }


            }
        }

        private List<WorkReport> GetReportGridData(List<WorkLog> workLogs, List<BreakLog> breakLogs)
        {
            List<WorkReport> workReports = workLogs.GroupJoin(breakLogs,
                                                w => w.WorkLogID, b => b.WorkLogID,
                                                (w, b) => new WorkReport
                                                {
                                                    Date = w.WorkLogDate,
                                                    StartTime = w.StartTime,
                                                    EndTime = w.EndTime,
                                                    WorkTime = w.EndTime - w.StartTime,
                                                    BreakTime = new TimeSpan(b.Sum(x => (x.EndTime - x.StartTime)?.Ticks) ?? 0),
                                                    TotalTime = new TimeSpan(((w.EndTime - w.StartTime)?.Ticks + b.Sum(x => (x.EndTime - x.StartTime)?.Ticks)) ?? 0)
                                                }).GroupBy(
                                                    y => y.Date
                                                 ).Select(
                                                    z => new WorkReport
                                                    {
                                                        Date = z.Key.Date,
                                                        StartTime = z.Min(s => s.StartTime),
                                                        EndTime = z.Max(et => et.EndTime),
                                                        WorkTime = new TimeSpan(z.Sum(wt => wt.WorkTime?.Ticks ?? 0)),
                                                        BreakTime = new TimeSpan(z.Sum(bt => bt.BreakTime?.Ticks ?? 0)),
                                                        TotalTime = new TimeSpan(z.Sum(tt => tt.TotalTime?.Ticks ?? 0))
                                                    }
                                                 ).ToList();


            return workReports;
        }

        
        


    }
}
