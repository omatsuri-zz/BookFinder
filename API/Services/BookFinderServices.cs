using API.Services.Interface;
using Data.Model;
using Data.Repositories.Interface;
using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class BookFinderServices : IBookFinderServices
    {
        private readonly IBookFinderRepository _bookFinderRepository;
        public BookFinderServices(IBookFinderRepository bookFinderRepository)
        {
            _bookFinderRepository = bookFinderRepository;
        }

        public async Task<BookFinderVM> cari(string Title, string Author, string Publisher, long ISBN, string keyword, int pageNumber, int pageSize)
        {
            return await _bookFinderRepository.cari(Title, Author, Publisher, ISBN, keyword, pageNumber, pageSize);
        }

        public int Create(BookFinderVM bookFinderVM)
        {
            return _bookFinderRepository.Create(bookFinderVM);
        }

        public int Delete(int Id)
        {
            return _bookFinderRepository.Delete(Id);
        }

        public Task<IEnumerable<BookFinder>> Get(int Id)
        {
            return _bookFinderRepository.Get(Id);
        }

        public Task<IEnumerable<BookFinderVM>> GetBooks()
        {
            return _bookFinderRepository.GetBooks();
        }

        public async Task<BookFinderVM> Paging(string keyword, int pageNumber, int pageSize)
        {
            return await _bookFinderRepository.Paging(keyword, pageNumber, pageSize);
        }

        public IEnumerable<BookFinderVM> Search(string keyword)
        {
            return _bookFinderRepository.Search(keyword);
        }

        public int Update(int Id, BookFinderVM bookFinderVM)
        {
            return _bookFinderRepository.Update(Id, bookFinderVM);
        }
    }
}
