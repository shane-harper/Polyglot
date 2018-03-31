namespace Polyglot
{
    using UnityEngine;

    public class LocalizationKeyAttribute : PropertyAttribute
    {
        public readonly LocalizationData.Type Type;
        
        public LocalizationKeyAttribute(LocalizationData.Type type)
        {
            Type = type;
        }
    }
}