using BookStore.Models;
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

        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookStoreRepository<Book> bookRepository, IBookStoreRepository<Author> autohrRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.bookRepository = bookRepository;
            this.autohrRepository = autohrRepository;
            _webHostEnvironment = webHostEnvironment;
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
                string bookFolder = CreateBookDirectory(book.Title);

                book.BookFilePath = await UploadFile(book.BookFile, bookFolder, "BookFiles");
                book.BookImagePath = await UploadFile(book.BookImage, bookFolder, "BookImage");

                var author = autohrRepository.Find(book.Author.Id);

                Book Newbook = new Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = author,
                    BookImagePath = book.BookImagePath,
                    BookFilePath = book.BookFilePath
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
        public async Task<IActionResult> EditAsync(int id, Book editedBook, IFormFile newImage, IFormFile newFile)
        {
            try
            {
                var existingBook = bookRepository.Find(id);

                if (existingBook == null)
                {
                    return NotFound();
                }

                if (editedBook.Author != null && editedBook.Author.Id > 0)
                {
                    var author = autohrRepository.Find(editedBook.Author.Id);
                    if (author != null)
                    {
                        editedBook.Author = author;
                    }
                }

                // Handle new image upload
                if (newImage != null)
                {
                    // Delete the old image
                    System.IO.File.Delete(existingBook.BookImagePath);

                    // Upload the new image and update the BookImagePath property
                    string bookFolder = Path.GetDirectoryName(existingBook.BookImagePath);
                    editedBook.BookImagePath = await UploadFile(newImage, bookFolder, "BookImage");
                }

                // Handle new file upload
                if (newFile != null)
                {
                    // Delete the old file
                    System.IO.File.Delete(existingBook.BookFilePath);

                    // Upload the new file and update the BookFilePath property
                    string bookFolder = Path.GetDirectoryName(existingBook.BookFilePath);
                    editedBook.BookFilePath = await UploadFile(newFile, bookFolder, "BookFiles");
                }

                existingBook.Title = editedBook.Title;
                existingBook.Description = editedBook.Description;
                existingBook.Author = editedBook.Author;

                // Only update the paths if a new image or file has been uploaded.
                if (newImage != null)
                    existingBook.BookImagePath = editedBook.BookImagePath;

                if (newFile != null)
                    existingBook.BookFilePath = editedBook.BookFilePath;

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
                    // Get the directory path of the book's files
                    string bookDirectory = Path.GetDirectoryName(book.BookFilePath);

                    // Delete the directory and its content
                    if (Directory.Exists(bookDirectory))
                    {
                        Directory.Delete(bookDirectory, true);  // The second parameter (true) means it will delete subdirectories and files
                    }

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
            if (System.IO.File.Exists(file_path))
            {
                System.IO.File.Delete(file_path);
            }
        }

        private void delete_directory(string directory_path)
        {
            if (Directory.Exists(directory_path))
            {
                Directory.Delete(directory_path, true);  // true means it will delete subdirectories and files
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
            Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, bookFolder));
            return bookFolder;
        }

        // This method is used to upload the book file and book image and give them uniqe names
        private async Task<string> UploadFile(IFormFile file, string parentFolder, string subFolder)
        {
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // use GUID to name the file
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, parentFolder, subFolder, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // ensure directory exists

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{parentFolder}/{subFolder}/{fileName}";
            }

            return null;
        }
    }
}