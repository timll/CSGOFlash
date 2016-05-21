using System;
using Newtonsoft.Json.Linq;
using CSGOGameObserverSDK;
using CSGOGameObserverSDK.GameDataTypes;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace CSGOFlash
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        static Process csgo;

        static bool FindCSGO()
        {
            Process[] ps = Process.GetProcessesByName("csgo");
            if (ps.Length > 1)
            {
                foreach (Process p in ps)
                {
                    if (p.MainWindowTitle == "Counter-Strike: Global Offensive")
                    {
                        csgo = p;
                        Console.WriteLine("CSGO FOUND");
                        return true;
                    }
                }
                return false;
            }
            else if (ps.Length == 1)
            {
                csgo = ps[0];
                Console.WriteLine("CSGO FOUND");
                return true;
            }
            else
            {
                Console.WriteLine("CSGO NOT FOUND");
                return false;
            }
        }

        static void RunServer()
        {
            var server = new CSGOGameObserverServer("http://127.0.0.1:3000/");
            server.receivedCSGOServerMessage += OnRecievedCSGOServerMessage;
            server.Start();
        }

        static void OnRecievedCSGOServerMessage(object sender, JObject gameData)
        {
            CSGOGameState csgoGameState = new CSGOGameState(gameData);

            if (csgoGameState.Round != null && csgoGameState.Round.Phase == "freezetime" && IsIconic(csgo.MainWindowHandle))
            {
                FlashWindow.Flash(csgo);
                Console.WriteLine("Flash");
            }
        }
   

        static void Main(string[] args)
        {
            while (!FindCSGO())
                Thread.Sleep(1000);

            RunServer();

            System.Windows.Forms.Application.Run();
        }
    }
}
