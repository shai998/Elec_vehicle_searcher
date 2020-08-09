using System;
using System.Xml;

namespace 오픈소스_서버
{
    public class ChargeStation
    {
        #region 프로퍼티
        public string StatId { get; private set; }
        public string StatNm { get; private set; }
        public int ChgerId { get; private set; }
        public int ChgerType { get; private set; }
        public int Stat { get; private set; }
        public string AddrDoro { get; private set; }
        public double Lat { get; private set; }
        public double Lng { get; private set; }
        public string UseTime { get; private set; }
        public string StatTime { get; private set; }
        #endregion

        //DB->서버 생성자
        public ChargeStation(string statId, string statNm, string addrDoro, double lat, double lng, string useTime, int chgerId, int chgerType, int stat,
          string stattime)
        {
            StatId = statId;
            StatNm = statNm;
            ChgerId = chgerId;
            ChgerType = chgerType;
            Stat = stat;
            AddrDoro = addrDoro;
            Lat = lat;
            Lng = lng;
            UseTime = useTime;
            StatTime = stattime;
        }

        //XML -> 서버 생성자
        public ChargeStation(string statId, string statNm, int chgerId, int chgerType, int stat,
    string addrDoro, double lat, double lng, string useTime )
        {
            StatId = statId;
            StatNm = statNm;
            ChgerId = chgerId;
            ChgerType = chgerType;
            Stat = stat;
            AddrDoro = addrDoro;
            Lat = lat;
            Lng = lng;
            UseTime = useTime;
        }

        #region 파서 (XML문서 ->객체화)
        static public ChargeStation MakeStation(XmlNode xn)
        {
            string statId = String.Empty;
            string statNm = String.Empty;
            int chgerId = 0;
            int chgerType = 0;
            int stat = 0;
            string addrDoro = String.Empty;
            double lat = 0;
            double lng = 0;
            string useTime = String.Empty;


            XmlNode staId_node = xn.SelectSingleNode("statId");
            statId = ConvertString(staId_node.InnerText);

            XmlNode staName_node = xn.SelectSingleNode("statNm");
            statNm = ConvertString(staName_node.InnerText);

            XmlNode chgerId_node = xn.SelectSingleNode("chgerId");
            chgerId = int.Parse(chgerId_node.InnerText);

            XmlNode chgerType_node = xn.SelectSingleNode("chgerType");
            chgerType = int.Parse(chgerType_node.InnerText);

            XmlNode stat_node = xn.SelectSingleNode("stat");
            stat = int.Parse(stat_node.InnerText);

            XmlNode addrDoro_node = xn.SelectSingleNode("addrDoro");
            addrDoro = ConvertString(addrDoro_node.InnerText);

            XmlNode lat_node = xn.SelectSingleNode("lat");
            lat = double.Parse(lat_node.InnerText);

            XmlNode lng_node = xn.SelectSingleNode("lng");
            lng = double.Parse(lng_node.InnerText);

            XmlNode useTime_node = xn.SelectSingleNode("useTime");
            if (useTime_node == null)
                return new ChargeStation(statId, statNm, chgerId, chgerType, stat,
                                    addrDoro, lat, lng, null); ;
            useTime = ConvertString(useTime_node.InnerText);

            return new ChargeStation(statId, statNm, chgerId, chgerType, stat,
                                    addrDoro, lat, lng, useTime);
        }

        private static string ConvertString(string str)
        {
            return str;
        }

        #endregion

        public string Pakcet()
        {
            string str = "DATA@";
            str += StatId + '#';
            str += StatNm + '#';
            str += AddrDoro + '#';
            str += Lat.ToString() + '#';
            str += Lng.ToString() + '#';
            str += UseTime + '#';
            str += ChgerId.ToString() + '#';
            str += ChgerType.ToString() + '#';
            str += Stat.ToString() + '#';
            str += StatTime;

            return str;
        }

        public string APakcet()
        {
            string str = "ADATA@";
            str += StatId + '#';
            str += StatNm + '#';
            str += AddrDoro + '#';
            str += Lat.ToString() + '#';
            str += Lng.ToString() + '#';
            str += UseTime + '#';
            str += ChgerId.ToString() + '#';
            str += ChgerType.ToString() + '#';
            str += Stat.ToString() + '#';
            str += StatTime;

            return str;
        }
    }
}