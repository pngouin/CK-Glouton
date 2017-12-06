using System.Collections.Generic;

namespace CK.Glouton.Model.Server
{
    public interface IHandlersManagerConfiguration
    {
        List<IGloutonHandlerConfiguration> GloutonHandlers { get; }

        IHandlersManagerConfiguration AddGloutonHandler( IGloutonHandlerConfiguration configuration );
        IHandlersManagerConfiguration Clone();
    }
}