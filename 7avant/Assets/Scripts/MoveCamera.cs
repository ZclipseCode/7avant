using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraPosition;
    bool sliding;

    private void Update()
    {
        if (!sliding)
        {
            transform.position = cameraPosition.position;
        }
        else
        {
            transform.position = new Vector3(cameraPosition.position.x, cameraPosition.position.y / 1.5f, cameraPosition.position.z);
        }
    }

    public void SetSliding(bool value) => sliding = value;
}
