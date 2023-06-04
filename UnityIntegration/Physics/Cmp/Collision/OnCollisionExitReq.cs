using Secs;
using UnityEngine;

namespace Secs
{
    public struct OnCollisionExitReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}