using System;
using System.Threading.Tasks;
using DatingApp.API.Model;
using DatingApp.API.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
                public DataContext Context { get; }

        public async Task<User> Login(string username, string password)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=> x.Username==username.ToLower());

            if(user==null)
                return null;

            if(!VerifyPassword(password,user.PasswordHash,user.PasswordSalt))
                return null;

            return user;

        }

 
        public async Task<User> Register(User user, string password)
        {
            byte[] PasswordHash,PasswordSalt;
            CreatePasswordHash( password,out PasswordHash,out PasswordSalt);
            user.PasswordHash=PasswordHash;
            user.PasswordSalt=PasswordSalt;
           await _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();
           return user;
        }


        public async Task<bool> UserExists(string username)
        {
            if( await _context.Users.AnyAsync(x=> x.Username==username)) return true;
            return false;

        }

        #region Private Methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
       private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var comutePasswordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i<comutePasswordHash.Length; i++)
                {
                    if(passwordHash[i]!=comutePasswordHash[i])
                        return false;
                }
                
            }
            return true;
        }


        #endregion
    }
}