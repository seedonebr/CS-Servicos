using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace IntegracaoSoftruckService
{
    static class CarsystemDbHelper
    {
        static readonly DatabaseHelper dbHelper = new DatabaseHelper("10.0.0.186", "Principal", "user_push", "!@#$Push");

        public static DataTable GetServiceOrderList()
        {
            try
            {
                // Leitura dos dados da tabela de ordens de serviço (tbl_OS_integracao)
                string selectQuery = "SELECT TOP (15) * FROM [truck].[tbl_OS_integracao] WHERE [fl_external_upd] = @flag ORDER BY id ASC";
                SqlParameter[] selectParameters = new SqlParameter[]
                {
                    new SqlParameter("@flag", SqlDbType.Int) { Value = 0 }
                };
                DataTable results = dbHelper.ExecuteSelectSql(selectQuery, selectParameters);

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static DataTable GetServiceOrderCreated(JObject serviceOrder)
        {
            try
            {
                // Verificar se existe a OS criada na tabela tbl_ordem_external_vinculo
                string selectExistsQuery = "SELECT * FROM [truck].[tbl_ordem_external_vinculo] WHERE [id_os] = @id_os";
                SqlParameter[] selectExistsParameters = new SqlParameter[]
                {
                    new SqlParameter("@id_os", SqlDbType.Int) { Value = serviceOrder["id_os"] }
                };
                DataTable existsResults = dbHelper.ExecuteSelectSql(selectExistsQuery, selectExistsParameters);
                return existsResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static DataTable GetOSTerms()
        {
            try
            {
                // Obtém os termos no banco de dados da Carsystem
                string selectQuery = "SELECT * FROM [truck].[tbl_OS_termos]";
                DataTable results = dbHelper.ExecuteSelectSql(selectQuery);

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static bool UpdateServiceOrderFlag(JObject serviceOrder, bool success = true, string error = "")
        {
            try
            {
                int flag = success ? 1 : 2;

                string dsRetorno = success ? null : error;
                object castDsRetorno = (object)dsRetorno ?? DBNull.Value;

                //string dsRetorno = success ? null : error;
                //dsRetorno = dsRetorno ?? DBNull.Value;

                // Atualiza a flag de controle, para indicar que a ordem de serviço já foi criada
                string updateQuery = "UPDATE [truck].[tbl_OS_integracao] " +
                                     "SET fl_external_upd = @flag, ds_retorno = @dsRetorno " +
                                     "WHERE id_os = @id";
                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@flag", SqlDbType.Int) { Value = flag },
                    new SqlParameter("@dsRetorno", SqlDbType.VarChar) { Value = castDsRetorno },
                    new SqlParameter("@id", SqlDbType.Int) { Value = serviceOrder["id_os"] },
                };
                var affectedRowsUpdate = dbHelper.ExecuteUpdateSql(updateQuery, updateParameters);
                Console.WriteLine("\n\nLinhas alteradas no update: " + affectedRowsUpdate);

                return affectedRowsUpdate > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool InsertServiceOrderIntegration(JObject serviceOrder, JObject osResponse)
        {
            try
            {
                // Insere os dados da ordem de serviço na tabela de integração (tbl_ordem_external_vinculo)
                // SET IDENTITY_INSERT [truck].[tbl_ordem_external_vinculo] ON; SET IDENTITY_INSERT [truck].[tbl_ordem_external_vinculo] OFF;
                string insertQuery = "INSERT INTO [truck].[tbl_ordem_external_vinculo] (id_os, dt_insert, external_id, uuid_veiculo, placa_veiculo, nr_contrato, user_id, acknowledgment_id, token, section_id, fl_external_update) VALUES (@id_os, @dt_insert, @external_id, @uuid_veiculo, @placa_veiculo, @nr_contrato, @user_id, @acknowledgment_id, @token, @section_id, @fl_external_update);";
                SqlParameter[] insertParameters = new SqlParameter[]
                {
                    new SqlParameter("@id_os", SqlDbType.Int) { Value = serviceOrder["id_os"] },
                    new SqlParameter("@dt_insert", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@external_id", SqlDbType.VarChar) { Value = osResponse["data"]["id"] },
                    new SqlParameter("@uuid_veiculo", SqlDbType.VarChar) { Value = osResponse["data"]["relationships"]["asset"]["id"] },
                    new SqlParameter("@placa_veiculo", SqlDbType.VarChar) { Value = serviceOrder["placa_veiculo"] },
                    new SqlParameter("@nr_contrato", SqlDbType.VarChar) { Value = serviceOrder["asset_id"] },
                    new SqlParameter("@user_id", SqlDbType.VarChar) { Value = osResponse["data"]["relationships"]["assignee"]["id"] },
                    new SqlParameter("@acknowledgment_id", SqlDbType.VarChar) {Value = osResponse["data"]["relationships"]["acknowledgment_Id"] },
                    new SqlParameter("@token", SqlDbType.VarChar) {Value = osResponse["data"]["attributes"]["token"] },
                    new SqlParameter("@section_id", SqlDbType.VarChar) { Value = osResponse["data"]["relationships"]["section"]["id"] },
                    new SqlParameter("@fl_external_update", SqlDbType.Int) { Value = 1 }
                };
                var affectedRowsInsert = dbHelper.ExecuteInsertSql(insertQuery, insertParameters);
                Console.WriteLine("\n\nLinhas inseridas: " + affectedRowsInsert);

                return affectedRowsInsert > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool UpdateServiceOrderIntegration(JObject serviceOrder)
        {
            try
            {
                // Atualiza a data de atualização e flag de controle, para indicar que a ordem de serviço foi atualizada
                string updateQuery = "UPDATE [truck].[tbl_ordem_external_vinculo] SET dt_upd_external = @date WHERE id_os = @id";
                SqlParameter[] updateParameters = new SqlParameter[]
                {
                    new SqlParameter("@date", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@id", SqlDbType.Int) { Value = serviceOrder["id_os"] }
                };
                var affectedRowsUpdate = dbHelper.ExecuteUpdateSql(updateQuery, updateParameters);
                Console.WriteLine("\n\nLinhas alteradas no update: " + affectedRowsUpdate);

                return affectedRowsUpdate > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
