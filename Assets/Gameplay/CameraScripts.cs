using UnityEngine;
using System.Collections;

public class CameraScripts : MonoBehaviour
{
    private Vector2 mousePos;
    private int height;
    private int width;

    private PartySystem partySystem = null;

    private bool followTargets = true;
    private Transform target;

    // Use this for initialization
    void Start()
    {
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
        //FMODUnity.RuntimeManager.LowlevelSystem.set3DSettings(100000f, 100000f, 100000f);
    }

    // Update is called once per frame
    void Update()
    {
        height = Screen.height;
        width = Screen.width;

        //Rullaa kameraa kauemmas.
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += Tuner.CAMERA_ZOOM_SPEED;
        }
        //Rulla kameraa lähemmäs.
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= Tuner.CAMERA_ZOOM_SPEED;
        }
        //Rajoittaa kameran max- ja minimietäisyydet.
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Tuner.CAMERA_MIN_DISTANCE, Tuner.CAMERA_MAX_DISTANCE);

        mousePos = Input.mousePosition;

        float scrollSpeed = Tuner.CAMERA_SCROLLING_SPEED + (Tuner.CAMERA_SCROLLING_SPEED * (Camera.main.orthographicSize/500f));

        if (!followTargets)
        {
            if (mousePos.x > (width - (width * 0.05f)))
            {
                Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(scrollSpeed, 0, 0);
            }
            if (mousePos.x < (height * 0.05f))
            {
                Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(-scrollSpeed, 0, 0);
            }
            if (mousePos.y > (height - (height * 0.05f)))
            {
                Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, scrollSpeed, 0);
            }
            if (mousePos.y < (width * 0.05f))
            {
                Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, -scrollSpeed, 0);
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(scrollSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(-scrollSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, scrollSpeed, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.position = Camera.main.gameObject.transform.position + new Vector3(0, -scrollSpeed, 0);
        }
        /*
        FMOD.VECTOR a, b, c, d;
        FMODUnity.RuntimeManager.LowlevelSystem.get3DListenerAttributes(0, out a, out b, out c, out d);
        a.x = Camera.main.gameObject.transform.position.x / 1000f;
        a.y = Camera.main.gameObject.transform.position.y / 1000f;
        a.z = 0;
        FMODUnity.RuntimeManager.LowlevelSystem.set3DListenerAttributes(0, ref a, ref b, ref c, ref d);
        print("Came: " + a.x + " " + a.y + " " + a.z);
        */
        /*
        FMOD.VECTOR pos, vel, forward, up;
        FMOD.ChannelGroup group;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out group);
        //group.set3DMinMaxDistance(2000000f, 2000000f);
        FMODUnity.RuntimeManager.LowlevelSystem.get3DListenerAttributes(0, out pos, out vel, out forward, out up);
        print("1: " + pos.x + " " + pos.y + " " + pos.z);
        pos.x = Camera.main.transform.position.x;
        pos.y = Camera.main.transform.position.y;
        pos.z = Camera.main.transform.position.z;
        print("2: " + pos.x + " " + pos.y + " " + pos.z);
        FMODUnity.RuntimeManager.LowlevelSystem.set3DListenerAttributes(0, ref pos, ref vel, ref forward, ref up);
        print("3: " + pos.x + " " + pos.y + " " + pos.z);
        FMODUnity.RuntimeManager.LowlevelSystem.get3DListenerAttributes(0, out pos, out vel, out forward, out up);
        */
    }

    public void toggleLock()
    {
        followTargets = !followTargets;
        updateTarget();
    }

    public void updateTarget()
    {
        if (followTargets && !partySystem.noneSelected())
        {
            GameObject selection = partySystem.getFirstSelectedCharacter();
            if (selection != null)
            {
                Camera.main.transform.parent = selection.transform;
                Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
            }
        }
        else
            Camera.main.transform.parent = null;
    }
}
