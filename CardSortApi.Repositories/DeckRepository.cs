using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CardSortApi.Domain.Data;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories.Interfaces;

namespace CardSortApi.Repositories
{
	public class DeckRepository : IDeckRepository
	{
		public Task<RequestResponse<IEnumerable<Deck>>> GetDecksForUser(int id)
		{
			using (var context = new DecksContext())
			{
				var decks =  context.Decks.Where(x => x.userId == id);
				var results = new RequestResponse<IEnumerable<Deck>>
				{
					Succeeded = true,
					ResponseBody = decks.ToArray()
				};

				return Task.FromResult(results);
			}
		}

		public async Task<List<Card>> GetCardsForDeck(int deckId)
		{
			using (var context = new CardsContext())
			{
				var cards = await context.Cards.Where(x => x.DeckId == deckId).ToListAsync();
				return cards;
			}
		}
	}
}