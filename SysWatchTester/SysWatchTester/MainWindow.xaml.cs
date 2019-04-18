using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public AddRequest requestDelegate;
        public delegate void AddResponse(string myString);
        public AddResponse responseDelegate;

        public MainWindow()
        {
            requestDelegate = AddRequestToListBox;
            responseDelegate = AddResponseToListBox;
            InitializeComponent();
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
                UrlLabel.Content = $"{Server.Url.Replace("*", "127.0.0.1")}";
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
            RequestListView.Items.Add(text);
        }

        private void AddResponseToListBox(string text)
        {
            ResponseListView.Items.Add(text);
        }
    }
}
