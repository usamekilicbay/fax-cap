using DG.Tweening;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common.Animations
{
    public static class UIAnimationExtension
    {
        public static async Task AnimateTextt(this TextMeshProUGUI text, int initialNumber, int targetNumber,
            Vector2 initialSize, Vector2 targetSize, float duration = 0.3f)
        {
            text.rectTransform.sizeDelta = initialSize;

            var numberUpdateTween = DOTween.To(() => initialNumber, x => initialNumber = x, targetNumber, duration);
            numberUpdateTween.SetEase(Ease.Linear);
            numberUpdateTween.SetAutoKill(true);
            numberUpdateTween.Pause();

            var scaleUpTween = text.rectTransform.DOSizeDelta(targetSize, duration);
            scaleUpTween.SetEase(Ease.OutBack);
            numberUpdateTween.SetAutoKill(true);
            scaleUpTween.Pause();

            var scaleResetTween = text.rectTransform.DOSizeDelta(initialSize, 0.3f);
            //scaleResetTween.SetEase(Ease.InBounce);
            scaleResetTween.SetAutoKill(true);
            scaleResetTween.Pause();

            var sequence = DOTween.Sequence();
            sequence.Append(numberUpdateTween);
            sequence.Join(scaleUpTween);
            sequence.OnUpdate(() => text.SetText(initialNumber.ToString()));
            sequence.OnComplete(() =>
            {
                text.SetText(targetNumber.ToString());
                scaleResetTween.Restart();
            });
            sequence.Restart();

            await sequence.AsyncWaitForCompletion();
        }


        //public static async Task AnimateTextt(this TextMeshProUGUI text, int initialNumber, int targetNumber,
        //    Vector2 initialSize, Vector2 targetSize, float duration = 10f)
        //{
        //    text.rectTransform.sizeDelta = initialSize;

        //    var maxSize = initialSize * 2.0f;

        //    var sizeTween = text.rectTransform.DOSizeDelta(initialSize, duration / targetNumber);
        //    //sizeTween.SetLoops(2, LoopType.Yoyo);
        //    sizeTween.Pause();

        //    var currentNumber = initialNumber;
        //    var numberUpdateTween = DOTween.To(() => initialNumber, x => initialNumber = x, targetNumber, duration);
        //    numberUpdateTween.OnUpdate(async () =>
        //    {
        //        if (initialNumber > currentNumber)
        //        {
        //            Debug.Log(initialNumber);
        //            currentNumber = initialNumber;
        //            text.SetText(initialNumber.ToString());

        //            var currentSize = text.rectTransform.sizeDelta;
        //            var newTargetSize = currentSize.magnitude < targetSize.magnitude 
        //                ? currentSize + currentSize * 0.1f 
        //                : targetSize;

        //            sizeTween.ChangeEndValue(newTargetSize);
        //            sizeTween.Restart();

        //            await sizeTween.AsyncWaitForCompletion();
        //        }
        //    });
        //    numberUpdateTween.OnComplete(() => text.SetText(targetNumber.ToString()));

        //    await numberUpdateTween.AsyncWaitForCompletion();
        //}


        public static async Task AnimateText(this TextMeshProUGUI text, int initialNumber, int targetNumber,
            Vector2 initialSize, Vector2 targetSize, float duration = 0.3f)
        {
            text.rectTransform.sizeDelta = initialSize;

            var numberUpdateTween = GetNumberUpdateTween(initialNumber, targetNumber, duration);

            var sizeTween = text.rectTransform.DOSizeDelta(targetSize, duration);
            sizeTween.SetEase(Ease.InBack);
            sizeTween.SetLoops(2, LoopType.Yoyo);
            sizeTween.Pause();

            var sequence = DOTween.Sequence();
            sequence.Append(numberUpdateTween);
            sequence.Join(sizeTween);
            sequence.OnUpdate(() => text.SetText(initialNumber.ToString()));
            sequence.OnComplete(() => text.SetText(targetNumber.ToString()));
            sequence.Restart();

            await sequence.AsyncWaitForCompletion();
        }

        private static Tween GetNumberUpdateTween(int initialNumber, int targetNumber, float duration)
        {
            var numberUpdateTween = DOTween.To(() => initialNumber, x => initialNumber = x, targetNumber, duration);
            numberUpdateTween.SetEase(Ease.Linear);
            numberUpdateTween.Pause();
            numberUpdateTween.SetAutoKill(true);

            return numberUpdateTween;
        }
    }
}
