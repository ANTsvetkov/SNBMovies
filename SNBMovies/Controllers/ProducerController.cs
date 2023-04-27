using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNBMovies.Data;
using SNBMovies.Data.Repository;
using SNBMovies.Data.Roles;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.ActorVMs;
using SNBMovies.Models.ViewModels.ProducerVMs;
using System.Data;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Това е контролер, който нарежда операциите за обработка на продуценти.
    /// Съдържа методи за дисплей, създаване, промяна, изтриване и търсене на продуценти.
    /// </summary>

    [Authorize(Roles = UserRoles.Admin)]
    public class ProducerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProducerRepository _producerRepository;
        public ProducerController(ApplicationDbContext context, ProducerRepository producerRepository)
        {
            _context = context;
            _producerRepository = producerRepository;
        }

        //
        // Обобщение:
        //     Дисплей на всички продуценти в приложението
        //
        // Връща:
        //     Изглед с модел - списък от обект "Producer"
        [AllowAnonymous]
        public IActionResult Index()
        {
            var producers = _producerRepository.GetAllProducers();
            //Взима всички продуценти от базата.
            return View(producers);
            //Връща резултата.
        }

        //
        // Обобщение:
        //     Дисплей на формата за създаване на продуцент
        //
        // Връща:
        //     Съответният изглед
        public IActionResult Create()
        {
            return View();
        }

        //
        // Обобщение:
        //     Извършва действието за създаване на продуцент
        //     
        // Параметри:
        //   producer:
        //      Моделът, съдържащ информацията за създаването на продуцент
        //
        // Връща:
        //     Изглед с модела "CreateProducerVM"
        //     или
        //     Пренасочване към действие "Index"

        [HttpPost]
        public async Task<IActionResult> Create(CreateProducerVM producer)
        {
            if (!ModelState.IsValid)
            {
                return View(producer);
            }
            await _producerRepository.AddNewProducer(producer);
            return RedirectToAction(nameof(Index));
        }

        //
        // Обобщение:
        //     Дисплей на информация за продуцент
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на продуцент
        //
        // Връща:
        //     Изглед с модел "Producer"
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var producerDetails = _producerRepository.GetProducerById(id);

            if (producerDetails == null) return View("NotFound");
            return View(producerDetails);
        }

        //
        // Обобщение:
        //     Дисплей на страница за проманя на инфромацията за актьор
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер
        ///
        // Връща:
        //     Изглед с модел "CreateProducerVM"
        public IActionResult Update(int id)
        {
            var producerDetails = _producerRepository.GetProducerById(id);
            if (producerDetails == null) return View("NotFound");
            var response = new CreateProducerVM()
            {
                Id = producerDetails.Id,
                FullName = producerDetails.FullName,
                Biography = producerDetails.Biography,
            };
            return View(response);
        }

        //
        // Обобщение:
        //     Извършва действието за промяна на продуцент
        //     
        // Параметри:
        //   producer:
        //      Моделът, съдържащ необходимата информация
        ///
        // Връща:
        //     Изглед с модела
        //     или
        //     Пренасочване към действие "Index"
        [HttpPost]
        public async Task<IActionResult> Update(CreateProducerVM producer)
        {
            if (!ModelState.IsValid)
            {
                return View(producer);
            }
            await _producerRepository.UpdateProducer(producer);
            return RedirectToAction(nameof(Index));
        }

        //
        // Обобщение:
        //     Дисплей на страница с инфромация на актьор, готов за изтриване
        //     
        // Параметри:
        //   id:
        //      Иденнтификационен номер на актьор
        ///
        // Връща:
        //     Изглед с модел "Producer"
        public IActionResult Delete(int id)
        {
            var producerDetails = _producerRepository.GetProducerById(id);
            if (producerDetails == null) return View("NotFound");
            return View(producerDetails);
        }

        //
        // Обобщение:
        //     Извършва действието за изтриване на продуцент
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер
        ///
        // Връща:
        //     Пренасочване към действие "Index"
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var producerDetails = _producerRepository.GetProducerById(id);
            if (producerDetails == null) return View("NotFound");

            _producerRepository.RemoveProducer(id); 
            return RedirectToAction(nameof(Index));
        }

        //
        // Обобщение:
        //     Търси продуцент, който съдържа съответния низ
        //     
        // Параметри:
        //   searchTerm:
        //      Низ, представляващ написаната стойност в полето за търсене.
        ///
        // Връща:
        //     Съответният изглед с резултата от търсенето
        [AllowAnonymous]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View("NotFound");
            }
            var results = _context.Producers.Where(x => x.FullName.Contains(searchTerm));
            return View("Index", results);
        }
    }
}
