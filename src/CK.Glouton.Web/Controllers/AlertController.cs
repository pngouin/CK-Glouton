using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Services;
using Microsoft.AspNetCore.Mvc;

namespace CK.Glouton.Web.Controllers
{
    [Route( "api/alert" )]
    public class AlertController : Controller
    {
        private readonly IAlertService _alertService;

        public AlertController( IAlertService alertService )
        {
            _alertService = alertService;
        }

        [HttpPost( "add" )]
        public object AddAlert( [FromBody] AlertExpressionModel alertExpressionModel )
        {
            if( _alertService.NewAlertRequest( alertExpressionModel ) )
                return NoContent();
            return BadRequest();
        }

        [HttpGet( "configuration/{key}" )]
        public object GetConfiguration( string key )
        {
            if( _alertService.TryGetConfiguration( key, out var configuration ) )
                return configuration;
            return BadRequest();

        }

        [HttpGet( "configuration" )]
        public string[] GetAllConfiguration()
        {
            return _alertService.AvailableConfiguration;
        }

        [HttpGet( "all" )]
        public object GetAllAlerts()
        {
            return _alertService.GetAllAlerts();
        }
    }
}