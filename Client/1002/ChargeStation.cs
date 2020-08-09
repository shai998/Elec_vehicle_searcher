namespace _1002
{
    //전기차 충전소 정보
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
    string addrDoro, double lat, double lng, string useTime)
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

        #region 오버라이드
        public override string ToString()
        {
            return StatNm;
        }

        #endregion
    }

   
}
