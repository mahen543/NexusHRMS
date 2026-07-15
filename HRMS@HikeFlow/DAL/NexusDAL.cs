using HRMS_Common;
using HRMS_HikeFlow.NexusProperties;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HRMS_HikeFlow.DAL
{
    public class NexusDAL
    {
        #region ConnectionString

        public SqlConnection _connectionString;
        public int _commandTimeOut { get; }

        public NexusDAL(AppDb objAppDb)
        {
            _connectionString = objAppDb.Connection;
            _commandTimeOut = objAppDb.CommandTimeOut;
        }
        #endregion

        #region Get Error Message
        //*************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   GetErrorMessage
        // Method Description           :   This method is used to get the error messages from the database.
        // Author                       :   Mahendra N
        // Creation Date                :   17 Nov 2026
        // Input Parameters             :   ErrorCode
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   ErrorMessage
        //-------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ------------------------------------------------------------------------------------------------
        //  1.0                 MahendraN                   17 Nov 2026                 Creation
        //*************************************************************************************************
        /// <summary>
        /// <c>GetErrorMessage</c> This method is used to get the error messages from the database.
        /// <param>objEmployeeListDTO</param>
        /// <returns>string</returns> //It returns the error message.
        /// </summary>
        public object GetErrorMessage(int errorCode)
        {

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_GetErrorMessage";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                sqlCmd.Parameters.Add("ErrorCode", SqlDbType.BigInt).Value = errorCode;

                _connectionString.Open();
                object objResult = sqlCmd.ExecuteScalar();
                _connectionString.Close();
                return objResult;
            }
        }
        #endregion


        public DataTable GetEmployees(Request objAPIRequest)
        {

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_GetEmployees";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                if (objAPIRequest.SearchWord != null)
                {
                    sqlCmd.Parameters.Add("SearchWord", SqlDbType.NVarChar).Value = objAPIRequest.SearchWord;
                }
                else
                {
                    sqlCmd.Parameters.Add("SearchWord", SqlDbType.NVarChar).Value = DBNull.Value;
                }

                _connectionString.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                DataTable dtResult = new DataTable();
                da.Fill(dtResult);
                _connectionString.Close();

                return dtResult;
            }
        }

        #region InsertEmployee
        //************************************************************************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   InsertEmployee
        // Method Description           :   This method is used to insert Employee record into the tblEmployee table.
        // Author                       :   Mahendra N
        // Creation Date                :   10 Nov 2023
        // Input Parameters             :   Dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  10 Nov 2026                 Creation
        //  1.1                 Mahendra N                  27 Nov 2026                 Modification(Added Extra parameters to the procedure)
        //  1.2                 Mahendra N                  02 Dec 2026                 Modification(Added DbNull.Value where possibility of Nulls pass to procedure)
        //************************************************************************************************************************************************************
        /// <summary>
        /// <c>InsertEmployee</c> This method is used to insert Employee record into the tblEmployee table.
        /// <param>Dynamic</param>
        /// </summary>
        public object InsertEmployee(dynamic obj)
        {
            obj.DateCreated = DateTime.Now;

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_InsertEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // Add parameters with proper SqlDbType
                sqlCmd.Parameters.Add("ReportName", SqlDbType.NVarChar).Value = obj.ReportName;
                sqlCmd.Parameters.Add("ReportFilePath", SqlDbType.NVarChar).Value = obj.ReportFilePath;
                sqlCmd.Parameters.Add("IsPublished", SqlDbType.Bit).Value = obj.IsPublished != null ? (object)obj.IsPublished : DBNull.Value;
                sqlCmd.Parameters.Add("CreatedBy", SqlDbType.NVarChar).Value = obj.CreatedBy;
                sqlCmd.Parameters.Add("DateCreated", SqlDbType.DateTime2).Value = obj.DateCreated;
                sqlCmd.Parameters.Add("ReleaseYear", SqlDbType.NVarChar).Value = obj.ReleaseYear;
                sqlCmd.Parameters.Add("IpAddress", SqlDbType.NVarChar).Value = obj.IpAddress != null ? (object)obj.IpAddress : DBNull.Value;
                sqlCmd.Parameters.Add("Tag", SqlDbType.NVarChar).Value = obj.Tag != null ? (object)obj.Tag : DBNull.Value;
                sqlCmd.Parameters.Add("Comments", SqlDbType.NVarChar).Value = obj.Comments != null ? (object)obj.Comments : DBNull.Value;

                _connectionString.Open();
                object objValue = sqlCmd.ExecuteScalar();
                _connectionString.Close();
                return objValue;
            }
        }
        #endregion

        #region UpdateQuarterlyReleaseById
        //***********************************************************************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   UpdateEmployeeById
        // Method Description           :   This method is used to update Employee record based on Id.
        // Author                       :   Mahendra N
        // Creation Date                :   14 Nov 2026
        // Input Parameters             :   Dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // ----------------------------------------------------------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  14 Nov 2026                 Creation
        //  1.1                 Mahendra N                  02 Dec 2026                 Modification(Added DbNull.Value where possibility of Nulls pass to procedure)
        //***********************************************************************************************************************************************************
        /// <summary>
        /// <c>UpdateEmployeeById</c> This method is used to update Employee record based on Id.
        /// <param>Dynamic</param>
        /// </summary>
        public object UpdateEmployeeById(dynamic obj)
        {
            obj.LastUpdated = DateTime.Now;

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_UpdateEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // Add parameters with proper SqlDbType
                sqlCmd.Parameters.Add("ReportName", SqlDbType.NVarChar).Value = obj.ReportName != null ? (object)obj.ReportName : DBNull.Value;
                sqlCmd.Parameters.Add("ReleaseYear", SqlDbType.NVarChar).Value = obj.ReleaseYear != null ? (object)obj.ReleaseYear : DBNull.Value;
                sqlCmd.Parameters.Add("ReportFilePath", SqlDbType.NVarChar).Value = obj.ReportFilePath != null ? (object)obj.ReportFilePath : DBNull.Value;
                sqlCmd.Parameters.Add("IsPublished", SqlDbType.Bit).Value = obj.IsPublished != null ? (object)obj.IsPublished : DBNull.Value;
                sqlCmd.Parameters.Add("UpdatedBy", SqlDbType.NVarChar).Value = obj.UpdatedBy;
                sqlCmd.Parameters.Add("LastUpdated", SqlDbType.DateTime2).Value = obj.LastUpdated;
                sqlCmd.Parameters.Add("Id", SqlDbType.NVarChar).Value = obj.Id;

                _connectionString.Open();
                object objValue = sqlCmd.ExecuteScalar();
                _connectionString.Close();
                return objValue;
            }
        }
        #endregion


        #region DeleteEmployeeById
        //*********************************************************************************************************
        // Layer                        :   DAL 
        // Method Name                  :   DeleteEmployeeById
        // Method Description           :   This method is used to delete the Employee record based on Id.
        // Author                       :   Mahendra N
        // Creation Date                :   08 Nov 2026
        // Input Parameters             :   Dynamic
        // Modified Date                : 
        // Modified Reason              :
        // Return Values                :   
        //---------------------------------------------------------------------------------------------------------
        //  Version             Author                      Date                        Remarks       
        // --------------------------------------------------------------------------------------------------------
        //  1.0                 Mahendra N                  08 Nov 2026                 Creation
        //*********************************************************************************************************
        /// <summary>
        /// <c>DeleteEmployeeById</c> This method is used to delete the Employee record based on Id.
        /// <param>Dynamic</param>
        /// </summary>
        public object DeleteEmployeeById(Request objAPIRequest)
        {
            objAPIRequest.DateDeleted = DateTime.Now;

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_DeleteEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // Add parameters with proper SqlDbType
                sqlCmd.Parameters.Add("DeletedBy", SqlDbType.NVarChar).Value = objAPIRequest.DeletedBy;
                sqlCmd.Parameters.Add("Id", SqlDbType.NVarChar).Value = objAPIRequest.Id;
                sqlCmd.Parameters.Add("DateDeleted", SqlDbType.DateTime2).Value = objAPIRequest.DateDeleted;

                _connectionString.Open();
                object i = (int)sqlCmd.ExecuteNonQuery();
                _connectionString.Close();
                return i;
            }
        }
        #endregion
    }
}
