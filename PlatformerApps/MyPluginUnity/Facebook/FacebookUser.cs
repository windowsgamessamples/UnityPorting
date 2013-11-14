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

        String Id { set; get; }
        String Name { set; get; }
    }
}
