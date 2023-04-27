using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNBMovies.Data.Repository;
using SNBMovies.Data;
using SNBMovies.Models;
using SNBMovies.Data.Roles;
using SNBMovies.Models.ViewModels.ActorVMs;
using SNBMovies.Models.ViewModels.MovieVMs;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Това е контролер, който нарежда операциите за обработка на актьори.
    /// Съдържа методи за дисплей, създаване, промяна, изтриване и търсене на актьор.
    /// </summary>

    [Authorize(Roles = UserRoles.Admin)]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ActorRepository _actorRepository;
        public ActorController(ApplicationDbContext context, ActorRepository actorRepository)
        {
            _context = context;
            _actorRepository = actorRepository;
        }

        //
        // Обобщение:
        //     Дисплей на всички актьори в прилоожението
        //
        // Връща:
        //     Изглед с модел - списък от обект "Actor"
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _actorRepository.GetAllActors();
            //Намира всички актьори.
            return View(data);
            //Подава ги на изгледа.
        }

        //
        // Обобщение:
        //     Дисплей на форма за създаване на актьор
        //
        // Връща:
        //     Съответният изглед
        public IActionResult Create()
        {
            return View();
        }

        //
        // Обобщение:
        //     Извършва действието за създаване на актьор
        //     
        // Параметри:
        //   actor:
        //      Моделът, съдържащ информацията нужна за създаване на актьор
        //
        // Връща:
        //     Пренасочване към действие "Index"
        //     или
        //     Изглед със съответния модел
        [HttpPost]
        public async Task<IActionResult> Create(CreateActorVM actor)
        {
            if (!ModelState.IsValid)
            {//Проверка за валидността на модела.
                return View(actor);
            }
            await _actorRepository.AddNewActor(actor);
            //Добая актьора в базата данни.
            return RedirectToAction(nameof(Index));
            //Пренасочва към изглед.
        }

        //
        // Обобщение:
        //     Дисплей на информация за определен актьор
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на актьор
        //
        // Връща:
        //     Изглед с модел "Actor"
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var actorDetails = _actorRepository.GetActorById(id);
            //Намира актьора съответстващ на подаденото ID.
            if (actorDetails == null) return View("NotFound");
            //Проверка дали съществува.
            return View(actorDetails);
            //Подава се на изгледа като модел.
        }

        //
        // Обобщение:
        //     Дисплей на страница за промяна на информацията за актьор
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на актьор
        //
        // Връща:
        //     Изглед със съответния актьор
        public IActionResult Update(int id)
        {
            var actorDetails = _actorRepository.GetActorById(id);
            // Взима данните за актьора с подаденото ID.
            if (actorDetails == null) return View("NotFound");
            // Ако не е намерен актьор с такъв ID, се връща грешка "Not Found".
            var response = new CreateActorVM()
            {// Създава се обект, който съдържа информацията за актьора.
                Id = actorDetails.Id,
                FullName = actorDetails.FullName,
                Biography = actorDetails.Biography,
            };
            return View(response);
            // Връща се изгледа, за да се покажат данните за актьора.
        }

        //
        // Обобщение:
        //     Извършва действието за промяна на актьор
        //     
        // Параметри:
        //   actor:
        //      Моделът, съдържащ информацията, нужна за промяна на актьор
        //
        // Връща:
        //     Пренасочване към действието "Index"
        //     или
        //     Изглед със съответния модел
        [HttpPost]
        public async Task<IActionResult> Update(CreateActorVM actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            await _actorRepository.UpdateActor(actor);
            //Промяна на атьора.
            return RedirectToAction(nameof(Index));
            //Пренасочване към страницата.
        }

        //
        // Обобщение:
        //     Дисплей на информация на даден актьор, готов за изтриване
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на актьор
        //
        // Връща:
        //     Изглед с модел "Actor"
        public IActionResult Delete(int id)
        {
            var actorDetails = _actorRepository.GetActorById(id);
            //Намира се актьора по даденото ID.
            if (actorDetails == null) return View("NotFound");
            //Проверка за наличността.
            return View(actorDetails);
        }

        //
        // Обобщение:
        //     Извършва действието за изтриване на актьор
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на актьор
        //
        // Връща:
        //     Пренасочване към действието "Index"
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var actorDetails = _actorRepository.GetActorById(id);
            //Намиране на актьора по даденото ID.
            if (actorDetails == null) return View("NotFound");
            //Проверка за наличността му.
            _actorRepository.RemoveActor(id);
            //Премахване от базата данни.
            return RedirectToAction(nameof(Index));
            //Пренасочване към изглед.
        }

        //
        // Обобщение:
        //     Търси актьор, който съдържа съответния низ
        //     
        // Параметри:
        //   searchTerm:
        //      Низ, представляващ написаната стойност в полето за търсене.
        //
        // Връща:
        //     Съответният изглед с резултата от търсенето
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {//Проверява за празен низ.
                return View("NotFound");
            }
            var results = _context.Actors.Where(x => x.FullName.Contains(searchTerm));
            //Намира всички актьори по критерий името да съдържа низа.
            return View("Index", results);
            //Връща изгледа с резултата.
        }
    }
}
