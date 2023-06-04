using Secs;
using UnityEngine;

namespace Secs
{
    public struct OnTriggerStayReq : IEcsComponent
    {
        public Transform senderObject;
        public Collider collider;
    }
}