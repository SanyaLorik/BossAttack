using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Object = UnityEngine.Object;

public enum PoolType {
    Bullets,
}


public class ObjectPoolManager : IInitializable {
    
    private GameObject _emptyHolder;
    private GameObject _bulletParent;

    private Dictionary<GameObject, ObjectPool<GameObject>> _objectPoolsDict;
    private Dictionary<GameObject, GameObject> _cloneToPrefabMap;
    
    // Чтоб не делать постоянный инжект
    private readonly HashSet<GameObject> _initialized = new();
    private DiContainer _container;
    
    
    [Inject]
    public void Init(DiContainer container) {
        Debug.Log("Init PoolManager");
        _container = container;
    }
    
    

    public void Initialize() {
        _objectPoolsDict = new();
        _cloneToPrefabMap = new();
        SetupEmpties();
    }


    
    public T Spawn<T>(GameObject prefab, Vector3 pos, PoolType poolType) where T : Component {
        if (!_objectPoolsDict.TryGetValue(prefab, out var pool)) {
            CreatePool(prefab);
            pool = _objectPoolsDict[prefab];
        }
        var obj = pool.Get();
        obj.transform.position = pos;
        _cloneToPrefabMap[obj] = prefab;
        obj.transform.SetParent(GetParent(poolType), false);
        
        
        obj.ActiveSelf();
        
        if (_initialized.Add(obj)) {
            _container.InjectGameObject(obj);
        }
        
        return obj.GetComponent<T>();
    }

    public void ReturnObjectToPool(GameObject obj, PoolType poolType) {
        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab)) {
            var parent = GetParent(poolType);
            if (obj.transform.parent != parent) {
                obj.transform.SetParent(parent);
            }

            if (_objectPoolsDict.TryGetValue(prefab, out var pool)) {
                pool.Release(obj);
            }
        }
        else {
            Debug.LogError("Обьект не найден при возвращении в пул");
        }
    }
    
    
    
    private Transform GetParent(PoolType poolType) {
        switch (poolType) {
            case PoolType.Bullets:
                return _bulletParent.transform;
            default:
                return _bulletParent.transform;;
        }
    }
    
    
    private void SetupEmpties() {
        _emptyHolder = new GameObject("Object Pool");
        
        _bulletParent = new GameObject("BulletParent Pool");
        _bulletParent.transform.SetParent(_emptyHolder.transform);
    }

    
    private void CreatePool(GameObject prefab) {
        ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
            createFunc:() => CreateObject(prefab),
            actionOnGet: OnGetObject,
            actionOnRelease: OnRealeseObject,
            actionOnDestroy: OnDestroyObject
        );
        _objectPoolsDict.Add(prefab, newPool);
    }
    
    private GameObject CreateObject(GameObject prefab) {
        prefab.DisactiveSelf();
        GameObject obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        prefab.ActiveSelf();
        return obj;
    }
    
    
    private void OnGetObject(GameObject obj) {
        
    }
    
    private void OnRealeseObject(GameObject obj) {
        obj.DisactiveSelf();
    }
    
    private void OnDestroyObject(GameObject obj) {
        _cloneToPrefabMap.Remove(obj);
    }



}
