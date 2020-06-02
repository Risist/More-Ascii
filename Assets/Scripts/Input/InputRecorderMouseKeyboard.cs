using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecorderMouseKeyboard : InputRecorder
{
    public InputBinding binding;

    Camera cam;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        cam = Camera.main;
    }

    private void Update()
    {
        Debug.Assert(binding);
        Debug.Assert(binding.keyAxisCode.Length >= inputHolder.keys.Length);

        

        /// position
        inputHolder.positionInput.x = Input.GetAxis(binding.positionAxisCodeX);
        inputHolder.positionInput.y = Input.GetAxis(binding.positionAxisCodeY);

        /// direction
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var box = GetComponent<BoxCollider>();

        if (box && box.Raycast(ray, out hit, float.PositiveInfinity))
        {
            Vector3 v = hit.point;
            inputHolder.directionInput = new Vector2(v.x - inputHolder.transform.position.x, v.y - inputHolder.transform.position.y);

        }
        else
            Debug.LogWarning("player direction raycast does not hit a collider");

        /// keys
        for (int i = 0; i < inputHolder.keys.Length; ++i)
        {
            inputHolder.keys[i] = Input.GetButton(binding.keyAxisCode[i]);
        }
    }
}
