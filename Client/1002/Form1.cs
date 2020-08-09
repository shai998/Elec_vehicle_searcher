using Microsoft.CognitiveServices.Speech;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace _1002
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class Form1 : Form 
    {

        #region 선언부
        wbClient client = new wbClient();
        StationSearcher s = StationSearcher.Instance;
        HtmlDocument hdoc;
        public List<ChargeStation> timest = new List<ChargeStation>();        
        public List<Favorite> fav = new List<Favorite>();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        string IP = "192.168.0.43";
        string Port = "9000";

        #endregion


        public Form1() //시작
        {
            InitializeComponent();
            client.ParentInfo(this);

        }

        
        #region 대화형 음성서비스
        kakao ko = new kakao();
        Thread voice = null;
        delegate void TextSetCallback(string str); // 비동기 전용 텍스트 바꾸기
        delegate void MapMoveCallback(string str);
        delegate void NearMoveCallback(double x, double y);

        public void initRS()
        {

            //btimer.Interval = 1000;
            //btimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed); //주기마다 실행되는 이벤트 등록
            //btimer.Start();

            RecognizeSpeechAsync().Wait();

            
        }

        public async Task RecognizeSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription("69e7cc3ace6c475cac5f0bdd910782ab", "koreacentral");
            config.SpeechRecognitionLanguage = "ko-KR";

            while (true)
            {
                using (var recognizer = new SpeechRecognizer(config))
                {
                    var result1 = await recognizer.RecognizeOnceAsync();

                    if (result1.Reason == ResultReason.RecognizedSpeech)
                    {
                        if (result1.Text.Contains("시리") || result1.Text.Contains("친구")|| result1.Text.Contains("쉬어")||result1.Text.Contains("싫어") || result1.Text.Contains("실현") || result1.Text.Contains("실행"))
                        {
                            #region 조건
                            ko.ConnectApi("네 부르셨나요");
                            var result2 = await recognizer.RecognizeOnceAsync();
                            if (result2.Text.Contains("검색"))
                            {
                                string str = result2.Text;
                                string[] res = str.Split(new string[] { "검색" }, StringSplitOptions.None);
                                TextSet(res[0]);
                                ko.ConnectApi("검색 완료했습니다");
                                
                                Mapmove(res[0]);
                                
                            }
                            else if (result2.Text.Contains("찾아"))
                            {
                                string str = result2.Text;
                                string[] res = str.Split(new string[] { "찾아" }, StringSplitOptions.None);
                                TextSet(res[0]);
                                ko.ConnectApi("찾았어요!");

                                Mapmove(res[0]);
                                
                            }
                            else if (result2.Text.Contains("근방") || result2.Text.Contains("가까") || result2.Text.Contains("가족"))
                            {
                                ko.ConnectApi("알겠어요");

                                Nearmove(37.50554, 126.96071);

                            }
                            else if ((result2.Text.Contains("밥") && result2.Text.Contains("먹었")) || result2.Text.Contains("어머니") || result2.Text.Contains("먹었니"))
                            {
                                ko.ConnectApi("배터리 백퍼센트에요");
                            }
                            else if (result2.Text.Contains("자기소개")|| result2.Text.Contains("자기 소개"))
                            {
                                ko.ConnectApi("저는 충전소 검색을 도와줄 시리라고 해요");
                            }
                            else if (result2.Text.Contains("아니"))
                            {
                                ko.ConnectApi("알겠습니다. 필요하시면 불러주세요.");
                            }

                            #endregion
                        }
                        

                    }
                    else if (result1.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    }
                    else if (result1.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result1);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
        #region UI바꾸기 백그라운드 콜백 메서드
        private void TextSet(string str)
        {
            if (this.InvokeRequired)
            {
                TextSetCallback d = new TextSetCallback(TextSet);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                textBox3.Text = str;
            }
        }

        private void Mapmove(string str)
        {
            if (this.InvokeRequired)
            {
                MapMoveCallback d = new MapMoveCallback(Mapmove);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                object[] ps = new object[] { str };
                hdoc.InvokeScript("Serch", ps);
            }
        }

        private void Nearmove(double x, double y)
        {
            if (this.InvokeRequired)
            {
                NearMoveCallback d = new NearMoveCallback(Nearmove);
                this.Invoke(d, new object[] { x, y });
            }
            else
            {
                object[] ps = new object[] { x, y };
                hdoc.InvokeScript("setCenter", ps);
            }
        }
        #endregion

        // 2차조건 실행

        #endregion

        #region 핸들러
        private void Form1_Load(object sender, EventArgs e)
        {
            voice = new Thread(new ThreadStart(initRS));
            voice.IsBackground = true;
            voice.Start();


            web_map.ObjectForScripting = this;
            hdoc = web_map.Document;
            if (client.Connect(IP, Port) == true)
            {
                MessageBox.Show("성공적으로 연결되었습니다!");
                client.AllData();
                client.Log();
                
            }
        }

        #region 근처 주차창 리스트 
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
                return;
            PrintInfo();
        }
        #endregion


        public void CheckMarker()
        {
            List<string> ar = new List<string>();
            foreach (ChargeStation ls in s)
            {
                listBox2.Items.Clear();
                object[] ps = null;
                if (checkBox2.Checked == true)
                {
                    if (ls.ChgerType == 03 || ls.ChgerType == 06 || ls.ChgerType == 07)
                    {
                        int stat = CheckState(ls);
                        ps = new object[] { ls.Lat, ls.Lng, stat }; ;
                        if (stat == 2)
                            ar.Add(ls.StatNm);
                        hdoc.InvokeScript("MakeMarker", ps);
                    }

                }
                else if (checkBox3.Checked == true)
                {
                    if (ls.ChgerType == 01 || ls.ChgerType == 03 || ls.ChgerType == 05 || ls.ChgerType == 06)
                    {
                        int stat = CheckState(ls);
                        ps = new object[] { ls.Lat, ls.Lng, stat }; ;
                        if(stat==2)
                            ar.Add(ls.StatNm);
                        hdoc.InvokeScript("MakeMarker", ps);
                    }

                }
                else if (checkBox4.Checked == true)
                {
                    if (ls.ChgerType == 03 || ls.ChgerType == 05 || ls.ChgerType == 06)
                    {
                        int stat = CheckState(ls);
                        ps = new object[] { ls.Lat, ls.Lng, stat }; ;
                        if (stat == 2)
                            ar.Add(ls.StatNm);
                        hdoc.InvokeScript("MakeMarker", ps);
                    }
                }
                else
                    return;

                ar = ar.Distinct().ToList();
                foreach (string name in ar)
                {
                    listBox2.Items.Add(name);
                }
            }
        }

      
        #endregion

        #region Form1 내부 메서드
        private string whattype(ChargeStation data)
        {
            switch (data.ChgerType)
            {
                case 01: return string.Format("DC차데모");
                case 02: return string.Format("AC완속");
                case 03: return string.Format("DC차데모+AC3상");
                case 04: return string.Format("DC콤보");
                case 05: return string.Format("DC차데모+DC콤보");
                case 06: return string.Format("DC차데모+AC3상+DC콤보");
                case 07: return string.Format("AC3상");
            }
            return "없음";
        }
        private string whatstat(ChargeStation data)
        {
            switch (data.Stat)
            {
                case 1: return string.Format("통신이상");
                case 2: return string.Format("충전대기");
                case 3: return string.Format("충전중");
                case 4: return string.Format("운영중지");
                case 5: return string.Format("점검중");

            }
            return "없음";
        }

        private int CheckState(ChargeStation ls)
        {
            foreach (ChargeStation ch in s)
            {
                if (ls.StatId.Equals(ch.StatId))
                {
                    if (ch.Stat == 2)
                        return 2;
                }
            }
            return ls.Stat;
        }

        private bool isEqual(double n1, double n2)
        {
            const double Eps = 0.0000000005;
            return (Math.Abs(n1 - n2) < Eps);
        }

        private void PrintInfo()
        {
            listView1.Items.Clear();
            ChargeStation data = null;
            foreach (ChargeStation d in s)
            {
                string place = (string)listBox2.SelectedItem;

                if (d.StatNm.Equals(place))
                {
                    data = d;
                    string id = string.Format("{0}", d.ChgerId);
                    string type = whattype(d);
                    string stat = whatstat(d);
                    string[] arr = { id, type, stat };
                    ListViewItem a = new ListViewItem(arr);
                    listView1.Items.Add(a);
                }
            }

            if (data != null)
            {
                textBox1.Text = data.AddrDoro;
                textBox2.Text = data.UseTime;
                analyze(data);
                object[] ps = new object[] { data.Lat, data.Lng };
                MarkerInfo(data.Lat, data.Lng);
                hdoc.InvokeScript("setCenter", ps);

                textBox4.Clear();
                textBox4.Text = (string)listBox2.SelectedItem;
            }
            else
            {
                listBox2.Items.Clear();
            }
            
            

        }

        private void analyze(ChargeStation data)
        {
            for (int i = 0; i < client.lglist.Count; i++)
            {
                if(data.StatId.Equals(client.lglist[i].StatId))
                {
                    //MessageBox.Show(DateTime.Now.Hour.ToString());
                    //MessageBox.Show(string.Format("시간대 척도:{0} , 빈도수:{1}", client.lglist[i].UseTime,client.lglist[i].CountMax));
                    #region 시간대별 분석
                    if (0 <= DateTime.Now.Hour && DateTime.Now.Hour <= 2)
                    {
                        if(client.lglist[i].UseTime == 1)
                        {
                            if(0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if(10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (2 < DateTime.Now.Hour && DateTime.Now.Hour <= 4)
                    {
                        if (client.lglist[i].UseTime == 2)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (4 < DateTime.Now.Hour && DateTime.Now.Hour <= 6)
                    {
                        if (client.lglist[i].UseTime == 3)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (6 < DateTime.Now.Hour && DateTime.Now.Hour <= 8)
                    {
                        if (client.lglist[i].UseTime == 4)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (8 < DateTime.Now.Hour && DateTime.Now.Hour <= 10)
                    {
                        if (client.lglist[i].UseTime == 5)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (10 < DateTime.Now.Hour && DateTime.Now.Hour <= 12)
                    {
                        if (client.lglist[i].UseTime == 6)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (12 < DateTime.Now.Hour && DateTime.Now.Hour <= 14)
                    {
                        if (client.lglist[i].UseTime == 7)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }

                    else if (14 < DateTime.Now.Hour && DateTime.Now.Hour <= 16)
                    {
                        if (client.lglist[i].UseTime == 8)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (16 < DateTime.Now.Hour && DateTime.Now.Hour <= 18)
                    {
                        if (client.lglist[i].UseTime == 9)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (18 < DateTime.Now.Hour && DateTime.Now.Hour <= 20)
                    {
                        if (client.lglist[i].UseTime == 10)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }

                    else if(20 < DateTime.Now.Hour && DateTime.Now.Hour <= 22)
                    {
                        if (client.lglist[i].UseTime == 11)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    else if (22 < DateTime.Now.Hour && DateTime.Now.Hour <= 24)
                    {
                        if (client.lglist[i].UseTime == 12)
                        {
                            if (0 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 5)
                            {
                                textBox5.Text = "원활";
                            }
                            else if (5 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 10)
                            {
                                textBox5.Text = "보통";
                            }
                            else if (10 < client.lglist[i].CountMax && client.lglist[i].CountMax <= 15)
                            {
                                textBox5.Text = "혼잡";
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        public void Atimer(ChargeStation ch)
        {
            timer.Interval = 1000; // 1초
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            GetTime();
        }

        private void GetTime()
        {
            listBox1.Items.Clear();
            var stattimes = timest.Select(x => x.StatTime).ToArray();
            var chgerid = timest.Select(x => x.ChgerId).ToArray();
            
            for(int i=0;i< stattimes.Length;i++)
            {
                string temp = stattimes[i];
                int temp1 = chgerid[i];

                DateTime start = Convert.ToDateTime(temp);
                DateTime endline = start + new TimeSpan(1, 30, 0);
                DateTime now = DateTime.Now;
                TimeSpan endtime = endline - now;
                if (endtime.Seconds < 0)
                {
                    endtime = new TimeSpan(0, 0, 0);
                }
                listBox1.Items.Add(string.Format("[충전기 ID :{0}]", temp1));
                listBox1.Items.Add(string.Format("예상대기시간 :{0}:{1}:{2}", endtime.Hours, endtime.Minutes, endtime.Seconds));
                listBox1.Items.Add(string.Format("예상완료시간 : {0}", endline));
                listBox1.Items.Add("--------------------------------------------------------------");
            }
        }

        #endregion

        #region JS호출 메서드

        public void SendToServer(object swLat, object swLng, object neLat, object neLng)
        {
            double swlat = (double)swLat;
            double swlng = (double)swLng;
            double nelat = (double)neLat;
            double nelng = (double)neLng;
            client.Packging(swlat, swlng, nelat, nelng);
        }
        
        public void MarkerInfo(object lat,object lng)
        {
            timer.Dispose();
            timest.Clear();
            listView1.Items.Clear();
            listBox1.Items.Clear();
            foreach (ChargeStation ch in s)
            {
                if (isEqual(ch.Lat,(double)lat) && isEqual(ch.Lng, (double)lng))
                {

                    string id = string.Format("{0}", ch.ChgerId);
                    string type = whattype(ch);
                    string stat = whatstat(ch);
                    string[] arr = { id, type, stat };
                    ListViewItem a = new ListViewItem(arr);
                    listView1.Items.Add(a);

                    textBox1.Text = ch.AddrDoro;
                    textBox2.Text = ch.UseTime;
                    textBox4.Text = ch.StatNm;

                    analyze(ch);

                    if(ch.StatTime !="")
                    {
                        timest.Add(ch);
                        GetTime();
                        Atimer(ch);
                    }
                    
                }
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            string addr = string.Format("'{0}'", textBox3.Text);
            object[] ps = new object[] { textBox3.Text };
            hdoc.InvokeScript("Serch", ps);
        }

        private void 프로그램종료XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 즐겨찾기
        List<Favorite> falist = new List<Favorite>();
        private void button2_Click(object sender, EventArgs e) //즐겨찾기 등록 
        {

            try
            {
                string addr = textBox1.Text;
                string station = textBox4.Text;

                falist.Add(new Favorite(addr, station));

                MessageBox.Show("즐겨찾기 등록 성공");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button3_Click(object sender, EventArgs e) //즐겨찾기 창 불러오기 및 마커 이동.
        {
            AddFavorite adf = new AddFavorite(falist);
            if(adf.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = adf.Addr;

                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox4.Text = string.Empty;

                listView1.Items.Clear();
                for (int i = 0; i < client.chlist.Count; i++)
                {
                    if (client.chlist[i].AddrDoro.Equals(adf.Addr))
                    {

                        object[] ps = new object[] { client.chlist[i].Lat, client.chlist[i].Lng };

                        string id = string.Format("{0}", client.chlist[i].ChgerId);
                        string type = whattype(client.chlist[i]);
                        string stat = whatstat(client.chlist[i]);
                        string[] arr = { id, type, stat };
                        ListViewItem a = new ListViewItem(arr);
                        listView1.Items.Add(a);

                        textBox1.Text = client.chlist[i].AddrDoro;
                        textBox2.Text = client.chlist[i].UseTime;
                        textBox4.Text = client.chlist[i].StatNm;

                        analyze(client.chlist[i]);

                        hdoc.InvokeScript("setCenter", ps);
                        

                    }
                }
            }
            
        }
        #endregion

    }

   
        
}


