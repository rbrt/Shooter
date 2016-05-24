using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour {

    public enum EnemyStates 
    {
        Idle,
        Running,
        Attacking,
        Chasing
    }

    EnemyStates currentState;
    Transform targetEnemy;

    void Awake()
    {
        
    }

    void Update()
    {
        
    }
}
