using CK.Glouton.Model.Server.Handlers;

namespace CK.Glouton.Service.Common
{
    public class AlertSenderParser
    {
        /// <summary>
        /// Retrieve and instantiate an alert sender.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IAlertSender Parse( string key )
        {
            // TODO: Create and/or manage senders
            /*
             * Possible workflow ( need to add a save mechanism and a configuration validation )
             *
             *  Parse the key
             *  If unknown
             *      Throw
             *  Else
             *      Retrieve key configuration
             *      Check if configuration associated sender was already used (and saved)
             *      If true
             *          Return that same sender
             *      Else
             *          Create a new sender a save it
             */

            return null;

            // Deprecated code:
            //switch( key )
            //{
            //    case "Mail":
            //        return new MailSender( new MailConfiguration() );

            //    default:
            //        throw new ArgumentException( nameof( key ) );
            //}
        }
    }
}