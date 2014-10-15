using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistenceLayer.LocalStorage.XMLStorage
{
    public static class PersistenceSettings
    {
        private static string _fileDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../PersistentData/";

        public static string FileDirectory
        {
            get { return _fileDirectory; }
            set { _fileDirectory = value; }
        }
    }
}
