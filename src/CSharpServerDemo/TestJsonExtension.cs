using CSharpServerFramework;
using CSharpServerFramework.Extension;
using CSServerJsonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServer
{
    [ExtensionInfo("TestJsonExtension")]
    class TestJsonExtension:JsonExtensionBase
    {
        public override void Init()
        {
            
        }

        [CommandInfo(1,"TestJson1",true)]
        public void TestJson(ICSharpServerSession session,dynamic obj)
        {
            var user = session.User as TestUser;
            Log(user.UserName + ":" + (string)obj.msg);
            this.SendJsonResponse(session, new { msg = "Server: Received Message " + ((int)obj.msgIndex) }, ExtensionName, 1);
        }
    }
}
