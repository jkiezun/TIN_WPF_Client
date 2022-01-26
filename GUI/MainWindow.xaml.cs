using GUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string name;
        string lobbyName;
        public MainWindow()
        {
            InitializeComponent();
          

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //App.Send(App.socket, "MAP_LOADED<EOF>");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*if (HelloButton.IsChecked == true)
            {
                MessageBox.Show("Hello.");
            }
            else if (ByeButton.IsChecked == true)
            {
                MessageBox.Show("Goodbye.");
            }*/
            char s = (char)1;
            Console.WriteLine((s + this.name).PadRight(128 - this.name.Length - 1).Length);
            var s2 = (s + this.name).PadRight(128).Length;
            App.Send(App.socket, (s + this.name).PadRight(128));

            /*var rnd = new Random();
            var window = Application.Current.MainWindow;
            var tempButton = new Button { Content = "NEW BUTTON", Width = 10, Height = 10, Margin = new Thickness(-500, 0, 0, 0), Name = "testbutton" };
            RegisterName(tempButton.Name, tempButton);
            MainGrid.Children.Add(tempButton);*/


        }


        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            this.name = NameTextBox.Text;
        }

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)4;
            App.Send(App.socket, (s + this.lobbyName).PadRight(128), 
                args: new Dictionary<string, string>() { { "lobbyName", this.lobbyName }, {"playerName", NameTextBox.Text } });
        }

        private void LobbyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.lobbyName = LobbyTextBox.Text;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            char s = (char)3;
            App.Send(App.socket, (s + this.lobbyName).PadRight(128));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void listView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                char s = (char)App.RequestCodes.JOIN_LOBBY;
                Trace.WriteLine(s.ToString() + (char)(item as Lobby).Id);
                App.Send(App.socket, (s.ToString() + (char)(item as Lobby).Id).PadRight(128),
                    args: new Dictionary<string, string>() { { "lobbyName", (item as Lobby).Name }, 
                        { "lobbyId", (item as Lobby).Id.ToString() }, 
                        { "playerName", NameTextBox.Text} });

            }
        }
    }
}
