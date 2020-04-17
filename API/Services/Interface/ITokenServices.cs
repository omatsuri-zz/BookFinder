using Data.Model;
using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Interface
{
    public interface ITokenServices
    {
        string GenerateRefreshToken();
        Token Get(string username);
        int Create(TokenVM tokenViewModel);
        int Update(TokenVM tokenViewModel);
    }
}
