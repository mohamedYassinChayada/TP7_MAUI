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
        _database.Execute(@"create table if not exists Note (Id TEXT PRIMARY KEY NOT NULL,Title TEXT NOT NULL,Content TEXT NOT NULL,CreatedAt TEXT NOT NULL)");
        _database.Execute(@"create table if not exists User (Id INTEGER PRIMARY KEY AUTOINCREMENT,FirstName TEXT NOT NULL,LastName TEXT NOT NULL,MemberSince TEXT NOT NULL,NotificationsEnabled INTEGER NOT NULL DEFAULT 0,DarkModeEnabled INTEGER NOT NULL DEFAULT 1,ProfileImagePath TEXT)");
    }

    public void CloseConnection(){
        _database?.Close();
    }
}

