using Secs;
using UnityEngine;

namespace Secs 
{
    public struct OnCollisionEnterReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}