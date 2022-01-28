using GUI.Models;
using GUI.WPFHelperMethods;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static GUI.GameLogicService;

namespace GUI
{

    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 128;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public int requestCode;
        public int numOfRequests = 1;
        public Dictionary<string, string> args;
    }

    public class SendStateObject
    {
        public Socket clientSocket = null;
        public String data;
        public Dictionary<string, string> args;
    }
    public class SocketMethods {
        
}
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static List<Lobby> lobbies = new List<Lobby>();
        public static Socket socket;
        private static String response = String.Empty;
        public static void StartClient(MainWindow window)
        {

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress[] addresses = Dns.GetHostEntry("2.tcp.ngrok.io").AddressList;
                IPEndPoint remoteEP = new IPEndPoint(addresses[0], 17998);
                /*IPAddress[] addresses = Dns.GetHostEntry("127.0.0.1").AddressList;
                IPEndPoint remoteEP = new IPEndPoint(addresses[0], 12000);*/

                // Create a TCP/IP socket.  
                Socket clientSocket = new Socket(addresses[0].AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                socket = clientSocket;

                // Connect to the remote endpoint.  
                clientSocket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), clientSocket);

                //connectDone.WaitOne();
                //Console.WriteLine(clientSocket.Connected);
                // Send test data to the remote device.  
                //Send(clientSocket, "This is a test<EOF>");
                //sendDone.WaitOne();

                // Receive the response from the remote device.  
                //Receive(clientSocket);
                
                //receiveDone.WaitOne();

                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);

                //clientSocket.Shutdown(SocketShutdown.Both);
                //clientSocket.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client, int requestCode = -1, Dictionary<string, string> args = null)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.requestCode = requestCode;
                state.workSocket = client;
                state.args = args;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                if (state.sb.Length < 128)
                {
                    // There might be more data, so store the data received so far.  
                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    response = state.sb.ToString();

                    // message from server
                    Trace.WriteLine((int)response[0]);
                    if ((int)response[0] >= 8 && (int)response[0] <= 21 && (int)response[0] != 12)
                    {
                        if (response[0] == (char)RequestCodes.ROOM_DATA)
                        {
                            if (state.numOfRequests > 0)
                            {
                                lobbies.Add(new Lobby() { Name = response.Substring(3, 123), Id = (int)response[2], Index = (int)response[1] });
                                state.sb.Clear();
                                state.numOfRequests--;
                                if (state.numOfRequests > 0)
                                {
                                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                new AsyncCallback(ReceiveCallback), state);
                                }
                                else
                                {
                                    Action newAction = delegate
                                    {
                                        var lobbiesListView = (ListView)Application.Current.MainWindow.FindName("lvLobies");
                                        lobbiesListView.ItemsSource = lobbies;
                                    };
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, newAction);

                                }
                            }
                        } else if (response[0] == (char)RequestCodes.PLAYER_JOINED) {
                            Trace.WriteLine("Player joined: " + response);
                            Receive(client, (int) RequestCodes.DECLARE_READY);
                        } else if (response[0] == (char)RequestCodes.PLAYER_LEFT)
                        {
                            Trace.WriteLine("Player left lobby" + response);
                            Receive(client, (int) RequestCodes.DECLARE_READY);
                        } else if (response[0] == (char)RequestCodes.PLAYER_READY)
                        {
                            Trace.WriteLine("Player ready" + response);
                            // display readyness
                            Action newAction = delegate
                            {
                                
                                var lobbyDetailsWindow = WPFHelpers.GetWindowByName(Application.Current, "LobbyDetails");
                                var playerTwoReadyLabel = (Label)lobbyDetailsWindow.FindName("PlayerTwoReady");
                                playerTwoReadyLabel.Content = "Ready!";
                            };
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, newAction);


                            // option to send ready for host
                        } else if (response[0] == (char)RequestCodes.ENEMY_DEPLOYED)
                        {
                            // resonse od razu w bajrtach
                            // modul odbiera ramki
                            // modul mapuje na polecenia
                            // modul logiki gry
                            // modul obslugujacy warstwe wizualna
                            byte[] bytes = BitConverter.GetBytes(response);
                            var unitType = (int)response[1];
                            Array.Reverse(bytes, 2, 2);
                            var unitId = BitConverter.ToUInt16(bytes, 2);
                            Array.Reverse(bytes, 4, 2);
                            var tickRecorded = BitConverter.ToUInt16(bytes, 4);
                            
                            Action spawnAction = delegate
                            {
                                var window = WPFHelpers.GetWindowByName(Application.Current, "WindowGame");
                                if (window != null)
                                {
                                    (window as GameWindow).spawnUnit((UnitType)unitType, unitId, false);
                                }
                                else
                                {
                                    var gameWindow = new GameWindow();
                                    gameWindow.Show();
                                }

                            };
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, spawnAction);
                            Receive(client, (int)RequestCodes.RECRUIT_UNIT);
                        } else if (response[0] == (char)RequestCodes.GAME_STARTED)
                        {
                            Action gameAction = delegate
                            {
                                var window = WPFHelpers.GetWindowByName(Application.Current, "WindowGame");
                                if (window == null)
                                {
                                    var gameWindow = new GameWindow();
                                    gameWindow.Show();
                                }
                            };
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, gameAction);
                            Receive(client, (int)RequestCodes.RECRUIT_UNIT);
                        }
                        else if (response[0] == (char)RequestCodes.UNIT_MOVED)
                        {

                            byte[] bytes = Encoding.ASCII.GetBytes(response);
                            Array.Reverse(bytes, 1, 2);
                            var tickRecorded = BitConverter.ToUInt16(bytes, 1);
                            Array.Reverse(bytes, 3, 2);
                            var unitId = BitConverter.ToUInt16(bytes, 3);
                            Array.Reverse(bytes, 5, 2);
                            var unitPosition = BitConverter.ToUInt16(bytes, 5);
                            Action spawnAction = delegate
                            {
                                var window = WPFHelpers.GetWindowByName(Application.Current, "WindowGame");
                                if (window != null)
                                {
                                    var leftTeamUnit = (window as GameWindow).leftUnitBlocks.FirstOrDefault(lub => lub.Unit.Id == unitId);
                                    var rightTeamUnit = (window as GameWindow).rightUnitBlocks.FirstOrDefault(rub => rub.Unit.Id == unitId);
                                    Trace.WriteLine(unitPosition);
                                    if (leftTeamUnit != null)
                                    {
                                        leftTeamUnit.move(unitPosition);
                                    } else if (rightTeamUnit != null)
                                    {
                                        rightTeamUnit.move(unitPosition);
                                    }
                                }
                                else
                                {
                                    var gameWindow = new GameWindow();
                                    gameWindow.Show();
                                }

                            };
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, spawnAction);
                            Receive(client, (int)RequestCodes.RECRUIT_UNIT);
                        }
                    }
                    else
                    {
                        // response message from server
                        if (response[0] == 'Y' || response[0] == 'W')
                        {
                            if (state.requestCode == (int)RequestCodes.HELLO)
                            {
                                char s = (char)RequestCodes.GET_LOBBIES;
                                App.Send(App.socket, s.ToString().PadRight(128));

                            }
                            else if (state.requestCode == (int)RequestCodes.GET_LOBBIES)
                            {
                                lobbies.Clear();
                                var numOfLobbies = (int)response[1];
                                if (numOfLobbies > 0)
                                {
                                    state.sb.Clear();
                                    state.numOfRequests = numOfLobbies; // get from parsed response
                                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                    new AsyncCallback(ReceiveCallback), state);
                                }
                                else
                                {
                                    Action Action = delegate
                                    {
                                        lobbies.Clear();
                                        var lobbiesListView = (ListView)Application.Current.MainWindow.FindName("lvLobies");
                                        lobbiesListView.ItemsSource = lobbies;
                                    };
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, Action);
                                }
                            }
                            else if (state.requestCode == (int)RequestCodes.CREATE_LOBBY)
                            {
                                //var testLobbyId = (int)response[1];
                                Action action = delegate
                                {
                                    var lobbyDetailsWindow = new LobbyDetailsWindow(new Lobby() { Id = (int)response[1], Name = state.args["lobbyName"]},
                                        state.args["playerName"], "Waiting for player to join");
                                    lobbyDetailsWindow.Show();
                                };
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                                Receive(client);

                            }
                            else if (state.requestCode == (int)RequestCodes.DECLARE_READY)
                            {
                                Action newAction = delegate
                                {
                                    var lobbyDetailsWindow = WPFHelpers.GetWindowByName(Application.Current, "LobbyDetails");
                                    
                                    var playerOneReadyLabel = (Label)lobbyDetailsWindow.FindName("PlayerOneReady");
                                    playerOneReadyLabel.Content = "Ready!";
                                        
                                };
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, newAction);
                                if (response[0] == 'W')
                                {
                                    Trace.WriteLine("Waiting for other player");
                                    Receive(client);
                                } else
                                {
                                    // start game
                                    Trace.WriteLine("Start game");
                                    Receive(client, (int) RequestCodes.RECRUIT_UNIT);
                                }
                            } else if (state.requestCode == (int)RequestCodes.JOIN_LOBBY)
                            {
                                Trace.WriteLine("Player in lobby: " + response.Substring(1, 127).Trim());
                                Action action = delegate
                                {
                                    var lobbyDetailsWindow = new LobbyDetailsWindow(new Lobby() { Id = Int32.Parse(state.args["lobbyId"]), Name = state.args["lobbyName"] },
                                        response.Substring(1, 127).Trim(), state.args["playerName"]);
                                    lobbyDetailsWindow.Show();
                                };
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                                Receive(client, requestCode: 7);
                            } else if (state.requestCode == (int)RequestCodes.RECRUIT_UNIT)
                            {
                                byte[] bytes = Encoding.ASCII.GetBytes(response);
                                var unitId = BitConverter.ToUInt16(bytes, 1);
                                var tickRecorded = BitConverter.ToUInt16(bytes, 3);
                                var window = WPFHelpers.GetWindowByName(Application.Current, "WindowGame");
                                (window as GameWindow).spawnUnit(GameLogicService.UnitType.Warrior, unitId, true);
                            }


                        } else
                        {
                            
                                Action action = delegate
                                {
                                    var errorWindow = new ErrorWindow(response.Substring(1, 127).Trim());
                                    errorWindow.Show();
                                };
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        public static void Send(Socket client, String data, Dictionary<string, string> args =  null)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            SendStateObject sendStateObject = new SendStateObject();
            sendStateObject.data = data;
            sendStateObject.clientSocket = client;
            sendStateObject.args = args;

            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), sendStateObject);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                SendStateObject sendStateObject = (SendStateObject)ar.AsyncState;
                Socket client = sendStateObject.clientSocket;
                string data = sendStateObject.data;
                Dictionary<string, string> args = sendStateObject.args;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                Receive(client, (int)data[0], args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public enum RequestCodes
        {
            HELLO = 1,
            DISCONNECT = 2,
            GET_LOBBIES = 3,
            CREATE_LOBBY = 4,
            JOIN_LOBBY = 5,
            LEAVE_LOBBY = 6,
            DECLARE_READY = 7,
            RECRUIT_UNIT = 12,

            //server codes
            PLAYER_JOINED = 8,
            PLAYER_LEFT = 9,
            PLAYER_READY = 10,
            ROOM_DATA = 11,
            ENEMY_DEPLOYED = 13,
            UNIT_DAMAGED = 14,
            BASE_DAMAGED = 15,
            UNIT_DESTROYED = 16,
            BASE_DESTROYED = 17,
            GAME_FINISHED = 18,
            UNIT_MOVED = 19,
            OPONNENT_DISCONNECTED = 20,
            GAME_STARTED = 21
        }
        MainWindow window;
        protected override void OnStartup(StartupEventArgs e)
        {
            this.window = new MainWindow();
            StartClient(window);
            this.window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            socket.Close();
        }

       
    }
}
