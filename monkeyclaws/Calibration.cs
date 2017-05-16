using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace monkeyclaws
{
    public partial class Calibration : Form
    {
        bool workerDone = false;
        List<double> settings;
        string settingsPath = "C:\\Users\\muham\\Desktop\\monkeyClawsSettings";
        StreamWriter settingsFileWriter;
        StreamReader settingsFileReader;
        //flex-fest-opened
        double flexError = 0;
        public List<double> flexStart = new List<double> { 10000, 10000, 10000, 10000,10000 };
        List<List<double>> flexDelayed = new List<List<double>>();
        List<double> flexCurr = new List<double> { 10000, 10000,10000, 10000,10000 };
        int flexDelayedIndex = 0;

        //FLEX -FEST-CLOSED
        
     
        public List<double> flexStartClosed = new List<double> { 10000, 10000,1000,1000,1000 };
        List<List<double>> flexDelayedClosed = new List<List<double>>();
       



        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        int delayedIndex = 0;
    public    bool calibrationDone = false;
        double gyroError = 0;
        //start timer after 5 seconds 
        int invoker = 0;
        //initial mpu gyro values
        public List<double> gyroStart= new List<double>
        {1000,1000,1000   //roll pitch  yaw
        };
        //mpu value every 2 seconds
        private List<double> gyroDelayedX = new List<double>
        {100000,3432,2,32423243,2,3232523,23,43543,342038,200
        };
        private List<double> gyroDelayedY = new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        };
        private List<double> gyroDelayedZ = new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        };
        //mpu curr value
        private List<double> gyroCurr = new List<double> { 0, 0, 0 };
        public SerialPort mySerial;
        BackgroundWorker worker;
        Timer calibration_1;
        Timer calibration_2;
        int secondCalibrationInvoker = 0;
        int Invoker = 0;
        private int flexNumber=5;
        private bool firstTimeToRead;
        private int minRead;

        public Calibration()
        {
            for(int i=0; i< flexNumber; i++)
            {
               List<double>temp = new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        };
                flexDelayed.Add(new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        });
                flexDelayedClosed.Add(new List<double>
        {100000,3000,20000,1000,5000,6000,1000,21312,342038,200
        });
            }


           /* for (int i = 0; i < flexNumber; i++)
            {
                for (int j = 0; j < 10; j++)
                    Console.WriteLine(flexDelayed[i][j]);
            }*/
            /*  InputSimulator sim = new InputSimulator();
              sim.Mouse.MoveMouseTo(65535, 0);*/
            InitializeComponent();
            mySerial = new SerialPort();
            mySerial.BaudRate =  9600;
            mySerial.ReadTimeout = 100;
            mySerial.PortName = "COM12";
           // mySerial.ReadTimeout = 10;
           
            // mySerial.DataReceived += serialPortData;
            mySerial.Open();

             worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += calibrate;
            worker.RunWorkerAsync();
            //initialize serialPort
            calibration_1 = new Timer();
            calibration_1.Tick += UpdateGyroDelayed;
            calibration_1.Interval = 250;
            calibration_1.Start();

            calibration_2 = new Timer();
            calibration_2.Tick += SecondCalibration;
            calibration_2.Interval = 250;
            //to start calibration after 5 seconds 

        }

        private void UpdateGyroDelayed(object sender, EventArgs e)
        {   //wait 5 seconds
            if (invoker < 10)
            {
                invoker++;
                return;
            }
            gyroDelayedX[delayedIndex] = gyroCurr[0];
            gyroDelayedY[delayedIndex] = gyroCurr[1];
            gyroDelayedZ[delayedIndex] = gyroCurr[2];
            delayedIndex++;
            delayedIndex %= 10;

            for(int i=0; i<flexNumber; i++)
            {
                flexDelayed[i][flexDelayedIndex] = flexCurr[i];
            }
            flexDelayedIndex++;
            flexDelayedIndex %= 10;

            //flexes

            //check if last 10 readings are close to each other 
            //check Y- error 
            gyroError = 0;
            for(int i=0; i<gyroDelayedX.Count-1; i++)
            {
                gyroError += Math.Abs(gyroDelayedX[i + 1] - gyroDelayedX[i]) +
                            Math.Abs(gyroDelayedY[i + 1] - gyroDelayedY[i]) +
                            Math.Abs(gyroDelayedZ[i + 1] - gyroDelayedZ[i]);
            }

            //check flexes error
            flexError = 0;
        for(int i=0; i<flexDelayed.Count-1;i++)
            {
                for(int j=0; j<flexDelayed[i].Count-1; j++)
                {
                    flexError += Math.Abs(flexDelayed[i][j + 1] - flexDelayed[i][j]);
                }
            }
            string flexLog = " ";
         for(int i=0; i<flexNumber; i++)
            {
                flexLog +=  flexCurr[i].ToString() + " | ";
            }
            string temp =  "curr x: " +
                   gyroCurr[0].ToString() + " y: " + gyroCurr[1].ToString()+" z: "+gyroCurr[2].ToString()+flexLog;
            label1.Text = temp;
            Console.WriteLine(temp);
            //1st calibration done then check second one ..
          if (gyroError < 0.05 &&flexError<0.1)
            {
                flexDelayedIndex = 0;
                //startX,startY
                calibrationDone = true;
                gyroStart[0] = gyroCurr[0];
                gyroStart[1] = gyroCurr[1];
                gyroStart[2] = gyroCurr[2];
                for (int i=0; i<flexNumber; i++)
                {
                    flexStart[i] = flexCurr[i];
                }
                      calibration_1.Stop();
                //   Console.Clear();
                //    label1.Text = " Alright, First calibration done !,  Now close your fest as powerful as you can";
                    
                settings = new List<double>();
                settings.Add(gyroStart[0]);
                settings.Add(gyroStart[1]);
                settings.Add(gyroStart[2]);
                //add flexes
                for (int i = 0; i < flexNumber; i++)
                    settings.Add(flexStart[i]);
               
                settingsFileWriter = new StreamWriter(settingsPath + ".txt");
                calibration_2.Start();


            }
        }

        private void SecondCalibration(object sender, EventArgs e)
        {   //wait 5 seconds
            if (secondCalibrationInvoker < 20)
            {
                secondCalibrationInvoker++;
                return;
            }
            

            for (int i = 0; i < flexNumber; i++)
            {
                flexDelayedClosed[i][flexDelayedIndex] = flexCurr[i];
            }
            flexDelayedIndex++;
            flexDelayedIndex %= 10;

            //flexes

            //check if last 10 readings are close to each other 
            //check Y- error 
        
            flexError = 0;
            for (int i = 0; i < flexNumber; i++)
            {
                for (int j = 0; j < flexDelayedClosed[i].Count - 1; j++)
                {
                    flexError += Math.Abs(flexDelayedClosed[i][j + 1] - flexDelayedClosed[i][j]);
                }
            }
            string flexLog = "flex error " + flexError.ToString() + " ";
            for (int i = 0; i < flexNumber; i++)
            {
                flexLog += "flex Curr " + i + " " + flexCurr[i].ToString() + "|| ";
            }
            label1.Text = flexLog;
           Console.WriteLine(flexLog);
            //1st calibration done then check second one ..
            if ( flexError < 0.1)
            {
                workerDone = true;
                worker.CancelAsync();
                worker.Dispose();
                worker = null;
                mySerial.Close();//startX,startY
                calibrationDone = true;
                for (int i = 0; i < flexNumber; i++)
                {
                    flexStartClosed[i] = Math.Min(flexCurr[i],0.4);
                    settings.Add(flexStartClosed[i]);
                }
                calibration_2.Stop();
                label1.Text = "2nd calibration done !";
                Console.Clear();
                String temp = "";


                temp += " GyroX : "+gyroStart[0].ToString()+" GyroY :"+gyroStart[1].ToString()+"\n";

                temp += " fingers : ";
                for (int i=0; i<flexNumber; i++)
                {
                    temp += " finger # " + i.ToString() + " min bending : " + flexStart[i].ToString()+
                        " max bending : "+flexStartClosed[i].ToString();
                }
                label1.Text = temp;


                //writting settings to file
                for (int i = 0; i < settings.Count; i++)
                {
                    settingsFileWriter.WriteLine(settings[i].ToString());
                }
                settingsFileWriter.Close();
              
               
                this.Close();
            }
        }
        private void serialPortData(object sender, SerialDataReceivedEventArgs e)
        {
            try { 
            string temp = ((SerialPort)sender).ReadLine();
            if (minRead<10)
            {
                minRead++;
                return;
            }

            if (temp != "" && temp[0] == 'g' && temp.LastIndexOf('g')==0&& temp.IndexOf('?') == -1 && temp.IndexOf(',') != -1)
            {
                string temp2 = (temp.Split('g'))[1];
                string[] temp3 = temp2.Split(',');
                gyroCurr[0] = (double.Parse(temp3[0]));
                gyroCurr[1] = (double.Parse(temp3[1]));
                gyroCurr[2] = (double.Parse(temp3[2]));
                //reading flexes(0,1)

                for (int i = 0; i < flexNumber; i++)
                { 
                    flexCurr[i] = (double.Parse(temp3[3 + i]));
                 //   Console.Write(flexCurr[i]+ ",");
                   }
               // Console.Write("\n");

                // Console.WriteLine(gyroCurr[0].ToString());
                // Console.WriteLine(gyroCurr[0]);
                //  Console.WriteLine(gyroCurr[1]);
                temp = "";
                };
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            
        }




        private void calibrate(object sender, DoWorkEventArgs e)
        {
            while (true&&!workerDone)
            {
                try
                {
                    string temp = mySerial.ReadLine();
                    
                 //   Console.WriteLine(temp);
                    while(minRead < 50)
                    {
                        minRead++;
                        mySerial.ReadLine();
                    }

                    if (temp != "" && temp[0] == 'g' && temp.LastIndexOf('g') == 0 && temp.IndexOf('?') == -1 && temp.IndexOf(',') != -1)
                    {
                        string temp2 = (temp.Split('g'))[1];
                        string[] temp3 = temp2.Split(',');
                        gyroCurr[0] = (double.Parse(temp3[0]));
                        gyroCurr[1] = (double.Parse(temp3[1]));
                        gyroCurr[2] = (double.Parse(temp3[2]));
                        //reading flexes(0,1)

                        for (int i = 0; i < flexNumber; i++)
                        {
                            flexCurr[i] = (double.Parse(temp3[3 + i]));
                            //   Console.Write(flexCurr[i]+ ",");
                        }
                        // Console.Write("\n");

                        // Console.WriteLine(gyroCurr[0].ToString());
                        // Console.WriteLine(gyroCurr[0]);
                        //  Console.WriteLine(gyroCurr[1]);
                        temp = "";
                    };
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }


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

            
            mySerial.Dispose();
            
            mySerial.Close();
        }
    }
}
