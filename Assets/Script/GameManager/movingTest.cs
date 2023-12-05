
using UnityEngine;
public class movingTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0"))//
        {
            Debug.Log("button0");
        }
        if (Input.GetKeyDown("joystick button 1"))//
        {
            Debug.Log("button1");
        }
        if (Input.GetKeyDown("joystick button 2"))//
        {
            Debug.Log("button2");
        }
        if (Input.GetKeyDown("joystick button 3"))//
        {
            Debug.Log("button3");
        }
        if (Input.GetKeyDown("joystick button 4"))//
        {
            Debug.Log("button4");
        }
        if (Input.GetKeyDown("joystick button 5"))//
        {
            Debug.Log("button5");
        }
        if (Input.GetKeyDown("joystick button 6"))//
        {
            Debug.Log("button6");
        }
        if (Input.GetKeyDown("joystick button 7"))
        {
            Debug.Log("button7");
        }
        if (Input.GetKeyDown("joystick button 8"))
        {
            Debug.Log("button8");
        }
        if (Input.GetKeyDown("joystick button 9"))
        {
            Debug.Log("button9");
        }
        if (Input.GetKeyDown("joystick button 10"))
        {
            Debug.Log("button10");
        }
        if (Input.GetKeyDown("joystick button 11"))
        {
            Debug.Log("button11");
        }
        if (Input.GetKeyDown("joystick button 12"))
        {
            Debug.Log("button12");
        }
        if (Input.GetKeyDown("joystick button 13"))
        {
            Debug.Log("button13");
        }
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        if ((hori != 0) || (vert != 0))
        {
            Debug.Log("stick:" + hori + "," + vert);
        }
        //L Stick
        
        //R Stick
        float rsh = Input.GetAxis("R_Stick_H");
        float rsv = Input.GetAxis("R_Stick_V");
        if ((rsh != 0) || (rsv != 0))
        {
            Debug.Log("R stick:" + rsh + "," + rsv);
        }
        float tri = Input.GetAxis("L_R_Trigger");
        if (tri > 0)
        {
            Debug.Log("L trigger:" + tri);
        }
        else if (tri < 0)
        {
            Debug.Log("R trigger:" + tri);
        }
    }
}
