using SQLite;
using TP6.Models.Entity;
using TP6.Services.Persistence;

namespace TP6.Infrastructure.Persistance;

public class NoteDAO : IDaoNote{
    private readonly SQLiteConnection _database;

    public NoteDAO(){
        _database = DaoContext.Instance.Database;
    }

    public List<Note> GetAll(){
        var query = "select * from Note order by CreatedAt desc";
        return _database.Query<Note>(query);
    }

    public Note? GetById(object id){
        var query = "select * from Note where Id = ?";
        return _database.Query<Note>(query, id).FirstOrDefault();
    }

    public int Insert(Note note){
        var query = @"insert into Note (Id, Title, Content, CreatedAt, IsSynced, PendingSync) values (?, ?, ?, ?, ?, ?)";
        return _database.Execute(query, note.Id, note.Title, note.Content, note.CreatedAt.ToString("o"), note.IsSynced ? 1 : 0, note.PendingSync ? 1 : 0);
    }

    public int Update(Note note){
        var query = @"update Note set Title = ?, Content = ?, CreatedAt = ?, IsSynced = ?, PendingSync = ? where Id = ?";
        return _database.Execute(query, note.Title, note.Content, note.CreatedAt.ToString("o"), note.IsSynced ? 1 : 0, note.PendingSync ? 1 : 0, note.Id);
    }

    public int Delete(object id){
        var query = "delete from Note where Id = ?";
        return _database.Execute(query, id);
    }

    public int DeleteAll(){
        var query = "delete from Note";
        return _database.Execute(query);
    }
}

