using UnityEngine;
namespace Kunedo
{
    public class FanRadiator : ModuleActiveRadiator
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
        public override void OnStart(StartState state)
        {
            loopStates = Utils.SetUpAnimation(loopName, part);
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
                if (IsCooling)
                {                    
                    if (loop.speed < AnimationSpeed)
                    {
                        loop.speed += AnimationAcceleration * Time.deltaTime;
                    }
                    if (loop.speed >= AnimationSpeed)
                    {
                        loop.speed = AnimationSpeed;
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
                if (!IsCooling)
                {
                    if (loop.speed > 0)
                    {
                        loop.speed -= AnimationAcceleration * Time.deltaTime;
                    }
                    if (loop.speed <= 0)
                    {
                        loop.speed = 0;
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
    }
}
