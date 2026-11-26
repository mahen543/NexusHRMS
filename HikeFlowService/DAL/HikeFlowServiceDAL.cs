using Microsoft.Data.SqlClient;
using System.Data;

namespace HikeFlowService.DAL
{
    public class HikeFlowServiceDAL
    {
        #region Common Variables
        private readonly string strConnectionString;
        private readonly int intCommandTimeout;

        public HikeFlowServiceDAL(IConfiguration config)
        {
            strConnectionString = config.GetConnectionString("connectionString")!;
            intCommandTimeout = Convert.ToInt32(config["AppSettings:commandTimeOut"]);
        }
        #endregion

        #region StageSalaryHikeRequests
        //***************************************************************************************************
        // Layer                        :   DAL
        // Method Name                  :   StageSalaryHikeRequestsDB
        // Method Description           :   This method is used to execute usp_StageSalaryHikeRequests which inserts Pending rows into tblSalaryHikeRequests.
        // Author                       :   MahendraN
        // Creation Date                :   01 Oct 2026
        // Input Parameters             :   out long reviewCycleId, out int insertedCount
        // Modified Date                :
        // Modified Reason              :
        // Return Values                :   void (values returned through the two out parameters)
        //                                  insertedCount : -1 no cycle, 0 already staged, >0 rows inserted
        //----------------------------------------------------------------------------------------------------
        //  Version             Author                       Date                        Remarks
        // ---------------------------------------------------------------------------------------------------
        //  1.0                 MahendraN                     01 Oct 2026                 Creation
        //****************************************************************************************************
        public void StageSalaryHikeRequestsDB(out long reviewCycleId, out int insertedCount)
        {
            using (SqlConnection sqlConnection = new SqlConnection(strConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandTimeout = intCommandTimeout;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "usp_StageSalaryHikeRequests";

                    var pReviewCycleId = new SqlParameter("@ReviewCycleId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                    var pInsertedCount = new SqlParameter("@InsertedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    sqlCommand.Parameters.Add(pReviewCycleId);
                    sqlCommand.Parameters.Add(pInsertedCount);

                    sqlCommand.ExecuteNonQuery();

                    reviewCycleId = pReviewCycleId.Value == DBNull.Value ? 0 : Convert.ToInt64(pReviewCycleId.Value);
                    insertedCount = pInsertedCount.Value == DBNull.Value ? -1 : Convert.ToInt32(pInsertedCount.Value);
                }
            }
        }
        #endregion
    }
}