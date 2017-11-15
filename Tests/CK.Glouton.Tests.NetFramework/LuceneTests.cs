using CK.Core;
using NUnit.Framework;

namespace CK.Glouton.Tests.NetFramework
{
    [TestFixture]
    public class LuceneTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
        }

        [Test]
        public void log_can_be_indexed()
        {
            var m = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
            GrandOutputHelper.GrandOutputClient.EnsureGrandOutputClient( m );

            using( var server = TestHelper.DefaultServer() )
            {
                server.Open();

                m.Info( "Hello world" );
                m.Error( "CriticalError" );
            }
        }
    }
}
