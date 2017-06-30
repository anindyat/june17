using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "atprac1204-iot.azure-devices.net";
        static string deviceKey = "hVwnCs7fSSB4Y3lNad2/w/lAOC/4+CmN7CQIA6O5wdk=";
        static string device_id = "andytFirstDevice";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated Device to send Alarm and Performance messages to IoT Hub\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(device_id, deviceKey), TransportType.Mqtt);

            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            string directory = Path.GetDirectoryName(path);
            while (true)
            {
                Console.WriteLine("Press A for Alarm or P for Performance data or Escape to end");
                ConsoleKeyInfo info = Console.ReadKey();
                string fullpath = String.Empty;
                bool found = false;
                if (info.KeyChar == 'A' || info.KeyChar == 'a')
                {
                    fullpath = directory + @"\Sample\rem_alarm.proto";
                    found = true;
                }
                else if (info.KeyChar == 'P' || info.KeyChar == 'p')
                {
                    fullpath = directory + @"\Sample\rem_message.proto";
                    found = true;
                }
                else if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid Charater try again");
                }

                if (found)
                {
                    Uri uri = new Uri(fullpath);
                    //SendMessageAsync(FileToByteArray(uri.LocalPath));
                    SendMessageAsync(File.ReadAllBytes(uri.LocalPath));
                    Console.WriteLine();
                    Console.WriteLine("Message Sent");
                }
            }
        }

        private static async void SendMessageAsync(byte[] protomessage)
        {
            var message = new Message(protomessage);
            message.Properties.Add("Component", "g3ms");
            message.Properties.Add("Channel", "remstatus");
            message.Properties.Add("SessionID", Guid.NewGuid().ToString());
            message.Properties.Add("Priority", string.Empty);

            await deviceClient.SendEventAsync(message);
        }
    }
}
