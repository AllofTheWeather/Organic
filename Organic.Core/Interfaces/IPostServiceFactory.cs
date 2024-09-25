namespace Organic.Core.Interfaces
{
	public interface IPostServiceFactory
	{
		ISocialMediaPostService GetPostService(string platformName);
	}
}
