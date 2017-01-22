using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float damping = 0.2f;

    private float m_OffsetZ;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;

    private void Start()
    {
        m_LastTargetPosition = target.position;
        m_OffsetZ = (transform.position - target.position).z;
        transform.parent = null;
    }

    private void Update()
    {
        Vector3 aheadTargetPos = target.position + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

        transform.position = newPos;

        m_LastTargetPosition = target.position;
    }
}
