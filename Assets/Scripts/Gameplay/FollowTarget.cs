using UnityEngine;

namespace PixelView.Time_.Gameplay
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform Target;

        public Vector3 Offset;


        private void LateUpdate()
        {
            if (Target == null)
                return;

            transform.position = Target.position + Offset;
        }
    }
}