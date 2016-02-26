using UnityEngine;
using Tiled2Unity;

public class AutoLayerSort : MonoBehaviour
{
    public bool autoUpdate = false;
    public float offset = 0;

    void Start()
    {
        sortLayer();
    }

    void Update()
    {
        if (autoUpdate)
            sortLayer();
        else
            Destroy(this);
    }

    public void sortLayer()
    {
        //GetComponent<SortingLayerExposed>().gameObject.GetComponent<MeshRenderer>().sortingOrder = (int)gameObject.transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.position.y / 100.0f + 800.0f + offset);
    }
}