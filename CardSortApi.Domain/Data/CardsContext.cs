using System.Data.Entity;

namespace CardSortApi.Domain.Data
{
	public partial class CardsContext :  DbContext
	{
		public CardsContext()
			: base("name=Cards")
		{ }

		public virtual DbSet<Card> Cards { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Card>()
				.Property(e => e.Name)
				.IsUnicode(false);
			modelBuilder.Entity<Card>()
				.Property(e => e.Type)
				.IsUnicode(false);
			modelBuilder.Entity<Card>()
				.Property(e => e.Colors)
				.IsUnicode(false);
			modelBuilder.Entity<Card>()
				.Property(e => e.Costs)
				.IsUnicode(false);
			modelBuilder.Entity<Card>()
				.Property(e => e.DeckName)
				.IsUnicode(false);
			modelBuilder.Entity<Card>()
				.Property(e => e.Rarity)
				.IsUnicode(false);

		}
	}
}