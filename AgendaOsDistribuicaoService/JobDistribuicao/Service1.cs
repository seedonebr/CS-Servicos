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

namespace JobDistribuicao
{
    public partial class Service1 : ServiceBase
    {
        // Tempo de espera para cada execução (ms)
        static int delayMs = 900000;

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
                    string connectionString = "sua_connection_string_aqui";
                    var distribuicaoService = new DistribuicaoOSService(connectionString);
                   
                    DataTable serviceOrderResults = await distribuicaoService.GetDistribuicaoOSAsync();
                    if (serviceOrderResults == null)
                    {
                        // Adiciona na contagem de erros e string de dump
                        ErrorHandler.ErrorCount++;
                        string error = $"Null exception ao obter dados da distribuição de OS\n\n";
                        ErrorHandler.ErrorDump += error;
                        Console.WriteLine(error);

                        await Wait(startTime);
                        continue;
                    }

                    // Cria um array com os resultados para tratar e criar as ordens de serviço separadamente
                    var jArray = JArray.FromObject(serviceOrderResults, JsonSerializer.CreateDefault(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
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
