using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds Input collected from Input recorders
/// Separation of input and input result soo ai, player, network agents can use the same body and actions
/// </summary>
public class InputHolder : MonoBehaviour
{
    [HideInInspector] public Vector2 positionInput;
    [HideInInspector] public Vector2 directionInput;
    [HideInInspector] public Vector2 rotationInput;

    [HideInInspector] public bool[] keys = new bool[4];

    [Range(0.0f, 1.0f), Tooltip("if input source has value lower that this value input is ignored")]
    public float inputThreshold = 0.25f;

    public bool atRotation  { get { return rotationInput .sqrMagnitude > inputThreshold * inputThreshold; } }
    public bool atMove      { get { return positionInput .sqrMagnitude > inputThreshold * inputThreshold; } }
    public bool atDirection { get { return directionInput.sqrMagnitude > inputThreshold * inputThreshold; } }

    public void ResetInput()
    {
        positionInput = Vector2.zero;
        directionInput = Vector2.zero;
        rotationInput = Vector2.zero;

        for (int i = 0; i < keys.Length; ++i)
            keys[i] = false;
    }
    public void ResetInput(bool positionInput, bool rotationInput, bool directionInput, bool keys)
    {
        if(positionInput)
            this.positionInput = Vector2.zero;
        if (directionInput)
            this.directionInput = Vector2.zero;
        if (rotationInput)
            this.rotationInput = Vector2.zero;

        if(keys)
            for (int i = 0; i < this.keys.Length; ++i)
                this.keys[i] = false;
    }



    /*public static Vector2 GetStrifeInput(Vector2 directionInput, Vector2 positionInput)
    {
        return directionInput * positionInput.y +
               new Vector2(directionInput.y, -directionInput.x) * positionInput.x;
    }
    public Vector2 GetAllignedDirection(Vector2 directionInput, Vector2 positionInput)
    {
        Vector2[] directions = {
            directionInput,
            -directionInput,
            new Vector2(directionInput.y, -directionInput.x),
            new Vector2(-directionInput.y, directionInput.x)
        };


        Vector2 inputDirection = transform.up;

        float maxDot = float.NegativeInfinity;
        inputDirection = Vector2.zero;
        int currentDirectionId = 0;
        if (positionInput.sqrMagnitude < inputThreshold * inputThreshold)
            return inputDirection;

        for (int i = 0; i < directions.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            Debug.Log(newDot);
            if (newDot > maxDot)
            {
                maxDot = newDot;
                inputDirection = directions[i];
                currentDirectionId = i;
            }
        }

        return inputDirection;
    }*/
    public Vector2 GetAllignedDirection(float[] movementSpeed, Vector2 directionInput, Vector2 positionInput, int defaultDirection = -1, float minDotValue = -1f)
    {
        Vector2[] directions = {
                directionInput,
                -directionInput,
                new Vector2(directionInput.y, -directionInput.x),
                new Vector2(-directionInput.y, directionInput.x)
            };


        float maxDot = float.NegativeInfinity;
        Vector2 initialInputDirection = defaultDirection == -1 ? Vector2.zero : directions[defaultDirection] * movementSpeed[defaultDirection];
        for (int i = 0; i < movementSpeed.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            if (newDot > minDotValue && newDot > maxDot)
            {
                maxDot = newDot;
                initialInputDirection = directions[i] * movementSpeed[i];
            }
        }
        return initialInputDirection;
    }
    public Vector2 GetAllignedDirection(float movementSpeed, Vector2 directionInput, Vector2 positionInput, int defaultDirection = -1, float minDotValue = -1f)
    {
        Vector2[] directions = {
                directionInput,
                -directionInput,
                new Vector2(directionInput.y, -directionInput.x),
                new Vector2(-directionInput.y, directionInput.x)
            };


        float maxDot = float.NegativeInfinity;
        Vector2 initialInputDirection = defaultDirection == -1 ? Vector2.zero : directions[defaultDirection] * movementSpeed;
        for (int i = 0; i < directions.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            if (newDot > minDotValue && newDot > maxDot)
            {
                maxDot = newDot;
                initialInputDirection = directions[i] * movementSpeed;
            }
        }
        return initialInputDirection;
    }

    public Vector2 GetAllignedDirection(float[] movementSpeed, int defaultDirection = -1, float minDotValue = -1f)
    {
        Vector2 directionInput = this.directionInput.normalized;
        Vector2 positionInput = this.positionInput.normalized;

        Vector2[] directions = {
                directionInput,
                -directionInput,
                new Vector2(directionInput.y, -directionInput.x),
                new Vector2(-directionInput.y, directionInput.x)
            };


        float maxDot = float.NegativeInfinity;
        Vector2 initialInputDirection = defaultDirection == -1 ? Vector2.zero : directions[defaultDirection] * movementSpeed[defaultDirection];
        for (int i = 0; i < movementSpeed.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            if (newDot > minDotValue && newDot > maxDot)
            {
                maxDot = newDot;
                initialInputDirection = directions[i] * movementSpeed[i];
            }
        }
        return initialInputDirection;
    }
    public Vector2 GetAllignedDirection(float movementSpeed, int defaultDirection = -1, float minDotValue = -1f)
    {
        Vector2[] directions = {
                directionInput,
                -directionInput,
                new Vector2(directionInput.y, -directionInput.x),
                new Vector2(-directionInput.y, directionInput.x)
            };


        float maxDot = float.NegativeInfinity;
        Vector2 initialInputDirection = defaultDirection == -1 ? Vector2.zero : directions[defaultDirection] * movementSpeed;
        for (int i = 0; i < directions.Length; ++i)
        {
            var newDot = Vector2.Dot(positionInput, directions[i]);
            if (newDot > minDotValue && newDot > maxDot)
            {
                maxDot = newDot;
                initialInputDirection = directions[i] * movementSpeed;
            }
        }
        return initialInputDirection;
    }
}
