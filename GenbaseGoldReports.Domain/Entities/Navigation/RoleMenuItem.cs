namespace GenbaseGoldReports.Domain.Entities.Navigation
{
    public class RoleMenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RoleName { get; set; } = string.Empty;
        public Guid MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
