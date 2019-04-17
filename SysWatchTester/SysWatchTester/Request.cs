using System.Collections.Generic;

namespace SysWatchTester
{
    class Request
    {
        public int WaitTime { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
    }
}
