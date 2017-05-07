using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace monkeyclaws
{
    public partial class Calibration : Form
    {
        int delayedIndex = 0;
    public    bool calibrationDone = false;
        double gyroError = 0;
        //start timer after 5 seconds 
        int invoker = 0;
        //initial mpu gyro values
        public List<double> gyroStart= new List<double>
        {1000,1000
        };
        //mpu value every 2 seconds
        private List<double> gyroDelayedX = new List<double>
        {100000,3432,2,32423243,2,3232523,23,43543,342038,200
        };
        private List<double> gyroDelayedY = new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        };
        //mpu curr value
        private List<double> gyroCurr = new List<double> { 0, 0, 0 };
        public SerialPort mySerial;
        BackgroundWorker worker;
        Timer timer;
        
        public Calibration()
        {

            InitializeComponent();
            mySerial = new SerialPort();
            mySerial.BaudRate = 115200;
            mySerial.PortName = "COM7";
            mySerial.NewLine = Environment.NewLine;
             mySerial.DataReceived += serialPortData;
            mySerial.Open();

             worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += calibrate;
            worker.RunWorkerAsync();
            //initialize serialPort
            timer = new Timer();
            timer.Tick += UpdateGyroDelayed;
            timer.Interval = 250;
            timer.Start();
            //to start calibration after 5 seconds 
            
        }

        private void UpdateGyroDelayed(object sender, EventArgs e)
        {
            if (invoker < 20)
            {
                invoker++;
                return;
            }
            gyroDelayedX[delayedIndex] = gyroCurr[0];
            gyroDelayedY[delayedIndex] = gyroCurr[1];
            delayedIndex++;
            delayedIndex %= 10;
            string p = "q";

            //check if last 10 readings are close to each other 
            //check Y- error 
            gyroError = 0;
            for(int i=0; i<gyroDelayedX.Count-1; i++)
            {
                gyroError += Math.Abs(gyroDelayedX[i + 1] - gyroDelayedX[i]) +
                            Math.Abs(gyroDelayedY[i + 1] - gyroDelayedY[i]);
            }
          
            Console.WriteLine("error" + gyroError.ToString()+"curr "+
                gyroCurr[0].ToString()+" y: "+gyroCurr[1].ToString());
            if (gyroError < 0.10)
            {   //startX,startY
                calibrationDone = true;
                gyroStart[0] = gyroCurr[0];
                gyroStart[1] = gyroCurr[1];
                timer.Stop();
                label1.Text = "Calibration Done !";
            }
        }

        private void serialPortData(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = ((SerialPort)sender).ReadLine();
            if (temp!=""&&temp[0] == 'g'&&temp.IndexOf('?')==-1&&temp.IndexOf(',')!=-1) { 
                string temp2 = (temp.Split('g'))[1];
                string[] temp3 = temp2.Split(',');
                gyroCurr[0]=(double.Parse(temp3[0]));
                gyroCurr[1]=(double.Parse(temp3[1]));
               // Console.WriteLine(gyroCurr[0].ToString());
               // Console.WriteLine(gyroCurr[0]);
              //  Console.WriteLine(gyroCurr[1]);
                temp = "";

            }
        }




        private void calibrate(object sender, DoWorkEventArgs e)
        {
            while (true)
            {


            }
        }

        private void Calibration_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CloseCalibration(object sender, FormClosingEventArgs e)
        {

            worker.CancelAsync();
            worker.Dispose();
            worker = null;
            mySerial.Close();
        }
    }
}
