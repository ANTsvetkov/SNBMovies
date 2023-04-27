using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SNBMovies.Data;
using SNBMovies.Data.Repository;
using SNBMovies.Data.Roles;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.MovieVMs;
using System.IO.Compression;
using System.Security.Claims;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Контролер отговорен за действията за обработка на филми.
    /// Съдържа методи за дисплей, създаване, промяняна, изтриване и търсене на филми.
    /// </summary>
    /// 
    [Authorize(Roles = UserRoles.Admin)]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieRepository _movieRepository;
        public MovieController(ApplicationDbContext context, MovieRepository movieRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
        }

        //
        // Обобщение:
        //     Дисплей на всички филми в приложението
        //
        // Връща:
        //     Изглед с модел - списък от обекта "Movie"
        [AllowAnonymous]
        public IActionResult Index()
        {
            var movies = _movieRepository.GetAllMovies();
            //Намират се всички филми.
            return View(movies);
            //Списъкът се подава като модел на изгледа.
        }

        //
        // Обобщение:
        //     Дисплей на повече информация за определен филм
        //     
        // Параметри:
        //   id:
        //      Идентификоционен номер на филм
        //
        // Връща:
        //     Изглед с обект - съответния филм ("Movie")
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var movieDetail = _movieRepository.GetMovieById(id);
            //Намира филма с дадения ID.
            return View(movieDetail);
            //Подава го на изгледа.
        }

        //
        // Обобщение:
        //     Дисплей на всички филми вмприложението
        //
        // Връща:
        //     Изглед с модел - списък от обекта "Movie"
        [AllowAnonymous]
        public IActionResult Genre()    
        {
            var genres = _movieRepository.GetAllMovies();
            return View(genres);
        }


        #region CRUD Operations
        //
        // Обобщение:
        //     Дисплей на страница за създаване на филм. Създава падащи менюта за продуценти и актьори
        //
        // Връща:
        //     Изглед за извършване на съответната операция
        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _movieRepository.GetMovieDropdowns();
            //Извиква се методът GetMovieDropdowns от _movieRepository, който връща данни за dropdown полетата на формата за добавяне на филм.
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            //Създава се SelectList за dropdown полето на продуцентите, като се подават списък с данните за продуцентите, "Id" и "FullName" като стойности за ID и за показване на име на dropdown полето.
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            //Създава се SelectList за dropdown полето на актьорите, като се подават списък с данните за актьорите, "Id" и "FullName" като стойности за ID и за показване на име на dropdown полето.
            return View();
        }

        //
        // Обобщение:
        //     Извършва действието за създаване на филм
        //     
        // Параметри:
        //   model:
        //      Моделът, с нужната информация за добавяне на филм
        //
        // Връща:
        //     Пренасочване към действието "Index"
        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieVM model)
        {
            if (!ModelState.IsValid)
            {// Проверка дали моделът е валиден.
                
                var movieDropdownsData = await _movieRepository.GetMovieDropdowns();
                // Извличане на данни за dropdowns.
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                // Изпращане на данните за Producer като SelectList към View-a.
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
                // Изпращане на данните за Actor като SelectList към View-a.

                return View(model);
                // Връщане на View с подадения модел, за да се покажат грешките валидацията и dropdown-ите.
            }

            await _movieRepository.AddNewMovie(model);
            // Създаване на нов филм в репозиторито
            return RedirectToAction(nameof(Index));
            // Пренасочване към началната страница за филми
        }

        //
        // Обобщение:
        //     Дисплей на страница за промяна на информация на филм
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на филм
        //
        // Връща:
        //     Изглед с данните от модел "CreateMovieVM"
        public async Task<IActionResult> Update(int id)
        {
            var movieDetails = _movieRepository.GetMovieById(id);
            // Взимаме данните на филма по зададен id.
            if (movieDetails == null) return View("NotFound");
            // Проверяваме дали филмът не е null.

            var response = new CreateMovieVM()
            {// Създаваме нов обект от тип CreateMovieVM, който ще попълним с данните на филма.
                Id = movieDetails.Id,
                Title = movieDetails.Title,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                ReleaseDate = movieDetails.ReleaseDate,
                Genre = movieDetails.Genre,
                Category = movieDetails.Category,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actor_Movie.Select(n => n.ActorId).ToList(),
            };

            var movieDropdownsData = await _movieRepository.GetMovieDropdowns();
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
            // Взимаме данни от репозиторито за "Производител" и "Актьори" на филма и ги добавяме към ViewBag.
            return View(response);
            // Връщаме изгледа с попълнените данни на филма и dropdown списъците.
        }

        //
        // Обобщение:
        //     Извършва операцията за промяна на информацията за филм
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на филм
        //   model:
        //      Моделът с информацията въведена във формата
        //
        // Връща:
        //     Изглед с модел - CreateMovieVM
        //     или
        //     Пренасочване към действие "Index"
        [HttpPost]
        public async Task<IActionResult> Update(int id, CreateMovieVM model)
        {
            if (id != model.Id) return View("NotFound");
            // Проверка дали id-то на модела съвпада с id - то на филма.

            if (!ModelState.IsValid)
            {// Проверка за невалиден ModelState.
                var movieDropdownsData = await _movieRepository.GetMovieDropdowns();
                // Извличане на данни за dropdowns за филма.
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");
                // Задаване на данните за dropdowns към ViewBag.
                return View(model);
                // Връщане на текущия View с модела.
            }

            await _movieRepository.UpdateMovie(model);
            // Актуализиране на филма в базата данни.
            return RedirectToAction(nameof(Index));
            // пренасочване към изглед Index.
        }

        //
        // Обобщение:
        //     Дисплей на определена информация за филм, готов за изтриване
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на филм
        //
        // Връща:
        //     Изглед с обект "Movie"
        public IActionResult Delete(int id)
        {
            var actorDetails = _movieRepository.GetMovieById(id);
            //Намира филма с конкретното ID.
            if (actorDetails == null) return View("NotFound");
            //Проверка дали филма съществува.
            return View(actorDetails);
        }

        //
        // Обобщение:
        //     Извършва действието за изтриване на филм
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на филм
        //
        // Връща:
        //     Пренасочване към действие "Index"
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var actorDetails = _movieRepository.GetMovieById(id);
            //Намира филма с даденото ID.
            if (actorDetails == null) return View("NotFound");
            //Проверка дали филма съществува.
            _movieRepository.RemoveMovie(id);
            //Премахване на филма.
            return RedirectToAction(nameof(Index));
            //Пренасочване към Index изглед.
        }
        #endregion

        //
        // Обобщение:
        //     Търси филм, който съдържа съответния низ
        //     
        // Параметри:
        //   searchTerm:
        //      Низ, представляващ написаната стойност в полето за търсене.
        //
        // Връща:
        //     Съответният изглед с резултата от търсенето
        [AllowAnonymous]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {//Проверка дали параметъра е null.
                return View("NotFound");
            }
            var results = _context.Movies.Where(x => x.Title.Contains(searchTerm));
            //Намиране на резултатите от търсенето.
            return View("Index", results);
            //Изглед с резултата.
        }

    }

}
