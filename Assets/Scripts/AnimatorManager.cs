using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private readonly static int[] animations =
    {
        Animator.StringToHash("IdleAnimation"),
        Animator.StringToHash("RunAnimation"),
        Animator.StringToHash("JumpAnimation"),
        Animator.StringToHash("FallingAnimation"),
        Animator.StringToHash("DeathAnimation")
    };

    protected Animator animator;
    private Animations[] currentAnimationArr;
    private bool[] layerLockedArr;
    // stores method called DefaultAnimation
    private Action<int> DefaultAnimation;

    // method to initialize animation. This is basically the Entry step in the Unity animator
    protected void Initialize(int layers, Animations startingAnimation, Animator animator, Action<int> DefaultAnimation){
        // set values
        layerLockedArr = new bool[layers];
        currentAnimationArr = new Animations[layers];
        this.animator = animator;
        this.DefaultAnimation = DefaultAnimation;

        // reset everything to default animation state
        for (int i = 0; i < layers; i++){
            layerLockedArr[i] = false;
            currentAnimationArr[i] = startingAnimation;
        }
    }

    // set animations for gameObject
    protected void Play(Animations animation, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f){
        // check if there is an animation and set it to default if there isn't
        if (animation == Animations.NONE){
            DefaultAnimation(layer);
            return;
        }

        // if the layer is locked and should not be bypassed, return
        if (layerLockedArr[layer] && !bypassLock){
            return;
        }
        // lock layer
        layerLockedArr[layer] = lockLayer;

        // animation is already playing so return
        if (currentAnimationArr[layer] == animation){
            return;
        }

        // update current animation for this layer
        currentAnimationArr[layer] = animation;
        // convert the animation hash for this layer to an int to work with CrossFade
        animator.CrossFade(animations[(int)currentAnimationArr[layer]], crossfade, layer);
    }

    // get the current animation from the layer passed
    public Animations GetCurrentAnimation(int layer){
        return currentAnimationArr[layer];
    }

    // lock the layer passed to the method
    public void LockLayer(int layer){
        layerLockedArr[layer] = true;
    }
}

public enum Animations{
    IDLE,
    RUN,
    JUMP,
    FALLING,
    DEATH,
    NONE
}
