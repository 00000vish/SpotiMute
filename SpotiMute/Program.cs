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
            var hWnd = FindWindow("SpotifyMainWindow", "Spotify");
            if (hWnd == IntPtr.Zero)
                return;

            uint pID;
            GetWindowThreadProcessId(hWnd, out pID);
            if (pID == 0)
                return;
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1((int)pID));
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

//