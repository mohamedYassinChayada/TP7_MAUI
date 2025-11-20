using System;
namespace TP6.Services.Persistence;
using TP6.Models.Entity;
public interface IDaoNote
{
    List<Note> GetAll();
    Note GetById(object id);
    int Insert(Note entity);
    int Update(Note entity);
    int Delete(object id);
    int DeleteAll();
}


