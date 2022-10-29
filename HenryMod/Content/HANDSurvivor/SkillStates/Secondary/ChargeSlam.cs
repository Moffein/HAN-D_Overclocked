﻿using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using UnityEngine;

namespace EntityStates.HANDMod.Secondary
{
    public class ChargeSlam : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_HOC_StartHammer", base.gameObject);
            this.minDuration = ChargeSlam.baseMinDuration / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();
            if (this.modelAnimator)
            {
                base.PlayCrossfade("Gesture, Override", "PrepHammer", "ChargeHammer.playbackRate", this.minDuration, 0.2f);
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(4f);
            }
            charge = 0f;
            chargePercent = 0f;
            chargeDuration = ChargeSlam.baseChargeDuration / this.attackSpeedStat;

            hammerController = base.GetComponent<HammerVisibilityController>();
            if (hammerController)
            {
                hammerController.SetHammerEnabled(true);
            }
        }

        public override void OnExit()
        {
            if (this.holdChargeVfxGameObject)
            {
                EntityState.Destroy(this.holdChargeVfxGameObject);
                this.holdChargeVfxGameObject = null;
            }
            if (!this.outer.destroying)
            {
                this.PlayAnimation("Gesture, Override", "Empty");
            }
            if (hammerController)
            {
                hammerController.SetHammerEnabled(false);
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            /*if (base.characterBody && base.characterBody.isSprinting)
            {
                base.characterBody.isSprinting = false;
            }*/

            if (base.fixedAge > this.minDuration && charge < chargeDuration)
            {
                if (!startedChargeAnim)
                {
                    startedChargeAnim = true;
                    base.PlayCrossfade("Gesture, Override", "ChargeHammer", "ChargeHammer.playbackRate", this.chargeDuration - this.minDuration, 0.2f);
                }

                charge += Time.deltaTime * this.attackSpeedStat;
                if (charge > chargeDuration)
                {
                    Util.PlaySound("Play_HOC_StartPunch", base.gameObject);
                    charge = chargeDuration;
                    EffectManager.SpawnEffect(chargeEffectPrefab, new EffectData
                    {
                        origin = base.transform.position
                    }, false);
                }
                chargePercent = charge / chargeDuration;
            }

            if (base.fixedAge >= this.minDuration)
            {
                if (base.isAuthority && base.inputBank && !base.inputBank.skill2.down)
                {
                    this.outer.SetNextState(new EntityStates.HANDMod.Secondary.FireSlam() { chargePercent = chargePercent });
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static float baseMinDuration = 0.6f;
        public static float baseChargeDuration = 1.4f;
        private float minDuration;
        private float chargeDuration;
        private float charge;
        private float chargePercent;
        private Animator modelAnimator;
        public static GameObject chargeEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXLoader");
        private bool startedChargeAnim = false;

        private HammerVisibilityController hammerController;

        public static GameObject holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
        private GameObject holdChargeVfxGameObject = null;
    }
}
