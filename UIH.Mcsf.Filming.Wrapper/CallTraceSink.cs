using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace UIH.Mcsf.Filming.Wrapper
{
    class CallTraceSink : IMessageSink
    {
        private readonly bool _ifTraceCostTime;

        public CallTraceSink(IMessageSink next, bool ifTraceCostTime)
        {
            NextSink = next;
            _ifTraceCostTime = ifTraceCostTime;
        }

        #region [--Implemented Interface--]

        public IMessage SyncProcessMessage(IMessage msg)
        {
            //拦截消息，做前处理
            Preprocess(msg);    

            var retMsg = _ifTraceCostTime ? Process(msg) : NextSink.SyncProcessMessage(msg);

            //调用返回时进行拦截，并进行后处理
            Postprocess(retMsg); 

            return retMsg;
        }

        //IMessageSink接口方法，用于异步处理，我们不实现异步处理，所以简单返回null,
        //不管是同步还是异步，这个方法都需要定义
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }

        public IMessageSink NextSink { get; private set; }

        #endregion [--Implemented Interface--]


        #region [--Operations--]
        private IMessage Process(IMessage msg)
        {
            var sw = new Stopwatch();
            sw.Start();

            //传递消息给下一个接收器
            IMessage retMsg = NextSink.SyncProcessMessage(msg);

            sw.Stop();
            Logger.LogInfo(string.Format("Time Cost (ms) : {0}", sw.ElapsedMilliseconds));
            //DebugHelper.Trace(TraceLevel.Info, string.Format("Time Cost (ms) : {0}", sw.ElapsedMilliseconds));
            return retMsg;
        }

        private void Preprocess(IMessage msg)
        {
            var call = msg as IMethodCallMessage;
            if (call == null)
            {
                Logger.LogWarning("Can't get CallMessage in PreProcess");
                //DebugHelper.Trace(TraceLevel.Warning, "Can't get CallMessage in PreProcess");
                return;
            }

            var argumentList = call.Args.Select(a => string.Format("{0}:{1}", a.GetType().Name, a.ToString()));
            var argumentsInfo = string.Join(",", argumentList);

            var type = call.TypeName.Split(',').First().Split('.').Last();
            Logger.LogInfo(string.Format("[Begin]{0}.{1}({2})", type, call.MethodName, argumentsInfo));
            //DebugHelper.Trace(TraceLevel.Info, string.Format("[Begin]{0}({1})", call.MethodName, argumentsInfo));

        }

        private void Postprocess(IMessage retMsg)
        {
            var mi = retMsg as IMethodReturnMessage;
            if (mi == null)
            {
                Logger.LogWarning("Can't get ReturnMessage in PostProcess");
                //DebugHelper.Trace(TraceLevel.Warning, "Can't get ReturnMessage in PostProcess");
                return;
            }

            var outArgumentsInfo = string.Empty;
            if (mi.OutArgCount > 0)
            {
                var outArgumentList = mi.OutArgs.Select(a => string.Format("{0}:{1}", a.GetType(), a.ToString()));
                outArgumentsInfo = string.Format("[Out]({0})", string.Join(",", outArgumentList));
            }
            var type = mi.TypeName.Split(',').First().Split('.').Last();
            Logger.LogInfo(string.Format("[End]{0}.{1}[Ret]{2}{3}", type, mi.MethodName, mi.ReturnValue, outArgumentsInfo));
            //DebugHelper.Trace(TraceLevel.Info, string.Format("[End]{0}[Ret]{1}{2}", mi.MethodName, mi.ReturnValue, outArgumentsInfo));

            var e = mi.Exception;
            if (e != null)
            {
                Logger.LogInfo(string.Format("[Exception]{0}", e.Message));
                //DebugHelper.Trace(TraceLevel.Info, string.Format("[Exception]{0}", e.Message));
            }
        }
        #endregion [--Operations--]
    }
}
