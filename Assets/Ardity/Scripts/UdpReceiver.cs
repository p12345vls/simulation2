using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


    public class UdpReceiver : MonoBehaviour
    {
    
        private Thread _receiveThread;
        private UdpClient _client;
        private int port = 8051;
        private readonly Process _process = new Process();
        public float[] udpData;
    
    
        void Start()
        {
            udpData = new float[14];
            UdpStart();
            _receiveThread = new Thread(ReceiveData) {IsBackground = true};
            _receiveThread.Start();
        }
    
        private void ReceiveData()
        {
            _client = new UdpClient(port);
            while (true)
            {
                var anyIP = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    var receive = _client.Receive(ref anyIP);
                    var encodedText = Encoding.UTF8.GetString(receive).Split(',');
                    for (var i = 0; i < udpData.Length; i++)
                    {
                        udpData[i] = float.Parse(encodedText[i]);
                    }
                }
                catch (Exception err)
                {
                    if (!err.ToString().Contains("System.Threading.ThreadAbortException"))
                        print(err.ToString());
                }
            }

        }

        private void UdpStart()
        {
            try
            {
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.FileName = @"C:\windows\system32\dist\realsense_python.exe";
                _process.StartInfo.CreateNoWindow = true;
                _process.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private void OnApplicationQuit()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.Equals("realsense_python"))
                {
                    process.Kill();
                }
            }
        }
    }

