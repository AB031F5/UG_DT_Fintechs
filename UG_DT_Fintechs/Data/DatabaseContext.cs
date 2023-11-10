using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Collections;
using System.Data;
using UG_DT_Fintechs.Logic;
using UG_DT_InternalAmolCall.Logic;
using UG_DT_InternalAmolCall.Models;

namespace UG_DT_Fintechs.Data
{
    public class DataBaseContext
    {
        static MySqlConnection databaseConnection = null;
        private MySqlCommand command = null;
        public DataBaseContext()
        {
            var init_dbConnection = Database.Connection;
            databaseConnection = new MySqlConnection(init_dbConnection);
        }
        public DataTable ExecuteQuery(string StoredProcedure, Hashtable Variables)
        {
            try
            {
                using (command = new MySqlCommand(StoredProcedure, databaseConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (DictionaryEntry variable in Variables)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(variable.Value.ToString()))
                            {
                                command.Parameters.AddWithValue(variable.Key.ToString(), variable.Value.ToString());
                            }
                            else
                            {
                                command.Parameters.AddWithValue(variable.Key.ToString(), "");
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    try
                    {
                        command.CommandTimeout = 200;
                        DataTable dt = new DataTable();
                        if (databaseConnection.State == ConnectionState.Open)
                        {
                            databaseConnection.Close();
                        }
                        databaseConnection.Open();
                        dt.Load(command.ExecuteReader());
                        databaseConnection.Close();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        ArrayList errors = new ArrayList();
                        Helpers helpers = new Helpers();
                        errors.Add($"Error: {ex.Message}");
                        helpers.writeToFile(errors);
                        throw;
                    }

                }
            }
            catch (Exception ex)
            {
                ArrayList errors = new ArrayList();
                Helpers helpers = new Helpers();
                errors.Add($"Error: {ex.Message}");
                helpers.writeToFile(errors);
                throw;
            }
            finally { databaseConnection.Close(); }
        }

        public bool ExecuteNonQuery(string StoredProcedure, Hashtable Variables)
        {
            bool isSuccessfull = false;
            try
            {
                using (command = new MySqlCommand(StoredProcedure, databaseConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (DictionaryEntry variable in Variables)
                    {
                        if (variable.Value != null)
                        {
                            command.Parameters.AddWithValue(variable.Key.ToString(), variable.Value.ToString());
                        }
                        else
                        {
                            command.Parameters.AddWithValue(variable.Key.ToString(), "");
                        }
                    }
                    command.CommandTimeout = 200;
                    if (databaseConnection.State == ConnectionState.Open)
                    {
                        databaseConnection.Close();
                    }
                    databaseConnection.Open();
                    int isExecuted = command.ExecuteNonQuery();
                    databaseConnection.Close();

                    if (isExecuted > 0)
                    {
                        isSuccessfull = true;
                    }

                }
            }
            catch (Exception ex)
            {
                ArrayList errors = new ArrayList();
                Helpers helpers = new Helpers();
                errors.Add($"Error: {ex.Message}");
                helpers.writeToFile(errors);
            }
            finally { databaseConnection.Close(); }

            return isSuccessfull;
        }

        public bool IsTxnDuplicate(string reference)
        {
            bool isDuplicate = false;
            try
            {
                databaseConnection.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(StoredProcedure.CHECK_DUPLICATE, databaseConnection);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("P_txn_reference", reference);
                DataTable dt = new DataTable();
                da.Fill(dt);
                databaseConnection.Close();
                if (dt.Rows.Count > 0)
                {
                    isDuplicate = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                databaseConnection.Close();
            }
            return isDuplicate;
        }
    }
}
