namespace GenbaseGoldReports.Domain.Entities.Navigation
{
    public class MenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Guid? ParentId { get; set; }
        public virtual MenuItem? Parent { get; set; }
        public virtual ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

        public virtual ICollection<RoleMenuItem> RoleMenuItems { get; set; } = new List<RoleMenuItem>();
    }
}
