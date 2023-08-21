using BookStore.Models;
using BookStore.Models.Reopsitories;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AuthorController : Controller
    {
        // depndency injection (dependency inversion principle)
        private readonly IBookStoreRepository<Author> autohrRepository;

        public AuthorController(IBookStoreRepository<Author> autohrRepository)
        {
            this.autohrRepository = autohrRepository;
        }

        // GET: AuthorController
        public ActionResult Index()
        {
            var authors = autohrRepository.List(); // get all authors
            return View(authors);
        }

        // GET: AuthorController/Details/5
        public ActionResult Details(int id)
        {
            var author = autohrRepository.Find(id);
            return View(author);
        }

        // GET: AuthorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Author author)
        {
            try
            {
                autohrRepository.Add(author);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthorController/Edit/5
        public ActionResult Edit(int id)
        {
            var author = autohrRepository.Find(id); // Get the author with the specified ID
            return View(author);
        }

        // POST: AuthorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Author author)
        {
            try
            {
                autohrRepository.Update(id, author);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthorController/Delete/5
        public ActionResult Delete(int id)
        {
            var author = autohrRepository.Find(id); // Get the author with the specified ID
            if (author == null)
            {
                return NotFound(); // Return a 404 Not Found response if the author doesn't exist
            }
            return View(author); // Pass the author to the view
        }

        // POST: AuthorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Author author)
        {
            try
            {
                autohrRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}