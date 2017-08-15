using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace SimpleClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async Task<TestList> GetDataFromService()
        {
            string surl = "http://localhost:46592";
            string surd = "http://service.bielecki.ru/test/index.php";
            Guid genstr = Guid.NewGuid();
            label1.Text = "";       // clear text befor get data
            string JsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(genstr));
            WebRequest request = WebRequest.Create(surd);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";     //на этот сервис важно отправлять данные именно с таким типом контента     
            string query = $"msg={JsonString}";
            byte[] contentBytes = Encoding.UTF8.GetBytes(query);
            request.ContentLength = contentBytes.Length;
            
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
            }
            WebResponse response = await request.GetResponseAsync();
            string answerjson = null;
            using(Stream stream = response.GetResponseStream())
            {
                using(StreamReader sr = new StreamReader(stream))
                {
                    answerjson = await sr.ReadToEndAsync();
                }
            }
            response.Close();
            var answerList = await Task.Factory.StartNew(()=>JsonConvert.DeserializeObject<TestList>(answerjson));
            return answerList;
        }
        private async void Form1_Shown(object sender, EventArgs e)
        {
            TestList testList = await GetDataFromService();
            label1.Text = testList.tm;
            label2.Text = testList.guid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1_Shown(sender, e);
        }
    }

    internal class TestList
    {
        [JsonProperty("status")]
        public string msg { get; set; }

        [JsonProperty("time")]
        public string tm { get; set; }

        [JsonProperty("guid")]
        public string guid { get; set; }
    }
}
