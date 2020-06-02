using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Holds binding of axis codes with input recorder params
     */
[CreateAssetMenu(fileName = "InputBinding", menuName = "Ris/InputBinding")]
public class InputBinding : ScriptableObject
{
    public string positionAxisCodeX = "Horizontal";
    public string positionAxisCodeY = "Vertical";
    [Space]
    public string directionAxisCodeX = "Mouse X";
    public string directionAxisCodeY = "Mouse Y";
    [Space]
    public string[] keyAxisCode = new string[4];
}
