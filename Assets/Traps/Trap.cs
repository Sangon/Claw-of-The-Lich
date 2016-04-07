using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{

    public float triggerDistance = 200f;
    private float cooldownMax = 2f;
    private float cooldownNow = 0f;

    public void trigger()
    {
        if (cooldownNow <= 0f)
        {
            GameObject unit = Instantiate(Resources.Load("Melee"), transform.position, Quaternion.identity) as GameObject;
            cooldownNow = cooldownMax;
        }
    }

    public float getTriggerDistance()
    {
        return triggerDistance;
    }

    void FixedUpdate()
    {
        cooldownNow -= Time.fixedDeltaTime;
        if (cooldownNow <= 0f)
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        else
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
