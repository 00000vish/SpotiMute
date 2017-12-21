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
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        static void Main(string[] args)
        {
            alreadyRunning();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(getSpotifyProcessId()));           
            
        }

        public static void alreadyRunning()
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process item in proc)
            {
                if(item.ProcessName.Equals("SpotiMute"))
                {
                    if(item.Id != Process.GetCurrentProcess().Id)
                    {
                        item.Kill();  
                    }
                }
            }
        }

        public static void spotifyController()
        {
            bool temp = true;
            foreach (var process in Process.GetProcessesByName("Spotify"))
            {                
                SetForegroundWindow(process.MainWindowHandle);
                SendKeys.SendWait(" ");
                System.Threading.Thread.Sleep(700);
                SendKeys.SendWait(" ");
                temp = false;               
            }
            if(Properties.Settings.Default.restartSpotify && temp) System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe");
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