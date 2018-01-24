using System;
using System.Net;
using System.Threading.Tasks;

namespace CK.Glouton.Tests
{
    public class HttpServerReceiver : IDisposable
    {
        public static string DefaultUrl => "http://localhost:4242/alert/";

        private readonly HttpListener _httpListener;

        public bool Alerted { get; set; }

        public HttpServerReceiver( string prefix )
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add( prefix );
            _httpListener.Start();
            Task.Run( Listen );
        }

        public async Task Listen()
        {
            while( true )
            {
                await _httpListener.GetContextAsync();
                Alerted = true;
            }
        }


        /// <summary>
        /// Set <see cref="Alerted"/> to <code>false</code>.
        /// </summary>
        public void Reset()
        {
            Alerted = false;
        }

        public void Dispose()
        {
            ( (IDisposable)_httpListener )?.Dispose();
        }


    }
}