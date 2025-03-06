using UnityEngine;

public class TrailControl : MonoBehaviour
{
    public TrailRenderer trail;
    public float speedThreshold = 3f;  // これ以上の速度でTrailをON

    public Rigidbody rb;

    void Start()
    {
        trail.enabled = false;
    }

    void Update()
    {
        // 速度が一定以上ならTrailをON、それ以下ならOFF
        trail.enabled = rb.linearVelocity.magnitude > speedThreshold;
    }
}
