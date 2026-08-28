using Microsoft.AspNetCore.Identity;

namespace GenbaseGoldReports.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string Description { get; set; } = string.Empty;

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            Description = description;
        }
    }
}
