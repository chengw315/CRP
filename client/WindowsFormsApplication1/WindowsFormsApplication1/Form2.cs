using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        private CRPClient client;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = getMessage();
            if(checkMessage(message))
                client.sendMessage(message);
        }

        private bool checkMessage(string message)
        {
            return message != "";
        }

        private string getMessage()
        {
            return textBox1.Text;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            client = CRPClient.defaultClient;
            client.setMessageHandler(delegate(string message) {
                showMessage(message);
            });
            client.setStartVideoHandler(delegate (bool isSuccess, string info)
            {
                if (isSuccess)
                    startLiveVideo();
                else
                    MessageBox.Show(info);
            });
            client.setVideoFrameHandler(delegate (Bitmap image)
            {
                showFrame(image);
            });
        }

        private void showFrame(Bitmap image)
        {
            if (pictureBox1.InvokeRequired) {
                Action<Bitmap> showImage = (x) => { pictureBox1.Image = image; };
                pictureBox1.Invoke(showImage, image);
            }
            else
                pictureBox1.Image = image;
        }

        private void showMessage(string message)
        {
            if (textBox2.InvokeRequired)
            {
                Action<string> showMessage = (x) =>
                {
                    textBox2.AppendText(x);
                    textBox2.AppendText("\r\n");
                };
                textBox2.Invoke(showMessage, message);
            }
            else
            {
                textBox2.AppendText(message);
                textBox2.AppendText("\r\n");
            }
        }

        int i = 0;
        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            if (i != 0)
            {
                i--;
            }
            else
            {
                client.sendVideoFrame(image);
                i = 2;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.sendRequestVideoStart();
        }

        public void startLiveVideo()
        {
            if (pictureBox1.InvokeRequired)
            {
                Action a = () => { pictureBox1.Visible = false; };
                pictureBox1.Invoke(a);
            }
            videoSourcePlayer1.VideoSource = (new VideoCaptureDevice(getDeviceName()));
            videoSourcePlayer1.Start();
        }

        private string getDeviceName()
        {
            return new FilterInfoCollection(FilterCategory.VideoInputDevice)[0].MonikerString;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            videoSourcePlayer1.Stop();
            client.sendRequestVideoStop();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
