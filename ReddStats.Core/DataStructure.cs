using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bsparser
{
    using System.Runtime.InteropServices;


    public class DataStructure
    {
        public void CreateScructure()
        {
            SQLiteConnection.CreateFile("c:\\Ik.db");
        }
    }
}
