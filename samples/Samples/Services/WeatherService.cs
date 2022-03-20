namespace Samples.Services
{
    public class WeatherService : IWeatherService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public string GetRandomSummary()
        {
            return Summaries[Random.Shared.Next(Summaries.Length)];
        }

        public string GetSummary(int i)
        {
            if (i < Summaries.Length)
                return Summaries[i];
            else
                return string.Empty;
        }
    }
}
