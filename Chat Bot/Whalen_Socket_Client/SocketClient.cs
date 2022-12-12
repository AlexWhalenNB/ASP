using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Whalen_Socket_Client
{
    public class SocketClient
    {
        public static int Main(string[] args)
        {
            StartClient();
            return 0;
        }

        public static string eof = "<EOF>";
        private static void StartClient()
        {
            // data buffer for incoming data
            byte[] bytes = new byte[1024];

            //variable for input
            string input = "";
            string userName = "";
            string resp = "";

            // Connect to a Remote server
            try
            {
                
                // Establish the remote endpoint for the socket
                // this example uses port 11000 on the local computer
                
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    
                    //get name from user
                    Console.WriteLine("\nHello my name is BeeBoo, what's your name?");
                    userName = Console.ReadLine();
                    Console.WriteLine("\nHi " + userName + " nice to meet you!\n");
                                        
                        while (true)
                        {
                            //user input
                            Console.WriteLine(userName + " What would you like to chat about? Type 'help' if you need help.");
                            while (true)
                            {
                                input = Console.ReadLine();
                                //Send and receive message
                                ProcessExchange(sender, bytes, input);                                
                                
                                break;
                                
                            }

                            if (input.ToLower() == "exit")
                            {
                                Console.WriteLine("Would you like to keep chatting? Type 'exit' to close program.");
                                input = Console.ReadLine();

                                if (input.ToLower() == "exit")
                                {
                                    byte[] msg = Encoding.ASCII.GetBytes("End of Stream" + eof);
                                    int bytesSent = sender.Send(msg);
                                    Console.WriteLine("So long " + userName + "! Lets chat again soon!");
                                    Console.ReadKey();
                                    break;
                                } else
                                {
                                Console.WriteLine("Okay! lets keep chatting!");
                            }
                                    
                            }
                        }
                    
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ProcessExchange(Socket sender, byte[] bytes, string input)
        {

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(input + eof);

            // Send the data through the socket.
            int bytesSent = sender.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("\n{0}\n",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));
        }
    }
}
