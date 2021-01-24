using System.Collections.Generic;
using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.Interfaces;
using UnityEngine;

namespace Libs.GameFramework.Systems
{
    public class ObjectSpawnSystem : GameSystem
    {
        private Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();
        private Dictionary<GameObject, int> instanceMap = new Dictionary<GameObject, int>(); //BAD!, better solution?


        public virtual T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component
        {
            T instance;
            var prefabID = prefab.gameObject.GetInstanceID();
            if (pools.TryGetValue(prefabID, out var queue) && queue.Count > 0)
            {
                var item = queue.Dequeue();

                item.gameObject.SetActive(true);
                item.gameObject.transform.position = position;
                item.gameObject.transform.rotation = rotation;
                item.gameObject.transform.parent = parent;

                instance = item.GetComponent<T>();
            }
            else
            {
                instance = Object.Instantiate(prefab, position, rotation, parent);
            }

            instanceMap[instance.gameObject] = prefabID;
            //call poolable spawn event
            return instance;
        }


        public virtual void Destroy<T>(T item) where T : Component
        {
            var gameObject = item.gameObject;
            gameObject.SetActive(false);
            gameObject.GetComponents<IPoolable>().Foreach(OnDisposeItem);
            if (instanceMap.TryGetValue(gameObject, out var prefabId))
            {
                if (!pools.TryGetValue(prefabId, out var queue))
                {
                    queue = new Queue<GameObject>();
                    pools.Add(prefabId, queue);
                }

                queue.Enqueue(gameObject);
            }
            else
            {
                item.gameObject.SetActive(false);
                Object.Destroy(gameObject);
                //TODO create prefab from this?
            }
        }


        private void OnDisposeItem(IPoolable item)
        {
            item.OnDispose();
        }

        public override void Subscribe()
        {
        }

        public override void Unsubscribe()
        {
        }
    }
}