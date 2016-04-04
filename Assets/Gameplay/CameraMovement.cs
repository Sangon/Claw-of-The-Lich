using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    private Vector2 mousePos;
    private int height;
    private int width;

	// Use this for initialization
	void Start () {
        height = Screen.height;
        width = Screen.width;
    }
	
	// Update is called once per frame
	void Update () {
        mousePos = Input.mousePosition;
        /*
        if (mousePos.x > width - 1 || Input.GetKey("right"))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(Tuner.CAMERA_SCROLLING_SPEED, 0, 0);
        }
        if (mousePos.x < 1 || Input.GetKey("left"))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(-Tuner.CAMERA_SCROLLING_SPEED, 0, 0);
        }
        if (mousePos.y > height - 1 || Input.GetKey("up"))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, Tuner.CAMERA_SCROLLING_SPEED, 0);
        }
        if (mousePos.y < 1 || Input.GetKey("down"))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, -Tuner.CAMERA_SCROLLING_SPEED, 0);
        }
        */

    }
}
