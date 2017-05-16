using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_2 : MonoBehaviour {
    public float armX, armY, armZ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.eulerAngles =
             new Vector3(armX, armY, armZ);
    }
}
