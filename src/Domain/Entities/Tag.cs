namespace Todo_App.Domain.Entities;
public  class Tag : BaseAuditableEntity
{
    public int TodItemId { get; set; }
    public string Name { get; set; }
    public TodoItem TodoItem { get; set; }
}
