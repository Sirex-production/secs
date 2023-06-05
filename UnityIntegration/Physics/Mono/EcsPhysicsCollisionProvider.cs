using System;
using UnityEngine;

namespace Secs
{
    public sealed class EcsPhysicsCollisionProvider : MonoBehaviour
    {
        [SerializeField] private bool onCollisionEnter;
        [SerializeField] private bool onCollisionStay;
        [SerializeField] private bool onCollisionExit;
        private void OnCollisionEnter(Collision other)
        {
            if(!onCollisionEnter)
                return;
   
            EcsPhysics.RegisterCollisionEnterEvent(transform,other.collider);
        }

        private void OnCollisionStay(Collision other)
        {
            if(!onCollisionStay)
                return;
            
            EcsPhysics.RegisterCollisionStayEvent(transform,other.collider);
        }

        private void OnCollisionExit(Collision other)
        {
            if(!onCollisionExit)
                return;
            
            EcsPhysics.RegisterCollisionExitEvent(transform,other.collider);
        }
    }
}