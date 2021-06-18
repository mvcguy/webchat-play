
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebChatPlay.Filters.Mfa
{
    public class RequireMfa : IAuthorizationRequirement { }

    public class RequireMfaHandler : AuthorizationHandler<RequireMfa>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequireMfa requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var amrClaim =
            context.User.Claims.FirstOrDefault(t => t.Type == "amr");

        if (amrClaim != null && true /*amrClaim.Value == Amr.Mfa*/)// TODO: Investigate further
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
}

