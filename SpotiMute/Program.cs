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
            initLogics();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(getSpotifyProcessId()));

        }

        public static void initLogics()
        {
            alreadyRunning();
            startSpotify();
        }

        //starts spotify
        public static void startSpotify()
        {
            if (Properties.Settings.Default.restartSpotify) {
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\SpotifyLauncher.exe");
            }
            
        }

        //if the program is already running its closed
        public static void alreadyRunning()
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process item in proc)
            {
                if (item.ProcessName.Equals("SpotiMute"))
                {
                    if (item.Id != Process.GetCurrentProcess().Id)
                    {
                        item.Kill();
                    }
                }
            }
        }

        //get all the process ID from the all the spotify process
        public static int[] getSpotifyProcessId()
        {
            int index = 0;
            Process[] process = Process.GetProcessesByName("Spotify");
            int[] pID = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            foreach (Process item in process)
            {
                if (item.Id.ToString() != "")
                {
                    pID[index] = item.Id;
                    index++;
                }
            }
            return pID;
        }

        //mutes all spotify process in the id array
        public static void muteSpotify(int[] id)
        {
            foreach (var item in id)
            {
                if (item != -1)
                    VolumeMixer.SetApplicationVolume(item, 0f);
            }

        }

        //unmute all spotify process in the id array
        public static void unMuteSpotify(int[] id)
        {
            foreach (var item in id)
            {
                if (item != -1)
                    VolumeMixer.SetApplicationVolume(item, 100f);
            }
        }

        //getWindow title of spotify
        public static string GetWindowTitle(int processId)
        {
            return Process.GetProcessById(processId).MainWindowTitle;
        }
    }
}