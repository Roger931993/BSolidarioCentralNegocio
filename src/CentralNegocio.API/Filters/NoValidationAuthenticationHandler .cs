namespace CentralNegocio.API.Filters
{
  using Microsoft.AspNetCore.Authentication;
  using Microsoft.Extensions.Options;
  using System.Security.Claims;
  using System.Text.Encodings.Web;
  public class NoValidationAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    public NoValidationAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var claims = new[] { new Claim(ClaimTypes.Name, "Anonymous") };
      var identity = new ClaimsIdentity(claims, "NoValidation");
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, "NoValidation");

      return Task.FromResult(AuthenticateResult.Success(ticket));
    }
  }
}
