using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1002
{
    public class Favorite
    {



        #region 프로퍼티
        public string Address { get; set; }
        public string Name { get; set; }
        

        #endregion

        #region 생성자
        public Favorite( string address,string name)
        {
            Address = address;
            Name = name;
            
        }



        #endregion

        public override string ToString()
        {
            return "충전소명 : " + Name + "//" + "주소 : " + Address;
        }
    }
}