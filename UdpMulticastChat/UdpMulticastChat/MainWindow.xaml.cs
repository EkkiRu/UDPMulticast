using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Net;
using System;
using System.Collections.ObjectModel;

namespace UdpMulticastChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> messages = new ObservableCollection<string>();
        private readonly string nameAsNumber = new Random().Next(8000, 65300).ToString();


        public MainWindow()
        {
            InitializeComponent();

            chatBox.ItemsSource = messages;

        }

        private async void OnReady(object sender, RoutedEventArgs e)
        {
            var receiverClient = new UdpClient(8001);
            try
            {
                receiverClient.JoinMulticastGroup(IPAddress.Parse("225.1.10.8"), 10);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            while (true)
            {
                var result = await receiverClient.ReceiveAsync();
                messages.Add(Encoding.UTF8.GetString(result.Buffer));
            }

        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            var senderClient = new UdpClient();

            if (!string.IsNullOrEmpty(messageTextBox.Text))
            {
                var datagrams = Encoding.UTF8.GetBytes($"{nameAsNumber} говорит :{messageTextBox.Text}");
                await senderClient.SendAsync(datagrams, datagrams.Length, new IPEndPoint(IPAddress.Parse("225.1.10.8"),8001));
            }
            senderClient.Close();
        }
    }
}
