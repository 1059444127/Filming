using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace UIH.Mcsf.Filming.Wrapper
{
    [AttributeUsage(AttributeTargets.All,Inherited = true)]
    public class CallTraceAttribute : ContextAttribute, IContributeObjectSink
    {
        private bool _ifLogTimeCost;

        public CallTraceAttribute(bool ifLogTimeCost = false)
            : base("log")
        {
            _ifLogTimeCost = ifLogTimeCost;
        }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            return new CallTraceSink(nextSink, _ifLogTimeCost);
        }
    }
}
