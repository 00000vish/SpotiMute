using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotiMute
{
    public partial class Form1 : Form
    {
        bool spotifyMuted = false;
        int spotifyID = 0;

        public Form1(int inSpotifyId)
        {
            InitializeComponent();               
            spotifyID = inSpotifyId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.muteSpotify(spotifyID);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.unMuteSpotify(spotifyID);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Program.GetWindowTitle(spotifyID));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label1.Text = "✘ SpotiMute stopped";
                label1.ForeColor = Color.Red;
                timer1.Stop();                
            }
            else
            {
                label1.Text = "✔ SpotiMute running";
                label1.ForeColor = Color.Green;
                timer1.Start();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void openSpotiMuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Show();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Show();
        }    

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Program.GetWindowTitle(spotifyID).Contains("-"))
            {
                if (spotifyMuted)
                {
                    Program.unMuteSpotify(spotifyID);
                    spotifyMuted = false;
                }                
            }
            else
            {
                Program.muteSpotify(spotifyID);
                spotifyMuted = true;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
