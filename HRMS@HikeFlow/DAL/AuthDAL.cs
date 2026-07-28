using HRMS_Common;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace HRMS_HikeFlow.DAL
{
    public class AuthDAL
    {
        #region ConnectionString

        public SqlConnection _connectionString;
        public int _commandTimeOut { get; }

        public AuthDAL(AppDb objAppDb)
        {
            _connectionString = objAppDb.Connection;
            _commandTimeOut = objAppDb.CommandTimeOut;
        }

        #endregion

        #region GetUserForAuth
        //*************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   GetUserForAuth
        // Method Description           :   Retrieves user authentication details by Username.
        // Author                       :   Mahendra N
        // Creation Date                :   27 July 2026
        // Input Parameters             :   strUsername
        // Return Values                :   DataTable
        //*************************************************************************************************
        public DataTable GetUserForAuth(string strUsername)
        {
            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_GetUserForAuth";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                if (!string.IsNullOrWhiteSpace(strUsername))
                {
                    sqlCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 64).Value = strUsername;
                }
                else
                {
                    sqlCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 64).Value = DBNull.Value;
                }

                _connectionString.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                DataTable dtResult = new DataTable();
                da.Fill(dtResult);
                _connectionString.Close();

                return dtResult;
            }
        }
        #endregion

        #region GetUserByIdForRefresh
        //*************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   GetUserByIdForRefresh
        // Method Description           :   Retrieves active user hierarchy & designation details by UserId
        //                                  for refreshing user tokens or re-verifying claims.
        // Author                       :   Mahendra N
        // Creation Date                :   27 July 2026
        // Input Parameters             :   longUserId
        // Return Values                :   DataTable
        //*************************************************************************************************
        public DataTable GetUserByIdForRefresh(long longUserId)
        {
            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_GetUserByIdForRefresh";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                sqlCmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = longUserId;

                _connectionString.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                DataTable dtResult = new DataTable();
                da.Fill(dtResult);
                _connectionString.Close();

                return dtResult;
            }
        }
        #endregion
    }
}