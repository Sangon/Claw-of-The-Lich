using UnityEngine;
using System.Collections;

public class blot_out_projectile_script : MonoBehaviour {

    float speed = 60f;
    float height = 800;
    Vector2 startPos;

	void Start() {
        startPos = gameObject.transform.position;
    }
	
	
	void FixedUpdate () {

        if (Vector2.Distance(gameObject.transform.position, startPos) >= height){
            Destroy(gameObject);
        }

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y - speed, transform.position.z);
        transform.position = newPos;

    }
}
