namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;

    public abstract class FilterBase : ScriptableObject
    {
        [SerializeField]
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

        public abstract void Reset();

        public virtual void OnValidate()
        { }

        public virtual bool Try(VectorFeatureUnity feature)
        {
            return true;
        }
    }
}