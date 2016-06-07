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

    public void trigger(GameObject triggerer)
    {
        float triggerDuration = Time.fixedDeltaTime * 2f;
        if (triggerer.tag.Equals("Player"))
        {
            //Only players can make houses transparent
            triggered = triggerDuration;
            Walls.SetActive(false);
        }
        triggerer.GetComponent<Buffs>().addBuff(Buffs.BuffType.layerOrder, triggerDuration);
    }
}
