namespace PadillaVittini.Api.Options;

public sealed class SmtpOptions
{
  public const string SectionName = "Email";

  public string Host { get; init; } = string.Empty;
  public int Port { get; init; } = 587;
  public string Security { get; init; } = "StartTls";
  public string Username { get; init; } = string.Empty;
  public string Password { get; init; } = string.Empty;
  public string FromAddress { get; init; } = string.Empty;
  public string FromName { get; init; } = string.Empty;
  public string ToAddress { get; init; } = string.Empty;
}
