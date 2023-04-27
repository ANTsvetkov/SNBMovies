using Microsoft.EntityFrameworkCore;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels.MovieVMs;
using SNBMovies.Data.Enums;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SNBMovies.Data.Repository
{
    /// <summary>
    ///  @Author Атанас Цветков
    /// </summary>
    public class MovieRepository
    {
        public readonly ApplicationDbContext _context;
        public static List<Movie> Movies { get; set; }
        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
            Movies = _context.Movies.ToList();
        }

        #region CRUD
        public async Task AddNewMovie(CreateMovieVM model)
        {
            
            var fileName = Path.GetFileName(model.ImageURL?.FileName);
            // Създава се променлива с име "fileName", която получава името на файла, свързан със снимката на филма, ако такъв файл съществува.
            var filePath = Path.Combine("wwwroot/Content/Images/Movies/", fileName);
            // Създава се променлива с име "filePath", която представлява пълния път до файла, където ще бъде запазена снимката на филма.
            using var fileStream = new FileStream(filePath, FileMode.Create);
            //Създава се променлива "fileStream", която представлява поток към новия файл, който ще бъде създаден на посоченото място.
            model.ImageURL?.CopyTo(fileStream);
            // Копира се съдържанието на файла, свързан с изображението на филма, в потока, който сочи към новия файл, създаден на посоченото място.

            var movieFileName = Path.GetFileName(model.MovieFile?.FileName);
            var movieFilePath = Path.Combine("wwwroot/MovieFiles/", movieFileName);
            using var movieFileStream = new FileStream(movieFilePath, FileMode.Create);
            model.MovieFile?.CopyTo(movieFileStream);

            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                ImageURL = "~/Content/Images/Movies/" + fileName,
                ReleaseDate = model.ReleaseDate,
                Genre = model.Genre,
                Category = model.Category,
                ProducerId = model.ProducerId,
                MovieFile = "~/MovieFiles/" + movieFileName,
            };
            //Създава нов обект Movie и му присвоява стойности на полетата, взети от модела.

            await _context.Movies.AddAsync(movie);
            //добавя новия филм към контекста на базата данни.

            await _context.SaveChangesAsync();
            //записва промените в базата данни.

            foreach (var actorId in model.ActorIds)
            {
                var newActorMovie = new Actor_Movie()
                {//Създаване на нов обект newActorMovie от тип Actor_Movie.
                    MovieId = movie.Id,//Идентификационният номер на филм.
                    ActorId = actorId,//ID на актьора, свързан с филма.
                };
                await _context.Actor_Movies.AddAsync(newActorMovie);
                //Добавяне на новия обект newActorMovie в контекста на базата данни
            }
            await _context.SaveChangesAsync();
            //Запазване на промените в базата данни
        }

        public void RemoveMovie(int id)
        {
            Movie movie = _context.Movies.Find(id);
            //Търси филм с идентификатор, подаден като аргумент, в контекста на базата данни.
            if (movie == null) { throw new Exception("This movie is null!"); }
            //Ако филмът е null, хвърля изключение, за да сигнализира, че филмът не съществува в базата данни.

            _context.Movies.Remove(movie);
            //Премахва филмът от контекста на базата данни.
            _context.SaveChanges();
            //Записва промените в базата данни.
        }

        public async Task UpdateMovie(CreateMovieVM model)
        {
            // save the image file to the server
            var fileName = Path.GetFileName(model.ImageURL?.FileName);
            var filePath = Path.Combine("wwwroot/Content/Images/Movies/", fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            model.ImageURL?.CopyTo(fileStream);

            //Взима файла на филма
            var movieFileName = Path.GetFileName(model.MovieFile?.FileName);
            var movieFilePath = Path.Combine("wwwroot/MovieFiles/", movieFileName);
            using var movieFileStream = new FileStream(movieFilePath, FileMode.Create);
            model.MovieFile?.CopyTo(movieFileStream);

            var movie = _context.Movies.FirstOrDefault(p => p.Id == model.Id);
            // Търси филм в контекста на базата данни със зададения в модела идентификационен номер.
            if (movie != null)
            {
                // Ако филмът е намерен, се променят съответните му полета.
                movie.Title = model.Title;
                movie.Description = model.Description;
                movie.ImageURL = "~/Content/Images/Movies/" + fileName;
                movie.ReleaseDate = model.ReleaseDate;
                movie.Price = model.Price;
                movie.Genre = model.Genre;
                movie.Category = model.Category;
                movie.ProducerId = model.ProducerId;
                movie.MovieFile = "~/MovieFiles/" + movieFileName;

                
                _context.SaveChanges();
                // Запазва промените в базата данни.
            }

            var existingActorsDb = _context.Actor_Movies.Where(n => n.MovieId == model.Id).ToList();
            // Търсене на всички актьори в контекста на базата данни, които са свързани с филма.
            _context.Actor_Movies.RemoveRange(existingActorsDb);
            // Изтриване на тези актьори от базата данни.
            await _context.SaveChangesAsync();
            // Запазване на промените в базата данни.

            //Add Movie Actors
            foreach (var actorId in model.ActorIds)
            {
                var newActorMovie = new Actor_Movie()
                {
                    MovieId = model.Id,
                    ActorId = actorId
                };
                await _context.Actor_Movies.AddAsync(newActorMovie);
            }
            await _context.SaveChangesAsync();
        }
        #endregion

        //Return Movies//
        public Movie GetMovieById(int id)
        {
            var movieDetails =  _context.Movies//Присвоява на movieDetails един запис от контекста на базата данни _context.Movies.
                .Include(p => p.Producer)?//Включва в резултата на заявката свойството Producer на Movie, заедно с нейните под-обекти.
                .Include(am => am.Actor_Movie)?//Включва в резултата на заявката свойството Actor_Movie на Movie, заедно с нейните под-обекти.
                .ThenInclude(a => a.Actor)?//Включва в резултата на заявката свойството Actor на Actor_Movie, заедно с нейните под-обекти.
                .FirstOrDefault(x => x.Id == id);//Извлича първия запис от Movies, който отговаря на условието x.Id == id.
            if (movieDetails == null) { throw new Exception("This movie is null!"); }
            //Ако обекта не е открит, се хвърля грешка.
            return movieDetails;
        }
        public List<Movie> GetAllMovies()
        {
            return Movies;
        }

        public async Task<MovieDropdownsVM> GetMovieDropdowns()     
        {
            var response = new MovieDropdownsVM()
            {//Създава обект MovieDropdownsVM
                Actors = await _context.Actors.OrderBy(n => n.FullName).ToListAsync(),// задава списък на актьори, след като са сортирани по азбучен ред.
                Producers = await _context.Producers.OrderBy(n => n.FullName).ToListAsync()// задава списък на продуценти, след като са сортирани по азбучен ред.
            };
            return response;
            // Връща обекта MovieDropdownsVM със списъците на актьори и продуценти от базата данни.
        }
        //-------------//
    }
}
