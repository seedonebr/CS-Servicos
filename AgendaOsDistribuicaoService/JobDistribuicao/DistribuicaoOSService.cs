using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;

internal class DistribuicaoOSService
{
    private readonly string _connectionString;

    public DistribuicaoOSService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Remove o static para que o método seja de instância
    public async Task<DataTable> GetDistribuicaoOSAsync()
    {
        DataTable dataTable = new DataTable();
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[SBCAgenda].[pro_setDistribuicaoOS]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@filtroData", SqlDbType.NVarChar, 10) { Value = "TODOS" });
                    cmd.Parameters.Add(new SqlParameter("@regiaoCep", SqlDbType.VarChar, 9) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@dataAgendamento", SqlDbType.Date) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@isRedistribuicao", SqlDbType.Bit) { Value = 0 });

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Erro ao executar a distribuição de OS: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }

        return dataTable;
    }

    public async Task<DataTable> GetDistribuicaoOSDiaAtualAsync()
    {
        DataTable dataTable = new DataTable();
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[SBCAgenda].[pro_setDistribuicaoOS]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@filtroData", SqlDbType.NVarChar, 10) { Value = DateTime.Now.ToString("dd-MM-yyyy") });
                    cmd.Parameters.Add(new SqlParameter("@regiaoCep", SqlDbType.VarChar, 9) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@dataAgendamento", SqlDbType.Date) { Value = DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@isRedistribuicao", SqlDbType.Bit) { Value = 0 });

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Erro ao executar a distribuição de OS: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }

        return dataTable;
    }
}
