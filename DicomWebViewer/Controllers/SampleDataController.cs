using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dicom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DicomWebViewer.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        ILogger _logger;

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public SampleDataController(ILogger<SampleDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            try
            {
                _logger.LogInformation("Get request for WeatherForecasts");

                DicomFile df = DicomFile.Open("test.dcm");
                string bp = df.Dataset.GetSingleValueOrDefault<string>(new DicomTag(0x18, 0x15), "default");
                _logger.LogInformation("Read body part from test.dcm {0}", bp);

                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                });
            }
            catch (Exception e)
            {
                _logger.LogError("Get error when call WeatherForecasts due to {0}", e.Message);
                _logger.LogError(e.StackTrace);

                throw;
            }
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
