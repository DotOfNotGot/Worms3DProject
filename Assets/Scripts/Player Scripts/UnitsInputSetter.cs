using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsInputSetter : MonoBehaviour
{


    public void EnableUnitInput(PlayerInputManager passedInputManager)
    {
        passedInputManager.enabled = true;
        passedInputManager.ToggleInputOn(true);
    }

    public void DisableUnitInput(PlayerInputManager passedInputManager)
    {
        passedInputManager.ToggleInputOn(false);
        passedInputManager.enabled = false;
    }
}
