using CK.Glouton.Model.Server;

namespace CK.Glouton.Server.Handlers
{
    public class LuceneGloutonHandlerConfiguration : IGloutonHandlerConfiguration
    {
        public IGloutonHandlerConfiguration Clone()
        {
            return new LuceneGloutonHandlerConfiguration();
        }
    }
}