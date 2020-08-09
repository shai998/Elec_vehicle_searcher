using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1002
{
    class kakao
    {
        #region 서비스 참조 Windows -> system32-> wmp.dll
        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        #endregion

        public void ConnectApi(string str)
        {


            try
            {
                #region 윈도우플레이어
                wplayer.close();
                string VoiceName = "MAN_READ_CALM";
                //WOMAN_READ_CALM: 여성 차분한 낭독체(default)
                //MAN_READ_CALM: 남성 차분한 낭독체
                //WOMAN_DIALOG_BRIGHT : 여성 밝은 대화체
                //MAN_DIALOG_BRIGHT : 남성 밝은 대화체

                string speed = "fast";
                string power = "loud";
                #endregion

                #region 조건
                #endregion

                #region 카카오API 연결
                string url = "https://kakaoi-newtone-openapi.kakao.com/v1/synthesize";
                //string query = string.Format("{0}?query={1}", url, input);        //종류에 따른 주소 지정
                string saveFileFullPath = @"C:\Users\kmh\Desktop\공모전 및 경진대회\오픈소스 경진대회 자료\오픈소스 경진대회\Test.wav";
                WebRequest request = WebRequest.Create(url);                        //서버로 요청

                string rest_api_key = "5130c28817920a6aa5f5cd24db39b550";   //자신의 키값과 헤더값 입력
                string header = "KakaoAK " + rest_api_key;
                request.Method = "POST";                                    //부가정보?
                request.ContentType = "application/xml";
                request.Headers.Add("Authorization", header);

                //byte[] byteDataParams = Encoding.UTF8.GetBytes("<speak><voice name='" + VoiceName + "'>" + inputText + "</voice></speak>"); //요청 음성형식같음.
                byte[] byteDataParams = Encoding.UTF8.GetBytes("<speak><prosody rate='" + speed + "' volume = '" + power + "'>" + str + "</prosody></speak>"); //요청 음성형식같음 SSML
                request.ContentLength = byteDataParams.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(byteDataParams, 0, byteDataParams.Length);
                stream.Close();

                #endregion
                //request.ContentType;
                #region 카카오API 응답
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //서버로부터의 응답
                string status = response.StatusCode.ToString();
                //MessageBox.Show("Status" + status);
                File.Delete(saveFileFullPath);

                using (Stream output = File.OpenWrite(saveFileFullPath))
                using (Stream input = response.GetResponseStream())
                {
                    input.CopyTo(output);
                }
                wplayer.URL = saveFileFullPath;

                //textBox1.Text = string.Empty;
                #endregion

                MessageBox.Show("succecs");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
