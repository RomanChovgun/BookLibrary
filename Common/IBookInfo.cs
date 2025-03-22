using System;
using System.Collections.Generic;

namespace BookLibrary
{
    public interface IBookInfo
    {
        Guid BookId { get; }

        Book Book { get; }

        string Owner { get; set; }

        BookStatus Status { get; set; }

        void Enqueue(string userName);

        void Dequeue();

        List<string> ShowQueue();
    }
}