using API.Services.Interface;
using Data.Model;
using Data.Repositories.Interface;
using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace API.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly ITokenRepository _tokenRepository;
        public TokenServices(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }
        public int Create(TokenVM tokenViewModel)
        {
            return _tokenRepository.Create(tokenViewModel);
        }

        public string GenerateRefreshToken()
        {
            
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
        }

        public Token Get(string username)
        {
            return _tokenRepository.Get(username);
        }

        public int Update(TokenVM tokenViewModel)
        {
            return _tokenRepository.Update(tokenViewModel);
        }
    }
}
