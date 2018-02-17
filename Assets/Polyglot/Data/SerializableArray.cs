//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using UnityEngine;

namespace Polyglot
{
    public abstract class SerializableArray<T1, T2>
    {
        [SerializeField] private T1 _key;
        [SerializeField] private T2[] _values;

        public T1 Key
        {
            get { return _key; }
        }

        public T2[] Values
        {
            get { return _values; }
        }

        private void OnEnable()
        {
            _values = new T2[0];
        }
    }

    /// <summary>
    ///     Unity Editor serializable keyed array for storing strings
    /// </summary>
    [Serializable]
    public sealed class SerializableStringArray : SerializableArray<string, string>
    {
    }

    /// <summary>
    ///     Unity Editor serializable keyed array for storing Sprites
    /// </summary>
    [Serializable]
    public sealed class SerializableSpriteArray : SerializableArray<string, Sprite>
    {
    }

    /// <summary>
    ///     Unity Editor serializable keyed array for storing AudioClips
    /// </summary>
    [Serializable]
    public sealed class SerializableAudioClipArray : SerializableArray<string, AudioClip>
    {
    }

    /// <summary>
    ///     Unity Editor serializable keyed array for storing Fonts
    /// </summary>
    [Serializable]
    public sealed class SerializableFontArray : SerializableArray<Font, Font>
    {
    }
}