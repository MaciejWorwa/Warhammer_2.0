using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private GameObject attackAnimationPrefab;
    [SerializeField] private GameObject damageAnimationPrefab;

    public void AttackAnimation(Vector3 position)
    {
        attackAnimationPrefab.transform.position = position;
        attackAnimationPrefab.SetActive(true);
        StartCoroutine(ExecuteAfterTime(1f, attackAnimationPrefab));
    }

    IEnumerator ExecuteAfterTime(float time, GameObject gameObject)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
