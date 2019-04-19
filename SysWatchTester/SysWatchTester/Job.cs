using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SysWatchTester
{
    public class Job : INotifyPropertyChanged
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private HttpStatusCode _statusCode;

        public HttpStatusCode StatusCode
        {
            get => _statusCode;
            set
            {
                _statusCode = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<HttpStatusCode> StatusCodes { get; set; } =
            Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>();

        public Job(int id, HttpStatusCode statusCode)
        {
            Id = id;
            StatusCode = statusCode;            
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
