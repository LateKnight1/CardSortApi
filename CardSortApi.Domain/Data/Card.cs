namespace CardSortApi.Domain.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cardsort_userdb.Cards")]
    public partial class Card
    {
        public int ID { get; set; }

        public int DeckId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [StringLength(100)]
        public string DeckName { get; set; }

        [Required]
        [StringLength(100)]
        public string Colors { get; set; }

        [Required]
        [StringLength(100)]
        public string Rarity { get; set; }

        [Required]
        [StringLength(25)]
        public string Costs { get; set; }
    }
}
