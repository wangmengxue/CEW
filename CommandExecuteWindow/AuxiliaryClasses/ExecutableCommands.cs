using CommandExecuteWindow.Model;
using PersistenceLayer.LocalStorage.XMLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandExecuteWindow.AuxiliaryClasses
{

    /// <summary>
    /// 可执行命令函数
    /// </summary>
    public class ExecutableCommands
    {

        #region constructor
        public ExecutableCommands()
        {

        }

        #endregion

        #region 成员及变量
        #endregion

        #region 可执行的命令

        /// <summary>
        /// 创建测试用户
        /// </summary>
        /// <param name="objs"></param>
        [ExecutableCommand("CreateUser")]
        private void CreateUserForTest(params string[] objs)
        {
            User user = new User();
            if (objs.Length == 0)
            {
                user.UserName = _askEnter("请输入用户名:");
                user.Password = _askEnter("请输入密码:");
            }
            else
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            user.UserName = objs[i];
                            break;
                        case 1:
                            user.Password = objs[i];
                            break;
                    }
                }
            }
            Console.WriteLine("---------------------创建测试用户---------------------\n用户名: {0} \n密码: {1}", user.UserName, user.Password);
            if (_confirm("确定要创建该用户吗?(Y/N)"))
            {
                PersistenceHelper.SaveSingleEntity<User>(user);
                Console.WriteLine("用户创建成功.");
            }
            else
            {
                Console.WriteLine("取消创建...");
            }
        }

        /// <summary>
        /// 更新测试用户
        /// </summary>
        /// <param name="objs"></param>
        [ExecutableCommand("UpdateUser")]
        private void UpdateUserForTest(params string[] objs)
        {
            Console.WriteLine("修改用户信息...");
            Console.WriteLine("用户信息修改完成");
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        [ExecutableCommand("RefreshToken")]
        private void RefreshToken(params string[] objs)
        {
            TokenHandler.RefreshToken();
        }

        /// <summary>
        /// 拉取部门信息
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        [ExecutableCommand("FetchDepartmentInfo")]
        private DepartmentList FetchDepartmentInfo(params string[] objs)
        {
            DepartmentList result = null;
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}", TokenHandler.Token.Access_Token);
            result = RequestHelper.TryFetchObject<DepartmentList>(url, 5000, "", null);
            if (result != null && result.errcode == 0)
            {
                Console.WriteLine("获取部门信息共{0}条", result.department.Count);
                PersistenceHelper.SaveSingleEntity<DepartmentList>(result);
            }
            else if (result.errcode == 42001)
            {
                RefreshToken();
                return FetchDepartmentInfo();
            }
            return result;
        }

        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="objs"></param>
        ///
        [ExecutableCommand("UDI")]
        private void UpdateDepartmentInfo(params string[] objs) {
            Console.WriteLine("这里是更新部门");
        }

        #endregion

        #region 其他方法
        /// <summary>
        /// 确认是否继续执行
        /// </summary>
        /// <param name="noticeStr">提示信息</param>
        /// <returns>继续/取消</returns>
        private bool _confirm(string noticeStr)
        {
            bool result = false;
            if (string.IsNullOrEmpty(noticeStr))
            {
                Console.WriteLine("确定继续吗?(Y/N)");
            }
            else
            {
                Console.WriteLine(noticeStr);
            }
            var key = Console.ReadLine().Trim().ToLower();
            result = (key == "y");
            return result;
        }

        /// <summary>
        /// 要求输入
        /// </summary>
        /// <param name="promptStr">提示信息</param>
        /// <returns>输入内容</returns>
        private string _askEnter(string promptStr)
        {
            Console.Write(promptStr);
            return Console.ReadLine();
        }
        #endregion

        #region 可执行命令属性声明
        /// <summary>
        /// 可执行命令的属性 生成对照关系
        /// </summary>
        public class ExecutableCommand : Attribute
        {
            #region Members & Attributes
            private string _commandName;
            /// <summary>
            /// 指令名称
            /// </summary>
            public string CommandName
            {
                get
                {
                    if (string.IsNullOrEmpty(_commandName))
                    {
                        throw new Exception("未初始化指令.");
                    }
                    else
                    {
                        return _commandName;
                    }
                }
            }
            #endregion

            #region Constructor
            public ExecutableCommand(string commandName)
            {
                this._commandName = commandName;
            }
            #endregion
        }
        #endregion

    }
}
