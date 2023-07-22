namespace Todo_App.Domain.Entities;
public  class Tag : BaseAuditableEntity
{
    public int TodoItemId { get; set; }
    public string Name { get; set; } = null!;
    public TodoItem TodoItem { get; set; } = null!;
}
