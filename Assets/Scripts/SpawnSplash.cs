using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSplash : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private Transform splashManager;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
        {
            Vector3 position = collisionEvent.intersection;
            //GameObject temp = Instantiate(splashPrefab, position, Quaternion.identity, splashManager);
            GameObject splash = ObjectPool.SharedInstance.GetPooledObject();
            if (splash != null)
            {
                splash.transform.position = position;
                splash.transform.up = collisionEvent.normal;
                splash.transform.Rotate(90, 0, 0, Space.Self);
                splash.SetActive(true);
            }
            //temp.transform.up = collisionEvent.normal;
            //temp.transform.Rotate(90, 0, 0, Space.Self);
        }
    }
}
