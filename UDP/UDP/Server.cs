using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace UDP
{
    public class Server
    {
        private const int PORT = 9000;

        //Stuff to receive
        private bool done;
        private string received_data;
        private byte[] receive_byte_array;
        private UdpClient listener;
        private IPEndPoint groupEP;

        
        //Stuff to send
        private Socket sending_socket;
        private Boolean exception_thrown;
        private IPAddress send_to_address;
        private IPEndPoint sending_end_point;

        public Server()
        {
            done = false;
            listener = new UdpClient(PORT);
            groupEP = new IPEndPoint(IPAddress.Any, PORT);

            //Stuff to send
            sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                ProtocolType.Udp);
            done = false;
            exception_thrown = false;
            send_to_address = IPAddress.Parse("10.0.0.1");
            sending_end_point = new IPEndPoint(send_to_address, PORT);
        }

        public void Start()
        {
            try
            {

                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast...");
                    
                    // Blocking call
                    receive_byte_array = listener.Receive(ref groupEP);
                    Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("Data: {0}\n", received_data);

                    switch (received_data)
                    {
                        case "u":
                        {
                            SendMsg(UpTime());
                        }
                        break;

                        case "U":
                        {   
                            SendMsg(UpTime());
                        }
                        break;

                        case "l":
                        {
                            SendMsg(AvgLoad());
                        }
                        break;

                        case "L":
                        {
                            SendMsg(AvgLoad());
                        }
                        break;

                        default:
                        {
                            Console.WriteLine("Data didn't match any commands");
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           listener.Close();
        }

        private void SendMsg(string text_to_send)
        {
            byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

            // Remind the user of where this is going.
            Console.WriteLine("Sending to address: {0} port: {1}",
                sending_end_point.Address,
                sending_end_point.Port);
            try
            {
                sending_socket.SendTo(send_buffer, sending_end_point);
            }
            catch (Exception send_exception)
            {
                exception_thrown = true;
                Console.WriteLine(" Exception {0}", send_exception.Message);
            }

            if (exception_thrown == false)
            {
                Console.WriteLine("Message has been sent to the broadcast address");
            }
            else
            {
                exception_thrown = false;
                Console.WriteLine("The exception indicates the message was not sent.");
            }
        }

        private string AvgLoad()
        {
            string text;

            try
            {
                using (StreamReader sr = new StreamReader("/~/../proc/loadavg"))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "File wasn't found"; 
            }

            return text;
        }

        private string UpTime()
        {
            string text;

            try
            {
                using (StreamReader sr = new StreamReader("/~/../proc/uptime"))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "File wasn't found";
            }
            
            return text;
        }
    }
}
