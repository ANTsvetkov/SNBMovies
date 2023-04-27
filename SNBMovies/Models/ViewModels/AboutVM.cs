namespace SNBMovies.Models.ViewModels
{
    public class AboutVM
    {
        public List<Movie> MoviesCount { get; set; }
        public List<OrderHistory> OrdersCount { get; set; }
        public List<ApplicationUser> UsersCount { get; set; }
    }
}
