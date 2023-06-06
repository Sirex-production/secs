using Secs;
using UnityEngine;

namespace Secs
{
    public struct OnCollisionStayReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}