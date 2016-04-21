using UnityEngine;
using System.Collections;

public class blot_out_projectile_script : MonoBehaviour {

    float speed = 120f;
    float height = 800;
    Vector3 startPos;
    public Sprite sprite;

	void Start() {
        startPos = gameObject.transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, (startPos.z+800) / 100.0f + 800.0f + 0);
    }
	
	
	void FixedUpdate () {

	    if (Vector2.Distance(gameObject.transform.position, startPos) >= height)
	    {
            GetComponent<SpriteRenderer>().sprite = sprite;
            Destroy(gameObject, (1f));
	    }else{
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - speed, (startPos.z+800) / 100.0f + 800.0f + 0);
            transform.position = newPos;
            
        }
    }
}
