using System;
using System.Net;
using System.Threading;
using System.Windows;

using System.Windows.Input;
using System.Windows.Threading;

namespace laborki
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int sendDelay = 200;
        const int uiRefreshDelay = 20;
        private delegate void Handler(string str);
        Robot robot;
        TCPConnection connection;
        DispatcherTimer sendTimer;
        DispatcherTimer uiTimer;

        public MainWindow()
        {
            InitializeComponent();
            sendTimer = new DispatcherTimer();
            sendTimer.Tick += new EventHandler(timer1_OnTick);
            sendTimer.Interval = new TimeSpan(0, 0, 0, 0, sendDelay); //every 0,5s
            uiTimer = new DispatcherTimer();
            uiTimer.Tick += new EventHandler(uiTimer_OnTick);
            uiTimer.Interval = new TimeSpan(0, 0, 0, 0, uiRefreshDelay); //every 0,5s
        }

        private void Button_Click(object sender, RoutedEventArgs e) //connect
        {
            string ip = textBox1.Text;
            Int32 port= Int32.Parse(textBox2.Text);
            robot = new Robot();
            connection = new TCPConnection();
            connection.UIMessage += new EventHandler(connection_OnUIMessage);
            connection.MessageReceived += new EventHandler(connection_MessageReceived);
            connection.Connect(ip,port);
            sendTimer.Start();
            uiTimer.Start();
            conButton.IsEnabled = false;
        }
        private void connection_OnUIMessage(object sender, EventArgs e)
        {
            if (e is TCPConnectionEventArgs)
            {
                TCPConnectionEventArgs foo = e as TCPConnectionEventArgs;
                Dispatcher.BeginInvoke(new Handler(SetText), foo.info);
            }
        }
        private void connection_MessageReceived(object sender, EventArgs e)
        {
            string toDecode = connection.inQueue.Remove();
            robot.decodeData(toDecode);
        }
        private void uiTimer_OnTick(object sender, EventArgs e)
        {
            this.UpdateUI();
        }
        private void timer1_OnTick(object sender, EventArgs e)
        { 
            connection.Send(robot.encodeData());
        }
        private void SetText(string str)
        {
            textBox3.Text += "->" + str + '\n';
            var prevFocus = FocusManager.GetFocusedElement(this);
            textBox3.Focus();
            textBox3.CaretIndex = textBox1.Text.Length;
            textBox3.ScrollToEnd();
            FocusManager.SetFocusedElement(this, prevFocus);
        }

        private void lSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (robot != null)
            {
                robot.leftEngine = (sbyte)lSlider.Value;
                if (checkBox3.IsChecked == true)
                {
                    rSlider.Value = lSlider.Value;
                    robot.rightEngine = (sbyte)rSlider.Value;

                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sendTimer.Stop();
            connection.Close();
            connection.UIMessage -= new EventHandler(connection_OnUIMessage);
            connection.MessageReceived -= new EventHandler(connection_MessageReceived);
            conButton.IsEnabled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (robot != null)
            {

                if (checkBox1.IsChecked == true)
                {
                    robot.led |= unchecked((byte)(1 << 0));

                }
            }
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (robot != null)
            {

              if (checkBox2.IsChecked == true)
                {
                    robot.led |= unchecked((byte)(1 << 1));

                }
            }
        }

        private void rSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (robot != null)
            {
                robot.rightEngine = (sbyte)rSlider.Value;

                if (checkBox3.IsChecked == true)
                {
                    lSlider.Value = rSlider.Value;
                    robot.leftEngine = (sbyte)lSlider.Value;
                }
            }
        }
        private void UpdateUI()
        {
            this.progressBar1.Value = robot.batteryLvl;
            this.progressBar2.Value = robot.sensor[0];
            this.progressBar3.Value = robot.sensor[1];
            this.progressBar4.Value = robot.sensor[2];
            this.progressBar5.Value = robot.sensor[3];
            this.progressBar6.Value = robot.sensor[4];
            this.statusLabel.Content = robot.currentStatus;

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window1 a = new Window1(robot);
            a.Show();
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (robot != null)
            {
                    robot.led &= unchecked((byte)~(1 << 0));
            }
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            if (robot != null)
            {

                if (checkBox2.IsChecked == false)
                {
                    robot.led &= unchecked((byte)~(1 << 1));

                }
            }
        }
    }
}
