using Data.Model;
using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface IBookFinderRepository
    {
        Task<IEnumerable<BookFinderVM>> GetBooks();
        Task<IEnumerable<BookFinder>> Get(int Id);

        IEnumerable<BookFinderVM> Search(string keyword);
        Task<BookFinderVM> Paging(string keyword, int pageNumber, int pageSize);
        Task<BookFinderVM> cari(string Title, string Author,string Publisher, long ISBN, string keyword, int pageNumber, int pageSize);

        int Create(BookFinderVM bookFinderVM);
        int Update(int Id, BookFinderVM bookFinderVM);
        int Delete(int Id);
    }
}
