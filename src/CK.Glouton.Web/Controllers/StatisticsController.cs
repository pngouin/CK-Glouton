using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Glouton.Lucene;
using Microsoft.AspNetCore.Mvc;

namespace CK.Glouton.Web.Controllers
{
    [Route("api/stats")]
    public class StatisticsController : Controller
    {
        LuceneStatistics luceneStatistics;
        public StatisticsController()
        {

        }

    }
}