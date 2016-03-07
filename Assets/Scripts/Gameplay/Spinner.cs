using UnityEngine;

namespace PixelView.Time_.Gameplay
{
    public class Spinner : MonoBehaviour
    {
        public Vector3 RotationSpeed;


        private void Update()
        {
            transform.rotation *= Quaternion.Euler(RotationSpeed * Time.deltaTime);
        }
    }
}