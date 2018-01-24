using System;
using System.Collections.Generic;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertModel
    {
        Func<AlertEntry, bool> Condition { get; set; }
        IList<IAlertSender> Senders { get; set; }
    }
}