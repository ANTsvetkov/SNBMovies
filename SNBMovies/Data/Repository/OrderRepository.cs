using Microsoft.EntityFrameworkCore;
using SNBMovies.Migrations;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.ShoppingCart;

namespace SNBMovies.Data.Repository
{
    /// <summary>
    /// @Author Атанас Цветков
    /// </summary>
    public class OrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShoppingCartItem>> GetShoppingCartItemsAsync(string userId)
        {
            var responce = await _context.ShoppingCartItems// Намиране на всички обекти "ShoppingCartItem" в базата данни.
                .Include(x => x.Movie)//Включва се в резултата.
                .Include(x => x.User)//Включва се в резултата.
                .Where(x => x.UserId == userId && x.IsPurchased == false)// - имат UserId, съвпадащ с подадения параметър "userId" и не са закупени
                .ToListAsync();
            return responce;
            // Връщане на резултата от метода - списък от обекти "ShoppingCartItem".
        }
        public async Task<List<OrderHistory>> GetOrderHistoryAsync(string userId)
        {
            return await _context.OrderHistories
                .Include(x => x.User)//Включва се в резултата.
                .Include(x => x.Movie)//Включва се в резултата.
                .Where(x => x.UserId == userId && x.IsPurchased == true)
                .ToListAsync();
        }
        public async Task<List<OrderHistory>> GetAllOrderHistoryAsync()
        {
            return await _context.OrderHistories
                .Include(x => x.User)
                .Include(x => x.Movie)
                .Where(x => x.IsPurchased == true)
                .ToListAsync();
        }

        public async Task AddToCartAsync(string userId, int movieId)
        {
            var cartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(c => c.Movie.Id == movieId && c.UserId == userId);
            // Инициализация на променлива cartItem, която намира първия запис от модела ShoppingCartItem в базата данни, който има Movie.Id, равно на подадения movieId и UserId, равно на подадения userId.
            var movie = await _context.Movies.FirstOrDefaultAsync(m=>m.Id == movieId);
            // Инициализация на променлива movie, която се опитва да намери първия запис от модела Movies в базата данни, който има Id, равно на подадения movieId.

            if (cartItem == null)
            {// Ако променливата cartItem е null, създава се нов обект ShoppingCartItem и го попълваме с информация.
                cartItem = new ShoppingCartItem
                {
                    UserId = userId,
                    Movie = movie,
                    Price = movie.Price,
                    IsPurchased = false
                };
                await _context.ShoppingCartItems.AddAsync(cartItem);
                //Добавя се новия обект в базата данни.
            }

            await _context.SaveChangesAsync();
            // Запазване на промените в базата данни.
        }

        public async Task RemoveFromCartAsync(int id)
        {
            var shoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x=>x.Movie.Id == id);
            // Инициализация на променлива shoppingCartItem, която намира първия запис в базата данни, който има Movie.Id, равно на подаденото id.
            if (shoppingCartItem != null)
            { // Ако shoppingCartItem не е null.
                _context.ShoppingCartItems.Remove(shoppingCartItem);
                // Изтрива се този запис от базата данни.
                await _context.SaveChangesAsync();
                // Запазват се промените в базата данни.
            }
        }
        public async Task ClearShoppingCart(string userId)
        {
            var items = await _context.ShoppingCartItems.Where(n=>n.UserId == userId).ToListAsync();
            // Инициализация на променлива items, която извлича всички записи от базата данни, които имат UserId, равно на подадения userId.
            _context.ShoppingCartItems.RemoveRange(items);
            // Изтриват се записите в базата данни с помощта на метода RemoveRange(), който приема като аргумент колекция от обекти, които трябва да бъдат изтрити.
            await _context.SaveChangesAsync();
            // Запазване на промените в базата данни.
        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> cartItems, string userId)
        {
            foreach (var movie in cartItems)
            {// За всяка една променлива movie от колекцията cartItems:
                movie.IsPurchased = true;
                // Поставя се стойност true на свойството IsPurchased на текущия филм в колекцията.
                var orderhistory = new OrderHistory
                { // Инициализация на нов обект от модела OrderHistory и попълване на полетата му с информация.
                    Price = movie.Price,
                    Movie = movie.Movie,
                    UserId = userId,
                    IsPurchased = movie.IsPurchased,
                    PurchaseDate = DateTime.Now,
                    OrderId = Guid.NewGuid().ToString()
                };

                await _context.OrderHistories.AddAsync(orderhistory);
                // Добавя се новия обект в базата данни.
                await _context.SaveChangesAsync();
                // Запазване на промените в базата данни.
            }
        }
    }
}
