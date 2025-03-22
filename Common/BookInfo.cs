using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLibrary
{
    public class BookInfo : IBookInfo
    {
        public BookInfo(Book book)
        {
            BookId = Guid.NewGuid();
            Book = book;
            Status = BookStatus.Free;
            Owner = null;
            _queue = new Queue<string>();
        }

        public Guid BookId { get; }

        public Book Book { get; }

        public BookStatus Status { get; set; }

        public string Owner { get; set; }

        public void Enqueue(string userName)
        {
            _queue.Enqueue(userName);
        }

        public void Dequeue()
        {
            _queue.Dequeue();
        }

        public List<string> ShowQueue()
        {
            return _queue.ToList();
        }

        private readonly Queue<string> _queue;
    }
}