using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PersistenceLayer.LocalStorage.XMLStorage
{

    /// <summary>
    /// XML序列化/反序列化 辅助类
    /// </summary>
    public static class XmlHelper
    {
        #region 对象模型操作
        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="source">XML字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>反序列化的对象</returns>
        public static T XmlToObj<T>(string source, Encoding encoding)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(encoding.GetBytes(source)))
            {
                return (T)ser.Deserialize(ms);
            }
        }

        /// <summary>
        /// 从xml文件加载对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">xml文件路径</param>
        /// <returns></returns>
        public static T XmlToObj<T>(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    return (T)ser.Deserialize(fs);
                }
            }
            catch (FileNotFoundException e)
            {

            }
            return default(T);
        }

        /// <summary>
        /// 将object对象序列化成XML
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象实例</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>序列化字符串</returns>
        public static string ObjectToXML<T>(T t, Encoding encoding)
        {
            XmlSerializer ser = new XmlSerializer(t.GetType());
            Encoding utf8EncodingWithNoByteOrderMark = new UTF8Encoding(false);
            using (MemoryStream mem = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(mem, utf8EncodingWithNoByteOrderMark))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("xs", "http://www.marsoln.org/xml");
                    ser.Serialize(writer, t, ns);
                    return encoding.GetString(mem.ToArray());
                }
            }
        }

        /// <summary>
        /// 将对象实体转换成xml文件存储
        /// </summary>
        /// <typeparam name="T">转换的对象类型</typeparam>
        /// <param name="t">对象实体</param>
        /// <param name="filePath">xml文件路径</param>
        public static void ObjectToXMLFile<T>(T t, string filePath)
        {
            XmlSerializer ser = new XmlSerializer(t.GetType());
            Encoding utf8EncodingWithNoByteOrderMark = new UTF8Encoding(false);
            FileStream fs;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                fs = File.OpenWrite(filePath);
            }
            else
            {
                string file_directory = filePath.Substring(0, filePath.LastIndexOf('/'));
                if (!Directory.Exists(file_directory))
                {
                    Directory.CreateDirectory(file_directory);
                }
                fs = File.Create(filePath);
            }
            using (fs)
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("xs", "http://www.marsoln.org/xml");
                ns.Add("time", DateTime.Now.ToShortDateString());
                ser.Serialize(fs, t, ns);
            }
        }
        #endregion

        #region GetXmlFullPath
        /// 
        /// 返回完整路径 
        /// 
        /// Xml的路径 
        /// 
        public static string GetXmlFullPath(string strPath)
        {
            //如果路径中含有:符号,则认定为传入的是完整路径 
            if (strPath.IndexOf(":") > 0)
            {
                return strPath;
            }
            else
            {
                //返回完整路径 
                return PersistenceSettings.FileDirectory + strPath;
            }
        }
        #endregion

        #region GetDataSetByXml
        /// <summary>
        /// 读取XML至DataSet
        /// </summary>
        /// <param name="strXmlPath">xml文件路径</param>
        /// <returns></returns>
        public static DataSet GetDataSetByXml(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                //读取XML到DataSet 
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables.Count > 0)
                {
                    return ds;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region GetDataViewByXml
        /// <summary> 
        /// 读取Xml返回一个经排序或筛选后的DataView 
        /// </summary> 
        /// <param name="strXmlPath"></param> 
        /// <param name="strWhere">筛选条件,如："name = 'kgdiwss'"</param> 
        /// <param name="strSort">排序条件,如："Id desc"</param> 
        /// <returns></returns> 
        public static DataView GetDataViewByXml(string strXmlPath, string strWhere, string strSort)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                //创建DataView来完成排序或筛选操作 
                DataView dv = new DataView(ds.Tables[0]);
                if (strSort != null)
                {
                    //对DataView中的记录进行排序 
                    dv.Sort = strSort;
                }
                if (strWhere != null)
                {
                    //对DataView中的记录进行筛选,找到我们想要的记录 
                    dv.RowFilter = strWhere;
                }
                return dv;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region WriteXmlByDataSet
        /// <summary> 
        /// 向Xml文件插入一行数据 
        /// </summary> 
        /// <param name="strXmlPath">xml文件相对路径</param> 
        /// <param name="Columns">要插入行的列名数组,如：string[] Columns = {"name","IsMarried"};</param> 
        /// <param name="ColumnValue">要插入行每列的值数组,如：string[] ColumnValue={"kgdiwss","false"};</param> 
        /// <returns>成功返回true,否则返回false</returns> 
        public static bool WriteXmlByDataSet(string strXmlPath, string[] Columns, string[] ColumnValue)
        {
            try
            {
                //根据传入的XML路径得到.XSD的路径,两个文件放在同一个目录下
                string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
                DataSet ds = new DataSet();
                //读xml架构,关系到列的数据类型 
                ds.ReadXmlSchema(GetXmlFullPath(strXsdPath));
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                DataTable dt = ds.Tables[0];
                //在原来的表格基础上创建新行 
                DataRow newRow = dt.NewRow();

                //循环给 一行中的各个列赋值 
                for (int i = 0; i < Columns.Length; i++)
                {
                    newRow[Columns[i]] = ColumnValue[i];
                }
                dt.Rows.Add(newRow);
                dt.AcceptChanges();
                ds.AcceptChanges();
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region UpdateXmlRow
        /// <summary> 
        /// 更行符合条件的一条Xml记录 
        /// </summary> 
        /// <param name="strXmlPath">XML文件路径</param> 
        /// <param name="Columns">列名数组</param> 
        /// <param name="ColumnValue">列值数组</param> 
        /// <param name="strWhereColumnName">条件列名</param> 
        /// <param name="strWhereColumnValue">条件列值</param> 
        /// <returns></returns> 
        public static bool UpdateXmlRow(string strXmlPath, string[] Columns, string[] ColumnValue, string strWhereColumnName, string strWhereColumnValue)
        {
            try
            {
                //同上一方法 
                string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";

                DataSet ds = new DataSet();
                //读xml架构,关系到列的数据类型 
                ds.ReadXmlSchema(GetXmlFullPath(strXsdPath));
                ds.ReadXml(GetXmlFullPath(strXmlPath));

                //先判断行数 
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //如果当前记录为符合Where条件的记录
                        if (ds.Tables[0].Rows[i][strWhereColumnName].ToString().Trim().Equals(strWhereColumnValue))
                        {
                            //循环给找到行的各列赋新值 
                            for (int j = 0; j < Columns.Length; j++)
                            {
                                ds.Tables[0].Rows[i][Columns[j]] = ColumnValue[j];
                            }
                            //更新DataSet 
                            ds.AcceptChanges();
                            //重新写入XML文件 
                            ds.WriteXml(GetXmlFullPath(strXmlPath));
                            return true;
                        }
                    }

                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DeleteXmlAllRows
        ///  <summary> 
        /// 删除所有行 
        ///  </summary> 
        ///  <param name="strXmlPath">XML路径 </param> 
        ///  <returns> </returns> 
        public static bool DeleteXmlAllRows(string strXmlPath)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                //如果记录条数大于0 
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //移除所有记录 
                    ds.Tables[0].Rows.Clear();
                }
                //重新写入,这时XML文件中就只剩根节点了 
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DeleteXmlRowByIndex
        ///  <summary> 
        /// 通过删除DataSet中iDeleteRow这一行,然后重写Xml以实现删除指定行 
        ///  </summary> 
        ///  <param name="strXmlPath"> </param> 
        ///  <param name="iDeleteRow">要删除的行在DataSet中的Index值 </param> 
        public static bool DeleteXmlRowByIndex(string strXmlPath, int iDeleteRow)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //删除符号条件的行 
                    ds.Tables[0].Rows[iDeleteRow].Delete();
                }
                ds.WriteXml(GetXmlFullPath(strXmlPath));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DeleteXmlRows
        /// <summary> 
        /// 删除strColumn列中值为ColumnValue的行 
        /// </summary> 
        /// <param name="strXmlPath">xml相对路径</param> 
        /// <param name="strColumn">列名</param> 
        /// <param name="ColumnValue">strColumn列中值为ColumnValue的行均会被删除</param> 
        /// <returns></returns> 
        public static bool DeleteXmlRows(string strXmlPath, string strColumn, string[] ColumnValue)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(GetXmlFullPath(strXmlPath));

                //先判断行数 
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //判断行多还是删除的值多,多的for循环放在里面 
                    if (ColumnValue.Length > ds.Tables[0].Rows.Count)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < ColumnValue.Length; j++)
                            {
                                //找到符合条件的行
                                if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
                                {
                                    //删除行 
                                    ds.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < ColumnValue.Length; j++)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //找到符合条件的行
                                if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
                                {
                                    //删除行 
                                    ds.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                    }
                    ds.WriteXml(GetXmlFullPath(strXmlPath));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
