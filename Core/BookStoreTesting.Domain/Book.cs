namespace BookStoreTesting.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public List<Author> Authors { get; set; }
        public string Publisher { get; set; }
        public DateTime PublishedIn { get; set; }
        public int Likes { get; set; }
        public List<Review> Reviews { get; set; }
        public string ImageUrl { get; set; }
    }
}
