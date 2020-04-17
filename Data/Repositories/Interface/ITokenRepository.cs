using Data.Model;
using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface ITokenRepository
    {
        Token Get(string username);
        int Create(TokenVM tokenViewModel);
        int Update(TokenVM tokenViewModel);
    }
}
