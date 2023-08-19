using BookStore.Models;
using BookStore.Models.Reopsitories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        //depndency injection (dependency inversion principle)
        private readonly IBookStoreRepository<Book> bookRepository;

        private readonly IBookStoreRepository<Author> autohrRepository;

        public BookController(IBookStoreRepository<Book> bookRepository, IBookStoreRepository<Author> autohrRepository)
        {
            this.bookRepository = bookRepository;
            this.autohrRepository = autohrRepository;
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
        public IActionResult Create(Book book)
        {
            if (book.Author == null || book.Author.Id == -1) // checking the default value
            {
                ModelState.AddModelError("Author.Id", "Please select an author.");
            }

            if (ModelState.IsValid)
            {
                var author = autohrRepository.Find(book.Author.Id); // Get the author with the specified ID
                Book Newbook = new Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = author
                };
                bookRepository.Add(Newbook);
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Please fill all the required fields.");
            ViewBag.AuthorsList = new SelectList(AuthorsDropdownList(), "Id", "FullName", book?.Author?.Id);  // Repopulate dropdown
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
        [Route("Book/Edit/{id}")] // to avoid the error "The current request for action 'Edit' on controller type 'BookController' is ambiguous between the following action methods"
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Book editedBook)
        {
            try
            {
                var existingBook = bookRepository.Find(id);

                if (existingBook == null)
                {
                    return NotFound(); // Or you can return a view with an error message
                }

                // Since Author is an object, it might be only partially filled in (maybe just the ID).
                // To avoid null or incomplete data, fetch the full author object from the repository.
                if (editedBook.Author != null && editedBook.Author.Id > 0)
                {
                    var author = autohrRepository.Find(editedBook.Author.Id);
                    if (author != null)
                    {
                        editedBook.Author = author;
                    }
                }

                bookRepository.Update(id, editedBook);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // If something goes wrong, repopulate the authors dropdown and return to the Edit view
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
                bookRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private List<Author> AuthorsDropdownList()
        {
            var authors = autohrRepository.List().ToList();
            authors.Insert(0, new Author { Id = -1, FullName = "---Please Select Author---" });
            return authors;
        }
    }
}