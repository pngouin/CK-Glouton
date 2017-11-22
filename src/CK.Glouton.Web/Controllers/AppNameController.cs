using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using CK.Glouton.Model;

namespace CK.Glouton.Web.Controllers
{
    /// <summary>
    /// Controller for App Name related action.
    /// </summary>
    [Route("api/[controller]")]
    public class AppNameController : Controller
    {
        private ILuceneSearcherService _luceneSearcher;

        public AppNameController(ILuceneSearcherService luceneSearcher)
        {
            _luceneSearcher = luceneSearcher;
        }

        /// <summary>
        /// Return all app name indexed with Lucene.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var appName = _luceneSearcher.GetAppNameList();
            return Ok( appName ?? new HashSet<string>());
        }
    }
}