using HikeFlowService.DAL;

namespace HikeFlowService.BAL
{
    public class HikeFlowServiceBAL
    {
        #region Constructor
        private readonly General objGeneral;
        private readonly IConfiguration objConfig;
        public HikeFlowServiceBAL(General obj, IConfiguration config)
        {
            objGeneral = obj;
            objConfig = config;
        }
        #endregion

        //***************************************************************************************************
        // Layer                        :   BAL
        // Method Name                  :   StageSalaryHikeRequestsBAL
        // Method Description           :   This method is used to stage Pending salary hike requests for the current review cycle.
        // Author                       :   MahendraN
        // Creation Date                :   01 Oct 2026
        // Input Parameters             :   NA
        // Modified Date                :
        // Modified Reason              :
        // Return Values                :   (long ReviewCycleId, int InsertedCount)
        //                                  InsertedCount : -2 failed, -1 no cycle, 0 already staged, >0 rows inserted
        //----------------------------------------------------------------------------------------------------
        //  Version             Author                             Date                        Remarks
        // ---------------------------------------------------------------------------------------------------
        //  1.0                 MahendraN                          01 Oct 2026                 Creation
        //****************************************************************************************************
        public (long ReviewCycleId, int InsertedCount) StageSalaryHikeRequestsBAL()
        {
            objGeneral.CreateCodeLog("HikeFlowServiceBAL", "StageSalaryHikeRequestsBAL", "Step 4 : StageSalaryHikeRequestsBAL Method is executing in BAL layer");
            try
            {
                objGeneral.CreateCodeLog("HikeFlowServiceBAL", "StageSalaryHikeRequestsBAL", "Step 5 : Before Calling StageSalaryHikeRequestsDB Method from StageSalaryHikeRequestsBAL Method");
                var objHikeFlowServiceDAL = new HikeFlowServiceDAL(objConfig);
                objHikeFlowServiceDAL.StageSalaryHikeRequestsDB(out long reviewCycleId, out int insertedCount);

                if (insertedCount == -1)
                    objGeneral.CreateErrorLog(new Exception("HikeFlowService: no active upcoming review cycle found, nothing was staged."));
                else if (insertedCount == 0)
                    objGeneral.CreateCodeLog("HikeFlowServiceBAL", "StageSalaryHikeRequestsBAL", $"Step 6 : ReviewCycleId {reviewCycleId} is already staged, nothing inserted");
                else
                    objGeneral.CreateCodeLog("HikeFlowServiceBAL", "StageSalaryHikeRequestsBAL", $"Step 6 : ReviewCycleId {reviewCycleId} staged, rows inserted : {insertedCount}");

                return (reviewCycleId, insertedCount);
            }
            catch (Exception ex)
            {
                objGeneral.CreateErrorLog(ex);
                return (0, -2);
            }
        }
    }
}