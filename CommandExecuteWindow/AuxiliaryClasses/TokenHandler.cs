using CommandExecuteWindow.Model;
using PersistenceLayer.LocalStorage.XMLStorage;
using System;

namespace CommandExecuteWindow.AuxiliaryClasses
{
    /// <summary>
    /// Token助手 获取和刷新TOKEN
    /// </summary>
    public static class TokenHandler
    {
        #region Attributes & Members
        private const string SECRET = "9BtnEznooVW-LoveHveoG7l6qAAefLLfICGV_-pmTbK7g2gLCVivdBtT-1SXoWaT";
        private const string CORPID = "wx3e04cc26f9331091";
        private static AccessToken _token;
        public static AccessToken Token
        {
            get
            {
                //获取配置文件内的token
                if (_token == null)
                {
                    _token = PersistenceHelper.ReadSingleEntityFromXml<AccessToken>();
                }
                return _token;
            }

            set
            {
                //设置配置文件的token
                _token = value;
                var t = new System.Threading.Thread(o =>
                {
                    PersistenceHelper.SaveSingleEntity<AccessToken>(_token);
                });
                t.Start();
            }
        }
        #endregion

        #region 重新生成token
        /// <summary>
        /// 刷新TOKEN值
        /// </summary>
        public static void RefreshToken()
        {
            string query_url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", CORPID, SECRET); //拼装访问的URL
            try
            {
                Token = RequestHelper.TryFetchObject<AccessToken>(query_url, null, "", null);   //通过URL获取新的TOKEN
                Console.WriteLine("成功刷新Token>> {0}", Token.Access_Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
