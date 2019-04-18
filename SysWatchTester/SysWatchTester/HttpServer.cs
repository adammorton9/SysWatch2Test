﻿using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace SysWatchTester
{
    public class HttpServer : IDisposable
    {
        private readonly ManualResetEvent _stop, _idle;
        private readonly Semaphore _busy;
        private readonly int _maxThreads = Environment.ProcessorCount;
        private const string Host = "http://*";
        private const string Endpoint = "test";

        private int Port { get; }
        public string Url { get; }
        private HttpListener HttpListener { get; }
        private Thread ListenerThread { get; set; }
        public bool IsRunning { get; set; }

        public HttpServer(int port)
        {
            Port = port;
            Url = $"{Host}:{Port}/{Endpoint}/";
            HttpListener = new HttpListener();
            HttpListener.Prefixes.Add(Url);
            ListenerThread = new Thread(HandleRequests);
            _busy = new Semaphore(_maxThreads, _maxThreads);
            _stop = new ManualResetEvent(false);
            _idle = new ManualResetEvent(false);
        }

        public void Start()
        {
            HttpListener.Start();
            ListenerThread.Start();
            IsRunning = true;
        }

        private void Stop()
        {
            _stop.Set();
            ListenerThread.Join();
            _idle.Reset();

            //aquire and release the semaphore to see if anyone is running, wait for idle if they are.
            _busy.WaitOne();
            if (_maxThreads != 1 + _busy.Release())
                _idle.WaitOne();
            HttpListener.Stop();
            IsRunning = false;
        }

        public void Dispose()
        {
            Stop();
        }

        public void HandleRequests()
        {
            while (HttpListener.IsListening)
            {
                var context = HttpListener.BeginGetContext(ListenerCallback, null);

                if (0 == WaitHandle.WaitAny(new[] {_stop, context.AsyncWaitHandle}))
                    return;
            }
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            _busy.WaitOne();
            try
            {
                HttpListenerContext context;
                try
                { context = HttpListener.EndGetContext(ar); }
                catch (HttpListenerException)
                { return; }

                if (_stop.WaitOne(0, false))
                    return;

                Console.WriteLine("{0} {1}", context.Request.HttpMethod, context.Request.RawUrl);
                context.Response.SendChunked = true;
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
            finally
            {
                if (_maxThreads == 1 + _busy.Release())
                    _idle.Set();
            }
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

        public static bool IsPortAvailable(int port)
        {
            
            bool isAvailable = true;
            if (port >= 49152 && port <= 65535)
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        isAvailable = false;
                        break;
                    }
                }
            }
            else
            {
                isAvailable = false;
            }

            return isAvailable;
        }
    }
}
