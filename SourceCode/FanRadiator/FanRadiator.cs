using System.Collections.Generic;
using UnityEngine;
namespace Kunedo
{
    public class FanRadiator : PartModule
    {
        [KSPField(isPersistant = false)]
        private string loopName = "heatsink";
        [KSPField(isPersistant = false)]
        public string SoundFile;
        [KSPField(isPersistant = false)]
        public float Volumen;
        [KSPField(isPersistant = false)]
        public float DistanceMax;
        [KSPField(isPersistant = false)]
        public float AnimationSpeed;
        [KSPField(isPersistant = false)]
        public float AnimationAcceleration;
        private AudioSource charge_up;
        private AnimationState[] loopStates;
        public bool activeanim = false;
        public ModuleActiveRadiator mar = null;
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            loopStates = SetUpAnimation(loopName, part);
            foreach (AnimationState loop in loopStates)
            {
                loop.speed = 0;
            }                
            charge_up = gameObject.AddComponent<AudioSource>();
            charge_up.volume = Volumen;
            charge_up.loop = true;
            charge_up.maxDistance = DistanceMax;
            charge_up.rolloffMode = AudioRolloffMode.Linear;
            charge_up.minDistance = 0.1f;            
            charge_up.spatialBlend = 1;
            if (GameDatabase.Instance.ExistsAudioClip(SoundFile))
            {
                charge_up.clip = GameDatabase.Instance.GetAudioClip(SoundFile);
                Debug.Log("Sound loaded! ");                
            }
            if (!GameDatabase.Instance.ExistsAudioClip(SoundFile))
            {
                Debug.LogError("Directory not find!" + GameDatabase.Instance.ToString());
            }
            mar = part.GetComponent<ModuleActiveRadiator>();
        }
        public static AnimationState[] SetUpAnimation(string animationName, Part part)  //Thanks Majiir!
        {
            var states = new List<AnimationState>();
            foreach (var animation in part.FindModelAnimators(animationName))
            {
                var animationState = animation[animationName];
                animationState.speed = 0;
                animationState.enabled = true;
                animationState.wrapMode = WrapMode.ClampForever;
                animation.Blend(animationName);
                states.Add(animationState);
            }
            return states.ToArray();
        }
        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                charge_up.volume = 0;
            }
            else
            {
                charge_up.volume = Volumen;                
            } 
            foreach (AnimationState loop in loopStates)
            {
                if (mar.IsCooling)
                {
                    if (loop.speed < AnimationSpeed)
                    {
                        loop.speed += AnimationAcceleration * TimeWarp.deltaTime;
                    }
                    if (loop.speed >= AnimationSpeed)
                    {
                        loop.speed = AnimationSpeed;
                        part.emissiveConstant = 18;
                    }
                    if (!activeanim)
                    {
                        loop.enabled = true;
                        loop.wrapMode = WrapMode.Loop;                        
                        if (charge_up.clip != null && !HighLogic.LoadedSceneIsEditor)                                
                        {
                            if (!charge_up.isPlaying)
                            {
                                charge_up.Play();
                                charge_up.loop = true;
                            }
                        }
                        activeanim = true;
                    }
                }
                if (!mar.IsCooling)
                {
                    if (loop.speed > 0)
                    {
                        loop.speed -= AnimationAcceleration * TimeWarp.deltaTime;
                    }
                    if (loop.speed <= 0)
                    {
                        loop.speed = 0;
                        part.emissiveConstant = 3;
                    }
                    if (activeanim)
                    {
                        loop.wrapMode = WrapMode.Loop;
                        if (charge_up.clip != null)
                        {
                            if (charge_up.isPlaying && !HighLogic.LoadedSceneIsEditor)
                            {
                                charge_up.Stop();
                                charge_up.loop = false;
                            }
                        }
                        activeanim = false;
                    }
                }
            }
        }
        public string GetModuleTitle()
        {
            return "Fan Radiator";
        }
        public override string GetInfo()
        {
            return "Ventilated radiator that cools with great efficiency.\n\n" + "with fans at maximum power cooling : " + "<b>" + "15000KW" + "</b>";
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }
    }
}
