using UnityEngine;
namespace Kunedo
{
    public class FanRadiator : ModuleActiveRadiator
    {
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
        private bool activeanim = false;
        public override void OnStart(StartState state)
        {
            loopStates = Utils.SetUpAnimation(loopName, part);
            foreach (AnimationState loop in loopStates)
            {
                loop.speed = 0;
            }                
            charge_up = gameObject.AddComponent<AudioSource>();
            charge_up.volume = Volumen;
            charge_up.maxDistance = DistanceMax;
            charge_up.rolloffMode = AudioRolloffMode.Linear;
            charge_up.minDistance = 0.1f;            
            charge_up.spatialBlend = 1;
            if (GameDatabase.Instance.ExistsAudioClip(SoundFile))
            {
                charge_up.clip = GameDatabase.Instance.GetAudioClip(SoundFile);
                Debug.Log("sonido cargado correctamente! ");                
            }
            if (!GameDatabase.Instance.ExistsAudioClip(SoundFile))
            {
                Debug.LogError("no se a especificado la ruta del sonido en loop" + GameDatabase.Instance.ToString());
            }            
        }
        public override void Activate()
        {
            base.Activate();
            activeanim = true;
            foreach (AnimationState loop in loopStates)
            {
                loop.enabled = true;
                loop.wrapMode = WrapMode.Loop;                                    
            }            
            if (charge_up.clip != null)
            {
                if (!charge_up.isPlaying)
                {
                    charge_up.Play();
                    charge_up.loop = true;
                }
            }                       
        }
        public override void Shutdown()
        {
            base.Shutdown();
            activeanim = false;
            foreach (AnimationState loop in loopStates)
            {
                loop.wrapMode = WrapMode.Loop;                                                 
            }            
            if (charge_up.clip != null)
            {
                if (charge_up.isPlaying)
                {
                    charge_up.Stop();
                    charge_up.loop = false;
                }                
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            foreach (AnimationState loop in loopStates)
            {
                if (activeanim)
                {
                    if (loop.speed < AnimationSpeed)
                    {
                        loop.speed += AnimationAcceleration * Time.deltaTime;
                    }
                    if (loop.speed >= AnimationSpeed)
                    {
                        loop.speed = AnimationSpeed;
                    }
                }
                if (!activeanim)
                {
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
    }
}
