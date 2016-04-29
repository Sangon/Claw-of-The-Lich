using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    private UnitCombat unitCombat;

    private Transform healthBarIndicator = null;
    private Transform healthBarBorder = null;

    private bool visible = Tuner.SHOW_HEALTHBARS;

    // Use this for initialization
    void Start()
    {
        unitCombat = gameObject.GetComponent<UnitCombat>();

        healthBarIndicator = transform.Find("Canvas").Find("HPBarIndicator");
        healthBarBorder = transform.Find("Canvas").Find("HPBarBorder");

        if (healthBarIndicator && healthBarBorder)
        {
            healthBarIndicator.gameObject.SetActive(visible);
            healthBarBorder.gameObject.SetActive(visible);
        }
    }

    public void LateUpdate()
    {
        if (healthBarIndicator == null)
            return;

        float size = (unitCombat.getHealth() / unitCombat.getMaxHealth());

        if (size < 0)
            size = 0;
        else
            size *= 0.5f;

        healthBarIndicator.localScale = new Vector3(size, healthBarIndicator.localScale.y, healthBarIndicator.localScale.z);
    }

    public void toggleVisibility()
    {
        visible = !visible;

        if (healthBarIndicator && healthBarBorder)
        {
            healthBarIndicator.gameObject.SetActive(visible);
            healthBarBorder.gameObject.SetActive(visible);
        }
    }
}
