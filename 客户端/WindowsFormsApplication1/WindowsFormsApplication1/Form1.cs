using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Controls;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private CRPClient client;

        private void Form1_Load(object sender, EventArgs e)
        {
            client = CRPClient.defaultClient;
            client.setLoginHandler(delegate(bool isSuccess,string info) {
                enableButton1();
                if (isSuccess)
                    goForm2();
                else
                    MessageBox.Show(info, "51213");
            });
        }

        private void goForm2()
        {
            if (this.InvokeRequired)
            {
                Action action = () =>
                {
                    this.Hide();
                    new Form2().Show();
                };
                this.Invoke(action);
            }
            else
            {
                this.Hide();
                new Form2().Show();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            client.setMyUDPPort(getUDPPort());
            client.login(getName(), getIp(), 4556, 4557);
        }
        private void enableButton1() {
            if (button1.InvokeRequired) {
                Action a = () => { button1.Enabled = true; };
                button1.Invoke(a);
            } else
                button1.Enabled = true;
        }

        private string getIp()
        {
            return textBox2.Text;
        }

        private int getUDPPort()
        {
            return int.Parse(textBox3.Text);
        }

        private string getName() {
            return textBox1.Text != "" ? textBox1.Text : getRandomName();
        }

        private string getRandomName()
        {
            byte[] nameBytes = new byte[40];
            new Random().NextBytes(nameBytes);
            return (BitConverter.ToString(nameBytes));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //byte b = 255;
            //MessageBox.Show(((int)b).ToString());

            //int i = 0, j = 0;
            //MessageBox.Show("1++",i++.ToString());
            //MessageBox.Show("0+=1",(j+=1).ToString());

            //ushort s = 65535;
            //s++;
            //MessageBox.Show(s.ToString());
        }

    }
}
