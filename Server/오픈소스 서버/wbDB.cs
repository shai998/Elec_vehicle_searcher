using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace 오픈소스_서버
{
    class wbDB
    {
        SqlConnection conn;
        SqlConnection conn1;
        string Constr1; //내서버  
        XmlDocument doc = null;

        public string XmlString { get; private set; }

        public wbDB()
        {
            conn = new SqlConnection();
            conn1 = new SqlConnection();
            Constr1 = @"Server=DESKTOP-4DQPBHI\SQLEXPRESS;database=Advanced_Restaurant;uid=team2_rest;pwd=qlxmcorhdi;";
            conn.ConnectionString = Constr1;
            conn1.ConnectionString = Constr1;
        }

        public void Connect()
        {
            try
            {
                conn.Open();    //  데이터베이스 연결
                conn1.Open();
                Thread th = new Thread(new ThreadStart(SearcherThread));
                th.Start();
                th.IsBackground = true;
            }
            catch (Exception)
            {
                Console.WriteLine("DB 연결실패");
            }
        }

        private bool ExSqlCommand(string comstr)
        {
            SqlCommand Scom = new SqlCommand();
            Scom.CommandTimeout = 300;
            Scom.Connection = conn;
            Scom.CommandText = comstr;
            Scom.CommandType = System.Data.CommandType.Text;
            //page112
            if (Scom.ExecuteNonQuery() > 0)
            {
                Scom.Dispose();
                return true;
            }
            Scom.Dispose();
            return false;
        }

        //딱한번만 돌려
        public bool InsertData(ChargeStation ch)
        {
            string str = string.Format("if exists(select statId from StationData where statId ='{0}') begin insert into ChgerData Values('{0}',{6},{7},{8},null) end " +
                "else begin insert into StationData Values ('{0}','{1}','{2}',{3},{4},'{5}');" +
                " insert into ChgerData Values ('{0}',{6},{7},{8},null);   end", ch.StatId, ch.StatNm, ch.AddrDoro, ch.Lat, ch.Lng, ch.UseTime, ch.ChgerId, ch.ChgerType, ch.Stat);
            return ExSqlCommand(str);
        }

        public bool UpdateData(ChargeStation ch)
        {
            string str = string.Format("if exists(select * from ChgerData where stat = 2 and 3 ={0} and statId='{1}' and chgerId={2}) begin update ChgerData set stat ={0}" +
                ",statTime = SYSDATETIME() where statId = '{1}'and chgerId={2} end else begin update ChgerData set statTime = null, stat ={0}" +
                " where statId = '{1}' and not stat ={0} and chgerId={2} end", 
                ch.Stat, ch.StatId,ch.ChgerId);
            return ExSqlCommand(str);
        }

        public List<ChargeStation> SelectData(double swlat, double swlng, double nelat, double nelng)
        {
            string str = string.Format("select * from StationData sd,ChgerData cd where sd.lat >{0} and sd.lat<{2} and sd.lng>{1} and sd.lng <{3} " +
                "and sd.statId=cd.statId", swlat, swlng, nelat, nelng);
            List<ChargeStation> cslist = new List<ChargeStation>();
            SqlCommand command = new SqlCommand(str, conn1);
            SqlDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                ChargeStation cs = new ChargeStation(reader["statId"].ToString(), reader["statNm"].ToString(), reader["addrDoro"].ToString()
                    , double.Parse(reader["lat"].ToString()), double.Parse(reader["lng"].ToString()), reader["useTime"].ToString(), 
                    int.Parse(reader["chgerId"].ToString()),int.Parse(reader["chgerType"].ToString())
                    , int.Parse(reader["stat"].ToString()),(reader["statTime"].ToString()));
                cslist.Add(cs);
            }
            reader.Close();
            command.Dispose();
            return cslist;
        }

        public List<Log> Selectlog()
        {
            string str = string.Format("Select x.StatId, x.UseTime, MAX(x.COUNT) as 'CountMax'" +
                                        " from(select StatId, UseTime, COUNT(UseTime) as 'count'" +
                                        " from UseStation group by StatId, UseTime)x" +
                                        " group by x.StatId, x.UseTime order by MAX(x.COUNT)desc;");
            List<Log> lglist = new List<Log>();
            SqlCommand command = new SqlCommand(str, conn1);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Log lg = new Log(reader["StatId"].ToString(), int.Parse(reader["UseTime"].ToString()), int.Parse(reader["CountMax"].ToString()));
                lglist.Add(lg);
            }

            reader.Close();
            command.Dispose();
            return lglist;
        }

        public List<ChargeStation> SelectAllData()
        {
            string str = string.Format("select * from StationData sd,ChgerData cd where sd.StatId = cd.StatId");
            List<ChargeStation> cslist = new List<ChargeStation>();
            SqlCommand command = new SqlCommand(str, conn1);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ChargeStation cs = new ChargeStation(reader["statId"].ToString(), reader["statNm"].ToString(), reader["addrDoro"].ToString()
                    , double.Parse(reader["lat"].ToString()), double.Parse(reader["lng"].ToString()), reader["useTime"].ToString(),
                    int.Parse(reader["chgerId"].ToString()), int.Parse(reader["chgerType"].ToString())
                    , int.Parse(reader["stat"].ToString()), (reader["statTime"].ToString()));
                cslist.Add(cs);
            }
            reader.Close();
            command.Dispose();
            return cslist;
        }

        private void SearcherThread()
        {
            while (true)
            {
                Console.WriteLine("업데이트 진행중...");
                Searcher();
                Console.WriteLine("업데이트 완료...");
                System.Threading.Thread.Sleep(60000);
            }
        }

        #region XML 코드
        public void Searcher()
        {
                XmlString = Find();
                doc = new XmlDocument();
                doc.LoadXml(XmlString);
                //string name = "data.xml";
                //doc.Save(name);
                ////=====================================================

                XmlNode node = doc.SelectSingleNode("response");
                XmlNode nd = node.SelectSingleNode("body");
                XmlNode n = nd.SelectSingleNode("items");

                ChargeStation station = null;
                foreach (XmlNode el in n.SelectNodes("item"))
                {
                    station = ChargeStation.MakeStation(el);
                    //딱한번만돌려
                    //InsertData(station);
                    //계속돌려!!
                    UpdateData(station);
                }  
        }

        public string Find()
        {
            string url = "http://open.ev.or.kr:8080/openapi/services/rest/EvChargerService?ServiceKey=";
            url += "NASFKFoeh%2F2ts2MSBlacOSq2N5KGilm3hY50LuAoKJ0s54vI7XBCtChZiFsLHbgt2DIQUw9VN%2FRJWssLOTMPCQ%3D%3D";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();

            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                return text;
            }
            else
            {
                return string.Format("Error 발생={0}" + status);
            }
        }
        #endregion

    }
}
