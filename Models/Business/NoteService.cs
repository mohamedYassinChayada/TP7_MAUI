using TP6.Models.Entity;
using TP6.Infrastructure.Persistance;

namespace TP6.Models.Business;

public class NoteService
{
    private static NoteService? _instance;
    public static NoteService Instance => _instance ??= new NoteService();

    private readonly NoteDAO _noteDao;

    private NoteService()
    {
        _noteDao = new NoteDAO();
        InitializeDefaultNotes();
    }

    public List<Note> getnotes() => _noteDao.GetAll();

    public void addnote(Note note)
    {
        _noteDao.Insert(note);
    }

    public int count() => _noteDao.GetAll().Count;

    private void InitializeDefaultNotes()
    {
        if (_noteDao.GetAll().Count == 0)
        {
            var defaultNotes = new List<Note>
            {
                new Note { Title = "cours", Content = "cours math", CreatedAt = DateTime.Now.AddDays(-2) },
                new Note { Title = "rev maths", Content = "chap 1 et 2", CreatedAt = DateTime.Now.AddDays(-1) },
                new Note { Title = "Anniversaire Ali", Content = "dont forget ali birthday", CreatedAt = DateTime.Now.AddHours(-3) }
            };

            foreach (var note in defaultNotes)
            {
                _noteDao.Insert(note);
            }
        }
    }
}