using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CK.Glouton.Tests.NetFramework
{
    [TestFixture]
    public class HandlerTest
    {
        [SetUp]
        public void SetUp()
        {
            GrandOuputServerHelper.SetupServer();
            GrandOutputHandlerHelper.SetupHandler();
        }

        [TearDown]
        public void TearDown()
        {
            GrandOuputServerHelper.TearDown();
            GrandOutputHandlerHelper.TearDown();
        }
    }
}
