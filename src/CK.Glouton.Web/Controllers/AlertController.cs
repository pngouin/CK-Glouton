using CK.Glouton.Model.Server.Handlers;
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
        public object AddAlert( [FromBody] IAlertExpressionModel alertExpressionModel )
        {
            if( _alertService.AddAlert( alertExpressionModel ) )
                return Ok();
            return BadRequest();
        }
    }
}