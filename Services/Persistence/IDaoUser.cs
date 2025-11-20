using System;
namespace TP6.Services.Persistence;
using TP6.Models.Entity;
	public interface IDaoUser
	{
        List<User> GetAll();
        User GetById(object id);
        int Insert(User entity);
        int Update(User entity);
        int Delete(object id);
        int DeleteAll();
    }


