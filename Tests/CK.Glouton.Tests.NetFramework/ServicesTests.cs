using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using CK.Glouton.Service;
using FluentAssertions;
using Lucene.Net.Index;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace CK.Glouton.Tests.NetFramework
{
    [TestFixture]
    public class ServicesTests

    {

#if NET461
        [TestFixtureSetUp]
#else
        [OneTimeSetUp]
#endif
        public void constructIndex()
        {
            LuceneTestIndexBuilder.ConstructIndex();
        }

        [Test]
        public void log_can_be_search()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine(TestHelper.GetTestLogDirectory(), "Lucene"),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };

            var searcherService = new LuceneSearcherService(luceneConfiguration);
        }
    }
}
