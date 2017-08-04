using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotiMute
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        static void Main(string[] args)
        {
            if(Properties.Settings.Default.restartSpotify)restartSpotify();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(getSpotifyProcessId()));
        }

        public static void restartSpotify()
        {
            bool temp = true;
            foreach (var process in Process.GetProcessesByName("Spotify"))
            {
                temp = false;
                //SendKeys.SendWait(Keys.MediaPlayPause.ToString());
                //process.Kill();
                //TODO pause spotify so it can be detected and then play 
            }
            if(temp)System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe");
        }

        public static int getSpotifyProcessId()
        {
            var hWnd = FindWindow("SpotifyMainWindow", "Spotify");
            if (hWnd == IntPtr.Zero)
                return -1;

            uint pID;
            GetWindowThreadProcessId(hWnd, out pID);
            if (pID == 0)
                return -1;
            return (int)pID;
        }

        public static void muteSpotify(int id)
        {
            VolumeMixer.SetApplicationVolume(id, 0f);
        }
        public static void unMuteSpotify(int id)
        {
            VolumeMixer.SetApplicationVolume(id, 100f);
        }

        public static string GetWindowTitle(int processId)
        {
            return Process.GetProcessById(processId).MainWindowTitle;
        }
    }
}