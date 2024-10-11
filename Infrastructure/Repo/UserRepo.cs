using Application.Contracts;
using Application.DTOs;
using Domain.Entites;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repo
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext appDbContext;

        public UserRepo(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task<LoginRespons> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!);
            if (getUser == null) return new LoginRespons(false, "User not found!! Sorry!!");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
                return new LoginRespons(true, "Login successfull", GenerateJWTToken(getUser));
            else
                return new LoginRespons(false, "Invalid");

        }

        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);


        public async Task<RegistrationResponse> RegisterUserAsync(RegistarUserDTO registarUserDTO)
        {
            var getUser = await FindUserByEmail(registarUserDTO.Email!);
            if (getUser != null)
                return new RegistrationResponse(false, "User already exist");

            appDbContext.Users.Add(new ApplicationUser()
            {
                Name = registarUserDTO.Name,
                Email = registarUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registarUserDTO.Password)

            });

            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Registraturadan utdi");

        }
    }
}
