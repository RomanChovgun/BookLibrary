using System;
using System.Collections.Generic;

namespace BookLibrary
{
    public interface IBookLibrary
    {

        IEnumerable<IBookInfoSnapshot> GetAllBooks();

        IBookInfoSnapshot GetBookById(Guid bookId);

        Guid AddBook(Book book);

        void CheckoutBook(Guid bookId, string userName);

        void ReturnBook(Guid bookId);

        void Enqueue(Guid bookId, string userName);
    }
}