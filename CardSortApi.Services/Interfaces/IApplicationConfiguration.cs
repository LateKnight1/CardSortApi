namespace CardSortApi.Services.Interfaces
{
	public interface IApplicationConfiguration
	{
		string Issuer();
		byte[] Secret();
	}
}
