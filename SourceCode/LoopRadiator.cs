using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
namespace Kunedo
{
    public class LoopRadiator : ModuleActiveRadiator
    {
        [KSPField(isPersistant = false)]
        public string loopName;
        [KSPField(isPersistant = false)]
        public string location;
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
        private bool activo = false;
        public override void OnStart(StartState state)
        {
            if (loopName == null)
            {
                Debug.LogError("no se a especificado el nombre de la animacion en loop");
            }
            if (loopName != null)
            {
                loopStates = Utils.SetUpAnimation(loopName, this.part);
                foreach (AnimationState loop in loopStates)
                {
                    loop.speed = 0;
                }
                Debug.Log("animacion cargada correctamente! ");
            }
            charge_up = gameObject.AddComponent<AudioSource>();
            charge_up.volume = Volumen;
            charge_up.maxDistance = DistanceMax;
            charge_up.rolloffMode = AudioRolloffMode.Linear;
            charge_up.minDistance = 0.1f;            
            charge_up.spatialBlend = 1;
            if (GameDatabase.Instance.ExistsAudioClip(location))
            {
                charge_up.clip = GameDatabase.Instance.GetAudioClip(location);
                Debug.Log("sonido cargado correctamente! ");                
            }
            if (!GameDatabase.Instance.ExistsAudioClip(location))
            {
                Debug.LogError("no se a especificado la ruta del sonido en loop" + GameDatabase.Instance.ToString());
            }
            
        }
        public override void Activate()
        {
            base.Activate();
            activo = true;            
        }
        public override void Shutdown()
        {
            base.Shutdown();
            activo = false;            
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (loopName != null)
            {
                foreach (AnimationState loop in loopStates)
                {
                    if (activo)
                    {
                        loop.enabled = true;
                        loop.wrapMode = WrapMode.Loop;
                        if (loop.speed < AnimationSpeed)
                        {
                            loop.speed += AnimationAcceleration * Time.deltaTime;
                        }
                        if (loop.speed >= AnimationSpeed)
                        {
                            loop.speed = AnimationSpeed;
                        }                        
                    }
                    else
                    {
                        loop.wrapMode = WrapMode.Loop;
                        if (loop.speed > 0)
                        {
                            loop.speed -= AnimationAcceleration * Time.deltaTime;
                        }
                        if (loop.speed <= 0)
                        {
                            loop.speed = 0;
                        }                        
                    }
                }
            }
            if (charge_up.clip != null)
            {
                if (activo)
                {
                    if (!charge_up.isPlaying)
                    {
                        charge_up.Play();
                        charge_up.loop = true;
                    }
                }
                else
                {
                    if (charge_up.isPlaying)
                    {
                        charge_up.Stop();
                        charge_up.loop = false;
                    }
                }
            }
        }        
    }
}
