using System.Linq;
using UnityEngine;

//  VhsFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/Vol/Vhs")]
    public class VhsPass : VolFx.Pass
    {
		private static readonly int s_VhsTex    = Shader.PropertyToID("_VhsTex");
        private static readonly int s_XScanline = Shader.PropertyToID("_xScanline");
        private static readonly int s_YScanline = Shader.PropertyToID("_yScanline");
        private static readonly int s_Rocking   = Shader.PropertyToID("_Rocking");
		private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");

		[HideInInspector]
		public  float       _frameRate = 20f;
		[HideInInspector]
        public  Texture2D[] _clip;
		private float       _playTime;
		private float       _yScanline;
		private float       _xScanline;

        protected override bool Invert => true;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<VhsVol>();

            var isActive = settings.IsActive();
			if (isActive == false)
                return false;

            // scale line
			_yScanline += Time.deltaTime * 0.01f * settings._bleed.value;
			_xScanline -= Time.deltaTime * 0.1f * settings._bleed.value;
            
			if (_yScanline >= 1)
				_yScanline = Random.value;
            
			if (_xScanline <= 0 || Random.value < 0.05)
				_xScanline = Random.value;
            
            mat.SetFloat(s_Intensity, settings._weight.value);
			mat.SetFloat(s_YScanline, _yScanline);
			mat.SetFloat(s_XScanline, _xScanline);
			mat.SetFloat(s_Rocking, settings._rocking.value * settings._weight.value);
            
            // params
			_playTime = (_playTime + Time.unscaledDeltaTime) % (_clip.Length / _frameRate);
			mat.SetTexture(s_VhsTex, _clip[Mathf.FloorToInt(_playTime * _frameRate)]);
            
            return true;
        }

        protected override bool _editorValidate => _clip == null || _clip.Length == 0 || (Application.isPlaying == false && _clip.Any(n => n == null));
        protected override void _editorSetup(string folder, string asset)
        {
#if UNITY_EDITOR
			_clip = UnityEditor.AssetDatabase.FindAssets("t:texture", new string[] {$"{folder}\\Vhs"})
							   .Select(n => UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath(n)))
							   .Where(n => n != null)
							   .ToArray();
#endif
        }
    }
}