using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;
using PadillaVittini.Api.Models;
using PadillaVittini.Api.Options;
using PadillaVittini.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 32 * 1024);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", policy =>
  {
    policy.WithOrigins(allowedOrigins)
          .WithMethods(HttpMethods.Get, HttpMethods.Post)
          .WithHeaders("Content-Type");
  });
});

builder.Services.AddRateLimiter(options =>
{
  options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
  options.AddPolicy("ContactForm", context =>
      RateLimitPartition.GetFixedWindowLimiter(
          context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
          _ => new FixedWindowRateLimiterOptions
          {
            AutoReplenishment = true,
            PermitLimit = 5,
            QueueLimit = 0,
            Window = TimeSpan.FromMinutes(15),
          }));
});

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseRateLimiter();

app.MapGet("/api/health", () => TypedResults.Ok(new { status = "ok" }))
    .RequireCors("Frontend");

app.MapPost("/api/contact", SendContactMessage)
    .RequireCors("Frontend")
    .RequireRateLimiting("ContactForm");

app.Run();

static async Task<Results<Ok<ContactResponse>, ValidationProblem, ProblemHttpResult>> SendContactMessage(
    ContactRequest request,
    IEmailSender emailSender,
    ILogger<Program> logger,
    CancellationToken cancellationToken)
{
  if (!string.IsNullOrWhiteSpace(request.Website))
  {
    return TypedResults.Ok(new ContactResponse("Mensaje recibido."));
  }

  var validationResults = new List<ValidationResult>();
  var validationContext = new ValidationContext(request);

  if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
  {
    var errors = validationResults
        .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
            .Select(member => new { Member = member, Message = result.ErrorMessage ?? "Valor invalido." }))
        .GroupBy(item => item.Member)
        .ToDictionary(group => group.Key, group => group.Select(item => item.Message).ToArray());

    return TypedResults.ValidationProblem(errors);
  }

  if (!emailSender.IsConfigured)
  {
    logger.LogWarning("El formulario de contacto fue llamado sin configuracion SMTP completa.");
    return TypedResults.Problem(
        "El servicio de correo aun no esta configurado.",
        statusCode: StatusCodes.Status503ServiceUnavailable);
  }

  try
  {
    await emailSender.SendContactAsync(request, cancellationToken);
    return TypedResults.Ok(new ContactResponse("Tu consulta fue enviada correctamente."));
  }
  catch (Exception exception)
  {
    logger.LogError(exception, "No fue posible enviar una consulta por correo.");
    return TypedResults.Problem(
        "No fue posible enviar la consulta. Intenta nuevamente mas tarde.",
        statusCode: StatusCodes.Status502BadGateway);
  }
}
