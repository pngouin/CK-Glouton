using System;
using System.Collections.Generic;
using CK.Monitoring;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertModel
    {
        Func<ILogEntry, bool> Condition { get; set; }
        IList<IAlertSender> Senders { get; set; }
    }
}