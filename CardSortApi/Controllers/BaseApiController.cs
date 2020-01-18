using System.Web.Http;
using CardSortApi.Domain.Identity;

namespace CardSortApi.Controllers
{
	public class BaseApiController : ApiController
	{
		protected CardSortIdentity CurrentIdentity => User?.Identity as CardSortIdentity;
	}
}