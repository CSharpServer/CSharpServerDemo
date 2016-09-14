using CSharpClientFramework;
using CSharpServerFramework.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpClientDemo
{
    class JsonMessage : CSharpServerClientBaseMessage
    {
        public dynamic result { get; set; }
    }

    class JsonMessageDeserializer : CSharpClientFramework.IDeserializeMessage
    {
        public CSharpServerClientBaseMessage GetMessageFromBuffer(byte[] receiveBuffer, int len)
        {
            var router = CSServerJsonProtocol.JsonProtocolUtil.DeserializeRoute(receiveBuffer, len);
            var msg = CSServerJsonProtocol.JsonProtocolUtil.DeserializeMessage(router.CmdId, receiveBuffer, len);
            return new JsonMessage
            {
                CmdId = router.CmdId,
                CmdName = router.CmdName,
                ExtName = router.ExtName,
                result = msg
            };
        }
    }

    class DemoClient : CSharpServerClientBase
    {
        static int user = 0;
        public DemoClient() : base(new JsonMessageDeserializer())
        {
            Password = "123";
            UserName = string.Format("User<{0}>", user++);
        }

        public string Password { get; set; }
        public string UserName { get; private set; }
    }

    public class Program
    {
        static private Program instance;

        public static void Main(string[] args)
        {
            instance = new Program();
            instance.Run();
        }

        private IList<DemoClient> clients = new List<DemoClient>();
        private IPAddress remoteIp = IPAddress.Parse("127.0.0.1");

        private void PrintControlTips()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(string.Format("Online Users:{0}", clients.Count));
            Console.WriteLine("Q:Quit");
            Console.WriteLine("N:New User Login");
            Console.WriteLine("M:New Message");
            Console.WriteLine("K:Close A Random User");
            Console.WriteLine();
            Console.WriteLine("-----------------Select-----------------");
            Console.Write(">");
        }

        private void Run()
        {
            var run = true;
            Console.WriteLine("Press Any Key To Start Program");
            Console.ReadKey();
            while (run)
            {
                PrintControlTips();
                var op = Console.ReadKey();
                Console.WriteLine();
                switch (op.Key)
                {
                    case ConsoleKey.N:NewClient(); break;
                    case ConsoleKey.M: RandomClientSendMsg(); break;
                    case ConsoleKey.K: RandomKillClient(); break;
                    case ConsoleKey.Q: run = false; break;
                    default:
                        break;
                }
            }
            foreach (var c in clients.ToArray())
            {
                c.Close();
            }
            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();
        }

        private void RandomKillClient()
        {
            if (clients.Count > 0)
            {
                var client = clients[random.Next() % clients.Count];
                client.Close();
            }
        }

        int msgIndex = 0;
        private Random random = new Random();
        private void RandomClientSendMsg()
        {
            if (clients.Count > 0)
            {
                var client = clients[random.Next() % clients.Count];
                var msg = CreateJsonData("TestJsonExtension", 1, new { msg = string.Format("{0} Say: Message {0}", client.UserName, msgIndex++), msgIndex = msgIndex });
                client.SendMessage(msg, msg.Length);
            }
        }

        private void NewClient()
        {
            var client = new DemoClient();
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;
            client.OnSendFailed += Client_OnSendFailed;
            client.OnMessageReceived += Client_OnMessageRecieve;
            client.AddHandlerCallback("LoginExtension", "Login", onLogin);
            client.AddHandlerCallback("TestJsonExtension", 1, onTestJson);
            client.Start(remoteIp, 8050);
        }


        private void onTestJson(object sender, CSharpServerClientEventArgs e)
        {
            var msg = e.State as JsonMessage;
            Console.WriteLine();
            Console.WriteLine((string)msg.result.msg);
            PrintControlTips();
        }

        private void onLogin(object sender, CSharpServerClientEventArgs e)
        {
            var client = e.Client as DemoClient;
            var msg = e.State as JsonMessage;
            Console.WriteLine();
            if ((bool)msg.result.suc)
            {
                Console.WriteLine("User Loginned:{0}", client.UserName);
                clients.Add(client);
            }
            else
            {
                Console.WriteLine("User Validate Failed:{0}", client.UserName);
                client.Close();
            }
            PrintControlTips();
        }

        private void Client_OnSendFailed(object sender, CSharpServerClientEventArgs e)
        {
            var client = e.Client as DemoClient;
            Console.WriteLine();
            Console.WriteLine("User Send Faild:{0}", client.UserName);
            PrintControlTips();
        }

        private void Client_OnDisconnected(object sender, CSharpServerClientEventArgs e)
        {
            var client = e.Client as DemoClient;
            Console.WriteLine();
            Console.WriteLine("User Disconnected:{0}", client.UserName);
            Console.WriteLine("Disconnected");
            clients.Remove(client);
            PrintControlTips();
        }

        private void Client_OnConnected(object sender, CSharpServerClientEventArgs e)
        {
            var client = e.Client as DemoClient;            
            Console.WriteLine();
            Console.WriteLine("User Connected:{0}", client.UserName);
            PrintControlTips();
            var data = CreateJsonData("LoginExtension", 1, new { Password = client.Password, UserName = client.UserName });
            e.Client.SendMessage(data, data.Length);
        }

        private void Client_OnMessageRecieve(object sender, CSharpServerClientReceiveMessageEventArgs e)
        {
        }

        private byte[] CreateJsonData(string Extension, int CommandId, object Message)
        {
            return CSServerJsonProtocol.JsonProtocolUtil.SerializeMessage(new MessageRoute
            {
                CmdId = CommandId,
                ExtName = Extension
            }, Message);
        }

    }
}
