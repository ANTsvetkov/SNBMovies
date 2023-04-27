using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SNBMovies.Data;
using SNBMovies.Data.Repository;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels;
using System.Diagnostics;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Дисплей на страници в приложението.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MovieRepository movieRepository, OrderRepository orderRepository, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _orderRepository = orderRepository;
            _context = applicationDbContext;
        }

        //
        // Обобщение:
        //     Дисплей на началната страница в приложението
        //
        // Връща:
        //     Изглед с модел - списък от обект "Movie"
        public IActionResult Index()
        {
            var movies = _movieRepository.GetAllMovies();
            return View(movies);
        }
        public IActionResult About()
        {
            return View();
        }

        //
        // Обобщение:
        //     Дисплей на страница "За нас"
        //
        // Връща:
        //     Изглед с модел "AboutVM"
        public async Task<IActionResult> Contact()
        {
            var movies = _movieRepository.GetAllMovies();
            var orders = await _orderRepository.GetAllOrderHistoryAsync();
            var users = _context.Users.ToList();

            var model = new AboutVM
            {
                MoviesCount = movies,
                OrdersCount = orders,
                UsersCount = users
            };
            return View(model);
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