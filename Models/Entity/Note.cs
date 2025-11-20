using SQLite;

namespace TP6.Models.Entity;

public class Note
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsSynced { get; set; } = false;
    public bool PendingSync { get; set; } = false;
}