using CSharpServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServer
{
    class BaseConfig : IGetServerConfig
    {

        public int GetBufferSize()
        {
            return 32; 
        }

        public int GetWorkerThreadCount()
        {
            return 4;
        }

        public int GetNetTimeOut()
        {
            return 15000;
        }


        public uint GetBufferInitCount()
        {
            return 1000;
        }

        public uint GetBufferAddPerTimeCount()
        {
            return 200;
        }

        public uint GetValidateTimeout()
        {
            return 5000;
        }
    }
}
