using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACM.Data
{
    public static class DBHelper
    {
        public static ACMEntities GetDBContext()
        {
            return new ACMEntities();
        }
    }
}
