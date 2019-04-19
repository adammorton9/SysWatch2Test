using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SysWatchTester
{
    /// <summary>
    /// Interaction logic for JobsWindow.xaml
    /// </summary>
    public partial class JobsWindow : Window
    {
        public JobsWindow()
        {
            InitializeComponent();
            Jobs.Add(new Job(1, HttpStatusCode.Accepted));
            Jobs.Add(new Job(2, HttpStatusCode.Forbidden));
            JobsListView.ItemsSource = Jobs;

        }

        public List<Job> Jobs = new List<Job>();

    }
}
