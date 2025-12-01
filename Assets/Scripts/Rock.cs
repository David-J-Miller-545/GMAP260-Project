using UnityEngine;

public class Rock : MonoBehaviour
{
    public void OnCollisionEnter2D()
    {
        if (CameraHorizontalMover.Instance.following == transform)
        {
            CameraHorizontalMover.Instance.following = null;
        }
    }
}
