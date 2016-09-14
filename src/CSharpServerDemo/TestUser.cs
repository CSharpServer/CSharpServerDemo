using CSharpServerFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServer
{
    
    public class TestUser: ICSharpServerUser
    {
        private static long _idSeed = 0;
        private static object _idSeedLock = new object();
        public static long NextUserIdSeed
        {
            get
            {
                lock (_idSeedLock)
                {
                    return ++_idSeed;
                }
            }
        }
        public string UserName { get; set; }

       public bool IsUserValidate { get { return true; } }

        public ICSharpServerSession Session { get; set; }
        
        public override string ToString()
        {
            return "UserName:" + UserName;
        }
    }
}
