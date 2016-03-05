using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    private Vector2 mousePos;
    private int height;
    private int width;

    private PartySystem partySystem = null;

    private bool followTargets = true;
    private Transform target;

	// Use this for initialization
	void Start () {
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(Tuner.CAMERA_SCROLLING_SPEED, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(-Tuner.CAMERA_SCROLLING_SPEED, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, Tuner.CAMERA_SCROLLING_SPEED, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, -Tuner.CAMERA_SCROLLING_SPEED, 0);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            toggleLock();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            HealthBar[] healthBars = FindObjectsOfType(typeof(HealthBar)) as HealthBar[];
            foreach (HealthBar bar in healthBars)
            {
                bar.toggleVisibility();
            }
        }
    }

    private void toggleLock()
    {
        followTargets = !followTargets;
        updateTarget();
    }
    public void updateTarget()
    {
        if (followTargets && !partySystem.noneSelected())
        {
            Camera.main.transform.parent = partySystem.getFirstSelectedCharacter().transform;
            if (Camera.main.transform.parent != null)
                Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
        }
        else
            Camera.main.transform.parent = null;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 30, 300, 20), "Press F to Toggle Camera Lock to Selection");
        GUI.Label(new Rect(10, 50, 300, 20), "Press V to Toggle Show Healthbars");
        GUI.Label(new Rect(10, 70, 300, 20), "Press (Shift +) Num to (De)select a Character");
    }
}
