using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Vault.BetterCoroutine;

namespace Vault {
    public static class VaultTextMeshPro {
        public static IAsyncRuntime StartTypeWriter(this TextMeshProUGUI textMeshProUGUI, string line) {
            return AsyncRuntime.Create(TypeWriter(line, textMeshProUGUI));
        }

        public static IAsyncRuntime StartTypeWriter(this TextMeshProUGUI textMeshProUGUI,
            string line,
            TypeSounds typeSounds) {
            return AsyncRuntime.Create(TypeWriter(line, textMeshProUGUI, typeSounds));
        }

        public static IAsyncRuntime StartCountUp(this TextMeshProUGUI textMeshProUGUI, string prefix, long count) {
            var cancellationTokenSource = new CancellationTokenSource();
            return AsyncRuntime.Create(CountUp(textMeshProUGUI, prefix, count, cancellationTokenSource.Token), cancellationTokenSource);
        }

        public static IAsyncRuntime StartCountUp(this TextMeshProUGUI textMeshProUGUI, long start, long add, float time) {
            var cancellationTokenSource = new CancellationTokenSource();
            return AsyncRuntime.Create(CountUp(textMeshProUGUI, start, add, time, cancellationTokenSource.Token), cancellationTokenSource);
        }

        public static IAsyncRuntime FadeTextOut(this TextMeshProUGUI textMesh, float time) {
            return AsyncRuntime.Create(FadeTextToZeroAlpha(time, textMesh));
        }

        public static IAsyncRuntime FadeTextOut(this TextMeshPro textMesh, float time) {
            return AsyncRuntime.Create(FadeTextToZeroAlpha(time, textMesh));
        }

        public static void ChangeOpacity(this TextMeshProUGUI me, float opacity) {
            me.color = new Color(me.color.r, me.color.g, me.color.b, opacity);
        }

        public static void ChangeOpacity(this TextMeshPro me, float opacity) {
            me.color = new Color(me.color.r, me.color.g, me.color.b, opacity);
        }

        private static async UniTask CountUp(TextMeshProUGUI me,
            string prefix,
            long count,
            CancellationToken cancellationToken) {
            for (int i = 0; i < count; i++) {
                me.text = prefix + i;
                await UniTask.Delay(TimeSpan.FromSeconds(0.01f), cancellationToken: cancellationToken);
            }
        }

        private static async UniTask CountUp(TextMeshProUGUI me,
            long start,
            long add,
            float seconds,
            CancellationToken cancellationToken) {
            me.text = start.ToString();
            var timePerNumber = seconds / add;
            for (int i = 0; i < add; i++) {
                me.text = (start + i).ToString();
                await UniTask.Delay(TimeSpan.FromSeconds(timePerNumber), cancellationToken: cancellationToken);
            }
        }

        private static async UniTask TypeWriter(string text, TextMeshProUGUI textMesh) {
            for (var i = 0; i < text.Length + 1; i++) {
                textMesh.text = text.Substring(0, i);
                await UniTask.Delay(TimeSpan.FromSeconds(.05f));
            }
        }

        private static async UniTask TypeWriter(string text,
            TextMeshProUGUI textMesh,
            TypeSounds typeSounds) {
            for (var i = 0; i < text.Length + 1; i++) {
                textMesh.text = text.Substring(0, i);
                typeSounds.PlaySound();
                await UniTask.Delay(TimeSpan.FromSeconds(.05f));
            }
        }

        private static async UniTask FadeTextToZeroAlpha(float time, TMP_Text textMesh) {
            var color = textMesh.color;
            color = new Color(color.r, color.g, color.b, 1);
            textMesh.color = color;
            while (textMesh.color.a > 0.0f) {
                var color1 = textMesh.color;
                color1 = new Color(color1.r, color1.g, color1.b, color1.a - Time.deltaTime / time);
                textMesh.color = color1;
                await UniTask.Yield();
            }

            textMesh.text = "";
        }
    }
}