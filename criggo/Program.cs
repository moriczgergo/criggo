using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace criggo
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = "0.1.0";

            Console.Write("Server address: ");
            var addr = Console.ReadLine();
            var entryAddr = addr;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(entryAddr);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();

            var entryReader = new StreamReader(resStream, Encoding.UTF8);

            string resJson = entryReader.ReadToEnd();

            entryResponse res = JsonConvert.DeserializeObject<entryResponse>(resJson);

            if (res.status == "alive")
            {
                Console.WriteLine("Server status is alive, proceeding.");
                if (res.ver != version)
                {
                    Console.WriteLine("Client version is " + version + ", server version is " + res.ver + ". Press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(1);
                } else
                {
                    Console.WriteLine("Client and server versions matching.");
                }
            } else
            {
                Console.WriteLine("Server status is " + res.status + ", press any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Console.Write("Username: ");
            var username = Console.ReadLine();
            var gameStartAddr = addr + "/gs/?username=" + username;

            request = (HttpWebRequest)WebRequest.Create(gameStartAddr);
            response = (HttpWebResponse)request.GetResponse();
            resStream = response.GetResponseStream();

            entryReader = new StreamReader(resStream, Encoding.UTF8);

            resJson = entryReader.ReadToEnd();

            gameStartResponse gsRes = JsonConvert.DeserializeObject<gameStartResponse>(resJson);

            while (true)
            {
                if (gsRes.error != null)
                {
                    Console.WriteLine("Game Start reponse was " + resJson + ", press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(2);
                }
                else
                {
                    Console.WriteLine("Game Start response was valid!");
                }

                Thread.Sleep(2000);

                Console.Clear();

                foreach (string line in gsRes.ttp)
                {
                    Console.WriteLine(line);
                }

                Console.WriteLine("");

                if (gsRes.halt)
                {
                    Console.WriteLine("Game Start returned halt, press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                if (gsRes.keyAnswer)
                {
                    var key = Console.ReadKey();
                    var updateAddr = addr + "/updat/?key=" + key;
                    request = (HttpWebRequest)WebRequest.Create(updateAddr);
                    response = (HttpWebResponse)request.GetResponse();
                    resStream = response.GetResponseStream();

                    entryReader = new StreamReader(resStream, Encoding.UTF8);

                    resJson = entryReader.ReadToEnd();

                    gsRes = JsonConvert.DeserializeObject<gameStartResponse>(resJson);
                } else
                {
                    break;
                }
            }

            Console.WriteLine("EOP");
            Console.ReadKey();
        }
    }

    class entryResponse
    {
        public string status { get; set; }
        public string ver { get; set; }
    }

    class gameStartResponse
    {
        public string error { get; set;  }
        public string[] ttp { get; set;  }
        public bool keyAnswer { get; set; }
        public bool halt { get; set; }
    }
}
