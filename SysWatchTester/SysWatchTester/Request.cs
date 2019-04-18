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
    }
}
