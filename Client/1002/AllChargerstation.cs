using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1002
{
    class AllChargerstation
    {
        public string StatId { get; set; }
        public string StatNm { get; set; }
        public string Adddoro { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Usetime { get; set; }

        public AllChargerstation(string statid, string statnm, string adddoro, double lat, double lng, string usetime)
        {
            StatId = statid;
            StatNm = statnm;
            Adddoro = adddoro;
            Lat = lat;
            Lng = lng;
            Usetime = usetime;
        }

        public string APacket()
        {
            string str = "ADATA@";
            str += StatId + '#';
            str += StatNm + '#';
            str += Adddoro + '#';
            str += Lat.ToString() + '#';
            str += Lng.ToString() + '#';
            str += Usetime;


            return str;
        }

    }
}
