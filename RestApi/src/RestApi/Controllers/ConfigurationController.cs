using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestApi.Configuration;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly IOptions<MySettings> _settings;

        public ConfigurationController(IOptions<MySettings> settings)
        {
            _settings = settings;
        }


        [HttpGet("appname")]
        public string AppName()
        {
            return _settings.Value.ApplicationName;
        }

        [HttpGet("maxlistcount")]
        public int MaxListCount()
        {
            return _settings.Value.MaxItemsPerList;
        }
    }
}