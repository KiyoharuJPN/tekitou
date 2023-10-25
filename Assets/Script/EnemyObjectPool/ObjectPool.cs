using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool instance;
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    //�I�u�W�F�N�g�v�[������P�̃I�u�W�F�N�g�����グ�Ďg��
    //�I�u�W�F�N�g�v�[���ɃI�u�W�F�N�g���Ȃ��Ƃ��͐V�����I�u�W�F�N�g������āA
    //�I�u�W�F�N�g�v�[���̒��ɓ����B
    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);
            if (pool == null)
            {
                pool = new GameObject("ObjectPool");
                pool.AddComponent<ObjectPoolScript>();
            }
                
            GameObject child = GameObject.Find(prefab.name);
            if (!child)
            {
                child = new GameObject(prefab.name);
                child.transform.SetParent(pool.transform);
            }
            _object.transform.SetParent(child.transform);
        }
        _object = objectPool[prefab.name].Dequeue();
        _object.SetActive(true);
        return _object;
    }

    //�K�v�̂Ȃ��I�u�W�F�N�g���I�u�W�F�N�g�v�[���ɒu���Ďg���̂�҂�
    public void PushObject(GameObject prefab)
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objectPool.ContainsKey(_name))
            objectPool.Add(_name, new Queue<GameObject>());
        objectPool[_name].Enqueue(prefab);
        prefab.SetActive(false);
    }
}
