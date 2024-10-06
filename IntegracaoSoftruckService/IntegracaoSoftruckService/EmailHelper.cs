using System;
using System.Net.Mail;
using System.Net;

namespace IntegracaoSoftruckService
{
    static class EmailHelper
    {
        public static void SendEmail(EmailType emailType, string errorDump, int queueLimit = 10000)
        {
            try
            {
                string email = "suporteti@carsystem.com";
                string emailSender = "alertasistemas@carsystem.com";
                // Configurações do servidor SMTP da Microsoft
                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587, // Porta padrão para SMTP
                    Credentials = new NetworkCredential(emailSender, "Bag43609"), // Suas credenciais
                    EnableSsl = true // Use SSL/TLS
                };

                // Detalhes do e-mail
                MailMessage mailMessage;
                switch (emailType)
                {
                    // caso seja erros na execução do programa
                    case EmailType.ErrorDump:
                        mailMessage = new MailMessage
                        {
                            From = new MailAddress(emailSender),
                            Subject = "Erro no Serviço Integração Softruck - Execução incorreta",
                            Body = $"{errorDump}"
                        };
                        break;
                    // caso seja a fila que passou o limite
                    case EmailType.QueueLimit:
                        mailMessage = new MailMessage
                        {
                            From = new MailAddress(emailSender),
                            Subject = "Erro no Serviço Integração Softruck - Fila muito grande",
                            Body = $"A fila do Integração Softruck passou de {queueLimit}! Recomenda-se a verificação da execução do processo."
                        };
                        break;
                    default:
                        mailMessage = new MailMessage();
                        break;
                }

                mailMessage.To.Add(email); // Destinatário

                smtpClient.Send(mailMessage);
                Console.WriteLine("\n\nE-mail enviado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nErro ao enviar o e-mail: " + ex.Message);
            }
        }
    }

    enum EmailType
    {
        ErrorDump,
        QueueLimit
    }
}
