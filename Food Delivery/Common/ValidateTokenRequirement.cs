using Microsoft.AspNetCore.Authorization;

namespace Food_Delivery.Common;

public class ValidateTokenRequirement : IAuthorizationRequirement
{
    public ValidateTokenRequirement()
    {
    }
}