using DragonBones;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public SkeletonDataAsset data;
	// Use this for initialization
	void Start () {
        // Load Data
        //UnityFactory.factory.LoadDragonBonesData("bingyao/bingyao_ske");
        //UnityFactory.factory.LoadTextureAtlasData("bingyao/bingyao_tex");
        //var sourceArmature = UnityFactory.factory.GetArmatureData("armatureName");
        //for (int i = 0; i < 800; i++)
        //{
        //    var _armatureComp = UnityFactory.factory.BuildArmatureComponent("armatureName");
        //    UnityFactory.factory.ReplaceAnimation(_armatureComp.armature, sourceArmature);
        //    _armatureComp.transform.localPosition = new Vector3(0.0f, 0.0f, i);
        //    _armatureComp.animation.Play(null, 0);
        //    //var animation = SkeletonAnimation.NewSkeletonAnimationGameObject(data);
        //    ////animation.TrySetAnimation("attack");
        //    //animation.loop = true;
        //    //animation.AnimationName = "attack";
        //}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
