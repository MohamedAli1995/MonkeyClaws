using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using KeyboardInterceptor;
using WindowsInput.Native;
using System.Runtime.InteropServices;
using System.IO;

namespace monkeyclaws
{
  
    public partial class Form1 : Form
    { 
        
        //mode  1->mouse ,0->keyboard
        string settingsPath = "C:\\Users\\muham\\Desktop\\monkeyClawsSettings";
        int mode = 1;
        int mouseMinAngle = -40;
        int mouseMaxAngle = 40;
        int flexNumber = 5;
        int minRead = 0;
        int gyroHighRotationSpeed = 3;
        int gyroLowRotationSpeed = 1;
        double gyroThresholdLowSpeed = 0.3;
        double gyroThresholdHighSpeed = 0.3;
        List<double> gyroStart = new List<double> { 0, 0, 0 };
        List<double> gyroCurr = new List<double> { 0, 0, 0 };
        List<double> gyroPrev = new List<double> { 0, 0, 0 };
        List<ushort> gyroKeyCC = new List<ushort> { 0x11, 0x1E, 0x0 };//x ,y,z
        List<ushort> gyroKeyAC = new List<ushort> { 0x1F, 0x20, 0x00 };
        List<bool> gyroKeyCCPressed = new List<bool> { false, false, false };
        List<bool> gyroKeyACPressed = new List<bool> { false, false, false };
        List<bool> gyroKeyCCWasPressed = new List<bool> { false, false, false };
        List<bool> gyroKeyACWasPressed = new List<bool> { false, false, false };

        List<int>gyroCurrRotation = new List<int> { 0, 0, 0 };
        //flex 
        List<double> flexStart=new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<double> flexStartClosed = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<double> flexCurr=new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<ushort> flexKey = new List<ushort> {
            0x39,0xC8,0x0,0xC8,0x30 };
            List<bool> flexPressed = new List<bool> { false, false, false,false,false };
        List<bool> flexWasPressed = new List<bool> { false, false, false, false, false };

        Calibration cal;
        BackgroundWorker worker;
        SerialPort mySerial;
        InputSimulator inputManager;
        Timer worker2;
        KeyboardSimulator k;
        private bool gyroStarted = false;

        public Form1()
        {
            InitializeComponent();
            inputManager = new InputSimulator();
            

        }

