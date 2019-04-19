using System.Collections.Generic;

namespace SysWatchTester
{
    public enum Environment
    {
        Dev, 
        QA,
        Test,
        UAT,
        Prod
    }

    public class Request
    {
        public Environment Environment { get; }
        public string Url { get; }
        public string QueryString { get; }

        public Request(Environment environment, string url, string queryString)
        {
            Environment = environment;
            Url = url;
            QueryString = queryString;
        }
    }
}
