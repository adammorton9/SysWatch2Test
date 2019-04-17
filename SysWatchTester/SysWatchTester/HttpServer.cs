using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace SysWatchTester
{
    public class HttpServer : IDisposable
    {
        private const string Host = "http://localhost";
        private const string Endpoint = "test";
        private int Port { get; }
        public string Url { get; }
        private HttpListener HttpListener { get; }
        public HttpServer(int port)
        {
            Port = port;
            Url = $"{Host}:{Port}/{Endpoint}/";
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(Url);
        }

        public void Start()
        {
            HttpListener.Start();
        }

        private void Stop()
        {
            HttpListener.Stop();
        }

        public void Dispose()
        {
            Stop();
        }

        public bool IsRunning()
        {
            return HttpListener.IsListening;
        }

        public void HandleRequests()
        {
            HttpListenerContext context = HttpListener.GetContext();
            HttpListenerRequest request = context.Request;

            Request requestObj = new Request();

            if (request.HasEntityBody)
            {
                // If a POST, stream the request body.
                string requestRaw;
                using (var reader = new StreamReader(request.InputStream,
                    request.ContentEncoding))
                {
                    requestRaw = reader.ReadToEnd();
                }

                requestObj = JsonConvert.DeserializeObject<Request>(requestRaw);
            }
            else
            {
                // If a GET, read the query params.
                var parameters = request.QueryString;
                foreach (var parameter in parameters.AllKeys)
                {
                    requestObj.Parameters.Add($"{parameter}-{parameters[parameter]}");
                }
            }
            string responseString = JsonConvert.SerializeObject(requestObj);

            Console.WriteLine($"Request Received: {responseString}");

            SendResponse(context, responseString);
        }

        private void SendResponse(HttpListenerContext context, string responseString)
        {
            using (HttpListenerResponse response = context.Response)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (var output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
