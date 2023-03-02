using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject progress;
        [SerializeField] private Image fill;


        private IProgress m_Progress;


        private void Awake()
        {
            if (!progress.TryGetComponent(out m_Progress))
            {
                var msg = $"{progress} does not implement {nameof(IProgress)}";
                UnityEngine.Debug.LogError(msg);
                return;
            }

            m_Progress.OnProgressChanged += OnProgressChanged;
        }

        private void OnDestroy()
        {
            m_Progress.OnProgressChanged -= OnProgressChanged;
        }


        private void OnProgressChanged(ProgressChangedArgs args)
        {
            fill.fillAmount = args.progressNormalized;
        }
    }
}