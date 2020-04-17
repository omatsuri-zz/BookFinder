using Dapper;
using Data.Model;
using Data.Repositories.Interface;
using Data.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        DynamicParameters parameters = new DynamicParameters();
        public readonly ConnectionStrings _connectionString; 
        public UserRepository(ConnectionStrings connectionString, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _connectionString = connectionString;
        }
        public int Create(UserVM userVM)
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramUserName", userVM.UserName);
                parameters.Add("@paramEmail", userVM.Email);
                parameters.Add("@paramPassword", userVM.Password);
                var result = conn.Execute("SP_InsertDataUsers", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public int Delete(int Id)
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramId", Id);
                var result = conn.Execute("SP_DeleteDataUsers", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
        public async Task<IEnumerable<User>> Login(UserVM userVM)
        {
            var data = (IEnumerable<User>)null;

            try
            {
                parameters.Add("@paramUserName", userVM.UserName);
                parameters.Add("@paramPassword", userVM.Password);
                using (var con = new SqlConnection(_connectionString.Value))
                {
                    data = await con.QueryAsync<User>("SP_Login",
                        parameters, commandType: CommandType.StoredProcedure);
                    await _signInManager.PasswordSignInAsync(userVM.UserName, userVM.Password, false, false);
                    return data;
                }
            }
            catch (Exception m) { }
            return data;
        }

        public async Task<IdentityResult> Register(UserVM userVM)
        {
            var user = new User(userVM);
            var result = await _userManager.CreateAsync(user, userVM.Password);
            return result;
        }

        public int Update(int Id, UserVM userVM)
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramId", Id);
                parameters.Add("@paramUserName", userVM.UserName);
                parameters.Add("@paramPassword", userVM.Password);
                var result = conn.Execute("SP_UpdateDataUsers", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
