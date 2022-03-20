namespace Samples.Services
{
    public interface IWeatherService
    {
        string GetRandomSummary();
        string GetSummary(int i);
    }
}