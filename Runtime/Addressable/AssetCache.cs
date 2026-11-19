using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Addressable
{
    internal sealed class AssetCache
    {
        private sealed class CashedItem
        {
            public AsyncOperationHandle Handle;
            public int RefCount;
        }

        private readonly Dictionary<AssetKey, CashedItem> _entries = new();

        public UniTask<T> LoadAsync<T>(AssetKey key, Func<AsyncOperationHandle<T>> startLoad, CancellationToken token)
            where T : Object
        {
            if (_entries.TryGetValue(key, out var cached))
            {
                cached.RefCount++;
                Log.Info(LogCategory.Assets, $"[AssetCache] HIT     {key} refs {cached.RefCount - 1} -> {cached.RefCount}");
                return AwaitOrRollback(key, cached.Handle.Convert<T>(), token);
            }

            Log.Info(LogCategory.Assets, $"[AssetCache] LOAD    {key} refs 0 -> 1, cached entries: {_entries.Count + 1}");

            var handle = startLoad();
            _entries[key] = new CashedItem { Handle = handle, RefCount = 1 };
            return AwaitOrRollback(key, handle, token);
        }

        private async UniTask<T> AwaitOrRollback<T>(AssetKey key, AsyncOperationHandle<T> handle, CancellationToken token)
            where T : Object
        {
            try
            {
                var asset = await handle.ToUniTask(cancellationToken: token);
                Log.Info(LogCategory.Assets, $"[AssetCache] READY   {key} refs {RefCountOf(key)}");
                return asset;
            }
            catch (Exception exception)
            {
                LogLoadFailure(key, exception);
                Release(key);
                throw;
            }
        }

        public void Release(AssetKey key)
        {
            if (!_entries.TryGetValue(key, out var entry))
            {
                Log.Warning(LogCategory.Assets, $"[AssetCache] Release of non-cached asset: {key}");
                return;
            }

            entry.RefCount--;

            if (entry.RefCount > 0)
            {
                Log.Info(LogCategory.Assets, $"[AssetCache] RELEASE {key} refs {entry.RefCount + 1} -> {entry.RefCount}");
                return;
            }

            _entries.Remove(key);

            if (!entry.Handle.IsValid())
            {
                Log.Warning(LogCategory.Assets, $"[AssetCache] UNLOAD  {key} refs 0, but handle is already invalid — nothing to release");
                return;
            }

            Addressables.Release(entry.Handle);
            Log.Info(LogCategory.Assets, $"[AssetCache] UNLOAD  {key} refs 0 -> Addressables.Release, cached entries left: {_entries.Count}");
        }

        public void Dispose()
        {
            LogSnapshot("Dispose");

            foreach (var entry in _entries.Values)
            {
                if (entry.Handle.IsValid())
                    Addressables.Release(entry.Handle);
            }

            _entries.Clear();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public void LogSnapshot(string reason)
        {
            if (_entries.Count == 0)
            {
                Log.Info(LogCategory.Assets, $"[AssetCache] {reason}: cache is empty — every loaded asset was released by its owner");
                return;
            }

            var report = new StringBuilder();
            report.Append($"[AssetCache] {reason}: {_entries.Count} asset(s) still cached — released by Dispose, not by their owners:");

            foreach (var pair in _entries)
                report.Append($"\n    {pair.Key} refs={pair.Value.RefCount}");

            Log.Warning(LogCategory.Assets, report.ToString());
        }

        private int RefCountOf(AssetKey key)
        {
            return _entries.TryGetValue(key, out var entry) ? entry.RefCount : 0;
        }

        private static void LogLoadFailure(AssetKey key, Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                Log.Info(LogCategory.Assets, $"[AssetCache] CANCEL  {key} — refcount rolled back");
                return;
            }

            Log.Warning(LogCategory.Assets, $"[AssetCache] FAILED  {key} ({exception.GetType().Name}) — refcount rolled back");
        }
    }
}
