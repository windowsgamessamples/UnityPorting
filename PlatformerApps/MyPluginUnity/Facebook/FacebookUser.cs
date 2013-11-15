using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPlugin.Facebook
{
    public class FacebookUser
    {
        public FacebookUser(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public String Id { set; get; }
        public String Name { set; get; }
    }
}
