using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLibrary
{
    public class BookLibrary : IBookLibrary
    {
        public virtual IEnumerable<IBookInfoSnapshot> GetAllBooks()
        {
            return _books.Select(x => new BookInfoSnapshot(x));
        }

        public virtual IBookInfoSnapshot GetBookById(Guid bookId)
        {
            return new BookInfoSnapshot(GetBookByIdInternal(bookId));
        }

        public virtual Guid AddBook(Book book)
        {
            if (book?.Title == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            var bookInfo = new BookInfo(book);
            if (_books.Any(x => x.BookId == bookInfo.BookId))
                throw new BookLibraryException(
                    $"Book with id '{bookInfo.BookId}' already exists.");
            _books.Add(bookInfo);
            return bookInfo.BookId;
        }

        public virtual void CheckoutBook(Guid bookId, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            var book = GetBookByIdInternal(bookId);
            if (book.Owner != null || book.Status != BookStatus.Free)
                throw new BookLibraryException("Book already occupied");
            var queue = book.ShowQueue();
            if (queue.Count != 0 && queue.First() != userName)
                throw new BookLibraryException(
                    $"Cannot checkout book, because queue is not empty and '{userName}' is not the first in queue");

            book.Owner = userName;
            book.Status = BookStatus.Occupied;

            if (queue.Count != 0)
                book.Dequeue();
        }

        public virtual void ReturnBook(Guid bookId)
        {
            var book = GetBookByIdInternal(bookId);
            if (book.Owner == null || book.Status == BookStatus.Free)
                throw new BookLibraryException("Cannot return free book");
            book.Owner = null;
            book.Status = BookStatus.Free;
        }

        public virtual void Enqueue(Guid bookId, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            var book = GetBookByIdInternal(bookId);
            if (book.Owner == userName)
                throw new BookLibraryException(
                    $"Cannot enqueue user '{userName}' for book '{book.Book.Title}' with id '{book.BookId}', which user holds");
            var currentQueue = book.ShowQueue();
            
            // Если книга свободна и очередь пуста, метод запрещает вставать в очередь
            if (currentQueue.Count == 0 && (book.Owner == null || book.Status == BookStatus.Free))
                throw new BookLibraryException(
                    "Cannot enqueue if book is free and queue is empty. Checkout book instead.");

            if (currentQueue.Contains(userName))
                throw new BookLibraryException($"User '{userName}' is already in queue");
            book.Enqueue(userName);
        }

        protected virtual BookInfo GetBookByIdInternal(Guid bookId)
        {
            var match = _books.Where(x => x.BookId == bookId).ToArray();
            var count = match.Length;
            if (count == 0)
                throw new BookLibraryException($"Book with bookId '{bookId.ToString()}' was not found.");
            return match.Single();
        }

        protected readonly List<BookInfo> _books = new List<BookInfo>();
    }
}
