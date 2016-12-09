using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace robot_interface
{
    class Robot
    {
    
     
        //Status
        public UInt16 batteryLvl{get; set;}
        public UInt16[] sensor { get; set; }
        public Dictionary<byte, string> status;
        public byte currentStatus{get; set;}
        //control
        public byte led { get; set; }
        public sbyte leftEngine { get; set; }
        public sbyte rightEngine{get; set;}
        private System.IO.StreamWriter file;


        public Robot()
        {
            status = new Dictionary<byte, string>();
            sensor = new UInt16[5];
            led = 0;
            leftEngine=0;
            rightEngine=0;
            currentStatus=0;
            status.Add(0,"Dane odczytane prawidłowo");
            status.Add(1, "Brak znaku kończącego ramkę");
            status.Add(2,"Ramka zbyt dluga");
            status.Add(3, "Brak znaku rozpoczynającego bramkę");
            status.Add(4, "Zła wielkośc ramki");
            status.Add(5, "Błąd dekodowania danych");
            status.Add(6, "Błąd połączenia z Robotem Polulu");
         
        }

        public string encodeData()
        {
            StringBuilder data = new StringBuilder();
            data.Append('[');
            data.Append(led.ToString("X2"));
            data.Append(leftEngine.ToString("X2"));
            data.Append(rightEngine.ToString("X2"));
            data.Append(']');
            return data.ToString();
        }
        public void decodeData(string data)
        {
            try
            {
                
                currentStatus = byte.Parse(data.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                byte[] bat = new byte[2];
                bat[0] = byte.Parse(data.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                bat[1] = byte.Parse(data.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                if (!BitConverter.IsLittleEndian)
                    Array.Reverse(bat);
                
                batteryLvl = BitConverter.ToUInt16(bat,0);
                int count = 0;
                byte[] sen = new byte[2];
                for (int i = 0; i < sensor.Length; i++, count += 4)
                {
                    sen[0] = byte.Parse(data.Substring(7 + count, 2), System.Globalization.NumberStyles.HexNumber);
                    sen[1] = byte.Parse(data.Substring(7 + count + 2, 2), System.Globalization.NumberStyles.HexNumber);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(sen);
                        sensor[i] = BitConverter.ToUInt16(sen, 0);
                    }
                    else
                        sensor[i] = BitConverter.ToUInt16(sen, 0);
                }
            }
     
            
            catch (Exception e)
            {
                MessageBox.Show("Nierozpoznana odpowiedź robota"+e.ToString());
            }
        }
        public void MoveForward(sbyte speed)
        {
            if (Enumerable.Range(0, 127).Contains(speed))
            {
                this.leftEngine = speed;
                this.rightEngine = speed;
            }
        }
        public void MoveBackward(sbyte speed)
        {
            if (Enumerable.Range(0, 127).Contains(speed))
            {
                this.leftEngine = (sbyte)-speed;
                this.rightEngine = (sbyte)-speed;
            }
        }
        public void TurnRight(sbyte speed)
        {
            if (Enumerable.Range(0, 127).Contains(speed))
            {
                this.leftEngine = 0;
                this.rightEngine = speed;
            }
        }
        public void TurnLeft(sbyte speed)
        {
            if (Enumerable.Range(0, 127).Contains(speed))
            {
                this.leftEngine = speed;
                this.rightEngine = 0;
            }
        }
       
   
    }
}
