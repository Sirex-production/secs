using Secs;
using UnityEngine;

namespace Secs
{
    public struct OnTriggerEnterReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}