namespace CardSortApi.Domain.Data
{
	using System.Data.Entity;

	public partial class DecksContext : DbContext
	{
		public DecksContext()
			: base("name=Decks")
		{
		}

		public virtual DbSet<Deck> Decks { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Deck>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<Deck>()
				.Property(e => e.type)
				.IsUnicode(false);
		}
	}
}
