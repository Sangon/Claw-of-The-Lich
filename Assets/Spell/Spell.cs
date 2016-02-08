using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{

	public Vector2 castLocation;
	public int spellID;
	public string spellName;

	public void destroy(){
		Destroy (gameObject);
	}

	public Vector2 getCurrentMousePos(){
		Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
		return hit.point;
	}
}
