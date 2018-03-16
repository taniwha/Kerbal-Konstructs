﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.UI.Screens;
using KSP;

namespace KerbalKonstructs.Core
{
    internal static class InstanceUtil
    {
        /// <summary>
        /// Returns a StaticObject object for a gives GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
		internal static StaticInstance GetStaticInstanceForGameObject(GameObject gameObject)
        {
            List<StaticInstance> objList = (from obj in StaticDatabase.allStaticInstances where obj.gameObject == gameObject select obj).ToList();

            if (objList.Count >= 1)
            {
                if (objList.Count > 1)
                {
                    Log.UserError("More than one StaticObject references to GameObject " + gameObject.name);
                }
                return objList[0];
            }

            Log.UserWarning("StaticObject doesn't exist for " + gameObject.name);
            return null;
        }

        /// <summary>
        /// Removes the wreck model from an KSC Object.
        /// </summary>
        internal static void MangleSquadStatic(GameObject gameObject)
        {
            gameObject.transform.parent = null;

            foreach (var bla in gameObject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                Log.Normal("MSS: " + bla.GetType().ToString() );

                switch (bla.GetType().ToString())
                {
                    case "DestructibleBuilding":
                        {
                            UnityEngine.Object.Destroy(bla);
                        }

                        break;
                    case "CollisionEventsHandler":
                        {
                            UnityEngine.Object.Destroy(bla);
                        }
                        break;
                    case ("CrashObjectName"):
                        {
                            UnityEngine.Object.Destroy(bla);
                        }
                        break;
                }
            }


            var transforms = gameObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (var transform in transforms)
            {

                if (transform.name.Equals("wreck", StringComparison.InvariantCultureIgnoreCase))
                {
                    transform.parent = null;
                    GameObject.Destroy(transform.gameObject);
                }

                if (transform.name.Equals("commnetnode", StringComparison.InvariantCultureIgnoreCase))
                {
                    transform.parent = null;
                    GameObject.Destroy(transform.gameObject);
                }
            }

            PQSCity2 pqs2 = gameObject.GetComponent<PQSCity2>();
            if (pqs2 != null)
            {
                GameObject.Destroy(pqs2);
            }

            CommNet.CommNetHome cnhome = gameObject.GetComponentInChildren<CommNet.CommNetHome>(true);
            if (cnhome != null)
            {
                UnityEngine.Object.Destroy(cnhome);
                Log.Normal("destroyed CommNet on: " + gameObject.name);
            }

            DestructibleBuilding destBuilding = gameObject.GetComponentInChildren<DestructibleBuilding>(true);
            if (destBuilding != null)
            {
                destBuilding.enabled = false;
                UnityEngine.Object.Destroy(destBuilding);
                Log.Normal("destroyed Destr Building on: " +gameObject.name);
            }


            Upgradeables.UpgradeableObject  upObject = gameObject.GetComponentInChildren<Upgradeables.UpgradeableObject>(true);
            if (upObject != null)
            {
                upObject.enabled = false;
                Log.Normal("destroyed " + upObject.GetType().ToString()  + " on : " + gameObject.name);
                UnityEngine.Object.Destroy(upObject);
                
            }

            ScenarioUpgradeableFacilities  scUpObject = gameObject.GetComponentInChildren<ScenarioUpgradeableFacilities>(true);
            if (scUpObject != null)
            {
                Log.Normal("destroyed " + scUpObject.GetType().ToString() + " on : " + gameObject.name);
                UnityEngine.Object.Destroy(scUpObject);
            }

             Upgradeables.UpgradeableFacility upfacObject = gameObject.GetComponentInChildren<Upgradeables.UpgradeableFacility>(true);
            if (upfacObject != null)
            {
                Log.Normal("destroyed " + scUpObject.GetType().ToString() + " on : " + gameObject.name);
                UnityEngine.Object.Destroy(scUpObject);
            }


        }

        internal static void SetActiveRecursively(StaticInstance instance, bool active)
        {
            
            if (instance.isActive != active)
            {
                instance.isActive = active;

                foreach (StaticModule module in instance.gameObject.GetComponents<StaticModule>())
                {
                    module.StaticObjectUpdate();
                }

                instance.gameObject.SetActive(active);
                
                var transforms = instance.gameObject.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < transforms.Length; i++)
                {
                    transforms[i].gameObject.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Sets tje Layer of the Colliders
        /// </summary>
        /// <param name="sGameObject"></param>
        /// <param name="newLayerNumber"></param>
        internal static void SetLayerRecursively(StaticInstance instance, int newLayerNumber)
        {

            var transforms = instance.gameObject.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                // don't set trigger collider 
                if ((transforms[i].gameObject.GetComponent<Collider>() != null) && (transforms[i].gameObject.GetComponent<Collider>().isTrigger))
                {
                    continue;
                }
                transforms[i].gameObject.layer = newLayerNumber;
            }
        }
    }
}


