using Dapper;
using Data.Model;
using Data.Repositories.Interface;
using Data.ViewModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        DynamicParameters parameters = new DynamicParameters();
        public readonly ConnectionStrings _connectionStrings;
        public TokenRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }
        public Token Get(string username)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionStrings.Value);
                var procName = "SP_GetTokenByUsername";
                parameters.Add("@paramUsername", username);
                var tokenrep = connection.QuerySingleOrDefault<Token>(procName, parameters, commandType: CommandType.StoredProcedure);
                return tokenrep;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int Create(TokenVM tokenViewModel)
        {
            using SqlConnection connection = new SqlConnection(_connectionStrings.Value);
            var procName = "SP_InsertRefreshToken";
            parameters.Add("@paramUsername", tokenViewModel.Username);
            parameters.Add("@paramAccessToken", tokenViewModel.AccessToken);
            parameters.Add("@paramExpirationAccessToken", tokenViewModel.ExpireToken);
            parameters.Add("@paramRefreshToken", tokenViewModel.RefreshToken);
            parameters.Add("@paramExpirationRefreshToken", tokenViewModel.ExpireRefreshToken);
            var tokenrep = connection.Execute(procName, parameters, commandType: CommandType.StoredProcedure);
            return tokenrep;
        }

        public int Update(TokenVM tokenViewModel)
        {
            using SqlConnection connection = new SqlConnection(_connectionStrings.Value);
            var procName = "SP_UpdateRefreshToken";
            parameters.Add("@paramUsername", tokenViewModel.Username);
            parameters.Add("@paramAccessToken", tokenViewModel.AccessToken);
            parameters.Add("@paramExpirationAccessToken", tokenViewModel.ExpireToken);
            parameters.Add("@paramRefreshToken", tokenViewModel.RefreshToken);
            parameters.Add("@paramExpirationRefreshToken", tokenViewModel.ExpireRefreshToken);
            var tokenrep = connection.Execute(procName, parameters, commandType: CommandType.StoredProcedure);
            return tokenrep;
        }
    }
}
