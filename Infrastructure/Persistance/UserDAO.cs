using SQLite;
using TP6.Models.Entity;
using TP6.Services.Persistence;

namespace TP6.Infrastructure.Persistance;

public class UserDAO : IDaoUser{
    private readonly SQLiteConnection _database;

    public UserDAO(){
        _database = DaoContext.Instance.Database;
    }

    public List<User> GetAll(){
        var query = "select * from User";
        return _database.Query<User>(query);
    }

    public User? GetById(object id){
        var query = "select * from User where Id = ?";
        return _database.Query<User>(query, id).FirstOrDefault();
    }

    public User? GetFirst(){
        var query = "select * from User limit 1";
        return _database.Query<User>(query).FirstOrDefault();
    }

    public int Insert(User user){
        var query = @"insert into User (FirstName, LastName, MemberSince, NotificationsEnabled, DarkModeEnabled, ProfileImagePath) VALUES (?, ?, ?, ?, ?, ?)";
        return _database.Execute(query,user.FirstName,user.LastName,user.MemberSince.ToString("o"),user.NotificationsEnabled ? 1 : 0,user.DarkModeEnabled ? 1 : 0,user.ProfileImagePath);
    }

    public int Update(User user){
        var query = @"update User set FirstName = ?, LastName = ?, MemberSince = ?, NotificationsEnabled = ?, DarkModeEnabled = ?, ProfileImagePath = ? where Id = ?";
        return _database.Execute(query, user.FirstName, user.LastName, user.MemberSince.ToString("o"), user.NotificationsEnabled ? 1 : 0, user.DarkModeEnabled ? 1 : 0, user.ProfileImagePath, user.Id);
    }

    public int Delete(object id){
        var query = "delete from User where Id = ?";
        return _database.Execute(query, id);
    }

    public int DeleteAll(){
        var query = "delete from User";
        return _database.Execute(query);
    }
}

