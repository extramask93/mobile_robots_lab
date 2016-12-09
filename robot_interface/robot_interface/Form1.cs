using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace robot_interface
{
    public partial class Form1 : Form
    {
    
        Robot robot;
        TCPConnection connection;
        const sbyte MotorMax = 127;
        const sbyte tick = 1;
        sbyte[] Times;
        sbyte MaxMultiSteer=(sbyte)Math.Round(MotorMax/(float)(80));//maximum nr. of steps for steering
        sbyte MotorMulti=(sbyte)Math.Round(MotorMax/(float)30);//#multiplier per step of acceleration/deceleration
        sbyte MotorMultiSteer = (sbyte)Math.Round(MotorMax /(float)12);//maximum nr. of steps for acc/dec
        sbyte MaxMulti;
        sbyte Motor1Speed;
        sbyte Motor2Speed;
        sbyte SteerSpeed;
        float MotorPercentage;
        




        public Form1()
        {
            InitializeComponent();
            Times = new sbyte[8];
            MaxMulti= (sbyte)Math.Round(MotorMax / (float)MotorMulti);//#calculate the maximum nr. of steps for acc/dec

        }

 
       

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                robot = new Robot();
                string ip = textBox1.Text;
                Int32 port = Int32.Parse(textBox9.Text);
                connection = new TCPConnection();
                connection.UIMessage += new EventHandler(connection_UIMessage);
                connection.MessageReceived += new EventHandler(connection_MessageReceived);
                connection.Connect(ip, port);
                connection.Send(robot.encodeData());
                timer1.Start();
                timer2.Start();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message,"Error");
            }

        }

        private void connection_UIMessage(object sender, EventArgs e)
        {
            if (e is TCPConnectionEventArgs)
            {
                TCPConnectionEventArgs te = e as TCPConnectionEventArgs;
                this.SetText(te.info);
            }
        }
        private void connection_MessageReceived(object sender, EventArgs e)
        {
            string toDecode = connection.inQueue.Remove();
            robot.decodeData(toDecode);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (robot != null)
            {

                if (checkBox1.Checked == false)
                {

                    robot.led &= unchecked((byte)~(1 << 0));

                }
                else if (checkBox1.Checked == true)
                {
                    robot.led |= unchecked((byte)(1 << 0));

                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (robot != null)
            {
                if (checkBox2.Checked == false)
                {
                    robot.led &= unchecked((byte)~(1 << 1));

                }
                else if (checkBox2.Checked == true)
                {
                    robot.led |= unchecked((byte)(1 << 1));

                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            connection.Close();
            robot = null;
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (robot != null)
            {
                robot.leftEngine = (sbyte)trackBar1.Value;
                if (checkBox3.Checked == true)
                {
                    trackBar2.Value = trackBar1.Value;
                    robot.rightEngine = (sbyte)trackBar2.Value;

                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (robot != null)
            {
                robot.rightEngine = (sbyte)trackBar2.Value;

                if (checkBox3.Checked == true)
                {
                    trackBar1.Value = trackBar2.Value;
                    robot.leftEngine = (sbyte)trackBar1.Value;
                }
            }
        }
        private void UpdateUI()
        {
            this.progressBar1.Value = robot.batteryLvl;
            this.label12.Text = robot.batteryLvl.ToString();
            this.progressBar2.Value = robot.sensor[0];
            this.label13.Text = robot.sensor[0].ToString();
            this.progressBar3.Value = robot.sensor[1];
            this.label14.Text = robot.sensor[1].ToString();
            this.progressBar4.Value = robot.sensor[2];
            this.label15.Text = robot.sensor[2].ToString();
            this.progressBar5.Value = robot.sensor[3];
            this.label16.Text = robot.sensor[3].ToString();
            this.progressBar6.Value = robot.sensor[4];
            this.label17.Text = robot.sensor[4].ToString();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.UpdateUI();
            connection.Send(robot.encodeData());
        }
        delegate void SetTextCallback(string text);

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void SetText(string txt)
        {
            if (this.textBox10.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { txt }); //txt is list of arguments
            }
            else
            {
                this.textBox10.Text += txt;
            }

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                this.Focus();
                this.KeyPreview = true;
            }
            else
                this.KeyPreview = false;


        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
           
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Times[4] = 1;break;
                case Keys.Down:
                    Times[7] = 1;break;
                case Keys.Right:
                    Times[5] = 1;break;
                case Keys.Left:
                    Times[6] = 1;break;
                default:{break;};
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Times[4] = 0; break;
                case Keys.Down:
                    Times[7] = 0; break;
                case Keys.Right:
                    Times[5] = 0; break;
                case Keys.Left:
                    Times[6] = 0; break;
                default: { break; };
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
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

            Motor1Speed = (sbyte)((Times[0] - Times[3]) * MotorMulti);//calculating the motor speed for linear movement
            MotorPercentage = Motor1Speed/MotorMax;//calc temp var its not needed?
            this.textBox10.AppendText(Motor1Speed.ToString());//motor speed is 0 when not pressed
            SteerSpeed = (sbyte)((Times[2] - Times[1]) * MotorMultiSteer);//calculating the current steering movement ye deleted motor percentage n works
            //this.textBox10.AppendText(SteerSpeed.ToString());//steer speed is 0
            if (Motor1Speed + Math.Abs(SteerSpeed) >= MotorMax)//limit the accumulated motor speeds so that steering can take effect
                Motor1Speed = (sbyte)(MotorMax - Math.Abs(SteerSpeed));
            if (Motor1Speed - Math.Abs(SteerSpeed) <= -MotorMax)
                Motor1Speed = (sbyte)(-MotorMax + Math.Abs(SteerSpeed));
            Motor2Speed = Motor1Speed;//linear movement --> both motors running at the same speed
            Motor1Speed += SteerSpeed;//adding the steering speed to the both motors
            Motor2Speed += (sbyte)-SteerSpeed;
            robot.leftEngine = Motor1Speed;
            robot.rightEngine = Motor2Speed;


        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
