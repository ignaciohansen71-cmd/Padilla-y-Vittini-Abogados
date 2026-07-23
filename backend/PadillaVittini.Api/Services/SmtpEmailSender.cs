using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PadillaVittini.Api.Models;
using PadillaVittini.Api.Options;

namespace PadillaVittini.Api.Services;

public sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
  private readonly SmtpOptions smtp = options.Value;

  public bool IsConfigured =>
      !string.IsNullOrWhiteSpace(smtp.Host)
      && smtp.Port > 0
      && !string.IsNullOrWhiteSpace(smtp.Username)
      && !string.IsNullOrWhiteSpace(smtp.Password)
      && !string.IsNullOrWhiteSpace(smtp.FromAddress)
      && !string.IsNullOrWhiteSpace(smtp.ToAddress);

  public async Task SendContactAsync(ContactRequest request, CancellationToken cancellationToken)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(smtp.FromName, smtp.FromAddress));
    message.To.Add(MailboxAddress.Parse(smtp.ToAddress));
    message.ReplyTo.Add(new MailboxAddress(request.Name, request.Email));
    message.Subject = $"Nueva consulta web: {SanitizeSubject(request.Service)}";
    message.Body = BuildBody(request);

    using var client = new SmtpClient();
    var security = Enum.TryParse<SecureSocketOptions>(smtp.Security, true, out var parsedSecurity)
        ? parsedSecurity
        : SecureSocketOptions.StartTls;

    await client.ConnectAsync(smtp.Host, smtp.Port, security, cancellationToken);
    await client.AuthenticateAsync(smtp.Username, smtp.Password, cancellationToken);
    await client.SendAsync(message, cancellationToken);
    await client.DisconnectAsync(true, cancellationToken);
  }

  private static MimeEntity BuildBody(ContactRequest request)
  {
    var name = WebUtility.HtmlEncode(request.Name);
    var email = WebUtility.HtmlEncode(request.Email);
    var phone = WebUtility.HtmlEncode(request.Phone ?? "No informado");
    var service = WebUtility.HtmlEncode(request.Service);
    var content = WebUtility.HtmlEncode(request.Message).Replace("\n", "<br>");

    return new BodyBuilder
    {
      TextBody = $"Nombre: {request.Name}\nCorreo: {request.Email}\nTelefono: {request.Phone ?? "No informado"}\nArea: {request.Service}\n\n{request.Message}",
      HtmlBody = $"""
                <h2>Nueva consulta desde el sitio web</h2>
                <p><strong>Nombre:</strong> {name}</p>
                <p><strong>Correo:</strong> {email}</p>
                <p><strong>Telefono:</strong> {phone}</p>
                <p><strong>Area:</strong> {service}</p>
                <hr>
                <p>{content}</p>
                """,
    }.ToMessageBody();
  }

  private static string SanitizeSubject(string value) =>
      value.Replace("\r", string.Empty).Replace("\n", string.Empty);
}
