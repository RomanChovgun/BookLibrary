using System;

namespace BookLibrary
{
    public class BookLibraryException : Exception
    {
        public BookLibraryException(string message) : base(message)
        {
        }
    }
}