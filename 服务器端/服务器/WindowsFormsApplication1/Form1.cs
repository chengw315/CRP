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
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private CRPSever server;
        private void Form1_Load(object sender, EventArgs e)
        {
            server = CRPSever.defaultSever;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(listenTCP).Start();
            new Thread(listenUDP).Start();
            this.Hide();
            new Form2().Show();
        }

        public void listenTCP() {
            server.startListening(getTCPPort());
        }
        public void listenUDP()
        {
            server.startUDPListening(getUDPPort());
        }

        private int getUDPPort()
        {
            return getTCPPort() + 1;
        }

        private int getTCPPort() {
            //return int.Parse(textBox1.Text);
            return 4556;
        }

        private string getRandomName()
        {
            byte[] nameBytes = new byte[40];
            new Random().NextBytes(nameBytes);
            return (BitConverter.ToString(nameBytes));
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
