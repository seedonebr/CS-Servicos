using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace IntegracaoSoftruckService
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public DatabaseHelper(string dataSource, string database, string user, string password)
        {
            //string connectionString = "Server=10.0.0.186;Database=Principal;User Id=user_push;Password=!@#$Push;";
            SqlConnectionStringBuilder sConnB = new SqlConnectionStringBuilder()
            {
                DataSource = dataSource,
                InitialCatalog = database,
                UserID = user,
                Password = password
            };
            _connectionString = sConnB.ConnectionString;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private void OpenConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        private void CloseConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void AddParameters(SqlCommand command, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
        }

        public DataTable ExecuteSelect(string storedProcedure, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                AddParameters(command, parameters);

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                try
                {
                    OpenConnection(connection);
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    Console.Write($"Erro ao executar o SELECT: {ex}");
                    return null;
                }
                finally
                {
                    CloseConnection(connection);
                }

                return dataTable;
            }
        }

        public int ExecuteInsert(string storedProcedure, SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, parameters);
        }

        public int ExecuteUpdate(string storedProcedure, SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, parameters);
        }

        public int ExecuteDelete(string storedProcedure, SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, parameters);
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(storedProcedure, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                AddParameters(command, parameters);

                try
                {
                    OpenConnection(connection);
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao executar a operação no banco de dados: {ex}");
                    return -1;
                }
                finally
                {
                    CloseConnection(connection);
                }
            }
        }

        public DataTable ExecuteSelectSql(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                try
                {
                    OpenConnection(connection);
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao executar a consulta SELECT.", ex);
                }
                finally
                {
                    CloseConnection(connection);
                }

                return dataTable;
            }
        }

        public int ExecuteInsertSql(string query, SqlParameter[] parameters = null)
        {
            return ExecuteNonQuerySql(query, parameters);
        }

        public int ExecuteUpdateSql(string query, SqlParameter[] parameters = null)
        {
            return ExecuteNonQuerySql(query, parameters);
        }

        public int ExecuteDeleteSql(string query, SqlParameter[] parameters = null)
        {
            return ExecuteNonQuerySql(query, parameters);
        }

        public int ExecuteNonQuerySql(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    OpenConnection(connection);
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao executar a operação no banco de dados.", ex);
                }
                finally
                {
                    CloseConnection(connection);
                }
            }
        }

        public void ExecuteSql(DatabaseHelper dbHelper)
        {
            // Parâmetros para a Stored Procedure
            SqlParameter[] selectParameters = new SqlParameter[]
            {
                new SqlParameter("@ParameterName", SqlDbType.Int) { Value = 1 }
            };

            try
            {
                // Executar SELECT
                DataTable results = dbHelper.ExecuteSelect("YourSelectStoredProcedure", selectParameters);

                foreach (DataRow row in results.Rows)
                {
                    Console.WriteLine(row["YourColumnName"]);
                }

                // Executar INSERT
                SqlParameter[] insertParameters = new SqlParameter[]
                {
                new SqlParameter("@ParameterName", SqlDbType.VarChar) { Value = "Value" }
                };
                dbHelper.ExecuteInsert("YourInsertStoredProcedure", insertParameters);

                // Executar UPDATE
                SqlParameter[] updateParameters = new SqlParameter[]
                {
                new SqlParameter("@ParameterName", SqlDbType.VarChar) { Value = "NewValue" }
                };
                dbHelper.ExecuteUpdate("YourUpdateStoredProcedure", updateParameters);

                // Executar DELETE
                SqlParameter[] deleteParameters = new SqlParameter[]
                {
                new SqlParameter("@ParameterName", SqlDbType.Int) { Value = 1 }
                };
                dbHelper.ExecuteDelete("YourDeleteStoredProcedure", deleteParameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string DataTableToJsonObj(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}