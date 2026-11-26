using Cronos;
using HikeFlowService.BAL;

namespace HikeFlowService
{
    public class Worker : BackgroundService
    {
        #region common variables
        private readonly General objGeneral;
        private readonly IConfiguration objConfig;
        #endregion

        public Worker(General general, IConfiguration config)
        {
            objGeneral = general;
            objConfig = config;
        }

        //************************************************************************************************************
        // Layer                        :   Service
        // Method Name                  :   ExecuteAsync
        // Method Description           :   Starts the service and waits for each CRON occurrence (8 Jun / 8 Dec)
        // Author                       :   MahendraN
        // Creation Date                :   01 Oct 2026
        // Input Parameters             :   CancellationToken stoppingToken
        // Modified Date                :   -
        // Modified Reason              :   -
        // Return Values                :   Task
        //------------------------------------------------------------------------------------------------------------
        //  Version             Author                    Date                       Remarks
        // -----------------------------------------------------------------------------------------------------------
        //  1.0                 MahendraN                 01 Oct 2026                Creation
        //************************************************************************************************************
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            objGeneral.CreateCodeLog("Worker", "ExecuteAsync", "          Step 1 : Service is started and ExecuteAsync Method is running");
            try
            {
                var cron = CronExpression.Parse(objConfig["AppSettings:cronExpression"]!);
                var tz = TimeZoneInfo.FindSystemTimeZoneById(objConfig["AppSettings:timeZone"]!);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var next = cron.GetNextOccurrence(DateTimeOffset.UtcNow, tz);
                    if (next is null) break;
                    objGeneral.CreateCodeLog("Worker", "ExecuteAsync", $"          Step 2 : Next run scheduled at {next}");

                    // Task.Delay cannot exceed ~49 days, so wait in chunks
                    while (DateTimeOffset.UtcNow < next.Value && !stoppingToken.IsCancellationRequested)
                    {
                        var remaining = next.Value - DateTimeOffset.UtcNow;
                        await Task.Delay(remaining > TimeSpan.FromHours(12) ? TimeSpan.FromHours(12) : remaining, stoppingToken);
                    }
                    if (stoppingToken.IsCancellationRequested) break;

                    ElapseTime();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                objGeneral.CreateErrorLog(ex);
            }
            objGeneral.CreateCodeLog("Worker", "ExecuteAsync", $"Service is stopped at {DateTime.Now}");
        }

        //**********************************************************************************************
        // Layer                        :   Service
        // Method Name                  :   ElapseTime
        // Method Description           :   Executes the salary hike staging when the CRON time is reached
        // Author                       :   MahendraN
        // Creation Date                :   01 Oct 2026
        // Input Parameters             :   -
        // Modified Date                :
        // Modified Reason              :
        // Return Values                :   void
        //----------------------------------------------------------------------------------------------
        //  Version             Author                          Date                        Remarks
        // ---------------------------------------------------------------------------------------------
        //  1.0                 MahendraN                       01 Oct 2026                 Creation
        //**********************************************************************************************
        private void ElapseTime()
        {
            try
            {
                objGeneral.CreateCodeLog("Worker", "ElapseTime", "       Step 3 : Before Calling StageSalaryHikeRequestsBAL method from the ElapseTime method");
                var objHikeFlowServiceBAL = new HikeFlowServiceBAL(objGeneral, objConfig);
                var (reviewCycleId, insertedCount) = objHikeFlowServiceBAL.StageSalaryHikeRequestsBAL();

                string strResult = insertedCount switch
                {
                    -2 => "FAILED, see the error log",
                    -1 => "SKIPPED, no active upcoming review cycle",
                    0 => $"NOTHING TO DO, ReviewCycleId {reviewCycleId} was already staged",
                    _ => $"SUCCESS, {insertedCount} salary hike requests staged for ReviewCycleId {reviewCycleId}"
                };

                objGeneral.CreateCodeLog("Worker", "ElapseTime", $"       Step 9 : Run finished at {DateTime.Now} : {strResult}");
            }
            catch (Exception ex)
            {
                objGeneral.CreateErrorLog(ex);
            }
        }
    }
}