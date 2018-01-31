using CK.AspNet;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Web.Services;
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
            var activityMonitor = HttpContext.GetRequestMonitor();
            if( _alertService.NewAlertRequest( activityMonitor, alertExpressionModel ) )
                return NoContent();
            return BadRequest();
        }

        [HttpGet( "configuration/{key}" )]
        public object GetConfiguration( string key )
        {
            var activityMonitor = HttpContext.GetRequestMonitor();
            if( _alertService.TryGetConfiguration( activityMonitor, key, out var configuration ) )
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