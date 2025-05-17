using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace FutureVendWeb.Services.User
{
    public class UserService : IUserService
    {
        VendingDbContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(VendingDbContext vendingDbContext , IHttpContextAccessor httpContextAccessor)
        {
            _context = vendingDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public void CreateUser(CreateUser createUser)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.Name = createUser.Name;
            userEntity.Email = createUser.Email;
            userEntity.Password = createUser.Password;

            _context.Add(userEntity);
            _context.SaveChanges();
        }

        public UserData? GetUser()
        {
            int? id = _httpContextAccessor.HttpContext.Session.GetInt32("id");
            string? name = _httpContextAccessor.HttpContext.Session.GetString("name");
            string? email = _httpContextAccessor.HttpContext.Session.GetString("email");

            if (id != null && name != null && email != null)
            {
                return new UserData { Id = (int)id, Name = name, Email = email};
            }
            else
            {
                return null;
            }
            
        }

        public UserData RegisterUser(string email, string password)
        {
            UserEntity? user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (user == null)
            {
                throw new ArgumentException("Invalid email or password");
            }

            return new UserData { Id = user.Id, Name = user.Name, Email = user.Email };
        }

        public void SetUser( UserData? user)
        {
            if(user == null)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("id", user.Id);
                _httpContextAccessor.HttpContext.Session.SetString("name", user.Name);
                _httpContextAccessor.HttpContext.Session.SetString("email", user.Email);
            }
        }
    }
}
