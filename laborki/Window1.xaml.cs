using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace laborki
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        const sbyte MotorMax = 127;
        const sbyte tick = 1;
        sbyte[] Times;
        sbyte MaxMultiSteer = (sbyte)Math.Round(MotorMax / (float)(80));//maximum nr. of steps for steering
        sbyte MotorMulti = (sbyte)Math.Round(MotorMax / (float)30);//#multiplier per step of acceleration/deceleration
        sbyte MotorMultiSteer = (sbyte)Math.Round(MotorMax / (float)12);//maximum nr. of steps for acc/dec
        sbyte MaxMulti;
        sbyte Motor1Speed;
        sbyte Motor2Speed;
        sbyte SteerSpeed;
        float MotorPercentage;
        DispatcherTimer keyboardTimer1;
        Robot robot;
        public Window1(Robot robot)
        {
            InitializeComponent();
            this.robot = robot;
            labell.DataContext = robot;
            Times = new sbyte[9];
            MaxMulti = (sbyte)Math.Floor(MotorMax / (float)MotorMulti);//#calculate the maximum nr. of steps for acc/dec =30
            keyboardTimer1 = new DispatcherTimer();
            keyboardTimer1.Tick += new EventHandler(keyboardTimer_OnTick);
            keyboardTimer1.Interval = new TimeSpan(0, 0, 0, 0, 100); //every 0,1s
            keyboardTimer1.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO
            if (e.Key == Key.Up)
                Times[4] = 1;
            else if (e.Key == Key.Down)
                Times[7] = 1;
            else if (e.Key == Key.Left)
                Times[5] = 1;
            else if (e.Key == Key.Right)
                Times[6] = 1;
            else if (e.Key == Key.Space) //breaks
                Times[8] = 1;
                
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
                Times[4] = 0;
            else if (e.Key == Key.Down)
                Times[7] = 0;
            else if (e.Key == Key.Left)
                Times[5] = 0;
            else if (e.Key == Key.Right)
                Times[6] = 0;
            else if (e.Key == Key.Space)
                Times[8] = 0;
        }
        private void keyboardTimer_OnTick(object sender, EventArgs e)
        {
            if (Times[4] == 0 && Times[0] > 0)      //#if flag for <Up Arrow> is cleared,
                Times[0] -= 1;  						//#decrement counter for <Up Arrow> if not already zero	
            if (Times[5] == 0 && Times[1] > 0)
                Times[1] -= 1;
            if (Times[6] == 0 && Times[2] > 0)
                Times[2] -= 1;
            if (Times[7] == 0 && Times[3] > 0)
                Times[3] -= 1;

            if (Times[4] == 1 && Times[0] < MaxMulti)		//flag of <Up Arrow> is set -> counter increment
                Times[0] += 1;  						//if not already at maximum
            if (Times[5] == 1 && Times[1] < MaxMultiSteer)
                Times[1] += 1;
            if (Times[6] == 1 && Times[2] < MaxMultiSteer)
                Times[2] += 1;
            if (Times[7] == 1 && Times[3] < MaxMulti)
                Times[3] += 1;
            if (Times[8] == 1)
                Times[0] = Times[1] = Times[2] = Times[3] = 0;

            Motor1Speed = (sbyte)((Times[0] - Times[3]) * MotorMulti);//calculating the motor speed for linear movement
            MotorPercentage = Motor1Speed / MotorMax;//calc temp var its not needed?
            SteerSpeed = (sbyte)((Times[2] - Times[1]) * MotorMultiSteer);//calculating the current steering movement ye deleted motor percentage n works
            if (Motor1Speed + Math.Abs(SteerSpeed) >= MotorMax)//limit the accumulated motor speeds so that steering can take effect
                Motor1Speed = (sbyte)(MotorMax - Math.Abs(SteerSpeed));
            if (Motor1Speed - Math.Abs(SteerSpeed) <= -MotorMax)
                Motor1Speed = (sbyte)(-MotorMax + Math.Abs(SteerSpeed));
            Motor2Speed = Motor1Speed;//linear movement --> both motors running at the same speed
            Motor1Speed += SteerSpeed;//adding the steering speed to the both motors
            Motor2Speed += (sbyte)-SteerSpeed;
            robot.leftEngine = Motor1Speed;
            robot.rightEngine = Motor2Speed;
            labell.Content = robot.leftEngine;
            labelr.Content = robot.rightEngine;

        }
    }
}
