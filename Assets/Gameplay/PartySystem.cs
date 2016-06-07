using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PartySystem : MonoBehaviour
{
    private GameObject character1;
    private GameObject character2;
    private GameObject character3;
    private GameObject character4;
    public List<GameObject> selectedCharacters;
    public List<GameObject> characters;
    public List<GameObject> aliveCharacters;
    private List<int> partyPositions;

    private CameraScripts cameraScripts;
    private PlayerHUD playerHUD;
    private GameHUD gameHUD;

    private GameObject mouseOverTarget;
    private bool mouseOverCharacter;

    public bool isMouseOverCharacter()
    {
        return mouseOverCharacter;
    }

    // Use this for initialization
    void Start()
    {
        cameraScripts = Camera.main.GetComponent<CameraScripts>();
        playerHUD = GameObject.Find("HUD").GetComponent<PlayerHUD>();
        gameHUD = GameObject.Find("HUD").GetComponent<GameHUD>();
        initPositions();
        initCharacterList();
        character1 = characters[0];
        character2 = characters[1];
        character3 = characters[2];
        character4 = characters[3];
        selectAll();
    }

    //1 - 4
    public GameObject getCharacter(int ID)
    {
        if (ID >= 1 && ID <= 4)
            return characters[ID - 1];
        return null;
    }

    public int getCharacterID(GameObject character)
    {
        int i = 1;
        foreach (GameObject c in characters)
        {
            if (character == c)
                return i;
            i++;
        }
        return -1;
    }

    public GameObject getFirstSelectedCharacter()
    {
        foreach (GameObject c in selectedCharacters)
        {
            if (c.GetComponent<UnitCombat>() != null && c.GetComponent<UnitCombat>().isAlive())
                return c;
        }
        return null;
    }

    public bool noneSelected()
    {
        if (selectedCharacters.Count == 0)
            return true;
        return false;
    }

    public bool noneAlive()
    {
        updateCharacterList();
        if (aliveCharacters.Count == 0)
            return true;
        return false;
    }

    // Returns -1 if the character is not selected, otherwise returns the index of the character in the selectedCharacters list
    public int getGroupID(GameObject character)
    {
        int groupID = 0;
        foreach (GameObject c in selectedCharacters)
        {
            if (c == character)
                return groupID;
            groupID++;
        }
        return -1;
    }
    public void initCharacterList()
    {
        characters.Add(GameObject.Find("Character#1"));
        characters.Add(GameObject.Find("Character#2"));
        characters.Add(GameObject.Find("Character#3"));
        characters.Add(GameObject.Find("Character#4"));
        aliveCharacters.Add(GameObject.Find("Character#1"));
        aliveCharacters.Add(GameObject.Find("Character#2"));
        aliveCharacters.Add(GameObject.Find("Character#3"));
        aliveCharacters.Add(GameObject.Find("Character#4"));
    }

    public void updateCharacterList()
    {
        for (int i = aliveCharacters.Count - 1; i >= 0; i--)
        {
            if (aliveCharacters[i].GetComponent<UnitCombat>() != null && !aliveCharacters[i].GetComponent<UnitCombat>().isAlive())
            {
                // Deselect selected dead character
                deselectCharacter(aliveCharacters[i]);
                //aliveCharacters[i].GetComponent<PlayerHUD>().Update();
                // Remove dead character from character list
                aliveCharacters.RemoveAt(i);
            }
        }
    }

    private void initPositions()
    {
        partyPositions = new List<int>(new int[4]);
        resetPositions();
    }

    public void resetPositions()
    {
        for (int i = 0; i < partyPositions.Count; i++)
            partyPositions[i] = -1;
    }

    public bool setPosition(int groupID, int position)
    {
        foreach (int p in partyPositions)
        {
            //print(groupID);
            if (p == position)
                return false;
        }
        partyPositions[groupID] = position;
        return true;
    }

    private void selectAll()
    {
        deSelectAll();
        selectCharacter(character1, true);
        selectCharacter(character2, true);
        selectCharacter(character3, true);
        selectCharacter(character4, true);
        //print("All characters selected");
        //Camera.main.gameObject.transform.parent = selectedCharacters[0].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
    }

    private void deSelectAll()
    {
        deselectCharacter(character1);
        deselectCharacter(character2);
        deselectCharacter(character3);
        deselectCharacter(character4);
    }

    public void deselectCharacter(GameObject character)
    {
        if (character != null && character.tag.Equals("Player"))
        {
            selectedCharacters.Remove(character);
            if (character.GetComponent<UnitCombat>() != null)
                if (character.GetComponent<UnitCombat>().isAlive())
                    character.GetComponent<SpriteRenderer>().color = Color.gray;
                else
                    character.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void selectCharacter(GameObject character, bool add = false)
    {
        if (!character.tag.Equals("Player"))
            return;

        if (character != null && character.GetComponent<UnitCombat>() != null && character.GetComponent<UnitCombat>().isAlive())
        {
            if (!add)
                deSelectAll();
            if (add && getGroupID(character) != -1)
                deselectCharacter(character);
            else {
                selectedCharacters.Add(character);
                character.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        else
        {
            deselectCharacter(character);
        }

        cameraScripts.updateTarget();
        //if (selectedCharacters.Count < 4)
        //Camera.main.gameObject.transform.parent = null;
        //Camera.main.gameObject.transform.parent = selectedCharacters[selectedCharacters.Count - 1].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);

    }

    void FixedUpdate()
    {
        updateCharacterList();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // The key below esc selects all characters
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                deSelectAll();
            else
                selectAll();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectCharacter(character1, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectCharacter(character2, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectCharacter(character3, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectCharacter(character4, Input.GetKey(KeyCode.LeftShift));
        }

        resetTarget();

        if (!gameHUD.isTargeting() && !playerHUD.isMouseOverHUD() && Input.GetKey(KeyCode.LeftControl))
            mouseOver(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (mouseOverTarget == null)
            mouseOverCharacter = false;
        else
            mouseOverCharacter = true;

        if (Input.GetMouseButtonDown(0) && mouseOverTarget != null)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
                deSelectAll();
            selectCharacter(mouseOverTarget, Input.GetKey(KeyCode.LeftShift));
        }

        //GameObject marker = GameObject.Find("DebugMarker");
        //marker.transform.position = character1.transform.position;
    }

    private void resetTarget()
    {
        if (mouseOverTarget != null)
        {
            if (getGroupID(mouseOverTarget) == -1)
                mouseOverTarget.GetComponent<SpriteRenderer>().color = Color.gray;
            else
                mouseOverTarget.GetComponent<SpriteRenderer>().color = Color.white;
            mouseOverTarget = null;
        }
    }

    // Select units by clicking them
    private void mouseOver(Vector2 ray)
    {
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;

        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);

        if (hits.Count > 0) //if no object was found there is no minimum
        {
            float min = hits[0].distance; //lets assume that the minimum is at the 0th place
            int minIndex = 0; //store the index of the minimum because thats how we can find our object

            for (int i = 1; i < hits.Count; ++i)// iterate from the 1st element to the last.(Note that we ignore the 0th element)
            {
                if (hits[i].distance < min) //if we found smaller distance and its not the player we got a new minimum
                {
                    min = hits[i].distance; //refresh the minimum distance value
                    minIndex = i; //refresh the distance
                }
            }
            GameObject bestTarget = hits[minIndex].gameObject;
            GameObject bestTargetOwner = bestTarget.transform.parent.gameObject.transform.parent.gameObject;
            if (bestTarget.tag.Equals("PlayerHitbox") && bestTargetOwner.GetComponent<UnitCombat>() != null && bestTargetOwner.GetComponent<UnitCombat>().isAlive())
            {
                mouseOverTarget = bestTargetOwner;
                mouseOverTarget.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
