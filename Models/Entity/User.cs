using SQLite;

namespace TP6.Models.Entity;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime MemberSince { get; set; } = DateTime.Now;
    public bool NotificationsEnabled { get; set; } = false;
    public bool DarkModeEnabled { get; set; } = true;
    public string ProfileImagePath { get; set; } = string.Empty;

    [Ignore]
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    [Ignore]
    public int DaysSinceMember => (DateTime.Now - MemberSince).Days;
}