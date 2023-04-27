namespace SNBMovies.Models.ViewModels.MovieVMs
{
    public class MovieDropdownsVM
    {
        public MovieDropdownsVM()
        {
            Producers = new List<Producer>();
            Actors = new List<Actor>();
        }

        public List<Producer> Producers { get; set; }
        public List<Actor> Actors { get; set; }
    }
}
