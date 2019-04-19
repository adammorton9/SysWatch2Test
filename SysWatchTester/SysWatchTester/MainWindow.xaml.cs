using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Int32;

namespace SysWatchTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpServer Server { get; set; }
        public delegate void AddRequest(string myString);
        public AddRequest RequestDelegate;
        public delegate void AddResponse(string myString);
        public AddResponse ResponseDelegate;
        public delegate void UpdateRequestCount(int count);
        public UpdateRequestCount RequestCountDelegate;


        public MainWindow()
        {
            InitializeComponent();
            EnvComboBox.ItemsSource = Enum.GetValues(typeof(Environment)).Cast<Environment>();
            EnvComboBox.SelectedIndex = 0;
            RequestDelegate = AddRequestToListBox;
            ResponseDelegate = AddResponseToListBox;
            RequestCountDelegate = UpdateRequestsRemainingLabel;
            PortTextBox.Text = HttpServer.GetRandomUnusedPort().ToString();
        }
        
        #region Button Click Events

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TryParse(PortTextBox.Text, out var portNo))
            {
                if (!HttpServer.IsPortAvailable(portNo))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Port is not available. Use random open port?", "Port unavailable", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        portNo = HttpServer.GetRandomUnusedPort();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                PortAvailLabel.Content = "Port number must be an integer.";
                return;
            }
            if (Server == null || !Server.IsRunning)
            {
                Server = new HttpServer(portNo, this);
                StatusLabel.Content = $"Listener running.";
                UrlLabel.Content = $"{Server.Url.Replace("*", GetLocalIPAddress())}";
                RequestListView.Items.Clear();
                ResponseListView.Items.Clear();
                Server.Start();
            }
            else
            {
                MessageBox.Show("Stop current listener before starting another.");
            }
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Server == null || !Server.IsRunning)
            {
                MessageBox.Show("No listener running.");
            }
            else
            {
                StatusLabel.Content = "Listener stopped.";
                UrlLabel.Content = string.Empty;
                Server.Dispose();
            }
        }

        private void PortAvail_Button_Click(object sender, RoutedEventArgs e)
        {
            ValidatePortIsAvailable();
        }

        #endregion
        
        private void ValidatePortIsAvailable()
        {
            if (TryParse(PortTextBox.Text, out var portNo))
            {
                PortAvailLabel.Content = HttpServer.IsPortAvailable(portNo)
                    ? "Port is available!"
                    : "Port is unavailable. Select another port.";
            }
            else
            {
                PortAvailLabel.Content = "Port number must be an integer.";
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void AddRequestToListBox(string text)
        {
            var numItemsInBox = RequestListView.Items.Count;
            RequestListView.Items.Add(new { Id = numItemsInBox + 1, Request = text});
        }

        private void AddResponseToListBox(string text)
        {
            var numItemsInBox = ResponseListView.Items.Count;
            ResponseListView.Items.Add(new { Id = numItemsInBox + 1, Response = text });
        }
                
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void UpdateRequestsRemainingLabel(int requestsRemaining)
        {
            RemainingRequestsLabel.Content = requestsRemaining;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParse(NumJobsTextBox.Text, out int numJobs))
            {
                MessageBox.Show("Number of jobs must be an integer.");
                return;
            }
            var environment = (Environment)Enum.Parse(typeof(Environment), EnvComboBox.SelectedValue.ToString());
            var request = new Request(environment, Server.Url.Replace("*", GetLocalIPAddress()), $"env={environment}");
            WebServiceClient.AddRequests(numJobs);
            UpdateRequestsRemainingLabel(WebServiceClient.RemainingRequests);
            if (WebServiceClient.CanSendRequest())
            {
                WebServiceClient.DecrementRequestCount();
                AddRequestToListBox($"{environment} - {Server.Url.Replace("*", GetLocalIPAddress())}");
                WebServiceClient.SendRequest(request);
            }
        }
    }
}
