namespace GenbaseGoldReports.Application.Features.Navigation.Dtos
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public List<MenuItemDto> Items { get; set; } = new();
    }
}
