using CommandExecuteWindow.AuxiliaryClasses;
using CommandExecuteWindow.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommandExecuteWindow
{
    /// <summary>
    /// 可执行的命令函数的委托
    /// </summary>
    delegate void ExecutableFunction(params object[] objs);

    class Program
    {
        #region 变量声明
        private static Dictionary<string, MethodInfo> _executableFunctions = new Dictionary<string, MethodInfo>();
        #endregion

        #region 启动入口
        static void Main(string[] args)
        {
            Initialization();
            while (true)
            {
                var command = Console.ReadLine();
                ExeCommand(command.ToLower());
            }
        }
        #endregion

        #region 业务功能

        /// <summary>
        /// 执行输入的命令
        /// </summary>
        /// <param name="p">输入的命令</param>
        private static void ExeCommand(string p)
        {
            //拆分出执行命令的关键字
            var commandName = p.Split(' ')[0];
            //尝试从加载的函数池中找到对应的处理函数
            var list = _executableFunctions.Where(func => func.Key == commandName);
            var del = list.FirstOrDefault();
            //判断是否有相对应的处理函数
            if (string.IsNullOrEmpty(del.Key))
            {
                Console.WriteLine("Invalid Command...");
                return;
            }
            else
            {
                //拆分出参数列表
                var param = p.Split(' ').Skip(1).ToArray();

                if (del.Value.IsStatic)
                {
                    //静态方法的调用
                    del.Value.Invoke(null, new object[] { param });
                }
                else
                {
                    //非静态方法的调用
                    var mi = del.Value;
                    mi.Invoke(mi.DeclaringType.GetConstructor(new Type[] { }).Invoke(null), new object[] { param });
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Initialization()
        {
            //获取所有定义的方法
            var methodInfos = typeof(ExecutableCommands).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            //初始化计数器
            int failureCounter = 0, successCounter = 0;
            foreach (var method in methodInfos)
            {
                var ca = method.CustomAttributes;
                CustomAttributeData cad = null;
                foreach (var attr in ca)
                {
                    if (attr.AttributeType.Equals(typeof(ExecutableCommands.ExecutableCommand)))
                    {
                        //获取到方法的ExecutableFunctionAttribute(如果有的话)
                        cad = attr;
                        break;
                    }
                }
                if (cad != null)
                {
                    try
                    {
                        //将方法的methodInfo添加到命令集中
                        var temp = method.GetCustomAttribute(typeof(ExecutableCommands.ExecutableCommand));
                        _executableFunctions.Add(((ExecutableCommands.ExecutableCommand)temp).CommandName.ToLower(), method);
                        successCounter++;
                    }
                    catch (Exception ex)
                    {
                        failureCounter++;
                    }
                }
            }
            Console.WriteLine("初始化完毕,共执行{0}条,失败{1}条...", successCounter, failureCounter);
        }
        #endregion

    }


}
