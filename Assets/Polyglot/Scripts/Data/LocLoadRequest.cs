using System;
using System.Collections;
using Polyglot.Data;
#if !POLYGLOT_ADDRESSABLES
using UnityEngine;
#else
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

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
        
        #if !POLYGLOT_ADDRESSABLES
        private AssetBundle _bundle;
        #else
        private IAsyncOperation<LocLanguage> _languageRequest;
        private IAsyncOperation<LocKeys> _keysRequest;
        #endif

        public LocLoadRequest(SuccessHandler successHandler, ErrorHandler errorHandler)
        {
            _successHandler = successHandler;
            _errorHandler = errorHandler;
        }

        public void Dispose()
        {
            #if !POLYGLOT_ADDRESSABLES
            if (_bundle != null) _bundle.Unload(true);
            #else
            if (_keysRequest != null) _keysRequest.Release();
            if (_languageRequest != null) _languageRequest.Release();
            #endif
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
            #if !POLYGLOT_ADDRESSABLES
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
            #endif

            // Load keys
            #if !POLYGLOT_ADDRESSABLES
            var keysRequest = _bundle.LoadAssetAsync<LocKeys>(LocManager.KeysName);
            yield return keysRequest;
            var keys = (LocKeys) keysRequest.asset;
            #else
            _keysRequest = Addressables.LoadAsset<LocKeys>(LocManager.KeysName);
            yield return _keysRequest;
            var keys = _keysRequest.Result;
            #endif

            // Handle keys load error
            if (keys == null)
            {
                if (_errorHandler != null) _errorHandler(ErrorCode.CouldNotFindKeysAsset);
                yield break;
            }

            // Load language
            #if !POLYGLOT_ADDRESSABLES
            var languageRequest = _bundle.LoadAssetAsync<LocLanguage>(name);
            yield return languageRequest;
            var language = (LocLanguage) languageRequest.asset;
            #else
            _languageRequest = Addressables.LoadAsset<LocLanguage>(name);
            yield return _languageRequest;
            var language = _languageRequest.Result;
            #endif

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