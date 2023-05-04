using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    IEnumerator TakeDamage(int _damage, float _knockback);
}
