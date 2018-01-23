using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
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
            if( _alertService.SendNewAlert( alertExpressionModel ) )
                return Ok();
            return BadRequest();
        }

        [HttpGet( "configuration/mail" )]
        public IMailConfiguration GetMailConfiguration()
        {
            return _alertService.GetMailConfiguration();
        }

        [HttpGet( "configuration/http" )]
        public IHttpConfiguration GetHttpConfiguration()
        {
            return _alertService.GetHttpConfiguration();
        }

        [HttpGet( "configuration" )]
        public string[] GetAllConfiguration()
        {
            return _alertService.AvailableConfiguration;
        }
    }
}