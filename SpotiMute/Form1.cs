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
        bool autoHide = true;
        bool spotifyMuted = false;
        int spotifyID = 0;

        public Form1(int inSpotifyId)
        {
            InitializeComponent();
            spotifyID = inSpotifyId;
            checkBox3.Checked = Properties.Settings.Default.restartSpotify;
            startMuter();
        }

        private void startMuter()
        {
            if (spotifyID == -1)
            {
                label2.Text = "✘ waiting for Spotify... (Spotify already running? click here)";
                label2.ForeColor = Color.Red;
                checkBox1.Checked = true;
                checkBox1.Enabled = false;
                timer2.Enabled = true;
            }
            else
            {                
                label2.Text = "✔ Spotify detected";
                label2.ForeColor = Color.Green;
                checkBox1.Enabled = true;
                checkBox1.Checked = false;
                timer2.Enabled = false;
            }
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
                Program.unMuteSpotify(spotifyID);
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
            autoHide = true;
            e.Cancel = true;
            Hide();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void openSpotiMuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSpotiMute();
        }

        private void showSpotiMute()
        {
            autoHide = false;
            WindowState = FormWindowState.Normal;
            Show();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            showSpotiMute();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Program.GetWindowTitle(spotifyID).Contains("-"))
                {
                    Text = "♫ " + Program.GetWindowTitle(spotifyID) + " ♫";
                    if (spotifyMuted)
                    {
                        Program.unMuteSpotify(spotifyID);
                        spotifyMuted = false;
                    }
                }
                else
                {
                    Text = "SpotiMute";
                    Program.muteSpotify(spotifyID);
                    spotifyMuted = true;
                }
            }
            catch (ArgumentException){ spotifyID = -1; startMuter(); }           
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if(autoHide) Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (label2.Text == "✘ waiting for Spotify... (Spotify already running? click here)")
            {
                MessageBox.Show(null,"If spotify is already playing music, puase it just for a second :)","SpotiMute",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }          
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/vishwenga/SpotiMute");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            spotifyID = Program.getSpotifyProcessId();
            startMuter();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                Properties.Settings.Default.restartSpotify = true;
            }
            else
            {
                Properties.Settings.Default.restartSpotify = false;
            }
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey regKey = default(Microsoft.Win32.RegistryKey);
            regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (checkBox2.Checked)
            {
                Properties.Settings.Default.startWindow = true;
                try
                {                                  
                    string KeyName = "SpotiMute";
                    string KeyValue = Application.ExecutablePath;                    
                    regKey.SetValue(KeyName, KeyValue, Microsoft.Win32.RegistryValueKind.String);                    
                }
                catch (Exception){}
            }
            else
            {
                Properties.Settings.Default.startWindow = false;                
                try
                {
                    regKey.DeleteValue("SpotiMute", true);
                }
                catch (Exception){}
            }
            Properties.Settings.Default.Save();
            regKey.Close();
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe");
        }
    }
}
