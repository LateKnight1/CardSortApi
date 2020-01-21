using CardSortApi.Domain.Data;

namespace CardSortApi.Domain.Dto
{
	public class CardsRequest
	{
		public Deck[] DeckList { get; set; }
	}
}