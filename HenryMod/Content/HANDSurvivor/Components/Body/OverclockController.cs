﻿using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class OverclockController : NetworkBehaviour
    {
        public void Awake()
        {
            ovcTimer = 0f;
            ovcActive = false;
            characterBody = base.GetComponent<CharacterBody>();
            healthComponent = characterBody.healthComponent;

            rectGauge = new Rect();
            rectGaugeArrow = new Rect();
        }

        public void FixedUpdate()
        {
            if (ovcActive)
            {
                characterBody.skillLocator.utility.rechargeStopwatch = initialOverclockCooldown;

                ovcTimer -= Time.fixedDeltaTime;
                ovcPercent = ovcTimer / OverclockController.OverclockDuration;

                if (ovcTimer > OverclockController.OverclockDuration)
                {
                    ovcTimer = OverclockController.OverclockDuration;
                }
                else if (ovcTimer < 0f)
                {
                    if (characterBody.skillLocator.utility.stock > 0)
                    {
                        characterBody.skillLocator.utility.stock--;
                    }
                    EndOverclock();
                }
            }
        }

        public void MeleeHit()
        {
            if (characterBody.skillLocator.special.stock < characterBody.skillLocator.special.maxStock && characterBody.skillLocator.special.skillDef == SkillDefs.SpecialDrone)
            {
                characterBody.skillLocator.special.rechargeStopwatch += 1.2f;
            }
        }

        public void ExtendOverclock(float time)
        {
            if (ovcActive)
            {
                ovcTimer += time;
            }
        }

        public void BeginOverclock()
        {
            if (hasAuthority)
            {
                ReiszeOverclockGauge();
                ovcTimer = OverclockController.OverclockDuration;
                ovcPercent = 1f;
                ovcActive = true;
                initialOverclockCooldown = characterBody.skillLocator.utility.rechargeStopwatch;

                CmdBeginOverclockServer();
            }
        }

        [Command]
        private void CmdBeginOverclockServer()
        {
            RpcPlayOverclockStart();
            if (!characterBody.HasBuff(Buffs.Overclock))
            {
                characterBody.AddBuff(Buffs.Overclock);
            }
        }

        public void EndOverclock()
        {
            if (hasAuthority)
            {
                ovcActive = false;
                ovcTimer = 0;
                CmdEndOverclockServer();
            }
        }

        [Command]
        private void CmdEndOverclockServer()
        {
            if (characterBody.HasBuff(Buffs.Overclock))
            {
                characterBody.RemoveBuff(Buffs.Overclock);
            }
            RpcPlayOverclockEnd();
        }

        [ClientRpc]
        public void RpcPlayOverclockEnd()
        {
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
        }

        [ClientRpc]
        public void RpcPlayOverclockStart()
        {
            Util.PlaySound("Play_MULT_shift_start", base.gameObject);
        }

        private void ReiszeOverclockGauge()
        {
            rectGauge.width = Screen.height * texGauge.width * gaugeScale / 1080f;
            rectGauge.height = Screen.height * texGauge.height * gaugeScale / 1080f;

            rectGauge.position = new Vector2(Screen.width / 2f - rectGauge.width / 2f, Screen.height / 2f + rectGauge.height * 2f);

            rectGaugeArrow.width = Screen.height * texGaugeArrow.width * gaugeScale / 1080f;
            rectGaugeArrow.height = Screen.height * texGaugeArrow.height * gaugeScale / 1080f;

            gaugeLeftBound = rectGauge.position.x - rectGaugeArrow.width / 2f;
            gaugeRightBound = gaugeLeftBound + rectGauge.width;
            gaugeArroyYPos = Screen.height / 2f + rectGauge.height * 2f;
        }
        private void OnGUI()
        {
            if (this.hasAuthority && ovcActive && !menuActive && !RoR2.PauseManager.isPaused && healthComponent && healthComponent.alive)
            {
                GUI.DrawTexture(rectGauge, texGauge, ScaleMode.StretchToFill, true, 0f);

                rectGaugeArrow.position = new Vector2(Mathf.Lerp(gaugeLeftBound, gaugeRightBound, ovcPercent), gaugeArroyYPos);
                GUI.DrawTexture(rectGaugeArrow, texGaugeArrow, ScaleMode.StretchToFill, true, 0f);
            }
        }

        public bool ovcActive
        {
            get
            {
                return _ovcActive;
            }
            protected set
            {
                _ovcActive = value;
            }
        }
        private bool _ovcActive;

        public float ovcTimer
        {
            get
            {
                return _ovcTimer;
            }
            protected set
            {
                _ovcTimer = value;
            }
        }
        private float _ovcTimer;

        public static float OverclockDuration = 4f;

        private CharacterBody characterBody;
        private HealthComponent healthComponent;

        public static SkillDef ovcDef;

        public static Texture2D texGauge;
        public static Texture2D texGaugeArrow;
        private Rect rectGauge;
        private Rect rectGaugeArrow;
        public static float gaugeScale = 0.3f;
        private float gaugeLeftBound;
        private float gaugeRightBound;
        private float gaugeArroyYPos;

        private float initialOverclockCooldown;
        private float ovcPercent;

        public bool menuActive = false;
    }
}
