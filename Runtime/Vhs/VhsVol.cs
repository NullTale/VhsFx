using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VhsFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Vhs")]
    public sealed class VhsVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter _weight     = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter _bleed      = new ClampedFloatParameter(1f, 0f, 10f);
        public ClampedFloatParameter _rocking    = new ClampedFloatParameter(0f, 0f, 0.1f);
        public ClampedFloatParameter _tape       = new ClampedFloatParameter(1f, 0f, 2f);
        public BoolParameter         _negative   = new BoolParameter(false);
        public ClampedFloatParameter _noise      = new ClampedFloatParameter(.5f, 0f, 1f);
        public ClampedFloatParameter _flickering = new ClampedFloatParameter(0f, 0f, 2f);
        public ColorParameter        _glitch     = new ColorParameter(new Color(1f, 0f, 0f, 1));
        public ModeParameter         _mode       = new ModeParameter(VhsPass.Mode.Tape, false);
        
        // =======================================================================
        [Serializable]
        public class ModeParameter : VolumeParameter<VhsPass.Mode>
        {
            public ModeParameter(VhsPass.Mode value, bool overrideState) : base(value, overrideState) { }
        } 
        // =======================================================================
        public bool IsActive() => active && _weight.value > 0f;

        public bool IsTileCompatible() => true;
    }
}