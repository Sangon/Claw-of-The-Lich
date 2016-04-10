using UnityEngine;
using System.Collections;

public class HouseTransparency : MonoBehaviour
{

    private float triggered = 0;
    private GameObject Walls = null;

    void Start()
    {
        Walls = gameObject.transform.parent.transform.Find("Walls").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Walls.activeSelf)
        {
            triggered -= Time.fixedDeltaTime;
            if (triggered <= 0)
                Walls.SetActive(true);
        }
    }

    public void trigger()
    {
        triggered = Time.fixedDeltaTime * 2;
        Walls.SetActive(false);
    }
}
