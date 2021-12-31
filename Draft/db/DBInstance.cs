using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draft.db
{
    public static class DBInstance
    {
        static test_DBEntities connection;
        static object objectLock = new object();
        public static test_DBEntities Get()
        {
            lock (objectLock)
            {
                if (connection == null)
                    connection = new test_DBEntities();
                return connection;
            }
        }
    }
}
