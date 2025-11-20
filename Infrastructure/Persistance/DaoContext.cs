using SQLite;

namespace TP6.Infrastructure.Persistance;

public class DaoContext{
    private static DaoContext? _instance;
    public static DaoContext Instance => _instance ??= new DaoContext();

    private readonly SQLiteConnection _database;
    private readonly string _databasePath;

    private DaoContext(){
        _databasePath = Path.Combine(FileSystem.AppDataDirectory, "tp6.dbtp6.db");
        _database = new SQLiteConnection(_databasePath);
        InitializeDatabase();
    }

    public SQLiteConnection Database => _database;

    private void InitializeDatabase(){
        // Create tables if they don't exist
        _database.Execute(@"create table if not exists Note (Id TEXT PRIMARY KEY NOT NULL,Title TEXT NOT NULL,Content TEXT NOT NULL,CreatedAt TEXT NOT NULL,IsSynced INTEGER NOT NULL DEFAULT 0,PendingSync INTEGER NOT NULL DEFAULT 0)");
        _database.Execute(@"create table if not exists User (Id INTEGER PRIMARY KEY AUTOINCREMENT,FirstName TEXT NOT NULL,LastName TEXT NOT NULL,MemberSince TEXT NOT NULL,NotificationsEnabled INTEGER NOT NULL DEFAULT 0,DarkModeEnabled INTEGER NOT NULL DEFAULT 1,ProfileImagePath TEXT)");

        // Migrate existing Note table to add new columns if they don't exist
        MigrateNoteTable();
    }

    private void MigrateNoteTable()
    {
        try
        {
            // Check if IsSynced column exists by trying to query it
            var checkQuery = "SELECT IsSynced FROM Note LIMIT 1";
            _database.ExecuteScalar<int>(checkQuery);
            // If we get here, column exists, no migration needed
        }
        catch
        {
            // Column doesn't exist, add it
            try
            {
                _database.Execute("ALTER TABLE Note ADD COLUMN IsSynced INTEGER NOT NULL DEFAULT 0");
                _database.Execute("ALTER TABLE Note ADD COLUMN PendingSync INTEGER NOT NULL DEFAULT 0");
                System.Diagnostics.Debug.WriteLine("Successfully migrated Note table with sync columns");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error migrating Note table: {ex.Message}");
            }
        }
    }

    public void CloseConnection(){
        _database?.Close();
    }
}

