using TP6.Models.Entity;
using TP6.Infrastructure.Persistance;

namespace TP6.Models.Business;

public class UserService
{
    private static UserService? _instance;
    public static UserService Instance => _instance ??= new UserService();

    private readonly UserDAO _userDao;
    private User? _currentUser;

    private UserService()
    {
        _userDao = new UserDAO();
        LoadUser();
    }

    public User GetCurrentUser()
    {
        return _currentUser ??= CreateDefaultUser();
    }

    public void UpdateUser(User user)
    {
        _currentUser = user;
        _userDao.Update(user);
    }

    public void UpdateUserProfile(string firstName, string lastName)
    {
        var user = GetCurrentUser();
        user.FirstName = firstName;
        user.LastName = lastName;
        _userDao.Update(user);
    }

    public void UpdateUserSettings(bool notificationsEnabled, bool darkModeEnabled)
    {
        var user = GetCurrentUser();
        user.NotificationsEnabled = notificationsEnabled;
        user.DarkModeEnabled = darkModeEnabled;
        _userDao.Update(user);
    }

    private void LoadUser()
    {
        try
        {
            _currentUser = _userDao.GetFirst();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading user: {ex.Message}");
        }

        _currentUser ??= CreateDefaultUser();
    }

    private User CreateDefaultUser()
    {
        var user = new User
        {
            FirstName = "Omar",
            LastName = "Soussi",
            MemberSince = DateTime.Now.AddDays(-3),
            NotificationsEnabled = false,
            DarkModeEnabled = true
        };

        _userDao.Insert(user);
        _currentUser = _userDao.GetFirst();
        return _currentUser!;
    }
}