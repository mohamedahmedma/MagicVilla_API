using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI
{
    public class Startup
    {
        public IConfiguration _Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JWT>(_Configuration.GetSection("ApiSettings"));
        }
    }
}
