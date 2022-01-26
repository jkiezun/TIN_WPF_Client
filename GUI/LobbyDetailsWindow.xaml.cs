using GUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LobbyDetailsWindow : Window
    {
        Lobby lobby;
        public LobbyDetailsWindow(Lobby lobby, string initialPlayerOneName, string initialPlayerTwoName)
        {
            InitializeComponent();
            this.lobby = lobby;
            LobbyTitle.Content = lobby.Name.ToString();
            PlayerOneName.Content = initialPlayerOneName;
            PlayerTwoName.Content = initialPlayerTwoName;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            char s = (char)App.RequestCodes.LEAVE_LOBBY;
            App.Send(App.socket, (s.ToString()).PadRight(128));
            s = (char)App.RequestCodes.GET_LOBBIES;
            App.Send(App.socket, (s.ToString()).PadRight(128));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)App.RequestCodes.DECLARE_READY;
            App.Send(App.socket, (s.ToString()).PadRight(128));
        }
    }
}
