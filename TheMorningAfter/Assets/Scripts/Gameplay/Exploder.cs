using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object go boom
/// </summary>
public class Exploder : MonoBehaviour
{
    // the radius affected by the blast
    [SerializeField]
    private float radius = 5f;

    // the power of the blast
    [SerializeField]
    private float power = 100f;

    // the uplift of the blast
    [SerializeField]
    private float upliftModifier = 3.0f;

    // the animator for the explosion
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // boom
        AudioManager.Instance.PlayOneShot(AudioClipName.Explosion, 1.0f);

        // impart a force to all colliders withing range of the explosion.
        // The force will decrease with distance.

        // get the location of the explosion
        Vector3 explosionPos = transform.position;

        // get all colliders that lie within the explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(explosionPos.x, explosionPos.y), radius);

        // iterate over all the colliders and apply a force to them
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            // ignore everything except the player and the scenery obstacles
            if (rb != null && (rb.CompareTag(GameConstants.PLAYER) || rb.CompareTag(GameConstants.SCENERY_OBSTACLE)))
            {
                // apply force based on radius
                AddExplosionForce(rb, power, explosionPos, radius, upliftModifier);
            }
        }
    }

    private void Update()
    {
        // once the animation has completed, destroy the explosion
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            Destroy(this.gameObject);
    }

    // Unity 2D does not have an equivalent of the 3D AddExplosionForce(), so this method
    // replicates that in 2D.
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 1.0f)
    {
        // add a appropriate force in a direct line from the explosion
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;
        body.AddForce(baseForce, ForceMode2D.Impulse);

        // also add a slight uplift force, which can make the explosion look more dramatic.
        float upliftWearoff = 1 - upliftModifier / explosionRadius;
        Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
        body.AddForce(upliftForce, ForceMode2D.Impulse);
    }
}

