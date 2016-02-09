using KeyboardListen;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            File.Delete("test.txt");

            //KeyboardEvent listen1 = new KeyboardEvent();
            //KeyboardEvent listen2 = new KeyboardEvent("F1");
            Process process = Process.GetCurrentProcess();
            //KeyboardEvent listen3 = new KeyboardEvent(null, process.Id);
            KeyboardEvent listen4 = new KeyboardEvent("F1", process.Id);
            KeyboardEvent.KeyEvent += KeyboardEvent_KeyEvent;
        }

        private void KeyboardEvent_KeyEvent(object sender, EventArgs e)
        {
            using (StreamWriter file = new StreamWriter("test.txt", true))
            {
                file.WriteLine(Convert.ToString(sender));
            }
        }
    }
}
