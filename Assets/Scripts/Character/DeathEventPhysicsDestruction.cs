using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpriteFader : MonoBehaviour
{
    public float fadeRatio;

    new SpriteRenderer renderer; 
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(renderer);
        a = renderer.color.a;
    }

    float a;

    private void FixedUpdate()
    {
        Color cl = renderer.color;
        a *= fadeRatio;
        cl.a = a;
        renderer.color = cl;
    }
}


public class DeathEventPhysicsDestruction : MonoBehaviour
{
    public float explosionForce;
    public float explosionRadius;
    public float bodyDrag;
    public float particleAliveTime = 0.5f;
    public float fadeRatio;
    public bool preSelectSprites = true;

    SpriteRenderer[] sprites;

    private void Awake()
    {
        if(preSelectSprites)
            sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        var health = GetComponentInParent<HealthController>();
        health.onDeathCallback += DeathEvent;
    }

    private void OnDestroy()
    {
        var health = GetComponentInParent<HealthController>();
        if(health)
            health.onDeathCallback -= DeathEvent;
    }


    void DeathEvent(DamageData data)
    {
        if(sprites == null)
            sprites = GetComponentsInChildren<SpriteRenderer>();

        int n = sprites.Length;
        for (int i = 0; i < n; ++i)
        {
            var obj = sprites[i].transform;
            obj.parent = null;

            var fader = obj.gameObject.AddComponent<SpriteFader>();
            fader.fadeRatio = Mathf.Lerp(fadeRatio, 1, 0.925f);

            var body = obj.gameObject.AddComponent<Rigidbody2D>();
            if (!body)
                body = obj.GetComponent<Rigidbody2D>();

            body.drag = bodyDrag;
            body.AddExplosionForce(-explosionForce, data.position, explosionRadius);

            Destroy(obj.gameObject, particleAliveTime * 1.25f);
        }

        Destroy(gameObject);
    }

}

public static class Rigidbody2DExtensions
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector2 explosionPosition, float explosionRadius)
    {
        var dir = (body.position - explosionPosition);
        if(dir.sqrMagnitude < 0.125f * 0.125f)
            dir = Random.insideUnitCircle * 1.5f;

        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff);
    }
}
