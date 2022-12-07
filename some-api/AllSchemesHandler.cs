using Microsoft.AspNetCore.Authorization;

namespace some_api
{
    public class AllSchemesHandler : AuthorizationHandler<AllSchemesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllSchemesRequirement requirement)
        {
            var issuer = string.Empty;

            var issClaim = context.User.Claims.FirstOrDefault(c => c.Type == "iss");
            if (issClaim != null)
                issuer = issClaim.Value;

            if (issuer == Consts.MY_AAD_ISS) // AAD
            {
                // "azp": "--your-azp-claim-value--",
                var azpClaim = context.User.Claims.FirstOrDefault(c => c.Type == "azp"
                    && c.Value == "9ffabb37-ed93-464c-b649-d5cd8297ca55");
                if (azpClaim != null)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
