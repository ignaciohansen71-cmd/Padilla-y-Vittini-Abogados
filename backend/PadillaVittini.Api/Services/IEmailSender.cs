using PadillaVittini.Api.Models;

namespace PadillaVittini.Api.Services;

public interface IEmailSender
{
  bool IsConfigured { get; }
  Task SendContactAsync(ContactRequest request, CancellationToken cancellationToken);
}
