﻿using RoR2;
using UnityEngine;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    //Doesn't work, hammer transform seems to ignore modifications to localScale.
    public class HammerVisibilityController : MonoBehaviour
    {
        private ChildLocator childLocator;
        private SkillLocator skillLocator;
        private GameObject hammer;

        private bool hammerEnabled = false;

        private void Awake()
        {
            childLocator = base.GetComponentInChildren<ChildLocator>();
            if (!childLocator)
            {
                Destroy(this);
                return;
            }
            skillLocator = base.GetComponent<SkillLocator>();
            hammer = childLocator.FindChildGameObject("HanDHammer");

            SetHammerEnabled(false);
        }

        private void FixedUpdate()
        {
            if (hammerEnabled)
            {
                ShowHammer();
            }
            else
            {
                HideHammer();
            }
        }

        private void ShowHammer()
        {
            hammer.SetActive(true);
        }

        private void HideHammer()
        {
            hammer.SetActive(false);
        }

        public void SetHammerEnabled(bool enabled)
        {
            hammerEnabled = enabled;
            if (!hammerEnabled)
            {
                HideHammer();
            }
            else
            {
                ShowHammer();
            }
        }
    }
}
