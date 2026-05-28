using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace maxi_movie_mvc.Service
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody, string? textBody = null);// para enviar un correo electrónico de forma asíncrona. El método toma los siguientes parámetros:
    }

    public sealed class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _cfg;
        public SmtpEmailService(IOptions<SmtpSettings> cfg) => _cfg = cfg.Value;

        public async Task SendAsync(string to, string subject, string htmlBody, string? textBody = null)// Implementación del método SendAsync para enviar un correo electrónico utilizando SMTP. El método realiza las siguientes acciones:
        {
            var message = new MimeMessage();// Crea una nueva instancia de MimeMessage, que representa el correo electrónico a enviar.
            message.From.Add(new MailboxAddress(_cfg.FromName, _cfg.User));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody, TextBody = textBody ?? string.Empty };// Crea un BodyBuilder para construir el cuerpo del correo electrónico. Se asigna el contenido HTML al HtmlBody y, si se proporciona, el contenido de texto al TextBody. Si no se proporciona un cuerpo de texto, se asigna una cadena vacía.
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();// Crea una nueva instancia de SmtpClient para enviar el correo electrónico.
            await client.ConnectAsync(_cfg.Host, _cfg.Port, _cfg.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);// Conecta al servidor SMTP utilizando la configuración proporcionada en _cfg. Se especifica el host, el puerto y si se debe usar StartTLS para asegurar la conexión.
            await client.AuthenticateAsync(_cfg.User, _cfg.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public sealed class SmtpSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromName { get; set; } = "Sistema";
        public bool UseStartTls { get; set; } = true;
    }
}