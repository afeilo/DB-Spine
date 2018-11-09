using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgSlot : UgTrans
{
    public int displayIndex = 0;
    public Color colorMul = Color.white;
    public Vector4 colorAdd = Vector4.zero;
    public UgBone parent;
    public List<UgSprite> sprites; 
    UgData.SlotFrameData[] frames;
    public void Init(UgData.SlotData data)
    {
        this.name = data.name;
        this.parentName = data.parent;
        this.displayIndex = data.displayIndex;
        this.colorMul = data.colorMul;
        this.colorAdd = data.colorAdd;
        sprites = new List<UgSprite>();
    }

    public void addSprite(UgSprite child) {
        sprites.Add(child);
    }

    public void SetAnimData(UgData.SlotFrameData[] frames)
    {
        this.frames = frames;
    }

    public void UpdateFrame(float frame)
    {

    }
}
