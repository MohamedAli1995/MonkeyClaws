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

namespace monkeyclaws
{
    public partial class Form1 : Form
    {

        int gyroHighRotationSpeed = 3;
        int gyroLowRotationSpeed = 1;

        bool gyroStarted = false;
        int gyroKeyXCC = 65;
        int gyroKeyYCC = 66;
        int gyroCurrRotationX = 0;

        int gyroKeyXAC = 67;
        int gyroKeyYAC = 68;
        int gyroCurrRotationY = 0;

        bool gyroKeyXCCPressed = false;
        bool gyroKeyYCCPressed =false;

        bool gyroKeyXACPressed =false;
        bool gyroKeyYACPressed = false;

        double gyroStartX = 1000;
        double gyroStartY = 1000;

        double gyroCurrX = 10000000;
        double gyroCurrY = 1000000;

        double gyroThresholdLowSpeed = 0.2;
        double gyroThresholdHighSpeed = 0.5;


        Calibration cal;
        BackgroundWorker worker;
        SerialPort mySerial;
        InputSimulator inputManager;
        Timer worker2;
        KeyboardSimulator k;
      
        public Form1()
        {
            InitializeComponent();
            inputManager = new InputSimulator();


        }

        private void serialPortData(object sender, SerialDataReceivedEventArgs e)
        {
            string temp = ((SerialPort)sender).ReadLine();
            if (temp != "" && temp[0] == 'g' && temp.IndexOf('?') == -1 && temp.IndexOf(',') != -1)
            {
                string temp2 = (temp.Split('g'))[1];
                string[] temp3 = temp2.Split(',');
                gyroCurrX = (double.Parse(temp3[0]));
                gyroCurrY = (double.Parse(temp3[1]));
                // Console.WriteLine(gyroCurr[0].ToString());
                // Console.WriteLine(gyroCurr[0]);
                //  Console.WriteLine(gyroCurr[1]);
                temp = "";
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
                gyroStartX = cal.gyroStart[0];
                gyroStartY = cal.gyroStart[1];
                Console.WriteLine("gyro started");
                //initialize the app
                Init();
            }
        }

        private void Init()
        {
            mySerial = new SerialPort();
            mySerial.BaudRate = 115200;
            mySerial.PortName = "COM7";
            mySerial.NewLine = Environment.NewLine;
            mySerial.DataReceived += serialPortData;
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
                HandleGyroX();
                ApplyGyroX();
                bool f = true;
                //Console.Write(gyroKeyXCCPressed);
                //Console.Write(gyroKeyXACPressed);
                
                Console.Write((gyroStartX + gyroCurrX).ToString());
                Console.Write("   ");
                Console.Write(gyroCurrRotationX.ToString());
                Console.Write("\n");
            }
        }
        //apply gyroEffect in x-axis
        private void ApplyGyroX()
        {


            if (gyroKeyXACPressed)
            {
                // inputManager.Keyboard.KeyDown((VirtualKeyCode)gyroKeyXCC);
                // PressKey(Keys.A, false);
                //   PressKey(Keys.A, true);
                //k.KeyDown(VirtualKeyCode.VK_A);

               
                SendKey(0x20);
                System.Threading.Thread.Sleep(gyroCurrRotationX);
                SendKeyUp(0x20);
                //  Console.WriteLine(gyroKeyXCCPressed);
            }
            if (gyroKeyXCCPressed)
            {
                // inputManager.Keyboard.KeyDown((VirtualKeyCode)gyroKeyXCC);
                // PressKey(Keys.A, false);
                //   PressKey(Keys.A, true);
                //k.KeyDown(VirtualKeyCode.VK_A);
              
                SendKey(0x1E);
                System.Threading.Thread.Sleep(gyroCurrRotationX);
                // Console.WriteLine(gyroKeyXCCPressed);
                SendKeyUp(0x1E);
            }
            
       
          

        }
        
        //to handle gyro readings
        private void HandleGyroX()
        {
            double gyroXCalculated = gyroStartX + gyroCurrX;
            if (gyroXCalculated > gyroThresholdLowSpeed&& gyroXCalculated < gyroThresholdHighSpeed)
            {
               
                gyroKeyXCCPressed = true;
                Console.WriteLine(gyroKeyXCCPressed);
                gyroKeyXACPressed = false;
                gyroCurrRotationX = gyroLowRotationSpeed;

            }
           else if (gyroXCalculated >gyroThresholdHighSpeed)
            {
                gyroKeyXCCPressed = true;
                gyroKeyXACPressed = false;
                gyroCurrRotationX = gyroHighRotationSpeed;
            }
            else if (gyroXCalculated < -gyroThresholdLowSpeed && gyroXCalculated > -gyroThresholdHighSpeed)
            {
                gyroKeyXCCPressed = false;
                gyroKeyXACPressed = true;
                gyroCurrRotationX = gyroLowRotationSpeed;
            }
            else if (gyroXCalculated < -gyroThresholdHighSpeed)
            {
                gyroKeyXCCPressed = false;
                gyroKeyXACPressed = true;
                gyroCurrRotationX = gyroHighRotationSpeed;
            }
            else if (gyroXCalculated < gyroThresholdLowSpeed && gyroXCalculated > -gyroThresholdHighSpeed)
            {
                gyroKeyXCCPressed = false;
                gyroKeyXACPressed = false;
            }

        }
    }
}
