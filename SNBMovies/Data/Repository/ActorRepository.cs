using Microsoft.EntityFrameworkCore;
using SNBMovies.Models.ViewModels.MovieVMs;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.ActorVMs;

namespace SNBMovies.Data.Repository
{
    /// <summary>
    ///  @Author Атанас Цветков
    /// </summary>
    public class ActorRepository
    {
        public readonly ApplicationDbContext _context;
        public static List<Actor> Actors { get; set; }
        public ActorRepository(ApplicationDbContext context)
        {
            _context = context;
            Actors = _context.Actors.ToList();
        }
        public async Task AddNewActor(CreateActorVM model)
        {
            var fileName = Path.GetFileName(model.ProfilePictureURL?.FileName);
            var filePath = Path.Combine("wwwroot/Content/Images/Actors/", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ProfilePictureURL?.CopyTo(fileStream);

            var newActor = new Actor
            {// Създава нов обект актьор със зададените данни.
                FullName = model.FullName,
                ProfilePictureURL = "~/Content/Images/Actors/" + fileName,
                Biography = model.Biography
            };
            
            await _context.Actors.AddAsync(newActor);
            // Добавя новият актьор в контекста на базата данни.
            await _context.SaveChangesAsync();
            // Записва промените в базата данни.
        }

        public void RemoveActor(int id)
        {
            Actor actor = _context.Actors.Find(id);
            // Търси актьор с идентификатор, подаден като аргумент, в контекста на базата данни.

            if (actor == null) { throw new Exception("No such actor!"); }
            // Ако актьорът не съществува, хвърля грешка.
            _context.Actors.Remove(actor);
            // Премахва актьора от контекста на базата данни.
            _context.SaveChanges();
            // Записва промените в базата данни.
        }

        public async Task UpdateActor(CreateActorVM model)
        {
            var fileName = Path.GetFileName(model.ProfilePictureURL?.FileName);
            var filePath = Path.Combine("wwwroot/Content/Images/Actors/", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ProfilePictureURL?.CopyTo(fileStream);

            var dbActor = await _context.Actors.FirstOrDefaultAsync(n => n.Id == model.Id);
            // Търси актьор с id, подаден като аргумент, в контекста на базата данни.
            if (dbActor != null)
            {// Ако актьорът е намерен:

                dbActor.FullName = model.FullName;// Променя се името.
                dbActor.ProfilePictureURL = "~/Content/Images/Actors/" + fileName;// Променя се пътя до снимката.
                dbActor.Biography = model.Biography;// Променя се биографията.
                await _context.SaveChangesAsync();
                //Промените се запазват.
            }  
        }

        public Actor GetActorById(int id)
        {
            var actorDetails = _context.Actors.Find(id);
            // Търси информация за актьор в базата данни чрез id.
            if (actorDetails == null)
            {
                throw new Exception("Actor not found!");
            }
            // Ако актьорът не съществува в базата данни, дава грешка.
            return actorDetails;
            // Връща информацията за открития актьор.
        }
        public async Task<List<Actor>> GetAllActors()
        {
            return Actors;
        }

    }
}
