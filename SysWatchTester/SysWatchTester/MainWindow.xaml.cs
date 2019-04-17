using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SysWatchTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpServer Server { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            int port = HttpServer.GetRandomUnusedPort();
            Server = new HttpServer(port);
            TestLabel.Content = Server.Url;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
