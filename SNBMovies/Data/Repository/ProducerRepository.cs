using Microsoft.EntityFrameworkCore;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.ProducerVMs;

namespace SNBMovies.Data.Repository
{
    /// <summary>
    /// @Author Атанас Цветков
    /// </summary>
    public class ProducerRepository
    {
        public readonly ApplicationDbContext _context;
        public static List<Producer> Producers { get; set; }
        public ProducerRepository(ApplicationDbContext context)
        {
            _context = context;
            Producers = _context.Producers.ToList();
        }
        public async Task AddNewProducer(CreateProducerVM model)
        {
            var fileName = Path.GetFileName(model.ProfilePictureURL?.FileName);
            var filePath = Path.Combine("wwwroot/Content/Images/Producers/", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ProfilePictureURL?.CopyTo(fileStream);
            
            var newProducer = new Producer
            {// Дефиниране на нов обект "newProducer" от тип "Producer".
                FullName = model.FullName,// Присвояване на пълното име на модела на обекта "newProducer".
                ProfilePictureURL = "~/Content/Images/Producers/" + fileName,// Създаване на път до файла за снимка на продуцента, като се използва името на файла.
                Biography = model.Biography// Присвояване на биографията на модела на обекта "newProducer".
            };
            await _context.Producers.AddAsync(newProducer);
            // Добавяне на новия продуцент към базата данни.
            await _context.SaveChangesAsync(); 
            // Запазване на промените в базата данни.
        }

        public void RemoveProducer(int id)
        {
            Producer producer = _context.Producers.Find(id);
            // Намиране на продуцента със зададеното id в базата данни.
            if (producer == null) { throw new Exception("No such producer!"); }
            // Проверка дали съществува продуцент с такова id.

            _context.Producers.Remove(producer);
            // Изтриване на продуцента от базата данни.
            _context.SaveChanges();
            // Запазване на промените в базата данни.
        }

        public async Task UpdateProducer(CreateProducerVM model)
        {
            var dbProducer = await _context.Actors.FirstOrDefaultAsync(n => n.Id == model.Id);
            // Намиране на продуцента в базата данни по зададено Id.

            var fileName = Path.GetFileName(model.ProfilePictureURL?.FileName);
            var filePath = Path.Combine("wwwroot/Content/Images/Producers/", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ProfilePictureURL?.CopyTo(fileStream);
                
            if (dbProducer != null)
            {// Проверка дали съществува продуцент с това Id в базата данни.

                dbProducer.FullName = model.FullName;// Присвояване на новото пълно име на продуцента.
                dbProducer.ProfilePictureURL = "~/Content/Images/Producers/" + fileName;// Присвояване на новия път на снимка на продуцента.
                dbProducer.Biography = model.Biography;// Присвояване на новата биография на продуцента.

                await _context.SaveChangesAsync();
                // Запазване на промените в базата данни.
            }
        }

        public Producer GetProducerById(int id) 
        {
            var producerDetails = _context.Producers.Find(id);
            if (producerDetails == null)
            {
                throw new Exception("Actor not found!");
            }

            return producerDetails;
        }
        public List<Producer> GetAllProducers()
        {
            return Producers;
        }
    }
}
