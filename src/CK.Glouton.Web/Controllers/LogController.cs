using CK.Glouton.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CK.Glouton.Web.Controllers
{

    /*
     * api/log?max=[max] -- GET
     * api/log/search?query=[query] -- GET
     * api/log/search/date?from=[date]&to=[date]&monitor=[monitor]&fields=[fields]&keywords=[keywords] -- GET
     * api/log/monitor?max=[max] -- GET
     * api/log/app?max=[max] -- GET
    */

    [Route( "api/log" )]
    public class LogController : Controller
    {
        private readonly ILuceneSearcherService _luceneSearcherService;

        public LogController( ILuceneSearcherService luceneSearcherService )
        {
            _luceneSearcherService = luceneSearcherService;
        }

        [HttpGet]
        public List<ILogViewModel> GetAll( int max )
        {
            return _luceneSearcherService.GetAll( max );
        }
    }
}
