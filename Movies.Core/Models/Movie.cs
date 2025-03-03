namespace Movies.Core.Models
{
    public class Movie : BaseEntity
    {
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public int Year { get; set; }

        public double Rate { get; set; }

        public string StoreLine { get; set; } = string.Empty;

        public byte[] Poster { get; set; } = default!;

        public int GeneraId { get; set; }

        public Genera Genera { get; set; } = default!;
    }
}
