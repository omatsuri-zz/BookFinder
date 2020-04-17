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
    public class BookFinderRepository : IBookFinderRepository
    {
        public readonly ConnectionStrings _connectionStrings;
        public BookFinderRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<BookFinderVM> cari(string Title, string Author, string Publisher, long ISBN, string keyword, int pageNumber, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramTitle", Title);
                parameters.Add("@paramAuthor", Author);
                parameters.Add("@paramPublisher", Publisher);
                parameters.Add("@paramISBN", ISBN);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@paramKeyword", keyword);
                parameters.Add("@length", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@filterLength", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = new BookFinderVM();
                result.data = await conn.QueryAsync<BookFinderVM>("SP_Cari_Barang", parameters, commandType: CommandType.StoredProcedure);
                int filterlength = parameters.Get<int>("@filterLength");
                result.filterLength = filterlength;
                int length = parameters.Get<int>("@length");
                result.length = length;
                return result;
            }
        }

        public int Create(BookFinderVM bookFinderVM)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramISBN", bookFinderVM.ISBN);
                parameters.Add("@paramTitle", bookFinderVM.Title);
                parameters.Add("@paramAuthor", bookFinderVM.Author);
                parameters.Add("@paramPublisher", bookFinderVM.Publisher);
                parameters.Add("@paramPublished", bookFinderVM.Published);
                parameters.Add("@paramURL", bookFinderVM.URL);
                var result = conn.Execute("SP_InsertDataBook", parameters, commandType: CommandType.StoredProcedure);


                return result;
            }
        }

        public int Delete(int Id)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramId", Id);
                var result = conn.Execute("SP_DeleteDataBooks", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<BookFinder>> Get(int Id)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramId", Id);
                var result = await conn.QueryAsync<BookFinder>("SP_GetByIdBookFinders", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<BookFinderVM>> GetBooks()
        {
            var result = (IEnumerable<BookFinderVM>)null;
            try
            {
                using (var conn = new SqlConnection(_connectionStrings.Value))
                {
                    result = await conn.QueryAsync<BookFinderVM>("SP_GetIsDelBooks");
                    return result;
                }
            }
            catch (Exception m) { }
            return result;
        }

        public async Task<BookFinderVM> Paging(string keyword, int pageNumber, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@pageNumber", pageNumber);
                parameters.Add("@paramKeyword", keyword);
                parameters.Add("@length", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@filterLength", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = new BookFinderVM();
                result.data = await conn.QueryAsync<BookFinderVM>("SP_FilterData", parameters, commandType: CommandType.StoredProcedure);
                int filterlength = parameters.Get<int>("@filterLength");
                result.filterLength = filterlength;
                int length = parameters.Get<int>("@length");
                result.length = length;
                return result;
            }
        }

        public IEnumerable<BookFinderVM> Search(string keyword)
        {
            using (var conn = new SqlConnection(_connectionStrings.Value))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@paramKeyword", keyword);
                var result = conn.Query<BookFinderVM>("SP_SearchBooks", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public int Update(int Id, BookFinderVM bookFinderVM)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionStrings.Value))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@paramId", Id);
                    parameters.Add("@paramISBN", bookFinderVM.ISBN);
                    parameters.Add("@paramTitle", bookFinderVM.Title);
                    parameters.Add("@paramAuthor", bookFinderVM.Author);
                    parameters.Add("@paramPublisher", bookFinderVM.Publisher);
                    parameters.Add("@paramPublished", bookFinderVM.Published);
                    parameters.Add("@paramURL", bookFinderVM.URL);

                    var result = conn.Execute("SP_UpdateDataBook", parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception e) {
                return 0;
            }
        }
    }
}
