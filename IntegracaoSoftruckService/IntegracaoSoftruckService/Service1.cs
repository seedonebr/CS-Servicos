using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntegracaoSoftruckService
{
    public partial class Service1 : ServiceBase
    {
        // Tempo de espera para cada execução (ms)
        static int delayMs = 4000;

        // Limite de erros permitidos
        static int errorLimit = 99999999;

        public Service1()
        {
            InitializeComponent();
        }

        protected async override void OnStart(string[] args)
        {
            ErrorHandler.ErrorLimit = errorLimit;

            while (true)
            {
                DateTime startTime = DateTime.Now;

                try
                {
                    // Obtém a lista das ordens de serviço que precisam ser alteradas ou criadas
                    DataTable serviceOrderResults = CarsystemDbHelper.GetServiceOrderList();
                    if (serviceOrderResults == null)
                    {
                        // Adiciona na contagem de erros e string de dump
                        ErrorHandler.ErrorCount++;
                        string error = $"Null exception ao obter dados da tbl_OS_integracao\n\n";
                        ErrorHandler.ErrorDump += error;
                        Console.WriteLine(error);

                        await Wait(startTime);
                        continue;
                    }

                    // Cria um array com os resultados para tratar e criar as ordens de serviço separadamente
                    var jArray = JArray.FromObject(serviceOrderResults, JsonSerializer.CreateDefault(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    // Faz um loop para tratar todas as ordens de serviço
                    foreach (JObject row in jArray)
                    {
                        // Verifica se a ordem de serviço já está criada
                        DataTable existsResults = CarsystemDbHelper.GetServiceOrderCreated(row);

                        // Se ordem de serviço já foi criada na Softruck, atualiza os dados
                        if (existsResults != null && existsResults.Rows.Count > 0)
                        {
                            JObject osResponse = await SoftruckHelper.UpdateServiceOrder(row, existsResults.Rows[0]["external_id"].ToString());
                            if (osResponse == null)
                            {
                                // Adiciona na contagem de erros e string de dump
                                ErrorHandler.ErrorCount++;
                                string error = $"Erro ao atualizar a ordem de serviço na Softruck\n\n";
                                ErrorHandler.ErrorDump += error;
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, error);
                                Console.WriteLine(error);

                                await Wait(startTime);
                                continue;
                            }

                            // Atualiza a flag de controle, para indicar que a ordem de serviço já foi criada
                            bool isUpdated = CarsystemDbHelper.UpdateServiceOrderFlag(row);
                            if (!isUpdated)
                            {
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, "Erro ao atualizar a flag de controle");
                            }

                            bool isUpdated2 = CarsystemDbHelper.UpdateServiceOrderIntegration(row);
                            if (!isUpdated2)
                            {
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, "Erro ao inserir a OS na tbl_ordem_external_vinculo");
                            }
                        }
                        // Se não existir ainda, cria uma nova ordem de serviço na Softruck
                        else
                        {
                            JObject osResponse = await SoftruckHelper.CreateServiceOrder(row);
                            if (osResponse == null)
                            {
                                // Adiciona na contagem de erros e string de dump
                                ErrorHandler.ErrorCount++;
                                string error = $"Erro ao criar a ordem de serviço na Softruck\n\n";
                                ErrorHandler.ErrorDump += error;
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, error);
                                Console.WriteLine(error);

                                await Wait(startTime);
                                continue;
                            }

                            JObject templateResponse = await SoftruckHelper.CreateCustomFields(osResponse, row["tipo_veiculo"].ToString());
                            if (templateResponse == null)
                            {
                                // Adiciona na contagem de erros e string de dump
                                ErrorHandler.ErrorCount++;
                                string error = $"Erro criar o template/check-list na Softruck\n\n";
                                ErrorHandler.ErrorDump += error;
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, error);
                                Console.WriteLine(error);
                            }

                            // Atualiza a flag de controle, para indicar que a ordem de serviço já foi criada
                            bool isUpdated = CarsystemDbHelper.UpdateServiceOrderFlag(row);
                            if (!isUpdated)
                            {
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, "Erro ao atualizar a flag de controle");
                            }

                            bool isInserted = CarsystemDbHelper.InsertServiceOrderIntegration(row, osResponse);
                            if (!isInserted)
                            {
                                CarsystemDbHelper.UpdateServiceOrderFlag(row, false, "Erro ao inserir a OS na tbl_ordem_external_vinculo");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.ErrorCount++;
                    string error = $"\n\nErro no loop principal:\n\n {ex.Message}";
                    ErrorHandler.ErrorDump += error;
                    Console.WriteLine(error);
                }
                finally { }

                await Wait(startTime);
            }
        }

        static async Task Wait(DateTime startTime)
        {
            if (ErrorHandler.ErrorCount >= errorLimit)
            {
                // Envia e-mail com detalhes do erro
                EmailHelper.SendEmail(EmailType.ErrorDump, ErrorHandler.ErrorDump);

                // Zera o contador de erros e a variável de dump
                ErrorHandler.ErrorCount = 0;
                ErrorHandler.ErrorDump = "";
            }

            // Calcula o tempo decorrido desde o início do processo
            TimeSpan elapsedTime = DateTime.Now - startTime;

            // Ao final da iteração, exibe o tempo decorrido
            Console.WriteLine($"\n\nTempo decorrido: {elapsedTime}\n\nAguardando início de nova execução\n\n");

            // Calcula o tempo restante para completar o tempo de delay
            int remainingTime = Math.Max(0, delayMs - (int)elapsedTime.TotalMilliseconds);

            // Verifica se a requisição foi concluída antes do tempo de delay
            if (remainingTime > 0)
            {
                // Aguarda o tempo restante apenas se não tiver sido excedido
                await Task.Delay(remainingTime);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
