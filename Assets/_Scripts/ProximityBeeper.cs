using UnityEngine;

public class ProximityBeeper : MonoBehaviour
{
    public float maxDistance = 8.0f;

    private float beepTimer;

    void Update()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        float nearest = maxDistance;

        foreach (GameObject obstacle in obstacles)
        {
            float distance = Vector2.Distance(transform.position, obstacle.transform.position);
            if (distance < nearest)
            {
                nearest = distance;
            }
        }

        if (nearest >= maxDistance)
        {
            return;
        }

        float closeness = 1f - (nearest / maxDistance);

        beepTimer -= Time.deltaTime;
        if (beepTimer <= 0f)
        {
            SoundManager.Instance.PlayBeep(1f + closeness);
            beepTimer = 1f - closeness * 0.9f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
