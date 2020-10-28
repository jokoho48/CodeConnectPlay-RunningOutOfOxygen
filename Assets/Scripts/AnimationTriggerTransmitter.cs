using UnityEngine;
using UnityEngine.Events;

public class AnimationTriggerTransmitter : MonoBehaviour
{
   [SerializeField] private UnityEvent animationEvent = new UnityEvent();
   public void TransmitAnimationEvent()
   {
      animationEvent.Invoke();
   }
}
