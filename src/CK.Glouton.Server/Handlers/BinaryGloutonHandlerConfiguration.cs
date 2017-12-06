
using CK.Glouton.Model.Server;

namespace CK.Glouton.Server.Handlers
{
    public class BinaryGloutonHandlerConfiguration : FileConfigurationBase
    {
        public bool UseGzipCompression { get; set; }

        public override IGloutonHandlerConfiguration Clone()
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