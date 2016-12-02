using System;
using System.Threading;

namespace UIH.Mcsf.Filming.Scaffold
{
    class Program
    {
        static void Main(string[] args)
        {
            //启动两个进程，一个进程读，一个进程写"StandAlone"
            Console.WriteLine("开始测试配置文件读写冲突");
            var readThread = new Thread(Read);
            var writeThread = new Thread(Write);
            readThread.Start();
            writeThread.Start();
        }

        private static void Read()
        {
            int count = 0;
            while (true)
            {
                Console.WriteLine("第{0}次读取配置", count++);
                Printers.Instance.ReloadDefaultConfig();
                Thread.Sleep(10);
            }
        }

        private static void Write()
        {
            int count = 0;
            while (true)
            {
                Console.WriteLine("第{0}次改写配置", count++);
                Printers.Instance.IfStandAlone = !Printers.Instance.IfStandAlone;
                Thread.Sleep(20);
            }
        }
    }
}
