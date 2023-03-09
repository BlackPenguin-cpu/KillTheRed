using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Entity
{
    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void Hit()
    {

    }
}