        private void SettingUp()
        {
      
            StreamReader settingsFileReader;
            settingsFileReader = new StreamReader(settingsPath + ".txt");
            for (int i = 0; i < 3; i++)
            {
                gyroStart[i] = (double.Parse(settingsFileReader.ReadLine()));
                gyroPrev[i] = gyroStart[i];
            }
            for (int i = 0; i < flexNumber; i++)
            {
                flexStart[i] = (double.Parse(settingsFileReader.ReadLine()));
                
            }
            for (int i = 0; i < flexNumber; i++)
                flexStartClosed[i] = (double.Parse(settingsFileReader.ReadLine()));

            settingsFileReader.Close();
        }
        private void calibrate(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    string temp = mySerial.ReadLine();

                    //   Console.WriteLine(temp);
                    while (minRead < 10)
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
        private void serialPortData(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = ((SerialPort)sender).ReadLine();
            if (temp != "" && temp[0] == 'g' && temp.IndexOf('?') == -1 && temp.IndexOf(',') != -1)
            {
                string temp2 = (temp.Split('g'))[1];
                string[] temp3 = temp2.Split(',');
                for (int i = 0; i < 3; i++)
                    gyroCurr[i] =(double.Parse( temp3[i]));

              /*  if(Math.Abs(gyroCurr[0]-gyroPrev[0])>=0.2)
                {
                    for (int i = 0; i < 3; i++)
                        gyroCurr[i] = gyroPrev[i];

                }
                else
                {
                    for (int i = 0; i < 3; i++)
                        gyroPrev[i] = gyroCurr[i];

                }*/
                for (int i = 0; i < flexNumber; i++)
                    flexCurr[i]= (double.Parse(temp3[3+i]));

                gyroStarted = true;

            }
        }

        private void Calibrate_Click(object sender, EventArgs e)
        {
             cal = new Calibration();
            cal.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Start_Click(object sender, EventArgs e)
        {
            if(cal==null||!cal.calibrationDone)
            {
                label1.Text = "you need to calibrate first sir .";
            }
            else
            {
                Init();
                SettingUp();
                worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += calibrate;
                worker.RunWorkerAsync();
                Console.WriteLine("gyro started");
             
                //initialize the app
               
            }
        }

        private void Init()
        {
            mySerial = new SerialPort();
            mySerial.BaudRate = 115200;
            mySerial.PortName = "COM12";
           // mySerial.NewLine = Environment.NewLine;
           // mySerial.DataReceived += serialPortData;
            mySerial.Open();
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Update;
            worker.RunWorkerAsync();

            worker2 = new Timer();
            worker2.Interval = 50;
            worker2.Tick += Update2;
            worker2.Start();

        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        public static void PressKey(Keys key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
            {
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            }
            else
            {
                keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            }
        }
        [Flags]
        private enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags]
        private enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008,
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        public static void SendKey(ushort key)
        {
            Input[] inputs =
            {
        new Input
        {
            type = (int) InputType.Keyboard,
            u = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = 0,
                    wScan = key,
                    dwFlags = (uint) (KeyEventF.KeyDown | KeyEventF.Scancode),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            }
        }
    };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        public static void SendKeyUp(ushort key)
        {
            Input[] inputs =
            {
        new Input
        {
            type = (int) InputType.Keyboard,
            u = new InputUnion
            {
                ki = new KeyboardInput
                {
                    wVk = 0,
                    wScan = key,
                    dwFlags = (uint) (KeyEventF.KeyUp),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            }
        }
    };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        private struct Input
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public readonly MouseInput mi;
            [FieldOffset(0)]
            public KeyboardInput ki;
            [FieldOffset(0)]
            public readonly HardwareInput hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public readonly int dx;
            public readonly int dy;
            public readonly uint mouseData;
            public readonly uint dwFlags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public readonly uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }

        private void Update2(object sender, EventArgs e)
        {
            //   SendKeys.Send("a");
            //SendKey(0x1E);
            //Console.WriteLine("z");
            // Console.Write("sa");

        }

        private void Update(object sender, DoWorkEventArgs e)
        {
            while (true)
            { if (gyroStarted) {
                  //  Console.WriteLine((gyroStartX + gyroCurrX).ToString());
                }
                if (mode == 0) { 
                HandleGyro();
                ApplyGyro();
                HandleFlex();
               ApplyFlex();
                }
               else  //mouse mode
                {
                    HandleMouse();
                    HandleFlex();
                    //ApplyMouse();

                }
                //Console.Write(gyroKeyXCCPressed);
                //Console.Write(gyroKeyXACPressed);
                
           /*     Console.Write((gyroStartX + gyroCurrX).ToString());
                Console.Write("   ");
                Console.Write(gyroCurrRotationX.ToString());
                Console.Write("\n");*/
            }
        }

        private void ApplyFlex()
        {
            for (int i = 0; i<flexNumber; i++)
            {
                if (flexPressed[i] == true)
                {
                    Console.WriteLine(i);
                    SendKey(flexKey[i]);
                  

                }
                else
                {
                    if(flexWasPressed[i] )
                    {
                        SendKeyUp(flexKey[i]);
                        flexWasPressed[i] = false;
                    }
                   
                }
            }
        }
        int counter = 0;
        private void HandleFlex()
        {
            counter++;
            if (counter < 30)
                return;
            counter = 10;
            for (int i = 0; i < flexNumber; i++)
            {
                //  if (flexCurr[i] <= (flexStart[i]-flexStartClosed[i])/2)
                if (flexCurr[i] <= 0.5f)
                {
                    flexPressed[i] = true;
                 //   flexWasPressed[i] = true;
                   // Console.WriteLine("key pressed");

                }
                else
                {
                    flexPressed[i] = false;
                }
            }

            }

        private void ApplyGyroMouse()
        {
            
        }
        int neglect = 0;
        private void HandleMouse()
        {
            InputSimulator input = new InputSimulator();
            if (neglect < 20)
            {

                neglect++;
                return;
            }


            List<double> gyroCalculated = new List<double>
            {
                -gyroStart[0]+gyroCurr[0],-gyroStart[1]+gyroCurr[1],-1*(-gyroStart[2]+gyroCurr[2])
        };

            for (int i = 0; i < 3; i++)
            {
                gyroCalculated[i] =
                    MapRange(gyroCalculated[i], -0.4, 0.5, 0, 65535);
            }
            input.Mouse.MoveMouseTo(gyroCalculated[2], gyroCalculated[0]);
            //  double gyroYCalculated = -gyroStartY + gyroCurrY;
            //block mouse to 90,-90

            Console.Write(gyroCurr[0].ToString() + " ," + gyroCurr[1].ToString());
            Console.Write("\n");
            //  input.Mouse.Sleep(1);
            if (flexPressed[1])
            {
                if (!flexWasPressed[1])
                {
                    input.Mouse.LeftButtonDown();
                    flexWasPressed[1] = true;
                }

            }
            else
            {
                if (flexWasPressed[1])
                {
                    flexWasPressed[1] = false;
                    input.Mouse.LeftButtonUp();
                }
            };
            //2
            if (flexPressed[3])
            {
                if (!flexWasPressed[3])
                {
                    input.Mouse.RightButtonDown();
                    flexWasPressed[3] = true;
                }

            }
            else
            {
                if (flexWasPressed[3])
                {
                    flexWasPressed[3] = false;
                    input.Mouse.RightButtonUp();
                }
            }

        }

        //apply gyroEffect in x-axis
        private void ApplyGyro()
        {

            if (gyroKeyACPressed[0])
            {

                SendKey(gyroKeyAC[0]);
                System.Threading.Thread.Sleep(1);

            }
             else
              {
               
                  SendKeyUp(gyroKeyAC[0]);
              }
            /*
        else 
            {


            }
    */

            if (gyroKeyCCPressed[0])
            {
                SendKey(gyroKeyCC[0]);
                System.Threading.Thread.Sleep(1);


            }
            else
            {
                SendKeyUp(gyroKeyCC[0]);

            }

            for (int i=1; i<3; i++)
            {
                if (gyroKeyACPressed[i])
                {

                    SendKey(gyroKeyAC[i]);
                    System.Threading.Thread.Sleep(2);
                      SendKeyUp(gyroKeyAC[i]);

                }
              /*  else
                {
                    if(true)
                    {
                        SendKeyUp(gyroKeyAC[i]);
                        gyroKeyACWasPressed[i] = false;
                    }
                    SendKeyUp(gyroKeyAC[i]);
                }*/
                /*
            else 
                {
                    
                   
                }
        */

                if (gyroKeyCCPressed[i])
                {
                    SendKey(gyroKeyCC[i]);
                    System.Threading.Thread.Sleep(2);
                    SendKeyUp(gyroKeyCC[i]);

                }
           /*     else
                {
                    SendKeyUp(gyroKeyCC[i]);
                }
                *//*
                else
                {
                    if (true)
                    {
                        SendKeyUp(gyroKeyCC[i]);
                        gyroKeyCCWasPressed[i] = false;
                    }
                }*/
            }


        }

        //to handle gyro readings
        private void HandleGyro()
        {
           for(int i=0; i<3; i++)
            {
                /*    double gyroCalculated = -gyroStart[0] + gyroCurr[i];
                   //Console.Write("gyro #" + i + " : " + gyroCalculated.ToString());
                   if (gyroCalculated >gyroThresholdHighSpeed)
                   {

                       gyroKeyCCPressed[i] = true;
                       gyroKeyCCWasPressed[i] = true;

                       gyroKeyACPressed[i] = false;
                       gyroKeyACWasPressed[i] = false;

                   }
                   else if(gyroCalculated<-gyroThresholdHighSpeed)
                   {

                       gyroKeyCCPressed[i] = false;
                       gyroKeyCCWasPressed[i] = false;

                       gyroKeyACPressed[i] = true;
                       gyroKeyACWasPressed[i] = true;



                   }
                   else 
                   {
                       Console.WriteLine("both false");
                       gyroKeyCCPressed[i] = false;

                       gyroKeyACPressed[i] = false;

                       gyroKeyCCWasPressed[i] = false;
                       gyroKeyACWasPressed[i] = false;

                   }*/
                double gyroCalculated = -gyroStart[i] + gyroCurr[i];
                if (gyroCalculated > gyroThresholdLowSpeed && gyroCalculated < gyroThresholdHighSpeed)
                  {

                      gyroKeyCCPressed[i] = true;

                      gyroKeyACPressed[i] = false;
                      gyroCurrRotation[i] = gyroLowRotationSpeed;

                  }
                  else if (gyroCalculated > gyroThresholdHighSpeed)
                  {
                      gyroKeyCCPressed[i] = true;
                      gyroKeyACPressed[i] = false;
                      gyroCurrRotation[i] = gyroHighRotationSpeed;
                  }
                  else if (gyroCalculated < -gyroThresholdLowSpeed && gyroCalculated > -gyroThresholdHighSpeed)
                  {
                      gyroKeyCCPressed[i] = false;
                      gyroKeyACPressed[i] = true;
                      gyroCurrRotation[i] = gyroLowRotationSpeed;
                  }
                  else if (gyroCalculated < -gyroThresholdHighSpeed)
                  {
                      gyroKeyCCPressed[i] = false;
                      gyroKeyACPressed[i] = true;
                      gyroCurrRotation[i] = gyroHighRotationSpeed;
                  }
                  else if (gyroCalculated < gyroThresholdLowSpeed && gyroCalculated > -gyroThresholdHighSpeed)
                  {
                      gyroKeyCCPressed[i] = false;
                      gyroKeyACPressed[i] = false;
                  }
                  
            }

        }
        public double MapRange(double oldValue,double oldMin,double oldMax,double newMin,double newMax)
        {
            double newValue;
            double oldRange = (oldMax - oldMin);
            if (oldRange == 0)
                newValue = newMin;
            else
            {
                double newRange = (newMax - newMin);
                newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
            }
            return newValue;
        }
    }
}
