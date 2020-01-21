using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CardSortApi.Domain.Data;
using CardSortApi.Domain.Dto;
using CardSortApi.Domain.Models;
using CardSortApi.Services.Interfaces;

namespace CardSortApi.Controllers
{
	public class DecksController : BaseApiController
	{
		private readonly IDeckService _deckService;

		public DecksController(IDeckService deckService)
		{
			_deckService = deckService;
		}

		[HttpGet]
		[Authorize]
		[ActionName("decks")]
		[ResponseType(typeof(IEnumerable<Deck>))]
		public async Task<IHttpActionResult> GetDecks()
		{
			try
			{
				var response = await _deckService.GetDecksForUser(CurrentIdentity.UserId);

				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}

		[HttpPost]
		[Authorize]
		[ActionName("cards")]
		[ResponseType(typeof(IEnumerable<Card>))]
		public async Task<IHttpActionResult> GetCards([FromBody] CardsRequest request)
		{
			try
			{
				var response = await _deckService.GetCardsForDecks(request.DeckList);

				if (response.Succeeded)
				{
					return Ok(response.ResponseBody);
				}

				return BadRequest(response.ErrorMessage);
			}
			catch (Exception e)
			{
				return InternalServerError(e);
			}
		}
	}
}