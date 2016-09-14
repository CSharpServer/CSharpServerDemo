using CSharpServerFramework;
using CSharpServerFramework.Log;
using CSServerJsonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new CSServer();
            server.UseNetConfig(new NetConfig());
            server.UseServerConfig(new BaseConfig());
            server.UseLogger(ConsoleLogger.Instance);
            server.UseMessageRoute(new JsonRouteFilter());
            server.UseExtension(new TestLoginExtension());
            server.UseExtension(new TestJsonExtension());
            server.OnSessionDisconnected += Server_OnSessionDisconnected;
            try
            {
                server.StartServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Write("任意键退出");
                Console.ReadKey();
                return;
            }
            while (true)
            {
                if (Console.ReadLine() == "exit")
                {
                    server.StopServer();
                    break;
                }
            }
        }

        private static void Server_OnSessionDisconnected(object sender, CSServerEventArgs e)
        {
            Console.WriteLine(e.State);   
        }

    }
}

