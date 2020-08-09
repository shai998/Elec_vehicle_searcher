using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1002
{
    class CarInfo
    {

        public string Name { get; private set; }
        public int Battery { get;  set; }

        public CarInfo(string n,int e)
        {
            Name = n;
            Battery = e;
        }

        
    }
}
