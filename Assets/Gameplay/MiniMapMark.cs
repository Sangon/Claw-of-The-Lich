using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniMapMark : MonoBehaviour
{
    private MiniMap miniMap;
    private GameObject panel;
    private RectTransform myRectTransform;
    private GameObject minimapIcon;
    private Image image;
    private PartySystem partySystem;


    // Use this for initialization
    void Start()
    {
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        panel = GameObject.Find("MinimapPanel");
        miniMap = panel.GetComponent<MiniMap>();
        minimapIcon = transform.Find("Canvas").Find("MinimapIcon").gameObject;
        minimapIcon.name = "Icon_" + transform.name;
        myRectTransform = minimapIcon.GetComponent<RectTransform>();
        minimapIcon.transform.SetParent(panel.transform);
        myRectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        image = minimapIcon.GetComponent<Image>();
    }

    public void hideIcon()
    {
        minimapIcon.SetActive(false);
    }

    public void showIcon()
    {
        minimapIcon.SetActive(true);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPosition = miniMap.TransformPosition(transform.position);
        myRectTransform.localPosition = newPosition;
        if (gameObject.name.Contains("Character"))
        {
            if (partySystem.getGroupID(gameObject) == -1 && !image.color.Equals(Color.gray))
            {
                myRectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                image.color = Color.gray;
            }
            else if (partySystem.getGroupID(gameObject) != -1 && !image.color.Equals(Color.green))
            {
                myRectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                image.color = Color.green;
            }
        }
    }
}
