using System.Collections.Generic;
using System.Threading.Tasks;
using CardSortApi.Domain.Data;
using CardSortApi.Domain.Models;

namespace CardSortApi.Repositories.Interfaces
{
	public interface IDeckRepository
	{
		Task<RequestResponse<IEnumerable<Deck>>> GetDecksForUser(int result);
		Task<List<Card>> GetCardsForDeck(int deckId);
	}
}
