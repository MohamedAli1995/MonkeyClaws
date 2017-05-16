using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;


public class script : MonoBehaviour {
    List<float> previous = new List<float> { 0, 0, 0 };
    string settingsPath = "C:\\Users\\muham\\Desktop\\monkeyClawsSettings";
    StreamWriter settingsFileWriter;
    StreamReader settingsFileReader;
    //gyro 
    List<float> gyroStart= new List<float> { 0, 0, 0 };
    List<float> gyroCurr = new List<float> { 0, 0, 0 };
    List<float> gyroPrev = new List<float> { 0, 0, 0 };
    List<float> gyroCalc = new List<float> { 0, 0, 0 };
    //flex
    List<float> flexStartMin=new List<float> { 0, 0, 0, 0, 0 };
	List<float> flexStartMax=new List<float> { 0, 0, 0, 0, 0 };
    private int flexNumber = 5;
    List<float> flexCurr = new List<float> { 0, 0, 0, 0, 0 };
    List<float> flexCalc = new List<float> { 0, 0, 0, 0, 0 };
    Timer timer;
    public SerialPort mySerial;
    Thread thread;
    string temp;

    //dummy
   public  float armX, armY, armZ;
    public Vector3 t, p1, p2, p3, p4,g;
    // Use this for initialization
    void Start () {
        Setup();
    
      
        p1 = palm_1.localEulerAngles;
       p2= palm_2.localEulerAngles;
        p3=palm_3.localEulerAngles;
        p4=palm_4.localEulerAngles;
        t=thumb.localEulerAngles;
        g = this.transform.eulerAngles+new Vector3(gyroStart[1], gyroStart[2], gyroStart[0]) ;

    }

    private void worker()
    {
        while(thread.IsAlive)
        {
          //  Debug.Log("gk");

             temp = mySerial.ReadLine();

           ///   Debug.Log(temp);

              string temp2 = (temp.Split('g'))[1];
              string[] temp3 = temp2.Split(',');
              gyroCurr[0] = (float.Parse(temp3[0]));
              gyroCurr[1] = (float.Parse(temp3[1]));
              gyroCurr[2] = (float.Parse(temp3[2]));
              if (firstTime)
              {
                  firstTime = false;
                  for (int i = 0; i < 3; i++)
                      previous[i] = gyroCalc[i];
              }

              for (int i = 0; i < flexNumber; i++)
              {
                  flexCurr[i] = (float.Parse(temp3[3 + i]));
              }
              gyroCalc[0] = gyroStart[0] - gyroCurr[0];
              gyroCalc[1] = gyroStart[1] - gyroCurr[1];
              gyroCalc[2] = gyroStart[2] - gyroCurr[2];

              /* if (error >= 0.5|| error <= -0.5 )
               {

                   Debug.Log("in");
                   for (int i = 0; i < 3; i++)
                       gyroCalc[i] = previous[i];

               }*/


            // Debug.Log("X : " + gyroCalculatedX.ToString() + " Y : " + gyroCalculatedY.ToString());
            for (int i = 0; i < flexNumber; i++)
              {
                  flexCurr[i] = (float.Parse(temp3[3 + i]));
                  flexCalc[i] = flexCurr[i] * -1 + flexStartMax[i];
              }
     
        }
    }

    Transform palm_1;
    Transform palm_2;
    Transform palm_3;
    Transform palm_4;
    Transform thumb;

    private void Setup()
    {
        thumb = GameObject.FindGameObjectWithTag("thumb").GetComponent<Transform>();
        palm_1 = GameObject.FindGameObjectWithTag("palm_1").GetComponent<Transform>();
        palm_2 = GameObject.FindGameObjectWithTag("palm_2").GetComponent<Transform>();
        palm_3 = GameObject.FindGameObjectWithTag("palm_3").GetComponent<Transform>();
        palm_4 = GameObject.FindGameObjectWithTag("palm_4").GetComponent<Transform>();

        mySerial = new SerialPort(@"\\.\" + "COM12", 115200);
        ;
        mySerial.ReadTimeout = 50;
     
       


        try
        {
            mySerial.Open();
            // mySerial.Close();
            thread = new Thread(new ThreadStart(worker));
           thread.Start();
        }
        catch (Exception ex)
        {
            // Failed to start thread
            Debug.Log("Error : " + ex.Message.ToString());
        }
        
        ReadSettings();

    }

    private void ReadSettings()
    {
        settingsFileReader = new StreamReader(settingsPath + ".txt");
        for(int i=0; i<3; i++)
         gyroStart[i] = (float.Parse(settingsFileReader.ReadLine()));
        for (int i = 0; i <flexNumber; i++)
            flexStartMax[i] = (float.Parse(settingsFileReader.ReadLine()));

        for (int i = 0; i < flexNumber; i++)
            flexStartMin[i] = (float.Parse(settingsFileReader.ReadLine()));

        settingsFileReader.Close();
/*
        for (int i = 0; i < 3; i++)
            Debug.Log(gyroStart[i]);

        for (int i = 0; i < flexNumber; i++)
            Debug.Log(flexStartMax[i]);

        for (int i = 0; i < flexNumber; i++)
            Debug.Log(flexStartMin[i]);
     
        for (int i = 0; i < 3; i++)
            gyroPrev[i] = gyroStart[i];*/
    }
    




    bool firstTime = true;
    // Update is called once per frame
    void Update() {
       
            palm_1.localEulerAngles= p1+new Vector3( Math.Abs(flexCalc[1]*180), 0, 0);
            palm_2.localEulerAngles = p2+new Vector3(Math.Abs(flexCalc[0] * 180), 0, 0);
            palm_3.localEulerAngles = p3+new Vector3(Math.Abs(flexCalc[4] * 180), 0, 0);
            palm_4.localEulerAngles =p4+ new Vector3(Math.Abs(flexCalc[2] * 220), 0, 0);
            thumb.localEulerAngles =t+ new Vector3(Math.Abs(flexCalc[3] * 220) *-1, 0, 0);
            this.transform.eulerAngles =g+new Vector3((gyroCalc[1] * 180+360)%360,
                (gyroCalc[2] * 180 + 360) % 360,
           ( (gyroCalc[0] * 180 + 360)% 360)*-1f);

        


        }


     void OnApplicationQuit()
    {
        mySerial.Close();
        Debug.Log("stopped");
    }

}




