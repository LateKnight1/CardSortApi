using System.Collections.Generic;
using System.Threading.Tasks;
using CardSortApi.Domain.Data;
using CardSortApi.Domain.Models;
using CardSortApi.Repositories.Interfaces;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Services
{
	public class DeckService : IDeckService
	{
		private readonly IDeckRepository _deckRepository;

		public DeckService(IDeckRepository deckRepository)
		{
			_deckRepository = deckRepository;
		}

		public async Task<RequestResponse<IEnumerable<Deck>>> GetDecksForUser(string userId)
		{
			int.TryParse(userId, out var id);
			return await _deckRepository.GetDecksForUser(id);
		}

		public async Task<RequestResponse<IEnumerable<Card>>> GetCardsForDecks(Deck[] requestDeckList)
		{
			var cards = new List<Card>();
			foreach (var deck in requestDeckList)
			{
				cards.AddRange(await _deckRepository.GetCardsForDeck(deck.ID));
			}
			return new RequestResponse<IEnumerable<Card>>
			{
				Succeeded = true,
				ResponseBody = cards
			};
		}
	}
}