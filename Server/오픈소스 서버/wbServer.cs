using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 오픈소스_서버
{
    class wbServer
    {
        #region 맴버 필드 및 프로퍼티
        private Socket server;
        private List<Socket> slist = new List<Socket>();
        wbDB db = new wbDB();

        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }

        #endregion

        #region 기능(외부 접근) Form1에서 접근
        public bool ServerRun(int port)
        {
            try
            {
                db.Connect();

                server = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
                server.Bind(ipep);
                server.Listen(20);

                IPEndPoint ip = (IPEndPoint)server.LocalEndPoint;
                ServerIp = ip.Address.ToString();
                ServerPort = port;

                Thread th = new Thread(new ParameterizedThreadStart(ServerThread));
                th.Start(this);
                th.IsBackground = true;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void ServerStop()
        {
            try
            {
                server.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(Socket sock, string msg)
        {
            try
            {
                if (sock.Connected)
                {
                    byte[] data = Encoding.Default.GetBytes(msg);
                    this.SendData(sock, data);
                }
                else
                {
                    Console.WriteLine("클라이언트 미연결");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendAll(string msg)
        {
            byte[] data = Encoding.Default.GetBytes(msg);
            SendAllData(data);
        }

        #endregion

        #region(내부 사용) 자체 호출 기능

        private void SendAllData(byte[] msg)
        {
            foreach (Socket s in slist)
            {
                SendData(s, msg);
            }
        }

        private void SendData(Socket sock, byte[] data)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                // 전송할 데이터의 크기 전달
                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = sock.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = sock.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveData(Socket sock, ref byte[] data)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기 
                byte[] data_size = new byte[4];
                recv_data = sock.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;

                data = new byte[size];

                // 실제 데이터 수신
                while (total < size)
                {
                    recv_data = sock.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        #endregion

        #region thread

        private void ServerThread(object data)
        {
            while (true)
            {
                Socket client = server.Accept();

                slist.Add(client);      //소켓 저장
                Console.WriteLine("클라이언트 접속");

                Thread tr = new Thread(new ParameterizedThreadStart(WorkThread));
                tr.Start(client);
                tr.IsBackground = true;
            }
        }

        private void WorkThread(object data)
        {
            Socket sock = (Socket)data;

            byte[] msg = null;
            try
            {
                while (true)
                {
                    //수신
                    ReceiveData(sock, ref msg);   // 수신한 문자열이 있으면 화면에 출력
                    PaserByteData(sock,msg);

                    //송신
                    //SendData(sock, msg);
                    //SendAllData(sock, msg);   //SendData(sock, msg)  

                    msg = null;
                }
            }
            catch (Exception ex)
            {
                ShortMessage(ex.Message);
                slist.Remove(sock); //소켓 제거
                sock.Close();
            }
        }

        public void PaserByteData(Socket sock, byte[] data)
        {
            string msg = Encoding.Default.GetString(data);
            string[] token = msg.Split('@');
            switch (token[0].Trim())
            {
                case "DATATYPE": NewQurey(sock, token[1]); break;
                case "ALLDATA": AllQuery(sock); break;
                case "LOGDATA": LogQuery(sock); break;
            }
        }

        void NewQurey(Socket sock, string msg)
        {
            ShortMessage(msg);
            string[] sp = msg.Split('#');
            double swlat = double.Parse(sp[0]);
            double swlng = double.Parse(sp[1]);
            double nelat = double.Parse(sp[2]);
            double nelng = double.Parse(sp[3]);
            List<ChargeStation> cslist=db.SelectData(swlat, swlng, nelat, nelng);
            SendData(sock, Encoding.Default.GetBytes("START@"));
            foreach (ChargeStation cs in cslist)
            {
               string str= cs.Pakcet();
                SendData(sock, Encoding.Default.GetBytes(str));
            }
            SendData(sock, Encoding.Default.GetBytes("END@"));
        }

        public void ShortMessage(string str)
        {
            str += "(" + DateTime.Now.ToString() + ")";
            Console.WriteLine(str);
        }

        void LogQuery(Socket sock)
        {
            //ShortMessage(msg);
            //string[] sp = msg.Split('#');
            //string StatId = sp[0];
            //int UseTime = int.Parse(sp[1]);
            //int CountMax = int.Parse(sp[2]);
            List<Log> lglist = db.Selectlog();
            //SendData(sock, Encoding.Default.GetBytes("START@"));
            foreach (Log lg in lglist)
            {
                string str = lg.Packing();
                SendData(sock, Encoding.Default.GetBytes(str));
            }
            //SendData(sock, Encoding.Default.GetBytes("END@"));
        }

        void AllQuery(Socket sock)
        {
            //ShortMessage(msg);
            //string[] sp = msg.Split('#');
            //string StatId = sp[0];
            //string StatNm = sp[1];
            //string Adddoro = sp[2];
            //double lat = double.Parse(sp[3]);
            //double lng = double.Parse(sp[4]);
            //string Usetime = sp[5];
            List<ChargeStation> stdlist = db.SelectAllData();
            //SendData(sock, Encoding.Default.GetBytes("START@"));
            foreach (ChargeStation std in stdlist)
            {
                string str = std.APakcet();
                SendData(sock, Encoding.Default.GetBytes(str));
            }
            //SendData(sock, Encoding.Default.GetBytes("END@"));

        }
        #endregion         
    }
        
}
