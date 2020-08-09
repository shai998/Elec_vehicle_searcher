using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _1002
{
    public delegate void Dele();
    class wbClient
    {
        private Socket server;
        StationSearcher s = StationSearcher.Instance;
        public List<ChargeStation> chlist = new List<ChargeStation>();
        public List<Log> lglist = new List<Log>();
        private Form1 form;
        public List<Favorite> favoirte = new List<Favorite>();

        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        Thread ComThread;

        public void ParentInfo(Form1 f)
        {
            form = f;
        }

        public bool Connect(string ip, string port)
        {
            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
                server = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);

                IPEndPoint addr = (IPEndPoint)server.LocalEndPoint;
                ServerIp = addr.Address.ToString();
                ServerPort = int.Parse(port);

                ComThread = new Thread(new ThreadStart(RecvMessage));
                ComThread.Start();
                ComThread.IsBackground = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            server.Close();
        }

        public void SendMessage(string msg)
        {
            try
            {
                if (server.Connected)
                {
                    SendData(Encoding.Default.GetBytes(msg));
                }
                else
                {
                    MessageBox.Show("서버 미연결 상태입니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendData(byte[] data)
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
                send_data = server.Send(data_size);

                // 실제 데이터 전송
                while (total < size)
                {
                    send_data = server.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RecvMessage()
        {
            byte[] data = null;
            while (true)
            {
                ReceiveData(ref data);
                //데이터 분석
                PaserByteData(data);
            }
        }

        private void ReceiveData(ref byte[] data)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                // 수신할 데이터 크기 알아내기 
                byte[] data_size = new byte[4];
                recv_data = server.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);
                left_data = size;

                data = new byte[size];

                // 실제 데이터 수신
                while (total < size)
                {
                    recv_data = server.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ComThread.Abort();
            }
        }

        public void Packging(double swlat, double swlng, double nelat, double nelng)
        {
            string str = "DATATYPE@";
            str += swlat.ToString() + '#';
            str += swlng.ToString() + '#';
            str += nelat.ToString() + '#';
            str += nelng.ToString();
            SendMessage(str);
        }
        public void Log()
        {
            string str = "LOGDATA@";
            
            SendMessage(str);
        }
        public void AllData()
        {
            string str = "ALLDATA@";
            
            SendMessage(str);
        }


        public void PaserByteData(byte[] data)
        {
            Dele de = new Dele(form.CheckMarker);
            string msg = Encoding.Default.GetString(data);
            string[] token = msg.Split('@');

            switch (token[0].Trim())
            {
                case "START": s.Clear(); break;
                case "DATA": AddData(token[1]); break;
                case "ADATA": AllData(token[1]); break;
                case "LOG": LogData(token[1]); break;
                //case "END": form.Invoke(de); break;
                case "END": form.Invoke(de); break;
            }
        }

        private void AddData(string data)
        {
            string[] sp = data.Split('#');
            ChargeStation ch = new ChargeStation(sp[0], sp[1], sp[2], float.Parse(sp[3]), float.Parse(sp[4]), sp[5], int.Parse(sp[6])
                                                , int.Parse(sp[7]), int.Parse(sp[8]), sp[9]);
            s.Add(ch);
        }

        private void AllData(string data)
        {
            string[] sp = data.Split('#');
            ChargeStation ac = new ChargeStation(sp[0], sp[1], sp[2], float.Parse(sp[3]), float.Parse(sp[4]), sp[5], int.Parse(sp[6])
                                                , int.Parse(sp[7]), int.Parse(sp[8]), sp[9]);
            chlist.Add(ac);
             
        }
        private void LogData(string data)
        {
            string[] sp = data.Split('#');
            Log lg = new Log(sp[0], int.Parse(sp[1]), int.Parse(sp[2]));

            lglist.Add(lg);
        }
    }


}
