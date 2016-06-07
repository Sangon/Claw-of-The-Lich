using UnityEngine;
using System.Collections;

public class CastBar : MonoBehaviour
{
    private UnitCombat unitCombat;

    private Transform castBarIndicator;
    private Transform castBarBorder;

    private bool visible = Tuner.SHOW_CASTBARS;

    // Use this for initialization
    void Start()
    {
        unitCombat = gameObject.GetComponent<UnitCombat>();

        castBarIndicator = transform.Find("Canvas").Find("CastBarIndicator");
        castBarBorder = transform.Find("Canvas").Find("CastBarBorder");

        if (castBarIndicator && castBarBorder)
        {
            castBarIndicator.gameObject.SetActive(false);
            castBarBorder.gameObject.SetActive(false);
        }
    }

    public void LateUpdate()
    {
        if (castBarIndicator == null || !visible)
            return;

        float size = ((0.5f / unitCombat.getCastTimeMax()) * (unitCombat.getCastTimeMax() - unitCombat.getCastTime()));

        if (size < 0 || float.IsNaN(size))
            size = 0;

        if (size < 0.5f && size > 0)
            setVisible(true);
        else
            setVisible(false);

        castBarIndicator.localScale = new Vector3(size, castBarIndicator.localScale.y, castBarIndicator.localScale.z);
    }

    private void setVisible(bool visibility)
    {
        if (visible && castBarIndicator && castBarBorder)
        {
            castBarIndicator.gameObject.SetActive(visibility);
            castBarBorder.gameObject.SetActive(visibility);
        }
    }

    public void toggleVisibility()
    {
        visible = !visible;
        setVisible(visible);
    }
}
