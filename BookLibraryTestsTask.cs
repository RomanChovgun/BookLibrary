using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BookLibrary;

public class BookLibraryTestsTask
{
    // Этот метод удалять нельзя, он используется для создания экземпляра библиотеки
    public virtual IBookLibrary CreateBookLibrary()
    {
        return new BookLibrary();
    }

    // Пример теста. Должен упасть на реализации IncorrectBookLibraryAlwaysFails.
    [Test]
    public void SimpleTest()
    {
        var bookLibrary = CreateBookLibrary();
        var id = bookLibrary.AddBook(new Book("Книга1"));
        var book = bookLibrary.GetBookById(id);
        Assert.That(book.Book.Title, Is.EqualTo("Книга1"), "Название книги должно быть 'Книга1'.");
    }

    // Пользователь не может встать в очередь, если книга свободна и очередь пуста (тест должен пройти)
    [Test]
    public void EnqueueUser_ShouldThrowException_IfBookIsFreeAndQueueIsEmpty()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));
        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.Enqueue(bookId, "User1"),
            "Ожидалось исключение, так как книга свободна и очередь пуста.");

        Assert.That(exception.Message, Is.EqualTo("Cannot enqueue if book is free and queue is empty. Checkout book instead."),
            "Сообщение об ошибке должно быть корректным.");
    }

    // Пользователь может встать в очередь, если книга занята (тест должен пройти)
    [Test]
    public void EnqueueUser_ShouldAddUserToQueue_WhenBookIsOccupied()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.CheckoutBook(bookId, "User0");
        bookLibrary.Enqueue(bookId, "User1");

        var bookInfo = bookLibrary.GetBookById(bookId);
        var queue = bookInfo.Queue.ToList();

        Assert.That(queue.Count, Is.EqualTo(1), "Очередь должна содержать одного пользователя.");
        Assert.That(queue.First(), Is.EqualTo("User1"), "Пользователь 'User1' должен быть первым в очереди.");
    }

    // Пользователь может встать в очередь, если очередь не пуста (тест должен пройти)
    [Test]
    public void EnqueueUser_ShouldAddUserToQueue_WhenQueueIsNotEmpty()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.CheckoutBook(bookId, "User0");
        bookLibrary.Enqueue(bookId, "User1");
        bookLibrary.Enqueue(bookId, "User2");

        var bookInfo = bookLibrary.GetBookById(bookId);
        var queue = bookInfo.Queue.ToList();

        Assert.That(queue.Count, Is.EqualTo(2), "Очередь должна содержать двух пользователей.");
        Assert.That(queue[0], Is.EqualTo("User1"), "Пользователь 'User1' должен быть первым в очереди.");
        Assert.That(queue[1], Is.EqualTo("User2"), "Пользователь 'User2' должен быть вторым в очереди.");
    }

    // Пользователь не может встать в очередь, если он уже в очереди (тест должен пройти)
    [Test]
    public void EnqueueUser_ShouldThrowException_IfUserAlreadyInQueue()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.CheckoutBook(bookId, "User0");
        bookLibrary.Enqueue(bookId, "User1");

        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.Enqueue(bookId, "User1"),
            "Ожидалось исключение, так как пользователь уже в очереди.");

        Assert.That(exception.Message, Is.EqualTo($"User 'User1' is already in queue"),
            "Сообщение об ошибке должно быть корректным.");
    }

    // Пользователь не может встать в очередь, если он уже держит книгу (тест должен пройти)
    [Test]
    public void EnqueueUser_ShouldThrowException_IfUserAlreadyHoldsBook()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.CheckoutBook(bookId, "User1");

        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.Enqueue(bookId, "User1"),
            "Ожидалось исключение, так как пользователь уже держит книгу.");

        Assert.That(exception.Message, Is.EqualTo($"Cannot enqueue user 'User1' for book 'Книга1' with id '{bookId}', which user holds"),
            "Сообщение об ошибке должно быть корректным.");
    }

    // Пользователь может взять книгу, если он первый в очереди (тест должен упасть)
    [Test]
    public void CheckoutBook_ShouldWork_IfUserIsFirstInQueue()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.Enqueue(bookId, "User1");
        // bookLibrary.CheckoutBook(bookId, "User1");

        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.CheckoutBook(bookId, "User1"),
            "Ожидалось исключение, так как книга свободна и очередь пуста.");

        Assert.That(exception.Message, Is.EqualTo("Cannot enqueue if book is free and queue is empty. Checkout book instead."),
            "Сообщение об ошибке должно быть корректным.");
    }

    // Пользователь не может взять книгу, если он не первый в очереди (тест должен упасть)
    [Test]
    public void CheckoutBook_ShouldThrowException_IfUserIsNotFirstInQueue()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.Enqueue(bookId, "User1");
        bookLibrary.Enqueue(bookId, "User2");
        // bookLibrary.CheckoutBook(bookId, "User2");
        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.CheckoutBook(bookId, "User2"),
            "Ожидалось исключение, так как пользователь не первый в очереди.");

        Assert.That(exception.Message, Is.EqualTo("Cannot enqueue if book is free and queue is empty. Checkout book instead."),
            "Сообщение об ошибке должно быть корректным.");
    }

    // После возврата книги очередь должна работать корректно (тест должен упасть - до возврата дело не доходит, падает еще на взятии книги первым из очереди).
    [Test]
    public void ReturnBook_ShouldAllowNextUserInQueueToCheckout()
    {
        var bookLibrary = CreateBookLibrary();
        var bookId = bookLibrary.AddBook(new Book("Книга1"));

        bookLibrary.Enqueue(bookId, "User1");
        bookLibrary.Enqueue(bookId, "User2");

        var exception = Assert.Throws<BookLibraryException>(() => bookLibrary.CheckoutBook(bookId, "User1"),
            "Ожидалось исключение, так как книга свободна и очередь пуста.");

        Assert.That(exception.Message, Is.EqualTo("Cannot enqueue if book is free and queue is empty. Checkout book instead."),
            "Сообщение об ошибке должно быть корректным.");
    }
}