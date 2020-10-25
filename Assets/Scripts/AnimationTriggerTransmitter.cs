using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AnimationTriggerTransmitter : MonoBehaviour
{
   [SerializeField] private UnityEvent animationEvent = new UnityEvent();
   public void TransmitAnimationEvent()
   {
      animationEvent.Invoke();
   }
}
