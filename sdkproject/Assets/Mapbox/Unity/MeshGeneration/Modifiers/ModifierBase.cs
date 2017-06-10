namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    using UnityEngine;
	using UnityEngine.PostProcessing;

	public class ModifierBase
    {
        [SerializeField]
        public bool Active = true;

		[SerializeField, GetSet("enabled")]
		bool m_Enabled;
		public bool enabled
		{
			get { return m_Enabled; }
			set
			{
				m_Enabled = value;

				if (value)
					OnValidate();
			}
		}

		public virtual void Reset() { }

		public virtual void OnValidate()
		{ }
	}
}
