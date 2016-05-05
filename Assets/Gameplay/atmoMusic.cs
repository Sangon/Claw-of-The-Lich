using UnityEngine;
using System.Collections;

/* 
Numpad 1-3 atmo äänet
Numpad 4-6 musiikit
Numpad 7 ja 8 sateen voimmakkuus
*/

public class AtmoMusic : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string inputSound = "event:/sfx/hit_arrow";

    [FMODUnity.EventRef]
    public string atmoAll = "event:/atmo/atmo_all_included";
    FMOD.Studio.EventInstance atmoEv;
    FMOD.Studio.ParameterInstance atmoLoc;
    FMOD.Studio.ParameterInstance atmoRain;

    [FMODUnity.EventRef]
    public string music = "event:/music/game_music";
    FMOD.Studio.EventInstance musicEv;
    FMOD.Studio.ParameterInstance musicType;

    bool audioType;
    bool block = true;
    bool play = true;
    float volume;

    float rainlvl;

    void Start()
    {
        atmoEv = FMODUnity.RuntimeManager.CreateInstance(atmoAll);
        atmoEv.getParameter("location", out atmoLoc);
        atmoEv.getParameter("rain", out atmoRain);

        musicEv = FMODUnity.RuntimeManager.CreateInstance(music);
        musicEv.getParameter("music_style", out musicType);

        musicEv.getVolume(out volume);

        audioType = true;

        rainlvl = 0;
    }

    // Update is called once per frame
    void Update()
    {
        FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position);
        atmoEv.set3DAttributes(attributes);
        musicEv.set3DAttributes(attributes);


        if (audioType == true)
        {
            musicEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (block == true)
            {
                atmoEv.start();
                block = false;
            }
        }
        else
        {
            atmoEv.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            if (block == false)
            {
                musicEv.start();
                block = true;
            }
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            play = !play;
            if (!play)
            {
                atmoEv.setVolume(0);
                musicEv.setVolume(0);
            } else
            {
                atmoEv.setVolume(volume);
                musicEv.setVolume(volume);
            }

        }

        if (Input.GetKeyDown("space"))
        {
            FMODUnity.RuntimeManager.PlayOneShot(inputSound, Camera.main.transform.position);
            print("played");
        }

        if (Input.GetKeyDown("[1]"))
        {
            atmoLoc.setValue(0);
            print("forest");
            audioType = true;
        }

        if (Input.GetKeyDown("[2]"))
        {
            atmoLoc.setValue(1);
            print("gravey");
            audioType = true;
        }

        if (Input.GetKeyDown("[3]"))
        {
            atmoLoc.setValue(2);
            print("castle");
            audioType = true;
        }

        if (Input.GetKeyDown("[7]"))
        {
            rainlvl += 0.1f;
            if (rainlvl > 1)
                rainlvl = 1;
            atmoRain.setValue(rainlvl);
            print("rainlvl =" + rainlvl);
        }

        if (Input.GetKeyDown("[8]"))
        {
            rainlvl -= 0.1f;
            if (rainlvl < 0)
                rainlvl = 0;
            atmoRain.setValue(rainlvl);
            print("rainlvl =" + rainlvl);
        }

        if (Input.GetKeyDown("[4]"))
        {
            musicType.setValue(0);
            print("Style 1");
            audioType = false;
        }

        if (Input.GetKeyDown("[5]"))
        {
            musicType.setValue(0.5f);
            print("Style 2");
            audioType = false;
        }

        if (Input.GetKeyDown("[6]"))
        {
            musicType.setValue(1);
            print("Style 3");
            audioType = false;
        }

    }
}
