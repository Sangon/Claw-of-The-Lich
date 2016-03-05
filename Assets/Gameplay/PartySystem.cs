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

    private CameraMovement cameraMovement = null;

    // Use this for initialization
    void Start()
    {
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        updateCharacterList();
        character1 = characters[0];
        character2 = characters[1];
        character3 = characters[2];
        character4 = characters[3];
        selectAll();

    }

    public GameObject getFirstSelectedCharacter()
    {
        if (selectedCharacters.Count == 0)
            return null;
        return selectedCharacters[0];
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
    public int isSelected(GameObject character)
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

    //Palauttaa true/false riippuen onko parametrinä annettu hahmo valittuna vai ei.
    public bool isSelected2(GameObject character)
    {

        foreach (GameObject c in selectedCharacters)
        {
            if (c == character)
                return true;
        }
        return false;
    }

    private void selectAll()
    {
        selectedCharacters.Clear();
        selectCharacter(1, true);
        selectCharacter(2, true);
        selectCharacter(3, true);
        selectCharacter(4, true);
        print("All characters selected");
        //Camera.main.gameObject.transform.parent = selectedCharacters[0].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
    }

    private void selectCharacter(int characterNumber, bool add = false)
    {
        if (!add)
        {
            foreach (GameObject c in selectedCharacters)
            {
                if (c != null)
                    c.GetComponent<SpriteRenderer>().color = Color.white;
            }
            selectedCharacters.Clear();
        }
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

        if (character != null)
        {
            if (add && isSelected(character) != -1)
            {
                selectedCharacters.Remove(character);
                character.GetComponent<SpriteRenderer>().color = Color.white;
                print("Character#" + characterNumber + " deselected.");
            }
            else {
                selectedCharacters.Add(character);
                character.GetComponent<SpriteRenderer>().color = Color.black;
                print("Character#" + characterNumber + " selected.");
            }
        }

        cameraMovement.updateTarget();
        //if (selectedCharacters.Count < 4)
        //Camera.main.gameObject.transform.parent = null;
        //Camera.main.gameObject.transform.parent = selectedCharacters[selectedCharacters.Count - 1].transform;
        //Camera.main.transform.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y, -5000);
    }

    private void updateCharacterList()
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
