using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Whalen_Socket_Listener
{
    public class SocketListener
    {
        public static int Main(string[] args)
        {
            StartServer();
            return 0;
        }

        //incoming data from the client 
        public static string data = null;
        public static string whisper = null;
        public static string eof = "<EOF>";
        private static void StartServer()
        {
            //data buffer for incoming  data
            byte[] bytes = new Byte[1024];
            //variable for substring
            string message = "";
            string userInput = "";
            //list of keywords
            string[] keywords = { "silver", "fizz", "hard coded", "help" , "math", "colors", "exit",
                "red", "crimson", "pink", "poppy", "maroon",
                "blue", "navy", "cyan", "aqua", "turquoise",
                "yellow", "canary", "mustard",
                "orange", "tangerine",
                "green", "olive", "emerald", "jade", 
                "purple", "lilac", "mauve", "lavender", "violet", "indigo",
                "white", "black", "brown" };
            //variable for math
            int num = 0;

            // Establish the loacal endpoint for the socket
            // DNS.GetHostName returns the name of the host running the application
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a new TCP Socket
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Bind Socket to local Endpoint and listen for incoming connections
                listener.Bind(localEndPoint);
                listener.Listen(10);

                //start listening for connections
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // program is suspended while waiting for incoming connection
                    Socket handler = listener.Accept();

                    while (true)
                    {
                        data = null;
                        userInput = null;

                        data = ProcessResponse(handler, bytes);
                        
                        //show data on the console
                        Console.WriteLine("Text received : {0}", data);

                        //create substring for switch
                        message = data.Substring(0, data.IndexOf("<EOF>"));

                        foreach (string keyword in keywords)
                        {
                            if (message.ToLower().Contains(keyword))
                            {
                                userInput = keyword;
                                break;
                            }
                        }

                        //process keyword
                        data = ProcessKeyword(userInput, message);

                        //try math
                        int.TryParse(message, out num);
                        if (num > 0)
                        {
                            int mathed = num * num;
                            data = (num + " to the power of two is equal to " + mathed + "!");
                        }

                        if (data == "End of Stream" + eof)
                        {
                            break;
                        }

                        //Echo the data back to the client
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(msg);                       
                    }                    
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
                
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }

        private static string ProcessKeyword(string userInput, string message)
        {
            switch (userInput)
            {
                case "help":
                    data = "You can ask me about 'colors', ask me to do some 'math', and maybe some other things!";
                    break;
                case "fizz":
                    data = "BUZZ! Ha ha I love that one!";
                    break;
                case "colors":
                    data = "If you tell me your favourite color, I can tell you if I recognize it!";
                    break;
                case "math":
                    data = "if you type in a whole number i can square it!";
                    break;
                case "silver":
                    data = message.Replace(userInput, "Gold");
                    break;
                case "red":
                case "crimson":
                case "pink":
                case "poppy":
                case "maroon":
                    data = ("I know " + userInput + " its a shade of red! \nDo you know what else is red? A apple!");
                    break;
                case "blue":
                case "navy":
                case "cyan":
                case "aqua":
                case "turquoise":
                    data = ("I know " + userInput + " its a shade of blue! \nDo you know what else is blue? Blue cheese!");
                    break;
                case "yellow":
                case "canary":
                case "mustard":
                    data = ("I know " + userInput + " its a shade of yellow! \nDo you know what else is yellow? A Banana!");
                    break;
                case "orange":
                case "tangerine":
                    data = ("I know " + userInput + " its a shade of orange, my favourite color! \nDo you know what else is orange? A Orange!");
                    break;
                case "green":
                case "olive":
                case "emerald":
                case "jade":
                    data = ("I know " + userInput + " its a shade of green! \nDo you know what else is green? Spinach!");
                    break;
                case "purple":
                case "lilac":
                case "mauve":
                case "violet":
                case "indigo":
                case "lavender":
                    data = ("I know " + userInput + " its a shade of purple! \nDo you know what else is purple? Eggplants!");
                    break;
                case "white":
                    data = ("I know " + userInput + " ! \nDo you know what else is white? Eggs!");
                    break;
                case "black":
                    data = ("I know " + userInput + " ! \nDo you know what else is black? Fresh PEI mussels!");
                    break;
                case "brown":
                    data = ("I know " + userInput + " ! \nDo you know what else is brown? Potatoes!");
                    break;
                case "hard coded":
                    data = message.Replace(userInput, "%$#^ $%#^@") + " \nHard Coding is bad development";
                    break;
                case "exit":
                    data = "Oh are you leaving?";
                    break;
                default:
                    data = "Sorry I didnt quite understand that, i'm still learning!";
                    break;
            }

            return data;
        }

        private static string ProcessResponse(Socket handler, byte[] bytes)
        {
            while (true)
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf(eof) > -1)
                {
                    break;
                }
            }
            return data;
        }
    }
}
