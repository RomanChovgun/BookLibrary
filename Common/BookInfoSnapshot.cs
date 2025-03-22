using System;
using System.Collections.Generic;

namespace BookLibrary
{
    public class BookInfoSnapshot : IBookInfoSnapshot
    {
        public BookInfoSnapshot(IBookInfo bookInfo)
        {
            BookId = bookInfo.BookId;
            Book = bookInfo.Book;
            Owner = bookInfo.Owner;
            Status = bookInfo.Status;
            Queue = bookInfo.ShowQueue();
        }

        public Guid BookId { get; }

        public Book Book { get; }

        public string Owner { get; }

        public BookStatus Status { get; }

        public IReadOnlyCollection<string> Queue { get; }
    }
}