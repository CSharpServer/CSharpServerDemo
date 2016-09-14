using CSharpServerFramework;
using CSharpServerFramework.Extension;
using CSharpServerFramework.Message;
using CSServerJsonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServer
{
    [ValidateExtension]
    [ExtensionInfo("LoginExtension")]
    class TestLoginExtension: ExtensionBaseEx
    {
        public TestLoginExtension()
            : base(JsonMessageDeserializer.Instance)
        {

        }

        [CommandInfo(1, "Login")]
        public void Login(ICSharpServerSession session, dynamic model)
        {
            
            if (model.Password == "123")
            {
                var User = new TestUser
                {
                    UserName = model.UserName
                };
                session.RegistUser(User);
                JsonProtocolExension.SendJsonResponse(this, session, new { suc = true }, ExtensionName, "Login");
            }
            else
            {
                JsonProtocolExension.SendJsonResponse(this, session, new { suc = false }, ExtensionName, "Login");
            }
        }

        [CommandInfo(2,"Logout")]
        public void Logout(ICSharpServerSession session,dynamic obj)
        {
            var User = session.User as TestUser;
            this.CloseSession(session);
        }

        public override void Init()
        {
            
        }
    }

}
