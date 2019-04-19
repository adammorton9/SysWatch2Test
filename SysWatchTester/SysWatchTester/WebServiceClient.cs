using System;

namespace SysWatchTester
{
    public static class WebServiceClient
    {
        public static int RemainingRequests { get; private set; }

        public static void AddRequests(int requests)
        {
            RemainingRequests += requests;
        }

        public static bool CanSendRequest()
        {
            return RemainingRequests > 0;
        }

        public static void DecrementRequestCount()
        {
            if (RemainingRequests > 0)
                RemainingRequests--;
        }

        public static void SendRequest(Request request)
        {
            switch (request.Environment)
            {
                case Environment.Dev:
                    var devSysWatchService = new DevSysWatch2.XmlService();
                    devSysWatchService.SubmitJob("0", 0, "2", request.Url, request.QueryString, 0, "test", "1");
                    break;
                case Environment.QA:
                    var qaSysWatchService = new QASysWatch2.XmlService();
                    qaSysWatchService.SubmitJob("0", 0, "2", request.Url, request.QueryString, 0, "test", "1");
                    break;
                case Environment.Test:
                    var testSysWatchService = new TestSysWatch2.XmlService();
                    testSysWatchService.SubmitJob("0", 0, "2", request.Url, request.QueryString, 0, "test", "1");
                    break;
                case Environment.UAT:
                    var uatSysWatchService = new UATSysWatch2.XmlService();
                    uatSysWatchService.SubmitJob("0", 0, "2", request.Url, request.QueryString, 0, "test", "1");
                    break;
                case Environment.Prod:
                    var prodSysWatchService = new ProdSysWatch2.XmlService();
                    prodSysWatchService.SubmitJob("0", 0, "2", request.Url, request.QueryString, 0, "test", "1");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
