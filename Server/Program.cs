using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Server
{
    class Program
    {
      
        class Player
        {
            public int ID { get; private set; } // индификатор отличия от других
            public string Surname { get; set; } // положение
            public string First_name { get; set; } // положение
            public string Place_of_work { get; set; } // Место работы
            public string Skill { get; set; } //квалификация qualification
            public string Result { get; set; }
            public int ResultX { get; set; }

            //Конструктор передаём параметры для всех свойств
            public Player(int id, string surname, string f_n, string p_o_w, string skill, string result,int resultX)
            {
                ID = id;
                Surname = surname;
                First_name = f_n;
                Place_of_work = p_o_w;
                Skill = skill;
                Result = result;

                ResultX = resultX;

            }

            public void Draw() 
            {

                Console.ForegroundColor = ConsoleColor.Blue; // устанавливаем цвет
               
                Console.WriteLine("===============================================================================");
               
                Console.ResetColor(); // сбрасываем в стандартный
              
                Console.WriteLine("Фамилия = " + Surname);
                Console.ResetColor(); // сбрасываем в стандартный
         
                Console.WriteLine("Имя = " + First_name);
                Console.WriteLine("Место работы = " + Place_of_work);
                Console.WriteLine("Квалификация = " + Skill);
               // Console.ResetColor(); // сбрасываем в стандартный
                if (ResultX == 1)
                {
               
                    Console.Write("Результат = ");
                   // Console.ResetColor(); // сбрасываем в стандартный
                    Console.ForegroundColor = ConsoleColor.Green; // устанавливаем цвет
                    Console.WriteLine(  Result);
                    Console.ResetColor(); // сбрасываем в стандартный
                }
                else {
                    //Console.ForegroundColor = ConsoleColor.DarkCyan; // устанавливаем цвет
                    Console.Write("Результат = ");
                    //Console.ResetColor(); // сбрасываем в стандартный
                    Console.ForegroundColor = ConsoleColor.Red; // устанавливаем цвет
                    Console.WriteLine(Result);
                    Console.ResetColor(); // сбрасываем в стандартный
                    
                }
                Console.ForegroundColor = ConsoleColor.Blue; // устанавливаем цвет
                Console.WriteLine("===============================================================================");
                Console.WriteLine(" ");

                Console.ResetColor(); // сбрасываем в стандартный


            }
        }

        class Client
        {
            public Socket Socket { get; set; }
            public int ID { get; set; }
            public Client(Socket socket)
            {
                Socket = socket;
            }
        }

   
        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        
        static List<Client> clients = new List<Client>();

        static Random random = new Random();


        static void Main(string[] args)
        {
           

            Console.Title = "Программа для контроля тестируемых МГО <<ОСВОД>>";


            Console.ForegroundColor = ConsoleColor.Blue; 
            Console.WriteLine("===============================================================================");
            Console.Write("===================="); Console.ResetColor(); 
            Console.Write("Ожидание подключения новых пользователей");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("===================");
            Console.WriteLine("===============================================================================");
            Console.ResetColor(); 
            //Действие 2: запускаем сокет
            socket.Bind(new IPEndPoint(IPAddress.Parse ("127.0.0.1"), 2048));
            socket.Listen(0);
           
            socket.BeginAccept(AcceptCallback, null);
            
            
            Console.ReadLine();
        }

        
        static void AcceptCallback(IAsyncResult ar) 
        {
           
            Client client = new Client(socket.EndAccept(ar));

         
       
            Thread thread = new Thread(HandleClient);
         

  
            thread.Start(client);
         
            clients.Add(client);
            Console.WriteLine(" ");
            Console.WriteLine("Получены данные:");
            // Начинаем приём подкл.
            socket.BeginAccept(AcceptCallback, null);
        }

        static void HandleClient(object o)
        {
            
            Client client = (Client)o;
          
            MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
            
            BinaryWriter writer = new BinaryWriter(ms);

            
            
            BinaryReader reader = new BinaryReader(ms);
           
            while (true)
            {
                ms.Position = 0;
              
                try
                { 
                  
                    client.Socket.Receive(ms.GetBuffer());
                }
                catch
                {
                    client.Socket.Shutdown(SocketShutdown.Both);
                    client.Socket.Disconnect(true);
                    clients.Remove(client);
                    Console.ForegroundColor = ConsoleColor.DarkRed; // устанавливаем цвет
                    Console.WriteLine($"Пользователь под номером (-{client.ID}-) завершил тестирование");
                    Console.ResetColor(); // сбрасываем в стандартный
                    return;
                }
                //  15.2: тут получим курсор и будет 0/
                int code = reader.ReadInt32();
            
                int id; 
                string f;
                string n;
                string m;
                string s;
                string r;
                int rx;

                switch (code)
                {
                        case 0:
                        while (true)
                        {
                         
                            id = random.Next(0, 1001);
                            if (clients.Find(c => c.ID == id) == null)
                            {
                               
                                writer.Write(id);
                          
                              

                                client.Socket.Send(ms.GetBuffer());
                           
                             
                                client.ID = id;
                                break;
                            }
                        } // нижний while
                        break;
                    //если код запроса 1 
                    case 1:
                        foreach (var c in clients)
                        {

                            reader.ReadInt32();
                                id = 0;
                                f = reader.ReadString();
                                n = reader.ReadString();
                                m = reader.ReadString();
                                s = reader.ReadString();
                                r = reader.ReadString();
                                rx= reader.ReadInt32();

                            Player plr;

                                plr = new Player(id, f, n, m, s, r, rx);
                            
                            
                            plr.Draw();
                            
                        }
                        break;
                }

            } 

        }

    }
}
