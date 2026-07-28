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


        public DataTable GetEmployees(GetRequest objAPIRequest)
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
        public object InsertEmployee(InsertRequest objAPIRequest)
        {

            using (SqlCommand sqlCmd = new())
            {

                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_InsertEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // Required Text / String parameters
                sqlCmd.Parameters.Add("FirstName", SqlDbType.NVarChar, 64).Value = objAPIRequest.FirstName;
                sqlCmd.Parameters.Add("Email", SqlDbType.NVarChar, 128).Value = objAPIRequest.Email;
                sqlCmd.Parameters.Add("Cmail", SqlDbType.NVarChar, 128).Value = objAPIRequest.Email;
                sqlCmd.Parameters.Add("MobileNo", SqlDbType.NVarChar, 16).Value = objAPIRequest.MobileNo;
                sqlCmd.Parameters.Add("Gender", SqlDbType.NVarChar, 8).Value = objAPIRequest.Gender;
                sqlCmd.Parameters.Add("EmployeeType", SqlDbType.NVarChar, 32).Value = objAPIRequest.EmployeeType;
                sqlCmd.Parameters.Add("IPAddress", SqlDbType.NVarChar, 64).Value = objAPIRequest.IPAddress;
                sqlCmd.Parameters.Add("CreatedBy", SqlDbType.NVarChar, 128).Value = objAPIRequest.CreatedBy;

                // Optional Text / String parameters
                sqlCmd.Parameters.Add("MiddleName", SqlDbType.NVarChar, 64).Value = objAPIRequest.MiddleName != null ? (object)objAPIRequest.MiddleName : DBNull.Value;
                sqlCmd.Parameters.Add("LastName", SqlDbType.NVarChar, 64).Value = objAPIRequest.LastName != null ? (object)objAPIRequest.LastName : DBNull.Value;
                sqlCmd.Parameters.Add("Tag", SqlDbType.NVarChar, 64).Value = objAPIRequest.Tag != null ? (object)objAPIRequest.Tag : DBNull.Value;
                sqlCmd.Parameters.Add("Comments", SqlDbType.NVarChar, 512).Value = objAPIRequest.Comments != null ? (object)objAPIRequest.Comments : DBNull.Value;

                // Structural & Numeric values
                sqlCmd.Parameters.Add("DepartmentId", SqlDbType.BigInt).Value = objAPIRequest.DepartmentId;
                sqlCmd.Parameters.Add("DesignationId", SqlDbType.BigInt).Value = objAPIRequest.DesignationId;
                sqlCmd.Parameters.Add("ReportingManagerId", SqlDbType.BigInt).Value = objAPIRequest.ReportingManagerId != null ? (object)objAPIRequest.ReportingManagerId : DBNull.Value;
                sqlCmd.Parameters.Add("CompanyId", SqlDbType.BigInt).Value = objAPIRequest.CompanyId;
                sqlCmd.Parameters.Add("SiteId", SqlDbType.BigInt).Value = objAPIRequest.SiteId;
                sqlCmd.Parameters.Add("CurrentSalary", SqlDbType.Decimal).Value = objAPIRequest.CurrentSalary;

                // Date parameter
                sqlCmd.Parameters.Add("DateOfJoining", SqlDbType.Date).Value = objAPIRequest.DateOfJoining != null ? (object)objAPIRequest.DateOfJoining : DBNull.Value;

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
        public object UpdateEmployeeById(UpdateRequest objAPIRequest)
        {

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_UpdateEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // 1. Mandatory Core Identifiers
                sqlCmd.Parameters.Add("Id", SqlDbType.NVarChar, 64).Value = objAPIRequest.Id;
                sqlCmd.Parameters.Add("UpdatedBy", SqlDbType.NVarChar, 128).Value = objAPIRequest.UpdatedBy;

                // 2. Optional Text & Contact Strings
                sqlCmd.Parameters.Add("FirstName", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.FirstName ?? DBNull.Value;
                sqlCmd.Parameters.Add("MiddleName", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.MiddleName ?? DBNull.Value;
                sqlCmd.Parameters.Add("LastName", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.LastName ?? DBNull.Value;
                sqlCmd.Parameters.Add("Email", SqlDbType.NVarChar, 128).Value = (object?)objAPIRequest.Email ?? DBNull.Value;
                sqlCmd.Parameters.Add("Cmail", SqlDbType.NVarChar, 128).Value = (object?)objAPIRequest.Cmail ?? DBNull.Value;
                sqlCmd.Parameters.Add("MobileNo", SqlDbType.NVarChar, 16).Value = (object?)objAPIRequest.MobileNo ?? DBNull.Value;
                sqlCmd.Parameters.Add("Gender", SqlDbType.NVarChar, 8).Value = (object?)objAPIRequest.Gender ?? DBNull.Value;
                sqlCmd.Parameters.Add("EmployeeType", SqlDbType.NVarChar, 32).Value = (object?)objAPIRequest.EmployeeType ?? DBNull.Value;

                // 3. Optional Foreign Key Identifiers
                sqlCmd.Parameters.Add("DepartmentId", SqlDbType.BigInt).Value = (object?)objAPIRequest.DepartmentId ?? DBNull.Value;
                sqlCmd.Parameters.Add("DesignationId", SqlDbType.BigInt).Value = (object?)objAPIRequest.DesignationId ?? DBNull.Value;
                sqlCmd.Parameters.Add("ReportingManagerId", SqlDbType.BigInt).Value = (object?)objAPIRequest.ReportingManagerId ?? DBNull.Value;
                sqlCmd.Parameters.Add("CompanyId", SqlDbType.BigInt).Value = (object?)objAPIRequest.CompanyId ?? DBNull.Value;
                sqlCmd.Parameters.Add("SiteId", SqlDbType.BigInt).Value = (object?)objAPIRequest.SiteId ?? DBNull.Value;

                // 4. Optional Decimals and Integers
                sqlCmd.Parameters.Add("CurrentSalary", SqlDbType.Decimal).Value = (object?)objAPIRequest.CurrentSalary ?? DBNull.Value;
                sqlCmd.Parameters.Add("ExperienceYears", SqlDbType.Decimal).Value = (object?)objAPIRequest.ExperienceYears ?? DBNull.Value;
                sqlCmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = (object?)objAPIRequest.SortOrder ?? DBNull.Value;

                // 5. Optional Flags
                sqlCmd.Parameters.Add("IsPublished", SqlDbType.Bit).Value = (object?)objAPIRequest.IsPublished ?? DBNull.Value;
                sqlCmd.Parameters.Add("DisplayOnWeb", SqlDbType.Bit).Value = (object?)objAPIRequest.DisplayOnWeb ?? DBNull.Value;

                // 6. System Audit & Metadata Strings
                sqlCmd.Parameters.Add("PublishedBy", SqlDbType.NVarChar, 128).Value = (object?)objAPIRequest.PublishedBy ?? DBNull.Value;
                sqlCmd.Parameters.Add("Tag", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.Tag ?? DBNull.Value;
                sqlCmd.Parameters.Add("Comments", SqlDbType.NVarChar, 512).Value = (object?)objAPIRequest.Comments ?? DBNull.Value;
                sqlCmd.Parameters.Add("IPAddress", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.IPAddress ?? DBNull.Value;

                // 7. Optional Date Field
                sqlCmd.Parameters.Add("DateOfJoining", SqlDbType.Date).Value = (object?)objAPIRequest.DateOfJoining ?? DBNull.Value;

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
        public object DeleteEmployeeById(DeleteRequest objAPIRequest)
        {

            using (SqlCommand sqlCmd = new())
            {
                sqlCmd.Connection = _connectionString;
                sqlCmd.CommandText = "usp_DeleteEmployee";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandTimeout = _commandTimeOut;

                // Add parameters with proper SqlDbType
                sqlCmd.Parameters.Add("DeletedBy", SqlDbType.NVarChar, 128).Value = objAPIRequest.DeletedBy;
                sqlCmd.Parameters.Add("Id", SqlDbType.NVarChar, 64).Value = objAPIRequest.Id;
                sqlCmd.Parameters.Add("IPAddress", SqlDbType.NVarChar, 64).Value = (object?)objAPIRequest.IPAddress ?? DBNull.Value;

                _connectionString.Open();
                object i = (int)sqlCmd.ExecuteNonQuery();
                _connectionString.Close();
                return i;
            }
        }
        #endregion
    }
}
