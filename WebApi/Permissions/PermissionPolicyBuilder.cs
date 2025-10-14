using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace WebApi.Permissions;

public class PermissionPolicyBuilder:IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _def;

    public PermissionPolicyBuilder(IOptions<AuthorizationOptions> opt)
    {
        _def = new DefaultAuthorizationPolicyProvider(opt);
    }
    
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(policyName));
        return policy.Build();
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return await _def.GetDefaultPolicyAsync();
    }

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return await _def.GetFallbackPolicyAsync();
    }
}