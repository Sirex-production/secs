using System;
using UnityEngine;

namespace Secs
{
    public sealed class EcsPhysicsTriggerProvider : MonoBehaviour
    {
        [SerializeField] private bool onTriggerEnter;
        [SerializeField] private bool onTriggerStay;
        [SerializeField] private bool onTriggerExit;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!onTriggerEnter)
                return;
            
            EcsPhysics.RegisterTriggerEnterEvent(transform,other);
        }

        private void OnTriggerStay(Collider other)
        {
            if(!onTriggerStay)
                return;
            
            EcsPhysics.RegisterTriggerStayEvent(transform,other);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!onTriggerExit)
                return;
            
            EcsPhysics.RegisterTriggerExitEvent(transform,other);
        }
    }
}