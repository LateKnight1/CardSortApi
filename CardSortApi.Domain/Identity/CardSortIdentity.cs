using System.Security.Principal;

namespace CardSortApi.Domain.Identity
{
	public class CardSortIdentity : IIdentity
	{
		public CardSortIdentity(string name, string authenticationType, bool isAuthenticated, string userId, string username)
		{
			Name = name;
			AuthenticationType = authenticationType;
			IsAuthenticated = isAuthenticated;
			UserId = userId;
			Username = username;
		}

		public string Name { get; }
		public string AuthenticationType { get; }
		public bool IsAuthenticated { get; }
		public string UserId { get; }
		public string Username { get; }
	}
}