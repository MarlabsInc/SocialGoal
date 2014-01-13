using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace SocialGoal.Web.Core.Models
{
     public class UserInfo
    {
        public string DisplayName { get; set; }
        public string UserIdentifier { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public override string ToString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserInfo));
            using (var stream = new System.IO.StringWriter())
            {
                serializer.Serialize(stream, this);
                return stream.ToString();
            }
        }

        public static UserInfo FromString(string userContextData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserInfo));
            using (var stream = new StringReader(userContextData))
            {
                return serializer.Deserialize(stream) as UserInfo;
            }
        }
    }
}

