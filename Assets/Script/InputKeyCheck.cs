using UnityEngine;

public class InputKeyCheck
{
    public static bool GetAnyKey()
    {
        if (Input.anyKey || AllJoystickButtonCheck()|| JoystickCheck())
        {
            return true;
        }
        return false;
    }

    private static bool AllJoystickButtonCheck()
    {
        for(int i = 0; i >= 13; i++)
        {
            if (Input.GetKey("joystick button " + i))
            {
                return true;
            }
        }
        return false;
    }

    private static bool JoystickCheck()
    {
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0
            || Input.GetAxis("R_Stick_H") != 0 || Input.GetAxis("R_Stick_V") != 0
            || Input.GetAxis("L_R_Trigger") != 0)
        {
            return true;
        }
        return false;
    }
}
