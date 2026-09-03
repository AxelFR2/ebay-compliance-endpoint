using System.Security.Cryptography;
using System.Text;

namespace EbayComplianceEndpoint;

public interface IEbayDeletionSettings
{
    string? VerificationToken { get; }

    string? EndpointUrl { get; }
}

public sealed class EnvironmentEbayDeletionSettings : IEbayDeletionSettings
{
    public string? VerificationToken => Environment.GetEnvironmentVariable("EBAY_DELETION_VERIFICATION_TOKEN");

    public string? EndpointUrl => Environment.GetEnvironmentVariable("EBAY_DELETION_ENDPOINT_URL");
}

public static class EbayAccountDeletionEndpoints
{
    public static IEndpointRouteBuilder MapEbayAccountDeletionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ebay/account-deletion", GetChallenge);
        endpoints.MapPost("/ebay/account-deletion", ReceiveNotification);

        return endpoints;
    }

    public static IResult GetChallenge(HttpContext context, IEbayDeletionSettings settings)
    {
        var challengeCode = context.Request.Query["challenge_code"].ToString();
        if (string.IsNullOrWhiteSpace(challengeCode))
        {
            return Results.BadRequest();
        }

        if (string.IsNullOrEmpty(settings.VerificationToken) || string.IsNullOrEmpty(settings.EndpointUrl))
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.Json(new { challengeResponse = CreateChallengeResponse(challengeCode, settings.VerificationToken, settings.EndpointUrl) });
    }

    // The notification body is deliberately neither read nor logged at this stage.
    public static IResult ReceiveNotification() => Results.NoContent();

    public static string CreateChallengeResponse(string challengeCode, string verificationToken, string exactEndpointUrl)
    {
        var input = string.Concat(challengeCode, verificationToken, exactEndpointUrl);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
