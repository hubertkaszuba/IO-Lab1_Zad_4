using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace IO_Lab_1_Zad_4
{
    class Program
    {
        public static readonly object Lock = new object();

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ThreadProc_server);
            ThreadPool.QueueUserWorkItem(ThreadProc_client, new object[] { 1 });
            ThreadPool.QueueUserWorkItem(ThreadProc_client, new object[] { 2 });
            Thread.Sleep(5000);
        }
        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            lock (Lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        static void ThreadProc_client(Object stateInfo)
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));

        

            var nr = ((object[])stateInfo)[0];
            byte[] message = new ASCIIEncoding().GetBytes("Wiadomosc od watku nr: " + nr);
            byte[] buffer = new byte[1024];

            
            client.GetStream().Write(message, 0, message.Length);
            client.GetStream().Read(buffer, 0, 1024);

            
            writeConsoleMessage(new ASCIIEncoding().GetString(buffer, 0, buffer.Length), ConsoleColor.Green);
        }

        static void ThreadProc_server(Object stateInfo)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                Thread.Sleep(1000);               
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Nawiazano polaczenie z klientem");              
                ThreadPool.QueueUserWorkItem(ThreadProc_connection, client);

            }
        }
        static void ThreadProc_connection(object stateInfo)
        {
            byte[] buffer = new byte[1024];
            byte[] message = new ASCIIEncoding().GetBytes("Wiadomosc od serwera");

            var client = (TcpClient)stateInfo;
            client.GetStream().Read(buffer, 0, buffer.Length);
            client.GetStream().Write(message, 0, message.Length);
            
            writeConsoleMessage(new ASCIIEncoding().GetString(buffer, 0, buffer.Length), ConsoleColor.Red);
            client.Close();           
        }



    }
}
