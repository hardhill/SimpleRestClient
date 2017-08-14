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

        private async void GetDataFromService()
        {
            Guid genstr = Guid.NewGuid();
            label1.Text = "";       // clear text befor get data
            string JsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(genstr));
            WebRequest request = WebRequest.Create("http://service.bielecki.ru/test");
            request.ContentType = "application/json";
            string query = $"msg=" + JsonString;
            byte[] contentBytes = Encoding.UTF8.GetBytes(query);
            request.ContentLength = contentBytes.Length;
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
            }
            WebResponse response = await request.GetResponseAsync();
            string answer = null;
            using(Stream stream = response.GetResponseStream())
            {
                using(StreamReader sr = new StreamReader(stream))
                {
                    answer = await sr.ReadToEndAsync();
                }
            }


        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            GetDataFromService();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetDataFromService();
        }
    }
}
