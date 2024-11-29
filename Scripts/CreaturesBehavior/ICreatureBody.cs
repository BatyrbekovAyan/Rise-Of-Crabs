using System.Collections;
using UnityEngine;

public interface ICreatureBody
{
    void Start();

    void FixedUpdate();

    void OnCollisionEnter2D(Collision2D collision);

    IEnumerator Hooked();

    IEnumerator Shot();

    void OnDestroy();

    IEnumerator DelayMove();

    void DisableScript();

    int GetNumber();

    void SetNumber(float multiplier);
}
