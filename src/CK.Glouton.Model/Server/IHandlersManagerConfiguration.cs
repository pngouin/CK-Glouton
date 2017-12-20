using System;
using System.Collections.Generic;

namespace CK.Glouton.Model.Server
{
    public interface IHandlersManagerConfiguration
    {
        List<IGloutonHandlerConfiguration> GloutonHandlers { get; }
        TimeSpan TimerDuration { get; set; }
        IHandlersManagerConfiguration AddGloutonHandler( IGloutonHandlerConfiguration configuration );
        IHandlersManagerConfiguration Clone();
    }
}