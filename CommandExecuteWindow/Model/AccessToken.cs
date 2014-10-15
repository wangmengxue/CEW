using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandExecuteWindow.Model
{
    public class AccessToken
    {
        private string access_token;
        /// <summary>
        /// 访问token
        /// </summary>
        public string Access_Token
        {
            get { return access_token; }
            set { access_token = value; }
        }

        private int expires_in;
        /// <summary>
        /// 过期时间
        /// </summary>
        public int Expires_in
        {
            get { return expires_in; }
            set { expires_in = value; }
        }

    }
}
