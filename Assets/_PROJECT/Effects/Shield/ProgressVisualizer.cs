using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public abstract class ProgressVisualizer : MonoBehaviour {
    [SerializeField] protected Transform _progressContainer;
    
    [Header("Анимация")]
    [SerializeField] private PairedValue<float> _progressShowDurations;
    [SerializeField] private PairedValue<Ease> _progressShowEase;
    
    [Header("Бар")]
    [SerializeField] private RectTransform _bar;
    [SerializeField] private RectTransform _barParent;
    [SerializeField] private float _changeBarDuration = 1f;
    [field: SerializeField] protected TextMeshProUGUI CountText;

    
    private CancellationTokenSource _tokenSource;
    private Sequence _progressSequence;

    [Inject] LocalizationData _localization;
    
    
    public void ShowBarAnimation(bool show) {
        // Целевой масштаб
        float targetScale = show ? 1f : 0f;
    
        // Если уже в нужном состоянии — выходим
        if(_progressContainer.localScale.x == targetScale) return;
    
    
        // Выбираем длительность и ease в зависимости от show
        float duration = show ? _progressShowDurations.From : _progressShowDurations.To;
        Ease ease = show ? _progressShowEase.From : _progressShowEase.To;
    
        // Запускаем новую
        _progressSequence?.Kill();
        _progressSequence = DOTween.Sequence();
        _progressSequence.Append(_progressContainer.DOScale(targetScale, duration).SetEase(ease));
    
        // Необязательно: чистим ссылку после завершения
        _progressSequence.OnComplete(() => {
            if(_progressSequence != null && _progressSequence.active == false)
                _progressSequence = null;
        });
    }
    
    
    protected void SetPercentage(float percentage, bool horizontal = true) {
        percentage = Mathf.Clamp01(percentage);
        Vector2 targetPos = default;
        if (horizontal) {
            Vector2 offset = _bar.offsetMax;
            offset.x = GetXPoseByPercent(percentage, _barParent);
            targetPos = offset;
        }
        else {
            Vector2 offset = _bar.offsetMax;
            offset.y = GetYPoseByPercent(percentage, _barParent);
            targetPos = offset;
        }
        _bar.offsetMax = targetPos;
        
    }


    protected void FastHide() {
        _progressContainer.localScale = Vector3.zero;
    }

    protected void FastShow() {
        _progressContainer.localScale = Vector3.one;
    }
    
    
    protected void SetProgressPercentage(float percentage, int value, bool setPretty = false) {
        percentage = Mathf.Clamp01(percentage);
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        ChangeProgressPercentageAsync(percentage, _tokenSource.Token).Forget();
        CountText.text = setPretty ? _localization.GetPrettyTime(value) : value.ToString();
    }
    
    
    private void DisposeProgress() {
        _progressSequence?.Kill();
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    
    
    private async UniTask ChangeProgressPercentageAsync(float percentage, CancellationToken token) {
        float elapsedTime = 0f;

        Vector2 initPos = _bar.offsetMax;
        Vector2 targetPos = new Vector2(GetXPoseByPercent(percentage, _barParent), 0);
        
        while (!token.IsCancellationRequested && elapsedTime < _changeBarDuration) {
            elapsedTime += Time.deltaTime;
            SetProgressByElapsedTime(elapsedTime, initPos, targetPos);
            // Debug.Log("interp = " + interp);
            await UniTask.Yield();
        }

        if (!token.IsCancellationRequested) {
            _bar.offsetMax = targetPos;
        }
    }

    private void SetProgressByElapsedTime(float elapsedTime, Vector2 initPos, Vector2 targetPos) {
        float progress = elapsedTime / _changeBarDuration;
        Vector2 interp = Vector2.Lerp(initPos, targetPos, progress);
        _bar.offsetMax = interp;
    }


    /// <summary>
    /// есть в RectTransformHelper просто приватное, саня верни доступ(((
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="xEnd"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private float GetXPoseByPercent(float percent, RectTransform parent) {
        float xEnd =  parent.rect.width;
        if (xEnd < 0) {
            Canvas.ForceUpdateCanvases();
            xEnd = parent.rect.width;
        }
        return -xEnd * (1f - percent);
    }
    
    private float GetYPoseByPercent(float percent, RectTransform parent) {
        float yEnd = parent.rect.height;
        if (yEnd < 0) {
            Canvas.ForceUpdateCanvases();
            yEnd = parent.rect.height;
        }
        return -yEnd * (1f - percent);
    }


    private void OnDestroy() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
}