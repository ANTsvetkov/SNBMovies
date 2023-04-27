using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SNBMovies.Data;
using SNBMovies.Data.Repository;
using SNBMovies.Migrations;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.ShoppingCart;
using System.IO.Compression;
using System.Security.Claims;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Контролерът, отговоерн за изпълнението на действията, свързани с пазарската количка и покупките.
    /// Съдържа методи за дисплей на кичката и история на покупките, добавяне и премахване на филми от количката,
    /// завършване на поръчката и изтегляне на файл.
    /// </summary>
    /// 
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private OrderRepository _orderRepository;
        public ShoppingCartController(ApplicationDbContext context, OrderRepository orderRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        //
        // Обобщение:
        //     Дисплей на страница с история на покупките
        //
        // Връща:
        //     Изглед с модел "OrderHistoryVM"
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Извличане на ID на потребителя, който е в момента логнат.
            var shoppingCartItems = await _orderRepository.GetOrderHistoryAsync(userId);
            // Извличане на история на поръчките на потребителя.
            var orderViewModel = new OrderHistoryVM
            {// Създаване на обект OrderHistoryVM, който ще съдържа историята на поръчките.
                OrderHistoryItems = shoppingCartItems,
                TotalPrice = shoppingCartItems.Sum(x=>x.Price),
                OrderId = shoppingCartItems.Select(x=>x.OrderId).ToString()
            };

            return View(orderViewModel);
        }

        //
        // Обобщение:
        //     Извършва действие за добавяне на филм в количката
        //     
        // Параметри:
        //   movie:
        //      Моделът, който се довавя в количката
        //
        // Връща:
        //     Пренасочване към действие "Cart"
        [HttpPost]
        public async Task<IActionResult> AddToCart(Movie movie)
        {
            var user = await _userManager.GetUserAsync(User);
            // Създаване на променлива user и извикване на асинхронния метод GetUserAsync().
            if (user != null)
            {// Проверка дали user не е null.
               await _orderRepository.AddToCartAsync(user.Id, movie.Id);
               // Асинхронно добавяне на movie.Id в кошницата на потребителя чрез извикване на AddToCartAsync().
            }
            return RedirectToAction("Cart");
            // Пренасочване към действието "Cart".
        }

        //
        // Обобщение:
        //     Действие за премахване на филм от количката
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на филма в количката
        //
        // Връща:
        //     Пренасочване към дейстеир "Cart"
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            await _orderRepository.RemoveFromCartAsync(id);
            //Премахва филма от количката.
            return RedirectToAction("Cart");
        }

        //
        // Обобщение:
        //     Дисплей на страница с пазарска количка
        //
        // Връща:
        //     Изглед с модел "ShoppingCartVM"
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Намиране на стойността на идентификатора на потребителя.
            var items = await _orderRepository.GetShoppingCartItemsAsync(userId);
            // Асинхронно извличане на елементите от кошницата на потребителя.
            var viewModel = new ShoppingCartVM
            {// Създаване на нов обект от клас ShoppingCartVM, като се инициализират ShoppingCartItems и TotalPrice.
                ShoppingCartItems = items.Select(x => new ShoppingCartItem
                {// Присвояване на ShoppingCartItems списък от обекти ShoppingCartItem.
                    Id = x.Id,
                    Movie = x.Movie,
                    UserId = userId,
                    Price = x.Movie.Price,
                }).ToList(),
                TotalPrice = items.Sum(x => x.Price)
                // Присвояване на TotalPrice общата цена на елементите в кошницата на потребителя. 
            };
            return View(viewModel);
            // Връщане на изгледа с модела viewModel.
        }

        //
        // Обобщение:
        //     Завършване на поръчка. Запазване в история на поръчките и изчистване на количката.
        //
        // Връща:
        //     Изглед "OrderCompleted" с модел - списък от обект "ShoppingCartItem"
        public async Task<IActionResult> CompleteOrder()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Намиране на стойността на идентификатора на потребителя чрез извикване на FindFirstValue() на обекта User.
            var items = _orderRepository.GetShoppingCartItemsAsync(userId);
            // Извличане на елементите от кошницата на потребителя.
            await _orderRepository.StoreOrderAsync(await items, userId);
            // Запазване на поръчката на потребителя.
            var purchasedMovies = _context.ShoppingCartItems.Where(m => m.IsPurchased == true).ToList();
            // Намиране на купените филми от таблицата ShoppingCartItems в базата данни.
            await _orderRepository.ClearShoppingCart(userId);
            // Изчистване на кошницата на потребителя.
            return View("OrderCompleted", purchasedMovies);
            // Връщане на изгледа "OrderCompleted" с модела purchasedMovies.
        }

        //
        // Обобщение:
        //     Извършва действието за изтегляне на файл
        //     
        // Параметри:
        //   id:
        //      Идентификационен ноемер на филма
        //
        // Връща:
        //     Файл за изтегляне от потребител
        [HttpPost]
        public async Task<IActionResult> Download(int id)
        {
            // Взиама филма от базата
            var movie = await _context.Movies.FirstOrDefaultAsync(m=>m.Id == id);

            // Проверка дали е null
            if (movie == null)
            {
                return NotFound();
            }

            string fileName = Path.GetFileName(movie.MovieFile);// Взема името на файла от MovieFile пропъртито на модела movie и го записва в променливата fileName.
            var imagePath = "wwwroot/MovieFiles/" + fileName;// Създава път до снимката на филма, като поставя fileName в папката "wwwroot/MovieFiles/".
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);// Прочита съдържанието на файла на снимката на филма в масив от байтове.
            return File(imageBytes, "image/jpeg", movie.MovieFile);// Връща файловете като FileStreamResult, като предава масива от байтове, типа на снимката и името на файла на филма.
        }
    }
}