using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandExecuteWindow.Model
{
    public class User
    {
        private string userid;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Userid
        {
            get { return userid; }
            set { userid = value; }
        }

        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

    }
}
