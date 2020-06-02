using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterUiIndicator : MonoBehaviour
{
    [Header("General")]
    /// how fast does indicators fade/show up?
    [Range(0, 1)] public float directionLerpFactor = 0.2f;
    [Range(0, 1)] public float fadeRatio = 0.2f;

    [Range(0, 1)] public float externalVisibility = 1.0f;

    #region AnimationIndicators

    [System.Serializable]
    public struct Indicator
    {
        /// casches renderers from obj
        [System.NonSerialized]
        public SpriteRenderer[] renderers;

        /// Maximal alpha value for all sprites
        [System.NonSerialized]
        public float alpha;

        /// data from last raycast
        [System.NonSerialized]
        public RaycastHit2D hit;

        public float rayDistance;

        /// object which owns Sprites 
        public GameObject obj;

        public bool use;
        public bool nonDependedOnInput;
    }
    [Header("Animation")]
    public Indicator[] animationIndicators;

    void UpdateIndicators(Indicator[] indicators, bool rev)
    {
        int i = 0;
        foreach (var ind in indicators)
        {
            foreach (var r in ind.renderers)
            {
                float desiredA = ind.use && (rev || ind.nonDependedOnInput) ? ind.alpha : 0f;
                Color cl = r.color;
                cl.a = Mathf.Lerp(r.color.a, desiredA*externalVisibility, fadeRatio);
                
                r.color = cl;
            }
            ++i;
        }
    }
    public void InitIndicators(Indicator[] indicators)
    {
        for (int i = 0; i < indicators.Length; ++i)
        {
            indicators[i].renderers = indicators[i].obj.GetComponentsInChildren<SpriteRenderer>();

            Debug.Assert(indicators[i].renderers != null && indicators[i].renderers.Length != 0);
            Color cl = indicators[i].renderers[0].color;
            indicators[i].alpha = cl.a;

            foreach (var it in indicators[i].renderers)
                it.color = new Color(cl.r, cl.g, cl.b, 0);
        }
    }
    #endregion AnimationIndicators

    #region EnvironmentIndicators
    [Header("Environment")]
    public float envRayLength;
    public float envRaySeparation;
    public float envRayInitialDistance;

    public Indicator[] environmentIndicators;

    bool CastRay(Vector2 position, Vector2 dir)
    {
        Vector2 origin = position + dir * envRayInitialDistance;
        RaycastHit2D hit = Physics2D.Raycast(
            origin, dir, envRayLength, 1 << LayerMask.NameToLayer("UiMarker")
        );
        Debug.DrawRay(origin, dir*envRayLength);

        if (hit.collider && !hit.collider.isTrigger)
        {
            Debug.Assert(hit.collider.gameObject != input.gameObject);

            environmentIndicators[0].use = true;

            var marker = hit.collider.GetComponent<CharacterUiMarker>();
            if (marker)
            {
                environmentIndicators[marker.type].use = true;
                environmentIndicators[marker.type].hit = hit;
                return true;
            }
        }
        return false;

    }
    void UpdateEnvironmentIndicators()
    {
        Vector2 mouseDir = input.directionInput.normalized;
        for (int i = 0; i < environmentIndicators.Length; ++i)
        {
            environmentIndicators[i].use = false;
        }

        bool b = false;
        b |= CastRay((Vector2)input.transform.position, mouseDir);
        b |= CastRay((Vector2)input.transform.position + new Vector2(-mouseDir.y, mouseDir.x) * envRaySeparation, mouseDir);
        b |= CastRay((Vector2)input.transform.position - new Vector2(-mouseDir.y, mouseDir.x) * envRaySeparation, mouseDir);
    }
    #endregion EnvironmentIndicators
    

    InputHolder input;
    float lastAngle;


    public void Awake()
    {
        input = GetComponentInParent<InputHolder>();
        InitIndicators(animationIndicators);
        InitIndicators(environmentIndicators);
    }

    void Update()
    {
        if (!transform.parent)
            return;

        if (input.atDirection)
        {
            float desiredAngle = Vector2.SignedAngle(Vector2.up, input.directionInput);
            lastAngle = Mathf.LerpAngle(lastAngle, desiredAngle, directionLerpFactor);
            transform.rotation = Quaternion.Euler(0, 0, lastAngle);

            UpdateEnvironmentIndicators();

            UpdateIndicators(animationIndicators, true);
            UpdateIndicators(environmentIndicators, true);
        }
        else
        {
            UpdateIndicators(animationIndicators, false);
            UpdateIndicators(environmentIndicators, false);
        }
    }



}
