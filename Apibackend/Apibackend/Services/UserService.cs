using Apibackend.Data;
using Apibackend.Models;
using Microsoft.AspNetCore.Identity;

namespace Apibackend.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Récupérer tous les utilisateurs
        /// </summary>
        public List<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        /// <summary>
        /// Récupérer un utilisateur par Id
        /// </summary>
        public User GetUserById(int id)
        {
            return _db.Users.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Ajouter un utilisateur avec mot de passe hashé
        /// </summary>
        public User AddUser(User user)
        {
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, user.Password);

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        /// <summary>
        /// Mettre à jour un utilisateur (hash si mot de passe modifié)
        /// </summary>
        public User UpdateUser(int id, User user)
        {
            var existingUser = _db.Users.FirstOrDefault(p => p.Id == id);
            if (existingUser == null) return null;

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            if (!string.IsNullOrEmpty(user.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                existingUser.Password = passwordHasher.HashPassword(existingUser, user.Password);
            }

            _db.Users.Update(existingUser);
            _db.SaveChanges();
            return existingUser;
        }

        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        public void DeleteUser(int id)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Récupérer un utilisateur par email
        /// </summary>
        public User GetUserByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Vérifier le mot de passe pour login
        /// </summary>
        public bool VerifyPassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
