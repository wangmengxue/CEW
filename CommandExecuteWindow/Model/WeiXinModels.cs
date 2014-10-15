using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandExecuteWindow.Model
{
    /// <summary>
    /// 部门列表
    /// </summary>
    public class DepartmentList {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<Department> department { get; set; }
    }

    /// <summary>
    /// 部门信息
    /// </summary>
    public class Department {
        public int id { get; set; }
        public string name { get; set; }
        public int parentid { get; set; }
    }
}
