using System.ComponentModel.DataAnnotations;

namespace PadillaVittini.Api.Models;

public sealed record ContactRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string Name,
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: StringLength(30)] string? Phone,
    [property: Required, StringLength(100)] string Service,
    [property: Required, StringLength(4000, MinimumLength = 10)] string Message,
    [property: StringLength(0)] string? Website);
