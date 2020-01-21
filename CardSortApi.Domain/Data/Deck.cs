namespace CardSortApi.Domain.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("cardsort_userdb.Decks")]
    public partial class Deck
    {
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string type { get; set; }

        [Required]
        public int userId { get; set; }
    }
}
