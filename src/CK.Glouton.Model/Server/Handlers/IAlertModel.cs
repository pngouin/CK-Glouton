using CK.Glouton.Model.Server.Sender;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertModel
    {
        Func<AlertEntry, bool> Condition { get; set; }
        IList<IAlertSender> Senders { get; set; }
    }
}