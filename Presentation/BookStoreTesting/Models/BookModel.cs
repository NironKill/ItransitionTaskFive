namespace BookStoreTesting.Models
{
    public class BookModel
    {
        public string Language { get; set; }
        public int Seed { get; set; }
        public double Likes { get; set; }
        public double Reviews { get; set; }
        public int NumberOfBooks { get; set; }
        public int LastRowId { get; set; } = 0;
    }
}
