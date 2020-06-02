using Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;


/*
 * Highly game specific class
 */
public static class AiBehaviourPackUtility
{
    #region AwarenessIds
    /*public const int awarenessEnemy = 0;
    public const int awarenessUnknown = 1;

    public const int awarenessWantsToTalk = 2;*/
    #endregion 

    // TODO setup attention utility params
    public static FocusPriority GetFocusPriority(AiBehaviourController controller)
    {
        // check if focus was already created and if soo return it
        FocusPriority focusPriority = (FocusPriority)controller.GetBlackboardValue<FocusOwned>("priorityFocus")?.value;
        if (focusPriority != null)
            return focusPriority;

        // otherwise create a new one
        FocusOwned CreatePriorityFocusDelegate()
            => new FocusPriority(controller.transform);
        focusPriority = (FocusPriority)controller.InitBlackboardValue("priorityFocus", CreatePriorityFocusDelegate).value;

        focusPriority.SetSustainUtility(0.1f);

        // prepare partial focuses
        FocusOwned enemyFocus = GetFocusEnemy(controller);
        FocusOwned painFocus = GetFocusPain(controller);
        FocusOwned touchFocus = GetFocusTouch(controller);
        FocusOwned noiseFocus = GetFocusNoise(controller);

        if (enemyFocus != null)
        {
            /// enemy
            AttentionMode CreateAttentionMode() => focusPriority.AddAttentionMode(new AttentionMode(enemyFocus));
            AttentionMode attentionMode = controller.InitBlackboardValue("attentionEnemy", CreateAttentionMode).value;

            AttentionUtility attentionUtility = new AttentionUtility() {
                attackTime = 0.5f,
                attackPeek = 1.0f,
                sustainValue = 0.5f,
            };
            attentionMode.SetUtility(() => attentionMode.focus.HasTarget() ? attentionUtility.GetUtility(attentionMode.activeTime) : 0 );
        }

        // noise
        if (noiseFocus != null)
        {
            /// enemy
            AttentionMode CreateAttentionMode() => focusPriority.AddAttentionMode(new AttentionMode(noiseFocus));
            AttentionMode attentionMode = controller.InitBlackboardValue("attentionNoise", CreateAttentionMode).value;
            
            AttentionUtility attentionUtility = new AttentionUtility() {
                attackTime = 0.25f,
                attackPeek = 0.85f,
                sustainValue = 0.45f,
            };
            attentionMode.SetUtility(() => attentionMode.hadTarget ? attentionUtility.GetUtility(attentionMode.activeTime) : 0 );
        }

        // pain
        if (painFocus != null)
        {
            /// enemy
            AttentionMode CreateAttentionMode() => focusPriority.AddAttentionMode(new AttentionMode(painFocus));
            AttentionMode attentionMode = controller.InitBlackboardValue("attentionPain", CreateAttentionMode).value;
            
            AttentionUtility attentionUtility = new AttentionUtility() {
                attackTime = 0.45f,
                attackPeek = 0.8f,
                sustainValue = 0.4f,
            };
            attentionMode.SetUtility(() => attentionMode.focus.HasTarget() ? attentionUtility.GetUtility(attentionMode.activeTime) : 0); ;
        }

        // touch
        if(touchFocus != null)
        {
            /// enemy
            AttentionMode CreateAttentionMode() => focusPriority.AddAttentionMode(new AttentionMode(touchFocus));
            AttentionMode attentionMode = controller.InitBlackboardValue("attentionTouch", CreateAttentionMode).value;
            
            AttentionUtility attentionUtility = new AttentionUtility() {
                attackTime = 0.25f,
                attackPeek = 0.65f,
                sustainValue = 0.3f,
            };
            attentionMode.SetUtility(() => attentionMode.hadTarget ? attentionUtility.GetUtility(attentionMode.activeTime) : 0 );
        }

        // idle
        {
            /// enemy
            AttentionMode CreateAttentionMode() => focusPriority.AddAttentionMode(new AttentionMode());
            AttentionMode attentionMode = controller.InitBlackboardValue("attentionIdle", CreateAttentionMode).value;
            
            attentionMode.SetUtility(() => 0.15f);
        }

        return focusPriority;
    }

    #region attention
    public static AttentionMode GetAttentionEnemy(AiBehaviourController controller)
    {
        return controller.GetBlackboardValue<AttentionMode>("attentionEnemy").value;
    }
    public static AttentionMode GetAttentionNoise(AiBehaviourController controller)
    {
        return controller.GetBlackboardValue<AttentionMode>("attentionNoise").value;
    }
    public static AttentionMode GetAttentionPain(AiBehaviourController controller)
    {
        return controller.GetBlackboardValue<AttentionMode>("attentionPain").value;
    }
    public static AttentionMode GetAttentionTouch(AiBehaviourController controller)
    {
        return controller.GetBlackboardValue<AttentionMode>("attentionTouch").value;
    }
    public static AttentionMode GetAttentionIdle(AiBehaviourController controller)
    {
        return controller.GetBlackboardValue<AttentionMode>("attentionIdle").value;
    }

    #endregion attention

    #region building focuses
    public static FocusFilterStorage GetFocusEnemy(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 0.0f;
        const float maxLifetime = 500;

        const string storageName = "enemyStorage";
        const string focusName = "enemyFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, distanceSortScale, timeSortScale, maxLifetime);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }
    public static FocusFilterStorage GetFocusEnemyBehind(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 1.0f;
        Vector2 offset = new Vector2(0, 4.0f);

        const string storageName = "enemyStorage";
        const string focusName = "enemyBehindFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, offset, distanceSortScale, timeSortScale);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }

    public static FocusFilterStorage GetFocusAlly(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 1.0f;

        const string storageName = "allyStorage";
        const string focusName = "allyFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, distanceSortScale, timeSortScale);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }
    public static FocusFilterStorage GetFocusNoise(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 1.0f;

        const string storageName = "noiseStorage";
        const string focusName = "noiseFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, distanceSortScale, timeSortScale);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }
    public static FocusFilterStorage GetFocusPain(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 1.0f;

        const string storageName = "painStorage";
        const string focusName = "painFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, distanceSortScale, timeSortScale);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }
    public static FocusFilterStorage GetFocusTouch(AiBehaviourController controller)
    {
        const float timeSortScale = 1.0f;
        const float distanceSortScale = 1.0f;

        const string storageName = "touchStorage";
        const string focusName = "touchFocus";

        StimuliStorage storage = controller.GetBlackboardValue<StimuliStorage>(storageName)?.value;
        if (storage == null)
            return null;

        GenericBlackboard.InitializeMethod<FocusFilterStorage> initializeMethod =
            () => new FocusFilterStorage(controller.transform, storage, distanceSortScale, timeSortScale);
        return controller.InitBlackboardValue(focusName, initializeMethod).value;
    }
    #endregion building focuses

}





namespace Ai
{

    public struct AttentionUtility
    {
        public float attackPeek;
        public float attackTime;
        public float sustainValue;

        public float GetUtility(float attentionTime)
        {
            if (attentionTime <= attackTime)
                return attackPeek;

            return sustainValue;
        }
    }
}