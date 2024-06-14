using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(policy: permission.ToString())
        {

        }
    }
}
