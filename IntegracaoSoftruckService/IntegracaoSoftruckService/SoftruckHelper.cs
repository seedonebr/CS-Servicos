using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoSoftruckService
{
    static class SoftruckHelper
    {
        static string baseUrl = $"https://localhost:7113";

        public async static Task<JObject> CreateServiceOrder(JObject serviceOrder)
        {
            try
            {
                var terms = CarsystemDbHelper.GetOSTerms();

                string termosRep = terms.Rows[0]["ds_termos"].ToString();

                termosRep.Replace("|NOME_CLIENTE|", "");
                termosRep.Replace("|NOME_OS|", "");
                termosRep.Replace("|ENDERECO|", "");
                termosRep.Replace("|DATA|", "");
                termosRep.Replace("|PERIODO|", "");
                termosRep.Replace("|MODELO|", "");
                termosRep.Replace("|PLACA|", "");

                //string termos = $"\"{{\\\"ops\\\":[{{\\\"insert\\\":\\\"Exemplo de termo com variáveis:\\\\n\\\\nNome: |{serviceOrder["nome_cliente"]}|\\\\nModelo do veículo: |{serviceOrder["modelo_veiculo"]}|\\\\n\\\"}}]}}\",";
                string termos = $"\"{{\\\"ops\\\":[{{\\\"insert\\\":\\\"Eu, \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\"{serviceOrder["nome_cliente"]}\\\"}},{{\\\"insert\\\":\\\", declaro que estou ciente e solicitei o atendimento técnico da \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\"|Nome da O.S| \\\"}},{{\\\"insert\\\":\\\"que será realizado em \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\"|Data da Ordem de Serviço|, \\\"}},{{\\\"insert\\\":\\\"no endereço \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\"|Endereço da O.S|, \\\"}},{{\\\"insert\\\":\\\"no período de \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\"|Manhã, Tarde ou Comercial|, \\\"}},{{\\\"insert\\\":\\\"para o veículo\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\" |MODELO|, \\\"}},{{\\\"insert\\\":\\\"da Placa\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"red\\\"}},\\\"insert\\\":\\\" |PLACA|.\\\"}},{{\\\"insert\\\":\\\"\\\\n\\\\nCompreendo que, para o sucesso do atendimento técnico, é necessário garantir as seguintes condições:\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\"·      \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\",\\\"bold\\\":true}},\\\"insert\\\":\\\"Presença de um responsável:\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" Um representante maior de idade, que possa assinar e na ausência do titular, responder pelas condições necessárias ao contrato.\\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\"·      \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\",\\\"bold\\\":true}},\\\"insert\\\":\\\"Documentos necessários:\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" Estará disponível em mão o documento do veículo, ou nota fiscal (em caso de veículo zero km). \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\"·      \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\",\\\"bold\\\":true}},\\\"insert\\\":\\\"Local adequado:\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" O local onde o serviço será realizado deve estar coberto (em caso de chuva), com iluminação adequada e acessível ao técnico.\\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\",\\\"bold\\\":true}},\\\"insert\\\":\\\" \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\"·      \\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\",\\\"bold\\\":true}},\\\"insert\\\":\\\"Condições do veículo:\\\"}},{{\\\"attributes\\\":{{\\\"color\\\":\\\"#002060\\\"}},\\\"insert\\\":\\\" Se tratando veículo, este deve estar em perfeitas condições de funcionamento, devidamente montado, com bateria funcionando, presente no local e com as chaves disponíveis.\\\"}},{{\\\"insert\\\":\\\"\\\\n\\\\nEstou ciente de que, caso o técnico não consiga realizar o serviço por qualquer um dos motivos mencionados acima, será cobrada uma taxa de visita improdutiva no valor de \\\"}},{{\\\"attributes\\\":{{\\\"bold\\\":true}},\\\"insert\\\":\\\"R$ 40,00 (quarenta reais).\\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"bold\\\":true}},\\\"insert\\\":\\\" \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\"}},{{\\\"attributes\\\":{{\\\"bold\\\":true}},\\\"insert\\\":\\\"Também declaro ciência que, caso sejam encontradas avarias que depreciem, o valor do bem em mercado, em caso aplicável de pacto adjeto de compra de documentos. O bem será depreciado, conforme tabela vigente na contratada. \\\"}},{{\\\"insert\\\":\\\"\\\\n\\\\nAssumo total responsabilidade por garantir que todas as condições acima sejam atendidas para o bom andamento do atendimento técnico.\\\\n\\\"}}]}}\"";

                // Criação do json para envio ao endpoint
                var json = new JObject()
                {
                    ["data"] = new JObject()
                    {
                        ["attributes"] = new JObject()
                        {
                            ["name"] = serviceOrder["id_os"].ToString(),
                            ["notes"] = serviceOrder["notas"],
                            ["priority"] = serviceOrder["prioridade"],
                            ["description"] = termos,
                            ["dueDate"] = serviceOrder["data"],
                            ["config"] = new JObject()
                            {
                                ["lockOnComplete"] = true,
                                //["setFillDate"] = "before",
                                ["deviceInteraction"] = true
                            }
                        },
                        ["relationships"] = new JObject()
                        {
                            ["enterprise"] = new JObject()
                            {
                                ["type"] = "enterprise",
                                ["id"] = "2gKGpVxgZvL6Bz9",
                            },
                            ["type"] = new JObject()
                            {
                                ["type"] = "type",
                                ["id"] = serviceOrder["type_id"],
                            },
                            ["asset"] = new JObject()
                            {
                                ["type"] = "asset",
                                ["id"] = serviceOrder["asset_id"],
                            },
                            ["assignee"] = new JObject()
                            {
                                ["type"] = "user",
                                ["id"] = serviceOrder["user_id"],
                            },
                            ["section"] = new JObject()
                            {
                                ["type"] = "section",
                                ["id"] = serviceOrder["section_id"],
                            }
                            ,
                            ["addresses"] = new JArray()
                            {
                                new JObject()
                                {
                                    ["type"] = "address",
                                    ["attributes"] = new JObject()
                                    {
                                        ["description"] = "Carsystem",
                                        ["order"] = "1",
                                        ["formattedAddress"] = serviceOrder["endereco"],
                                        ["coords"] = new JArray()
                                        {
                                            0, 0
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                // Enviar os dados da ordem de serviço para o endpoint
                JObject osResponse;
                string url = $"{baseUrl}/api/ordemservico";
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    var ordemServicoResponse = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("\n\n" + ordemServicoResponse);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    osResponse = JObject.Parse(ordemServicoResponse);
                    return osResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async static Task<JObject> UpdateServiceOrder(JObject serviceOrder, string id)
        {
            try
            {
                // Criação do json para envio ao endpoint
                var json = new JObject()
                {
                    ["data"] = new JObject()
                    {
                        ["attributes"] = new JObject()
                        {
                            ["name"] = serviceOrder["id_os"].ToString(),
                            ["notes"] = serviceOrder["notas"],
                            ["priority"] = serviceOrder["prioridade"],
                            ["dueDate"] = serviceOrder["data"],
                        },
                        ["relationships"] = new JObject()
                        {
                            ["section"] = new JObject()
                            {
                                ["type"] = "section",
                                ["id"] = serviceOrder["section_id"],
                            }
                        }
                    }
                };

                // Enviar os dados da ordem de serviço para o endpoint
                JObject osResponse;
                string url = $"{baseUrl}/api/ordemservico/{id}";
                using (var client = new HttpClient())
                {
                    //var request = new HttpRequestMessage
                    //{
                    //    Method = new HttpMethod("PATCH"),
                    //    RequestUri = new Uri(url),
                    //    Content = new StringContent(JsonConvert.SerializeObject(json.ToString()), Encoding.UTF8, "application/json")
                    //};
                    //var response = await client.SendAsync(request);

                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(url, content);

                    var ordemServicoResponse = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("\n\n" + ordemServicoResponse);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    osResponse = JObject.Parse(ordemServicoResponse);
                    return osResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<JObject> CreateCustomFields(JObject osResponse, string tipo_veiculo)
        {
            try
            {
                // Criar template/custom-fields
                JObject vehicle = new JObject()
                {
                    ["tipo_veiculo"] = tipo_veiculo
                };

                // Enviar os dados da ordem de serviço para o endpoint
                JObject templateResponse;
                string urlTemplate = $"{baseUrl}/api/ordemservico/customfields/{osResponse["data"]["id"]}";
                using (var client = new HttpClient())
                {
                    var content = new StringContent(vehicle.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(urlTemplate, content);
                    var templResponse = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("\n\n" + templResponse);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    templateResponse = JObject.Parse(templResponse);
                    return templateResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


    }
}
