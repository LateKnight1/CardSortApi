using System.Collections.Generic;
using System.Threading.Tasks;
using CardSortApi.Domain.Data;
using CardSortApi.Domain.Models;

namespace CardSortApi.Services.Interfaces
{
	public interface IDeckService
	{
		Task<RequestResponse<IEnumerable<Deck>>> GetDecksForUser(string userId);
		Task<RequestResponse<IEnumerable<Card>>> GetCardsForDecks(Deck[] requestDeckList);
	}
}