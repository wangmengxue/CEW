using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PersistenceLayer.LocalStorage.XMLStorage
{

    /// <summary>
    /// @marsoln
    /// 2014年3月18日14:55:26
    /// 持久化层的帮助类
    /// 完成了实例对象和集合的持久化功能
    /// </summary>
    public static class PersistenceHelper
    {
        #region 静态构造函数

        static PersistenceHelper()
        {

        }

        #endregion

        #region 持久化方法

        /// <summary>
        /// 保存单个实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="entity">实例对象</param>
        /// <param name="files">子级文件夹名称</param>
        /// <returns>执行成功/失败</returns>
        public static bool SaveSingleEntity<T>(T entity, params string[] files)
        {

            bool result = true;
            //获取集合对象的类名作为文件名
            string objClassName = typeof(T).Name;
            string fileName = string.Empty;
            //判断是否传入子文件夹名称
            if (files == null || files.Length == 0)
            {
                fileName = PersistenceSettings.FileDirectory + objClassName + ".xml";
            }
            else
            {

                fileName = PersistenceSettings.FileDirectory;
                for (int i = 0; i < files.Length; i++)
                {
                    fileName += files[i] + @"/";
                }
                fileName += objClassName + ".xml";
            }
            try
            {
                XmlHelper.ObjectToXMLFile<T>(entity, fileName);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 保存(覆盖原有)实例集合
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="entities">实例对象</param>
        /// <param name="files">子级文件夹名称</param>
        /// <returns>执行成功/失败</returns>
        public static bool SaveEntityList<T>(List<T> entities, params string[] files)
        {
            bool result = true;
            //获取集合对象的类名作为文件名
            string objClassName = typeof(T).Name;
            string fileName = string.Empty;
            //判断是否传入子文件夹名称
            if (files == null || files.Length == 0)
            {
                fileName = PersistenceSettings.FileDirectory + objClassName + ".xml";
            }
            else
            {

                fileName = PersistenceSettings.FileDirectory;
                for (int i = 0; i < files.Length; i++)
                {
                    fileName += files[i] + @"/";
                }
                fileName += objClassName + ".xml";
            }
            try
            {
                XmlHelper.ObjectToXMLFile<List<T>>(entities, fileName);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 追加实例集合到指定集合文件
        /// </summary>
        /// <typeparam name="T">实例的类型</typeparam>
        /// <param name="entities">实例集合</param>
        /// <param name="files">子级文件夹名称</param>
        /// <returns>执行成功/失败</returns>
        public static bool AppendEntities<T>(List<T> entities, params string[] files)
        {

            bool result = true;
            //获取集合对象的类名作为文件名
            string objClassName = typeof(T).Name;
            string fileName = string.Empty;
            //判断是否传入子文件夹名称
            if (files == null || files.Length == 0)
            {
                fileName = PersistenceSettings.FileDirectory + objClassName + ".xml";
            }
            else
            {

                fileName = PersistenceSettings.FileDirectory;
                for (int i = 0; i < files.Length; i++)
                {
                    fileName += files[i] + @"/";
                }
                fileName += objClassName + ".xml";
            }
            try
            {
                if (File.Exists(fileName))
                {
                    //如果存在该文件 读取文件 并追加
                    List<T> list = XmlHelper.XmlToObj<List<T>>(fileName);
                    File.Delete(fileName);
                    entities.AddRange(list);
                }
                XmlHelper.ObjectToXMLFile<List<T>>(entities, fileName);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 从xml文件中读取对象实体集合
        /// </summary>
        /// <typeparam name="T">集合中单个对象的类型</typeparam>
        /// <param name="files">子级文件夹名称</param>
        /// <returns>对象实体集合</returns>
        public static List<T> ReadMultiEntitiesFromXml<T>(params string[] files)
        {
            List<T> result = new List<T>();
            string fileName = string.Empty;
            //获取集合对象的类名作为文件名
            string objClassName = typeof(T).Name;
            //判断是否传入子文件夹名称
            if (files == null || files.Length == 0)
            {
                fileName = PersistenceSettings.FileDirectory + objClassName + ".xml";
            }
            else
            {
                //拼接子级文件夹名称
                fileName = PersistenceSettings.FileDirectory;
                for (int i = 0; i < files.Length; i++)
                {
                    fileName += files[i] + @"/";
                }
                fileName += objClassName + ".xml";
            }
            result = XmlHelper.XmlToObj<List<T>>(fileName);
            return result;
        }

        /// <summary>
        /// 从xml文件中读取单个对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="files">子级文件夹名称</param>
        /// <returns>对象实体</returns>
        public static T ReadSingleEntityFromXml<T>(params string[] files)
        {
            T result = default(T);
            string fileName = string.Empty;
            //获取集合对象的类名作为文件名
            string objClassName = typeof(T).Name;
            //判断是否传入子文件夹名称
            if (files == null || files.Length == 0)
            {
                fileName = PersistenceSettings.FileDirectory + objClassName + ".xml";
            }
            else
            {
                //拼接子级文件夹名称
                fileName = PersistenceSettings.FileDirectory;
                for (int i = 0; i < files.Length; i++)
                {
                    fileName += files[i] + @"/";
                }
                fileName += objClassName + ".xml";
            }
            result = XmlHelper.XmlToObj<T>(fileName);
            return result;
        }
        #endregion

    }
}
