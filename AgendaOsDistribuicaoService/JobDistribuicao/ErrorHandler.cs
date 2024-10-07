using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JobDistribuicao
{
    static class ErrorHandler
    {
        public static int ErrorCount = 0;
        public static int ErrorLimit = 20;
        public static string ErrorDump = "";

        // Método para tentar converter texto para objeto. Retorna false caso não consiga
        public static bool TryParseJson<T>(string text, out T result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(text, settings);
            return success;
        }

        // Método para verificar se o valor de um objeto de json é vazio ou nulo
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null) ||
                   (token.Type == JTokenType.Undefined);
        }
    }
}
