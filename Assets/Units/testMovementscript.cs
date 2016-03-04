using UnityEngine;
using System.Collections;

public class testMovementscript : MonoBehaviour {

    Vector2 asdpos;
	// Use this for initialization
	void Start () {
        asdpos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector2 asdasd = new Vector2(asdpos.x + Mathf.Sin(Time.frameCount / 30f) * 300f, asdpos.y + Mathf.Cos(Time.frameCount / 30f) * 300f);
        gameObject.GetComponent<UnitMovement>().moveTo(asdasd);

	}
}
