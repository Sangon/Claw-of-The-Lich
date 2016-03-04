using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

    }
    public void update(float size)
    {
        if (size < 0)
            size = 0;
        transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);
    }
}
