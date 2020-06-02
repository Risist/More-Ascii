using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputHolder))]
public class RigidbodyMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    [Range(0.0f, 1.0f), Tooltip("learp factor used to rotate body towards given direction")]
    public float rotationSpeed = 0.3f;
    [Space]
    [SerializeField] bool moveToDirection = true;
    [SerializeField] bool rotateToDirection = true;

    [System.NonSerialized] public bool atExternalRotation;

    Rigidbody2D body;
    InputHolder inputHolder;

    public float desiredRotation;
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        inputHolder = GetComponent<InputHolder>();

        desiredRotation = body.rotation;
    }

    void FixedUpdate()
    {
        UpdateRotation();
        UpdatePosition();
        atExternalRotation = false;
    }

    public void ApplyExternalRotation(Vector2 externalRotation, float rotationSpeed)
    {
        atExternalRotation = true;
        desiredRotation = -Vector2.SignedAngle(externalRotation, Vector2.up);

        float currentRotation = body.rotation;
        body.rotation = Mathf.LerpAngle(currentRotation, desiredRotation, rotationSpeed);
    }
    public void ApplyExternalRotationSide(Vector2 externalRotation, float maxDifference)
    {
        atExternalRotation = true;
        desiredRotation = -Vector2.SignedAngle(externalRotation, Vector2.up);

        float currentRotation = body.rotation;
        float difference = Mathf.Clamp(desiredRotation - currentRotation, -maxDifference, maxDifference);
        body.rotation = currentRotation + difference; //Mathf.Lerp(currentRotation, desiredRotation, rotationSpeed);
    }


    void UpdateRotation()
    {
        if (atExternalRotation || !rotateToDirection)
            return;

        else if (inputHolder.atRotation)
            desiredRotation = -Vector2.SignedAngle(inputHolder.rotationInput, Vector2.up);
        else if (inputHolder.atMove)
            desiredRotation = -Vector2.SignedAngle(inputHolder.positionInput, Vector2.up);
        // else;

        float currentRotation = body.rotation;
        body.rotation = Mathf.LerpAngle(currentRotation, desiredRotation, rotationSpeed);
    }
    void UpdatePosition()
    {
        if (!moveToDirection || !inputHolder.atMove)
            return;
            
        body.AddForce(inputHolder.positionInput.normalized * movementSpeed * body.mass);
    }
}
