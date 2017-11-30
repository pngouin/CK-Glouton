using CK.Glouton.Model.Server;
using CK.Monitoring;
using CK.Monitoring.Handlers;

namespace CK.Glouton.Server.Handlers
{
    public class BinaryGloutonHandlerConfiguration : FileConfigurationBase, IGloutonHandlerConfiguration
    {
        public bool UseGzipCompression { get; set; }

        public override IHandlerConfiguration Clone()
        {
            return CloneThis();
        }

        IGloutonHandlerConfiguration IGloutonHandlerConfiguration.Clone()
        {
            return CloneThis();
        }

        private BinaryGloutonHandlerConfiguration CloneThis()
        {
            return new BinaryGloutonHandlerConfiguration
            {
                Path = Path,
                MaxCountPerFile = MaxCountPerFile,
                UseGzipCompression = UseGzipCompression
            };
        }
    }
}