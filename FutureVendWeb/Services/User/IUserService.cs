using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace FutureVendWeb.Services.User
{
    public interface IUserService
    {
        UserData RegisterUser(string email , string password);

        void CreateUser(CreateUser createUser);

        UserData? GetUser();

        void SetUser(UserData? user);
    }
}
