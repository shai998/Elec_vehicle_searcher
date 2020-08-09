using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1002
{
    class Log
    {
        public string StatId { get; set; }
        public int UseTime { get; set; }
        public int CountMax { get; set; }

        public Log(string statid, int usetime, int countmax)
        {

            StatId = statid;
            UseTime = usetime;
            CountMax = countmax;
        }

        public string Packing()
        {
            string str = "LOG@";

            str += StatId + '#';
            str += UseTime.ToString() + '#';
            str += CountMax.ToString();

            return str;
        }

    }
}
