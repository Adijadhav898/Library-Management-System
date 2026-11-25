namespace LMS.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}