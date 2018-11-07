using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgRole : MonoBehaviour
{
    public UgData data;
    public Dictionary<string, UgArmature> armatures = new Dictionary<string, UgArmature>();

    UgArmature currentArmature;

    void Start()
    {
        Create();
    }


    void Create()
    {
        if (data == null)
            return;

        for (int i = 0; i < data.armatures.Length; i++)
        {
            UgArmature arm = new UgArmature();
            armatures.Add(data.armatures[i].name, arm);
            arm.Init(data.armatures[i]);
            currentArmature = arm;
            arm.CreateTest(transform);
        }

        //Play("stand");
    }

    public void SetCurrentAramture(string name)
    {
        currentArmature = armatures[name];
    }


    bool isPlaying = false;
    float pt;

    public void Play(string clipName)
    {
        pt = Time.time;
        isPlaying = true;
        currentArmature.Play(clipName);
    }

    void Update()
    {
        if (isPlaying)
        {
            float delta = Time.time - pt;
            pt = Time.time;
            currentArmature.UpdateFrame(delta);
        }
    }

}
