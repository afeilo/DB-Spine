using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UgBone : UgTrans
{
    UgData.BoneFrameData[] frames;

    int frameIndex = 0;
    UgData.BoneFrameData cacheFrame;
    Vector2 cacheFrameRange;
    //public UgBone parent;
    //public List<UgSlot> slots;
    //public List<UgBone> bones;

    public void Init(UgData.BoneData data)
    {
        base.Init(data);

        //slots = new List<UgSlot>();
        //bones = new List<UgBone>();
        
    }

    //public void addSlot(UgSlot slot) {
    //    slots.Add(slot);
    //}

    //public void addBone(UgBone bone)
    //{
    //    bones.Add(bone);
    //}


    public void SetAnimData(UgData.BoneFrameData[] frames)
    {
        if (frames.Length == 0)
        {
            frames = null;
            return;
        }
        this.frames = frames;
        frameIndex = 0;
        CacheFrame();
    }

    void CacheFrame()
    {
        cacheFrame = frames[frameIndex];
        float min = cacheFrame.frameStart;
        float max = min + cacheFrame.duration;
        cacheFrameRange = new Vector2(min, max);
    }



    public void UpdateFrame(float frame)
    {
        //Debug.Log("UpdateFrame1");
        if (frames == null) return;
        //Debug.Log("UpdateFrame2");

        if (frame < cacheFrameRange.x)
            frameIndex = -1;
        while (frame >= cacheFrameRange.y || frame < cacheFrameRange.x)
        {
            frameIndex++;
            if (frameIndex >= frames.Length)
            {
                frameIndex = 0;
                CacheFrame();
                break;
            }
            CacheFrame();
        }


        

        if (frameIndex + 1 < frames.Length)
        {
            UgData.BoneFrameData next = frames[frameIndex + 1];

            float g = frame - cacheFrameRange.x;
            float t = cacheFrameRange.y - cacheFrameRange.x;
            float d = g / t;

            if (next.transPos)
                pos = Vector2.Lerp(startPos, startPos + next.pos, d);
            if (next.transScale)
                scale = Vector2.Lerp(startScale, next.scale, d);
            if (next.transRotate)
                rotation = Mathf.Lerp(startRotation,startRotation + next.rotation, d);
        }
        testObj.transform.localScale = new Vector3(scale.x, scale.y, 1);
        testObj.transform.localPosition = pos;
        testObj.transform.localEulerAngles = new Vector3(0, 0, rotation);
    }
}
