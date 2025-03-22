using System;
using System.Collections.Generic;

namespace BookLibrary
{
    public interface IBookInfoSnapshot
    {
        Guid BookId { get; }

        Book Book { get; }

        string Owner { get; }

        BookStatus Status { get; }

        IReadOnlyCollection<string> Queue { get; }
    }
}