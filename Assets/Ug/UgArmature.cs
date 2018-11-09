using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UgArmature : UgTrans
{
    public int frameRate;
    public string type;

    public Dictionary<string, UgBone> bones = new Dictionary<string, UgBone>();
    public Dictionary<string, UgSlot> slots = new Dictionary<string, UgSlot>();
    public Dictionary<string, UgSprite> sprites = new Dictionary<string, UgSprite>();
    Dictionary<string, UgData.AnimationData> animations = new Dictionary<string, UgData.AnimationData>();
    public List<UgBone> bonesList = new List<UgBone>();
    public List<UgSlot> slotList = new List<UgSlot>();
    public List<UgSprite> spriteList = new List<UgSprite>();
    UgData.AnimationData anim;
    public void Init(UgData.ArmatureData data)
    {
        this.name = data.name;
        this.frameRate = data.frameRate;
        this.type = data.type;


        for (int i = 0; i < data.boneDatas.Length; i++)
        {
            UgData.BoneData boneData = data.boneDatas[i];
            UgBone bone = new UgBone();
            if (boneData.parent != "root")
            {
                UgBone parent;
                bones.TryGetValue(boneData.parent, out parent);
                bone.parent = parent;
            }
            bone.Init(boneData);
            bones.Add(boneData.name, bone);
            bonesList.Add(bone);
        }

        for (int i = 0; i < data.slotDatas.Length; i++)
        {
            UgData.SlotData slotData = data.slotDatas[i];
            Debug.Log(slotData.name);
            UgSlot slot = new UgSlot();
            slot.Init(slotData);
            slots.Add(slot.name, slot);
            UgBone parent;
            bones.TryGetValue(slotData.parent, out parent);
            slot.parent = parent;
            parent.addSlot(slot);
            slotList.Add(slot);
        }

        for (int i = 0; i < data.displayDatas.Length; i++)
        {
            UgData.DisplayData disData = data.displayDatas[i];
            UgSprite sprite = new UgSprite();
            sprite.Init(disData);
            sprites.Add(disData.name, sprite);
            UgSlot parent;
            slots.TryGetValue(disData.parent, out parent);
            parent.addSprite(sprite);
            spriteList.Add(sprite);
        }

        for (int i = 0; i < data.animations.Length; i++)
        {
            UgData.AnimationData animData = data.animations[i];
            animations.Add(animData.name, animData);
        }

        foreach (var item in bones)
        {
            UgTrans trans = item.Value;
            if (trans.parentName != "")
            {
                bones[trans.parentName].AddChild(trans);
            }
            else
            {
                this.AddChild(trans);
            }
        }

        foreach (var item in slots)
        {
            UgTrans trans = item.Value;
            bones[trans.parentName].AddChild(trans);
        }

        foreach (var item in sprites)
        {
            UgTrans trans = item.Value;
            slots[trans.parentName].AddChild(trans);
        }
    }

    float frame = 0;
    public void Play(string name)
    {
        frame = 0;
        anim = animations[name];
        for (int i = 0; i < anim.boneAnimations.Length; i++)
        {
            UgData.BoneAnimationCollect bac = anim.boneAnimations[i];
            bones[bac.name].SetAnimData(bac.frames);
        }

        for (int i = 0; i < anim.slotAnimations.Length; i++)
        {
            UgData.SlotAnimationCollect sac = anim.slotAnimations[i];
            slots[sac.name].SetAnimData(sac.frames);
        }

        UpdateFrame(0);
    }

    public void UpdateFrame(float deltaTime)
    {
        frame += deltaTime * frameRate;
        frame %= anim.duration;

        for (int i = 0; i < anim.boneAnimations.Length; i++)
        {
            UgData.BoneAnimationCollect bac = anim.boneAnimations[i];
            bones[bac.name].UpdateFrame(frame);
        }

        for (int i = 0; i < anim.slotAnimations.Length; i++)
        {
            UgData.SlotAnimationCollect sac = anim.slotAnimations[i];
            slots[sac.name].UpdateFrame(frame);
        }


    }
}