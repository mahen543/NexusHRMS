using HRMS_Common;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace HRMS_HikeFlow.DAL
{
    public class AuthDAL
    {

        public SqlConnection _connectionString;
        public int _commandTimeOut { get; }

        public AuthDAL(AppDb objAppDb)
        {
            _connectionString = objAppDb.Connection;
            _commandTimeOut = objAppDb.CommandTimeOut;
        }

        /// <summary>
        /// Retrieves user authentication details from the database using their username.
        /// </summary>
        /// <param name="strUsername">The username of the user to authenticate.</param>
        /// <returns>A DataTable containing the matching user's authentication details.</returns>
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


        /// <summary>
        /// Retrieves active user hierarchy and designation details by user ID to refresh authentication tokens or re-verify claims.
        /// </summary>
        /// <param name="longUserId">The unique numeric identifier of the target user.</param>
        /// <returns>A DataTable populated with active user details and security claims.</returns>
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
    }
}