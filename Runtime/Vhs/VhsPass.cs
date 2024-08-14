using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//  VhsFx © NullTale - https://x.com/NullTale
namespace VolFx
{
    [ShaderName("Hidden/Vol/Vhs")]
    public class VhsPass : VolFx.Pass
    {
		private static readonly int s_VhsTex     = Shader.PropertyToID("_VhsTex");
		private static readonly int s_XScanline  = Shader.PropertyToID("_xScanline");
		private static readonly int s_YScanline  = Shader.PropertyToID("_yScanline");
		private static readonly int s_Rocking    = Shader.PropertyToID("_Rocking");
		private static readonly int s_Intensity  = Shader.PropertyToID("_Intensity");
		private static readonly int s_Glitch     = Shader.PropertyToID("_Glitch");
		private static readonly int s_Tape       = Shader.PropertyToID("_Tape");
		private static readonly int s_Noise      = Shader.PropertyToID("_Noise");
		private static readonly int s_Flickering = Shader.PropertyToID("_Flickering");

        [Tooltip("Use single tape type to smaller build size")]
		[HideInInspector]
        public Optional<Mode> _singleTape = new Optional<Mode>(Mode.Tape, false);
		[Tooltip("Default Use tape texture as a negative")]
		public bool  _negative;
		[Tooltip("Default Glitch color")]
		public Color _glitch = Color.red;
		[HideInInspector]
		public  float       _frameRate = 20f;
		[HideInInspector]
        public  Texture2D[] _tape;
		[HideInInspector]
        public  Texture2D[] _noise;
		[HideInInspector]
        public  Texture2D[] _shades;
		[HideInInspector]
        public  Texture2D[] _clip;
		
		private float _playTime;
		private float _yScanline;
		private float _xScanline;

		protected override bool Invert => true;

		// =======================================================================
		public enum Mode
		{
			Tape,
			Noise,
			Shades
		}

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<VhsVol>();

            var isActive = settings.IsActive();
			if (isActive == false)
                return false;
			
			if (_singleTape.Enabled)
			{
				_clip = _singleTape.Value switch
				{
					Mode.Tape   => _tape,
					Mode.Noise  => _noise,
					Mode.Shades => _shades,
					_           => throw new ArgumentOutOfRangeException()
				};
			}

            // scale line
			_yScanline += Time.deltaTime * 0.01f * settings._bleed.value;
			_xScanline -= Time.deltaTime * 0.1f * settings._bleed.value;
            
			var glitch = settings._glitch.overrideState ? settings._glitch.value : _glitch;
			
			if (_yScanline >= 1)
				_yScanline = Random.value;
            
			if (_xScanline <= 0 || Random.value < 0.05)
				_xScanline = Random.value;
            
            mat.SetFloat(s_Intensity, settings._weight.value);
			mat.SetFloat(s_YScanline, _yScanline);
			mat.SetFloat(s_XScanline, _xScanline);
			mat.SetFloat(s_Rocking, settings._rocking.value * settings._weight.value);
			mat.SetColor(s_Glitch, glitch);
			mat.SetFloat(s_Flickering, settings._flickering.value);
			mat.SetFloat(s_Tape, settings._tape.value);
			mat.SetFloat(s_Noise, Mathf.Lerp(1000, 2, settings._noise.value));
            
            // params
			_playTime = (_playTime + Time.unscaledDeltaTime) % (_clip.Length / _frameRate);
			mat.SetTexture(s_VhsTex, _clip[Mathf.FloorToInt(_playTime * _frameRate)]);
            
            return true;
        }

        protected override bool _editorValidate => _clip == null || _clip.Length == 0 || (Application.isPlaying == false && _clip.Any(n => n == null))
												   || ((_singleTape.Enabled && _tape != null && _noise != null && _shades != null) || (_singleTape.Enabled == false && (_tape == null || _noise == null || _shades == null)));
        protected override void _editorSetup(string folder, string asset)
        {
#if UNITY_EDITOR
			var sep = Path.DirectorySeparatorChar;
			
			_clip = UnityEditor.AssetDatabase.FindAssets("t:texture", new string[] {$"{folder}{sep}Vhs{sep}Tape"})
							   .Select(n => UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath(n)))
							   .Where(n => n != null)
							   .ToArray();
			
			_playTime = 0f;
#endif
        }
    }
}