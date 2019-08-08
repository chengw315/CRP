using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        private CRPSever server;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = getMessage();
            if(checkMessage(message))
                server.forwardMessage("服务器:"+message);
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
            server = CRPSever.defaultSever;
            server.setAddUserHandler(delegate(MySocket socket) {
                addUser(socket);
            });
            server.setRemoveUserHandler(delegate (MySocket socket)
            {
                deleteUser(socket);
            });
            server.setVideoFrameHandler(delegate (Bitmap image)
            {
                pictureBox1.Image = image;
            });
            server.setMessageHandler(delegate (string message)
            {
                showMessage(message);
            });
            server.setVideoStartHandler(delegate(MySocket socket) {
                setLivingName(socket.Name);
            });
            server.setVideoStopHandler(delegate () {
                setLivingName("");
            });
        }

        private void showMessage(string message)
        {
            if (textBox3.InvokeRequired)
            {
                Action<string> a = (x) =>
                {
                    textBox3.AppendText(x);
                    textBox3.AppendText("\r\n");
                };
                textBox3.Invoke(a, message);
            }
            else
            {
                textBox3.AppendText(message);
                textBox3.AppendText("\r\n");
            }
        }
        private void setLivingName(string name) {
            if (label1.InvokeRequired)
            {
                Action<string> a = (x) =>
                {
                    label1.Text = x;
                };
                label1.Invoke(a, name);
            }
            else
                label1.Text = name;
        }
        private void deleteUser(MySocket socket)
        {
            if (dataGridView1.InvokeRequired)
            {
                Action<MySocket> action = (x) =>
                {
                    dataGridViewDeleteUser(x);
                };
                dataGridView1.Invoke(action, socket);
            }
            else
                dataGridViewDeleteUser(socket);
        }
        private void dataGridViewDeleteUser(MySocket socket)
        {
            for(int i = 0; i < dataGridView1.Rows.Count; i++) {
                if (dataGridView1.Rows[i].Tag.ToString() == socket.Name) {
                    dataGridView1.Rows.RemoveAt(i);
                }
            };
        }
        private void addUser(MySocket socket)
        {
            if (dataGridView1.InvokeRequired)
            {
                Action<MySocket> action = (x) =>
                {
                    dataGridViewAddUser(x);
                };
                dataGridView1.Invoke(action, socket);
            }
            else
                dataGridViewAddUser(socket);
        }
        private void dataGridViewAddUser(MySocket socket)
        {            
            DataGridViewRow row = new DataGridViewRow();
            var data1 = new DataGridViewTextBoxCell();
            data1.Value = socket.Name;
            row.Cells.Add(data1);
            var data2 = new DataGridViewTextBoxCell();
            data2.Value = socket.getSocketIP().ToString();
            row.Cells.Add(data2);
            var data3 = new DataGridViewTextBoxCell();
            data3.Value = socket.getTCPPort().ToString();
            row.Cells.Add(data3);
            var data4 = new DataGridViewTextBoxCell();
            data4.Value = socket.getUDPPort().ToString();
            row.Cells.Add(data4);
            row.Tag = socket.Name;
            dataGridView1.Rows.Add(row);    
        }
    }
}
