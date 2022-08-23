using UnityEngine;

public class Missle : MonoBehaviour
{
    public bool OnTargetReached { get; private set; }

    public Vector3 TargetPos { get; set; }

    [SerializeField] float m_speed;
    [SerializeField] GameObject m_visual;
    [SerializeField] Transform m_starPoint, m_endPoint; //local point


    private Vector3 m_direct;
    private float m_timeNeed;
    public void Start()
    {
        Vector3 distance = m_endPoint.position - m_starPoint.position;
        m_direct = distance.normalized;

        m_timeNeed = ((Vector2)distance).magnitude / m_speed;

        OnTargetReached = false;
    }

    public void Fire()
    {
        if (m_visual)
        {
            Vector3 distance = m_endPoint.position - m_starPoint.position;
            m_direct = distance.normalized;

            m_timeNeed = ((Vector2)distance).magnitude / m_speed;
            
            OnTargetReached = false;

            m_visual.transform.position = m_starPoint.position;

            m_visual.LeanMove(m_endPoint.position, m_timeNeed).setEaseInCirc()
                .setOnComplete(() =>
                {
                    OnTargetReached = true;
                });
        }
    }
}