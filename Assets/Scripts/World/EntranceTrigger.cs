using System;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    
    private Action<Collider> _onTriggerEnter;
    private Action<Collider> _onTriggerExit;

    public void Init(Action<Collider> onEnter, Action<Collider> onExit)
    {
        _onTriggerEnter = onEnter;
        _onTriggerExit = onExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        _onTriggerEnter?.Invoke(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        _onTriggerExit?.Invoke(other);
    }

    public void SetVisionDistance(float distance)
    {
        if (collider == null) return;
        if (collider is SphereCollider sphere) sphere.radius = distance;
        else if (collider is BoxCollider box) box.size = new Vector3(distance * 2, box.size.y, box.size.z);
    }
}