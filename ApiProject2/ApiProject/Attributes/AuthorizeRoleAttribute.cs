using Microsoft.AspNetCore.Authorization;

namespace ApiProject.Attributes
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    public static class Roles
    {
        public const string Manager = "manager";
        public const string Customer = "customer";
    }
}
