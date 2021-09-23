using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HemisphereBehavior : MonoBehaviour
{

    
    public TrainingSettings trainingSettings;
    public Renderer rend;

    float uniColor;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();


        // load stuff
        trainingSettings = JsonUtility.FromJson<TrainingSettings>(File.ReadAllText(Application.persistentDataPath + "/train_settings.json"));

        // update the single objects
        uniColor = trainingSettings.BackBright/10;
        Color emissionColor = new Color(uniColor, uniColor, uniColor);
        rend.material.SetColor("_EmissionColor", emissionColor);
    }

}
