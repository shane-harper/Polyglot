using System;
using System.Collections;
using Polyglot.Data;
using UnityEngine;

namespace Polyglot
{
    public class LocLoadRequest : IDisposable
    {
        public delegate void ErrorHandler(ErrorCode error);
        public delegate void SuccessHandler(int index, LocKeys keys, LocLanguage language);

        public enum ErrorCode
        {
            InvalidRequest,
            CouldNotFindAssetBundle,
            CouldNotFindKeysAsset,
            CouldNotFindLanguageAsset
        }

        private readonly SuccessHandler _successHandler;
        private readonly ErrorHandler _errorHandler;
        private AssetBundle _bundle;

        public LocLoadRequest(SuccessHandler successHandler, ErrorHandler errorHandler)
        {
            _successHandler = successHandler;
            _errorHandler = errorHandler;
        }

        public void Dispose()
        {
            if (_bundle != null) _bundle.Unload(true);
        }

        public void Get(int index, string name)
        {
            // If there's no success handler, don't waste time
            if (_successHandler == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.InvalidRequest);
                return;
            }

            // Load bundle
            var path = string.Format("{0}/{1}", Application.streamingAssetsPath, LocManager.BundleName);
            try
            {
                _bundle = AssetBundle.LoadFromFile(path);
            }
            catch (Exception ex)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindAssetBundle);
            }

            // Handle bundle load error
            if (_bundle == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindAssetBundle);
                return;
            }

            // Load keys
            var keys = _bundle.LoadAsset<LocKeys>(LocManager.KeysName);

            // Handle keys load error
            if (keys == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindKeysAsset);
                return;
            }

            // Load language
            var language = _bundle.LoadAsset<LocLanguage>(name);

            // Handle language load error
            if (language == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindLanguageAsset);
                return;
            }

            _successHandler(index, keys, language);
        }

        public IEnumerator GetAsync(int index, string name)
        {
            // If there's no success handler, don't waste time
            if (_successHandler == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.InvalidRequest);
                yield break;
            }

            // Load bundle
            var path = string.Format("{0}/{1}", Application.streamingAssetsPath, LocManager.BundleName);
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            yield return bundleRequest;
            _bundle = bundleRequest.assetBundle;

            // Handle bundle load error
            if (_bundle == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindAssetBundle);
                yield break;
            }

            // Load keys
            var keysRequest = _bundle.LoadAssetAsync<LocKeys>(LocManager.KeysName);
            yield return keysRequest;
            var keys = (LocKeys) keysRequest.asset;

            // Handle keys load error
            if (keys == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindKeysAsset);
                yield break;
            }

            // Load language
            var languageRequest = _bundle.LoadAssetAsync<LocLanguage>(name);
            yield return languageRequest;
            var language = (LocLanguage) languageRequest.asset;

            // Handle language load error
            if (language == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindLanguageAsset);
                yield break;
            }

            _successHandler(index, keys, language);
        }
    }
}