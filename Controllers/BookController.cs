﻿using BookStore.Models;
using BookStore.Models.Reopsitories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        //depndency injection (dependency inversion principle)

        private readonly IBookStoreRepository<Book> bookRepository;

        private readonly IBookStoreRepository<Author> autohrRepository;

        private readonly IWebHostEnvironment Hosting;

        public BookController(IBookStoreRepository<Book> bookRepository, IBookStoreRepository<Author> autohrRepository, IWebHostEnvironment Hosting)
        {
            this.bookRepository = bookRepository;
            this.autohrRepository = autohrRepository;
            this.Hosting = Hosting;
        }

        // GET: BookController
        public ActionResult Index()
        {
            var books = bookRepository.List(); // get all books
            return View(books);
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            ViewBag.AuthorsList = new SelectList(AuthorsDropdownList(), "Id", "FullName");
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Book book)
        {
            HandleAuthorSelection(book);

            if (ModelState.IsValid)
            {
                book.BookFolderPath = CreateBookDirectory(book.Title);
                book.BookFilePath = await UploadFile(book.BookFile, book.BookFolderPath);
                book.BookImagePath = await UploadFile(book.BookImage, book.BookFolderPath);

                var author = autohrRepository.Find(book.Author.Id);

                Book Newbook = new Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = author,
                    BookImagePath = book.BookImagePath,
                    BookFilePath = book.BookFilePath,
                    BookFolderPath = book.BookFolderPath
                };

                bookRepository.Add(Newbook);

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Please fill all the required fields.");
            ViewBag.AuthorsList = new SelectList(AuthorsDropdownList(), "Id", "FullName", book?.Author?.Id);

            return View(book);
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            var book = bookRepository.Find(id); // Get the book with the specified ID

            ViewBag.AuthorsList = new SelectList(AuthorsDropdownList(), "Id", "FullName", book?.Author?.Id);
            return View(book);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [Route("Book/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Book editedBook)
        {
            try
            {
                var existingBook = bookRepository.Find(id);

                if (existingBook == null)
                {
                    return NotFound();
                }

                // Handle new image upload

                existingBook.Title = editedBook.Title;
                existingBook.Description = editedBook.Description;
                existingBook.Author = editedBook.Author;

                // Update the existingBook object in the database.
                bookRepository.Update(id, existingBook);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var authors = autohrRepository.List();
                ViewBag.AuthorsList = new SelectList(authors, "Id", "FullName", editedBook.Author?.Id);
                return View(editedBook);
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            var book = bookRepository.Find(id); // Get the book with the specified ID
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [Route("Book/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Book collection)
        {
            try
            {
                var book = bookRepository.Find(id);

                if (book != null)
                {
                    delete_directory(book.BookFolderPath);

                    bookRepository.Delete(id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //=============Methods===============

        private void delete_file(string file_path)
        {
            try
            {
                if (System.IO.File.Exists(file_path))
                {
                    System.IO.File.Delete(file_path);
                }
            }
            catch (IOException ioEx)
            {
                // Log or handle the specific IO exception
                Console.WriteLine($"IO Error: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                // Log or handle the unauthorized access exception
                Console.WriteLine($"Access Error: {uaEx.Message}");
            }
            catch (Exception ex)
            {
                // Log or handle any generic exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void delete_directory(string? directory_path)
        {
            try
            {
                if (Directory.Exists(directory_path))
                {
                    Directory.Delete(directory_path, true);  // true means it will delete subdirectories and files
                }
            }
            catch (IOException ioEx)
            {
                // Log or handle the specific IO exception
                Console.WriteLine($"IO Error: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                // Log or handle the unauthorized access exception
                Console.WriteLine($"Access Error: {uaEx.Message}");
            }
            catch (Exception ex)
            {
                // Log or handle any generic exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // This method is used to populate the authors dropdown list
        private List<Author> AuthorsDropdownList()
        {
            var authors = autohrRepository.List().ToList();
            authors.Insert(0, new Author { Id = -1, FullName = "---Please Select Author---" });
            return authors;
        }

        // This method is used to validate the author selection
        private void HandleAuthorSelection(Book book)
        {
            if (book.Author == null || book.Author.Id == -1)
            {
                ModelState.AddModelError("Author.Id", "Please select an author.");
            }
        }

        // This method is used to create a directory for the book
        private string CreateBookDirectory(string bookTitle)
        {
            var bookFolder = Path.Combine("uploads", bookTitle);
            Directory.CreateDirectory(Path.Combine(Hosting.WebRootPath, bookFolder));
            return bookFolder;
        }

        // This method is used to upload the book file and book image and give them uniqe names
        private async Task<string> UploadFile(IFormFile file, string parentFolder)
        {
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // use GUID to name the file
                var filePath = Path.Combine(Hosting.WebRootPath, parentFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{parentFolder}/{fileName}";
            }

            return null;
        }
    }
}