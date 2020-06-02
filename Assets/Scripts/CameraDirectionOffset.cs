using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirectionOffset : MonoBehaviour
{
    [Range(0.0f, 1.0f) ]public float directionLerpFactor = 0.1f;
    public float directionInputOffset = 0.0f;
    Vector2 direction;

    void Start()
    {
        var cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.directionOffset = GetOffset;
        cameraController.target = transform;
    }

    private void OnDestroy()
    {
        var cam = Camera.main;
        if (!cam) return;
        var cameraController = cam.GetComponent<CameraController>();
        if (!cameraController) return;
        if(cameraController.directionOffset == GetOffset)
            cameraController.directionOffset = () => { return Vector3.zero; };
    }


    Vector3 GetOffset()
    {
        var inputHolder = GetComponentInParent<InputHolder>();
        var desired = inputHolder.atDirection ?
            inputHolder.directionInput.normalized * directionInputOffset :
            Vector2.zero;

        direction = Vector2.Lerp(direction, desired, directionLerpFactor);
        return direction;
    }
}
