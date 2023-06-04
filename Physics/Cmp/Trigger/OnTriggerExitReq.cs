using Secs;
using UnityEngine;

namespace Secs
{
    public struct OnTriggerExitReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}