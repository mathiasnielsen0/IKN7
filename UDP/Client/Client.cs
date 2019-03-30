using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public class Client
    {
        //Stuff for client
        private const int PORT = 9000;
        private Boolean done;

        private Socket sending_socket;
        private Boolean exception_thrown;
        private IPAddress send_to_address;
        private IPEndPoint sending_end_point;

        //Stuff to receive
        private string received_data;
        private byte[] receive_byte_array;
        private UdpClient listener;
        private IPEndPoint groupEP;

        

        public Client()
        {
            sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            done = false;
            exception_thrown = false;
            send_to_address = IPAddress.Parse("10.0.0.2"); 
            sending_end_point = new IPEndPoint(send_to_address, PORT);

            //Stuff to receive
            listener = new UdpClient(PORT);
            groupEP = new IPEndPoint(IPAddress.Any, PORT);
        }

        public void Start()
        {
            Console.WriteLine("Write a text to the server. 'u' for uptime and 'l' for loadavg.");
            Console.WriteLine("Enter text to send, blank line to quit");
            while (!done)
            {
                string text_to_send = Console.ReadLine();
                if (text_to_send.Length == 0)
                {
                    done = true;
                }
                else
                {
                    byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

                    Console.WriteLine("sending to address: {0} port: {1}",
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
                        if (text_to_send == "u" || text_to_send == "U" || text_to_send == "l" || text_to_send == "L")
                        {
                            Console.WriteLine("Waiting for response from server");
                            ReceiveMsg();
                        }
                    }
                    else
                    {
                        exception_thrown = false;
                        Console.WriteLine("The exception indicates the message was not sent.");
                    }
                    Console.WriteLine();
                }
            }
        }

        private void ReceiveMsg()
        {
            receive_byte_array = listener.Receive(ref groupEP);
            Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
            received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
            Console.WriteLine("Data: \n{0}\n\n", received_data);
        }
    }
}

