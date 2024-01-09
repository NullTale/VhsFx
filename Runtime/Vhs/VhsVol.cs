using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VhsFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Vhs")]
    public sealed class VhsVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter _weight  = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _bleed   = new ClampedFloatParameter(1f, 0f, 10f);
        public ClampedFloatParameter _rocking = new ClampedFloatParameter(0f, 0f, 0.1f);
        
        // =======================================================================
        public bool IsActive() => active && _weight.value > 0f;

        public bool IsTileCompatible() => true;
    }
}