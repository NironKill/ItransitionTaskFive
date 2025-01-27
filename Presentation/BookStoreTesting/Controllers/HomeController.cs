using BookStoreTesting.Application.Services.Interfaces;
using BookStoreTesting.Domain;
using BookStoreTesting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStoreTesting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenerateBooks(BookModel model)
        {
            IEnumerable<Book> books = _bookService.GenerateBooks(language: model.Language,
                seed: model.Seed,
                likes: model.Likes,
                reviews: model.Reviews,
                numberOfBooks: model.NumberOfBooks,
                lastRowId: model.LastRowId);

            return Json(new
            {
                data = books
            });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
