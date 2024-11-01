using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using RandomGeneration.Interactable;
using System;
using ValkoGames.Labyrinthine.Cases.Inventory;
using ValkoGames.Labyrinthine.Cases.Items;
using ValkoGames.Labyrinthine.Interactions.Story;
using ValkoGames.Labyrinthine.Interactions.Story.Zone5;
using ValkoGames.Labyrinthine.Monsters;
using ValkoGames.Labyrinthine.Saves;
using ValkoGames.Labyrinthine.VR;
using ValkoGames.Labyrinthine.VR.Interactions;
using ValkoGames.Labyrinthine.VR.Interactions.Drag;
using ValkoGames.Labyrinthine.VR.Interactions.Items;
using ValkoGames.Labyrinthine.VR.Player;
using ValkoGames.Labyrinthine.VR.Player.Items;
using Valve.VR.InteractionSystem;
using MyTrueGear;

namespace Labyrinthine_TrueGear
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;

        private static int leftHandID = 999999;
        private static int rightHandID = 999999;
        private static int leftHandPullID = 99999;
        private static int rightHandPullID = 99999;
        private static int leftHandObjectID = 99999;
        private static int rightHandObjectID = 99999;

        private static TrueGearMod _TrueGear = null;

        public override void Load()
        {
            // Plugin startup logic
            Log = base.Log;

            Harmony.CreateAndPatchAll(typeof(Plugin));

            _TrueGear = new TrueGearMod();

            _TrueGear.Play("HeartBeat");
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        }


        [HarmonyPostfix, HarmonyPatch(typeof(PlayerNetworkSync), "OnKillSequenceStart")]
        private static void PlayerNetworkSync_OnKillSequenceStart_Postfix(PlayerNetworkSync __instance)
        {
            if (!__instance.isLocalPlayer)
            {
                return;
            }
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("MonsterCatch");
            _TrueGear.Play("MonsterCatch");
            Log.LogInfo(__instance.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerNetworkSync), "OnDeathStatusChange")]
        private static void PlayerNetworkSync_OnDeathStatusChange_Postfix(PlayerNetworkSync __instance, bool oldValue, bool newValue)
        {
            if (!__instance.isLocalPlayer)
            {
                return;
            }
            if (!oldValue && newValue)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("PlayerDeath");
                _TrueGear.Play("PlayerDeath");
            }
            else if (!newValue && oldValue) 
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("LevelStarted");
                _TrueGear.Play("LevelStarted");
            }

            //Log.LogInfo(oldValue);
            //Log.LogInfo(newValue);
            //Log.LogInfo(__instance.isLocalPlayer);
        }



        [HarmonyPostfix, HarmonyPatch(typeof(PlayerNetworkSync), "OnStartLocalPlayer")]
        private static void PlayerNetworkSync_OnStartLocalPlayer_Postfix(PlayerNetworkSync __instance)
        {
            if (!__instance.isLocalPlayer)
            {
                return;
            }
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("LevelStarted");
            _TrueGear.Play("LevelStarted");
            leftHandID = __instance.VRPlayer.LeftHand.GetInstanceID();
            rightHandID = __instance.VRPlayer.RightHand.GetInstanceID();
            Log.LogInfo(__instance.ID);
            Log.LogInfo(__instance.isLocalPlayer);
            Log.LogInfo(__instance.VRPlayer.LeftHand.GetInstanceID());
            Log.LogInfo(__instance.VRPlayer.RightHand.GetInstanceID());
        }


        [HarmonyPostfix, HarmonyPatch(typeof(VRGlowstickItem), "Drop")]
        private static void VRGlowstickItem_Drop_Postfix(VRGlowstickItem __instance)
        {
            if (!__instance.owner.isLocalPlayer)
            {
                return;
            }
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("LeftHandDropGlowstick");
            _TrueGear.Play("LeftHandDropGlowstick");
            Log.LogInfo(__instance.owner.isLocalPlayer);
        }




        [HarmonyPostfix, HarmonyPatch(typeof(LeverNetworkSync), "IsPulledOut")]
        private static void LeverNetworkSync_IsPulledOut_Postfix(LeverNetworkSync __instance,bool __result)
        {
            if (__instance.gameObject.transform.parent.GetInstanceID() == leftHandPullID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("LeftHandPull");
                _TrueGear.Play("LeftHandPull");
                leftHandPullID = 99999;
                Log.LogInfo(__instance.name);
                Log.LogInfo(__instance.GetInstanceID());
                Log.LogInfo(__instance.gameObject.transform.parent.name);
                Log.LogInfo(__instance.gameObject.transform.parent.GetInstanceID());
                Log.LogInfo(__result);
                Log.LogInfo(__instance.isLocalPlayer);
            }
            else if (__instance.gameObject.transform.parent.GetInstanceID() == rightHandPullID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("RightHandPull");
                _TrueGear.Play("RightHandPull");
                rightHandPullID = 99999;
                Log.LogInfo(__instance.name);
                Log.LogInfo(__instance.GetInstanceID());
                Log.LogInfo(__instance.gameObject.transform.parent.name);
                Log.LogInfo(__instance.gameObject.transform.parent.GetInstanceID());
                Log.LogInfo(__result);
                Log.LogInfo(__instance.isLocalPlayer);
            }

        }



        [HarmonyPostfix, HarmonyPatch(typeof(VRPlayerHand), "AttachObject")]
        private static void VRPlayerHand_AttachObject_Postfix(VRPlayerHand __instance, VRInteractable objectToAttach)
        {
            if (__instance.HandType == Valve.VR.SteamVR_Input_Sources.LeftHand && __instance.GetInstanceID() == leftHandID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("LeftHandGrabItem");
                _TrueGear.Play("LeftHandGrabItem");
                leftHandObjectID = objectToAttach.gameObject.transform.parent.GetInstanceID();
                if (objectToAttach.gameObject.transform.parent.parent.parent.parent != null)
                {
                    leftHandPullID = objectToAttach.gameObject.transform.parent.parent.parent.parent.GetInstanceID();
                }
            }
            else if (__instance.HandType == Valve.VR.SteamVR_Input_Sources.RightHand && __instance.GetInstanceID() == rightHandID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("RightHandGrabItem");
                _TrueGear.Play("RightHandGrabItem");
                rightHandObjectID = objectToAttach.gameObject.transform.parent.GetInstanceID();
                if (objectToAttach.gameObject.transform.parent.parent.parent.parent != null)
                {
                    rightHandPullID = objectToAttach.gameObject.transform.parent.parent.parent.parent.GetInstanceID();
                }
            }

            //Log.LogInfo(objectToAttach.name);
            //Log.LogInfo(objectToAttach.GetInstanceID());
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.name);
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.GetInstanceID());
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.name);
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.GetInstanceID());
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.parent.name);
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.parent.GetInstanceID());
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.parent.parent.name);
            //Log.LogInfo(objectToAttach.gameObject.transform.parent.parent.parent.parent.GetInstanceID());
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRPlayerHand), "DetachObject")]
        private static void VRPlayerHand_DetachObject_Postfix(VRPlayerHand __instance)
        {
            if (__instance.HandType == Valve.VR.SteamVR_Input_Sources.LeftHand && __instance.GetInstanceID() == leftHandID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("LeftHandDetachItem");
                leftHandPullID = 99999;
            }
            else if (__instance.HandType == Valve.VR.SteamVR_Input_Sources.RightHand && __instance.GetInstanceID() == rightHandID)
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("RightHandDetachItem");
                rightHandPullID = 99999;
            }
          
            Log.LogInfo(__instance.wasModelVisible);
        }



        [HarmonyPostfix, HarmonyPatch(typeof(VRPlayerBelt), "AttachItemToBelt")]
        private static void VRPlayerBelt_AttachItemToBelt_Postfix(VRPlayerBelt __instance)
        {
            if (leftHandObjectID == __instance.gameObject.transform.parent.parent.GetInstanceID())
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("LeftHandSetBolt");
                _TrueGear.Play("LeftHandSetBolt");
            }
            else if (rightHandObjectID == __instance.gameObject.transform.parent.parent.GetInstanceID())
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("RightHandSetBolt");
                _TrueGear.Play("RightHandSetBolt");
            }

            //Log.LogInfo(__instance.name);
            //Log.LogInfo(__instance.GetInstanceID());
            //Log.LogInfo(__instance.gameObject.transform.parent.name);
            //Log.LogInfo(__instance.gameObject.transform.parent.GetInstanceID());
            //Log.LogInfo(__instance.gameObject.transform.parent.parent.name);
            //Log.LogInfo(__instance.gameObject.transform.parent.parent.GetInstanceID());
        }





        [HarmonyPostfix, HarmonyPatch(typeof(ExamineObject), "OnPickup")]
        private static void ExamineObject_OnPickup_Postfix(ExamineObject __instance)
        {
            if (leftHandObjectID == __instance.gameObject.transform.parent.GetInstanceID() || rightHandObjectID == __instance.gameObject.transform.parent.GetInstanceID())
            {
                Log.LogInfo("------------------------------------------");
                Log.LogInfo("PickupItem");
                _TrueGear.Play("PickupItem");
            }

            //Log.LogInfo(__instance.name);
            //Log.LogInfo(__instance.GetInstanceID());
            //Log.LogInfo(__instance.gameObject.transform.parent.name);
            //Log.LogInfo(__instance.gameObject.transform.parent.GetInstanceID());
        }




        [HarmonyPostfix, HarmonyPatch(typeof(VRCompassAttractorItem), "GetInput")]
        private static void VRCompassAttractorItem_GetInput_Postfix(VRCompassAttractorItem __instance)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("VRCompassAttractorItemGetInput");
            Log.LogInfo(__instance.PlayerNetworkSync.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRSprintDrinkItem), "UseItem")]
        private static void VRSprintDrinkItem_UseItem_Postfix(VRSprintDrinkItem __instance)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("VRSprintDrinkItemUseItem");
            Log.LogInfo(__instance.PlayerNetworkSync.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRSprayPaintItem), "GetStartPlacingSprayInput")]
        private static void VRSprayPaintItem_GetStartPlacingSprayInput_Postfix(VRSprayPaintItem __instance)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("VRSprayPaintItemGetStartPlacingSprayInput");
            Log.LogInfo(__instance.PlayerNetworkSync.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRSprayPaintItem), "GetPlaceSprayInput")]
        private static void VRSprayPaintItem_GetPlaceSprayInput_Postfix(VRSprayPaintItem __instance)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("VRSprayPaintItemGetPlaceSprayInput");
            Log.LogInfo(__instance.PlayerNetworkSync.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRSprayPaintItem), "GetSwitchTextureInputButton")]
        private static void VRSprayPaintItem_GetSwitchTextureInputButton_Postfix(VRSprayPaintItem __instance)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("VRSprayPaintItemGetSwitchTextureInputButton");
            Log.LogInfo(__instance.PlayerNetworkSync.isLocalPlayer);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRPlayerDraggable), "HandAttachedUpdate")]
        private static void VRPlayerDraggable_HandAttachedUpdate_Postfix(VRPlayerDraggable __instance, VRPlayerHand hand)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("HandAttachedUpdate");
            Log.LogInfo(hand.HandType);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VRPlayerDragInteractionController), "TryStartInteraction")]
        private static void VRPlayerDragInteractionController_TryStartInteraction_Postfix(VRPlayerDragInteractionController __instance, VRPlayerHand hand)
        {
            Log.LogInfo("------------------------------------------");
            Log.LogInfo("TryStartInteraction");
            Log.LogInfo(hand.HandType);
        }


    }
}
