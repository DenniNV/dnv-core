using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using Libraries.DNV.MVC.Core;
using Libraries.DNV.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Libraries.DNV.MVC.Transition
{
    public class TransitionView : UIElement
    {
        private const float FullTurn = 360f;
        private const EasingUtilities.EaseType Easing = EasingUtilities.EaseType.EaseOutCubic;

        [SerializeField]
        private float _showDelay;

        [SerializeField]
        private float _hideDelay;

        [SerializeField]
        private Image _background;

        [SerializeField]
        private Image _loadCircleBackground;

        [SerializeField]
        private Image _loadCircle;

        [SerializeField]
        private TextMeshProUGUI[] _textMeshPro;

        private readonly List<MaskableGraphic> _fadedGraphics = new();
        private CancellationTokenSource _fade;
        private CancellationTokenSource _spin;

        public override void Show()
        {
            Activate = true;
            FadeAsync(0f, 1f, _showDelay, Restart(ref _fade)).Forget();
            SpinLoadCircleAsync(Restart(ref _spin)).Forget();
        }

        public override void Hide()
        {
            HideAsync(Restart(ref _fade)).Forget();
        }

        public override void Construct()
        {
            Log.Info(LogCategory.Debug, $"{nameof(TransitionView)}.{nameof(Construct)}");

            _fadedGraphics.Clear();
            _fadedGraphics.Add(_background);
            _fadedGraphics.Add(_loadCircle);
            _fadedGraphics.AddRange(_textMeshPro);
        }

        public override void Release()
        {
            Cancel(ref _fade);
            Cancel(ref _spin);
        }

        private async UniTaskVoid HideAsync(CancellationToken token)
        {
            var cancelled = await FadeAsync(1f, 0f, _hideDelay, token);

            if(cancelled)
            {
                return;
            }

            Cancel(ref _spin);
            Activate = false;
        }

        private async UniTask<bool> FadeAsync(float from, float to, float duration, CancellationToken token)
        {
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                SetAlpha(EasingUtilities.Ease(from, to, elapsed / duration, Easing));

                if(await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow())
                {
                    return true;
                }
            }

            SetAlpha(to);
            return false;
        }

        private async UniTaskVoid SpinLoadCircleAsync(CancellationToken token)
        {
            if(_hideDelay <= 0f)
            {
                return;
            }

            var circle = _loadCircle.transform;

            while(!token.IsCancellationRequested)
            {
                var startAngle = circle.localEulerAngles.z;

                for (var elapsed = 0f; elapsed < _hideDelay; elapsed += Time.deltaTime)
                {
                    var angle = startAngle + EasingUtilities.Ease(0f, FullTurn, elapsed / _hideDelay, Easing);
                    circle.localEulerAngles = new Vector3(0f, 0f, angle);

                    if(await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow())
                    {
                        return;
                    }
                }
            }
        }

        private void SetAlpha(float alpha)
        {
            foreach (var graphic in _fadedGraphics)
            {
                var color = graphic.color;
                color.a = alpha;
                graphic.color = color;
            }
        }

        private CancellationToken Restart(ref CancellationTokenSource source)
        {
            Cancel(ref source);
            source = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            return source.Token;
        }

        private static void Cancel(ref CancellationTokenSource source)
        {
            if(source == null)
            {
                return;
            }

            source.Cancel();
            source.Dispose();
            source = null;
        }
    }
}
