namespace CardSortApi.Repositories.Models
{
	public class AuthResponse
	{
		public string Jwt { get; set; }
		public User User { get; set; }
	}
}