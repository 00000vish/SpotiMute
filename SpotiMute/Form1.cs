using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        int[] spotifyID = null;

        //init logics
        public Form1(int[] inSpotifyId)
        {
            spotifyID = inSpotifyId;
            InitializeComponent();
            initLogics();
            startMuter();
        }

        private void initLogics()
        {
            Program.muteSpotify(spotifyID);
            Program.unMuteSpotify(spotifyID); //sometimes the initial unmute does not get registered idk why            
            checkBox2.Checked = Properties.Settings.Default.startWindow;
            checkBox3.Checked = Properties.Settings.Default.restartSpotify;
        }

        private void startMuter()
        {
            if (spotifyID[0] == -1)
            {
                label2.Text = "✘ waiting for Spotify...";
                label2.ForeColor = Color.Red;
                checkBox1.Checked = true;
                checkBox1.Enabled = false;                
                linkLabel2.Visible = true;
                linkLabel2.Text = "(Luanch Spotify)";
                timer2.Start();
            }
            else
            {
                label2.Text = "✔ Spotify detected";
                label2.ForeColor = Color.Green;
                checkBox1.Enabled = true;
                checkBox1.Checked = false;                
                linkLabel2.Visible = false;
            }
        }

        //controls is spotiMute is enabled or not
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

        //stop from closing when x is clicked, instead hides
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        //close when quit is clicked
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        //show when open button is clicked
        private void openSpotiMuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        //show when double click on icon
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }

        //decide if ad is playing or not and control mute and unmute 
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool adPlaying = true;
            string songName = "";
            try
            {
                foreach (var item in spotifyID)
                {
                    if (item != -1)
                    {
                        String tempsongName = Program.GetWindowTitle(item);
                        if (tempsongName.Contains(" - "))
                        {
                            songName = tempsongName;

                            adPlaying = false;

                        }
                    }
                }

                if (!adPlaying)
                {
                    Text = "♫ " + songName + " ♫";
                    linkLabel3.Tag = songName;
                    linkLabel3.Visible = true;
                    linkLabel4.Visible = true;
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
                    linkLabel3.Visible = false;
                    linkLabel4.Visible = false;
                }
            }
            catch (ArgumentException) { spotifyID[0] = -1; startMuter(); }
        }
        
        //auto start spotify
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

        //add autostart registry key
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
                catch (Exception) { }
            }
            else
            {
                Properties.Settings.Default.startWindow = false;
                try
                {
                    regKey.DeleteValue("SpotiMute", true);
                }
                catch (Exception) { }
            }
            Properties.Settings.Default.Save();
            regKey.Close();
        }

        //luanch spotify
        private void linkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\SpotifyLauncher.exe");
        }

        //lyrics link
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://genius.com/search?q=" + linkLabel3.Tag.ToString());
        }

        //youtube link
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/results?search_query=" + linkLabel3.Tag.ToString());
        }

        //github link
        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/vishwenga/SpotiMute");
        }

        //when spotiMute is waiting for spotify to start
        private void timer2_Tick(object sender, EventArgs e)
        {
            spotifyID = Program.getSpotifyProcessId();
            startMuter();
        }
    }
}
