using Apibackend.Data;
using Apibackend.Models;

namespace Apibackend.Services
{
    public class UserService
    {
        public AppDbContext _db;
        public UserService(AppDbContext db)
        {
            _db = db;
        }
        /// <summary>
        /// get users
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }
        public User AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }
        public User UpdateUser(int id, User user)
        {
            try
            {
                var existingUser = _db.Users.FirstOrDefault(p => p.Id == id);
                if (existingUser == null)
                {
                    return null;
                }
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
                _db.Users.Update(existingUser);
                _db.SaveChanges();
                return existingUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error modify User: " + ex.Message);
            }

        }
        public void DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }
        public User GetUserByEmail(string email)
        {
            try
            {
                // Essaie de récupérer l'utilisateur
                var utilisateur = _db.Users.FirstOrDefault(u => u.Email == email);

                // Si aucun utilisateur trouvé, retourne null
                return utilisateur;
            }
            catch (Exception ex)
            {
                // Ici on capture les erreurs réelles (ex: problème DB)
                // On peut logger l'erreur puis renvoyer null ou relancer l'exception
                Console.WriteLine("Erreur lors de la récupération de l'utilisateur : " + ex.Message);
                return null;
            }
        }

    }
}