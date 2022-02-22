using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay
{
    public class GameplayItemSpawner : MonoBehaviour
    {
        [SerializeField] private BoxCollider spawnZone;
        [SerializeField, SuffixLabel( "s" )] private float spawnTimer;
        [SerializeField] private List<GameObject> itemsToSpawn;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;
            if( timer > spawnTimer )
            {
                timer = 0f;
                float x = Random.Range( spawnZone.bounds.min.x, spawnZone.bounds.max.x );
                float z = Random.Range( spawnZone.bounds.min.z, spawnZone.bounds.max.z );
                int index = Random.Range( 0, itemsToSpawn.Count );

                Instantiate( itemsToSpawn[index], new Vector3( x, 1f, z ), Quaternion.identity, transform );
            }
        }
    }
}
