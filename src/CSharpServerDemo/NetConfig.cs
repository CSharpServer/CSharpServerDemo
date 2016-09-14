using CSharpServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServer
{
    class NetConfig : IGetNetConfig
    {
        public int GetListenPort()
        {
            return 8050;
        }

        public int GetMaxListenConnection()
        {
            return 500;
        }

        public System.Net.IPAddress GetServerBindIP()
        {
            return System.Net.IPAddress.Any;
        }

    }
}
