using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartySystem : MonoBehaviour
{

    private GameObject character1;
    private GameObject character2;
    private GameObject character3;
    private GameObject character4;
    public List<GameObject> selectedCharacters;
    public List<GameObject> characters;

    private CameraScripts cameraScripts = null;

    // Use this for initialization
    void Start(){
        cameraScripts = Camera.main.GetComponent<CameraScripts>();
        updateCharacterList();
        character1 = characters[0];
        character2 = characters[1];
        character3 = characters[2];
        character4 = characters[3];
        selectAll();
    }

    public GameObject getFirstSelectedCharacter()
    {
        foreach (GameObject c in selectedCharacters)
        {
            if (c.activeSelf)
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
        if (characters.Count == 0)
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

    private void selectAll()
    {
        selectedCharacters.Clear();
        selectCharacter(1, true);
        selectCharacter(2, true);
        selectCharacter(3, true);
        selectCharacter(4, true);
        //print("All characters selected");
        //Camera.main.gameObject.transform.parent = selectedCharacters[0].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
    }

    private void selectCharacter(int characterNumber, bool add = false)
    {
        GameObject character = null;
        switch (characterNumber)
        {
            case 1:
                character = character1;
                break;
            case 2:
                character = character2;
                break;
            case 3:
                character = character3;
                break;
            case 4:
                character = character4;
                break;
            default:
                print("ERROR! selectCharacter(): characterNumber must be 1 - 4!");
                return;
        }
        if (character != null && character.activeSelf)
        {
            if (!add)
            {
                foreach (GameObject c in selectedCharacters)
                {
                    if (c.activeSelf)
                        c.GetComponent<SpriteRenderer>().color = Color.white;
                }
                selectedCharacters.Clear();
            }
            if (add && getGroupID(character) != -1)
            {
                selectedCharacters.Remove(character);
                character.GetComponent<SpriteRenderer>().color = Color.white;
                //print("Character#" + characterNumber + " deselected.");
            }
            else {
                selectedCharacters.Add(character);
                character.GetComponent<SpriteRenderer>().color = Color.black;
                //print("Character#" + characterNumber + " selected.");
            }
        } else
        {
            selectedCharacters.Remove(character);
        }

        cameraScripts.updateTarget();
        //if (selectedCharacters.Count < 4)
        //Camera.main.gameObject.transform.parent = null;
        //Camera.main.gameObject.transform.parent = selectedCharacters[selectedCharacters.Count - 1].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
    }

    public void updateCharacterList()
    {
        characters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    void FixedUpdate()
    {
        updateCharacterList();
    }

    // Update is called once per frame
    void Update()
    {
        // The key below esc selects all characters
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            selectAll();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectCharacter(1, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectCharacter(2, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectCharacter(3, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectCharacter(4, Input.GetKey(KeyCode.LeftShift));
        }
    }
}
