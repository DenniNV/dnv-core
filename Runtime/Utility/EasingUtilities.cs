using UnityEngine;

namespace Libraries.DNV.Utility
{
    public static class EasingUtilities
    {
        public static float Ease(float start, float end, float t, EaseType easeType = EaseType.Linear)
        {
            t = Mathf.Clamp01(t);
            var easedT = ApplyEasing(t, easeType);
            return Mathf.Lerp(start, end, easedT);
        }

        private static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            return t switch
            {
                < 1 / d1 => n1 * t * t,
                < 2 / d1 => n1 * (t -= 1.5f / d1) * t + 0.75f,
                < 2.5f / d1 => n1 * (t -= 2.25f / d1) * t + 0.9375f,
                _ => n1 * (t -= 2.625f / d1) * t + 0.984375f
            };
        }


        private static float ApplyEasing(float t, EaseType easeType)
        {
            switch (easeType)
            {
                // Линейный
                case EaseType.Linear:
                    return t;

                // Квадратичные
                case EaseType.EaseInQuad:
                    return t * t;
                case EaseType.EaseOutQuad:
                    return t * (2 - t);
                case EaseType.EaseInOutQuad:
                    return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

                // Кубические
                case EaseType.EaseInCubic:
                    return t * t * t;
                case EaseType.EaseOutCubic:
                    return (--t) * t * t + 1;
                case EaseType.EaseInOutCubic:
                    return t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

                // Четвертой степени
                case EaseType.EaseInQuart:
                    return t * t * t * t;
                case EaseType.EaseOutQuart:
                    return 1 - (--t) * t * t * t;
                case EaseType.EaseInOutQuart:
                    return t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;

                // Синусоидальные
                case EaseType.EaseInSine:
                    return 1 - Mathf.Cos(t * Mathf.PI / 2);
                case EaseType.EaseOutSine:
                    return Mathf.Sin(t * Mathf.PI / 2);
                case EaseType.EaseInOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

                // Экспоненциальные
                case EaseType.EaseInExpo:
                    return t == 0 ? 0 : Mathf.Pow(2, 10 * (t - 1));
                case EaseType.EaseOutExpo:
                    return Mathf.Approximately(t, 1) ? 1 : 1 - Mathf.Pow(2, -10 * t);
                case EaseType.EaseInOutExpo:
                    if (t == 0) return 0;
                    if (Mathf.Approximately(t, 1)) return 1;
                    return t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;

                // Круговые
                case EaseType.EaseInCirc:
                    return 1 - Mathf.Sqrt(1 - t * t);
                case EaseType.EaseOutCirc:
                    return Mathf.Sqrt(1 - (--t) * t);
                case EaseType.EaseInOutCirc:
                    return t < 0.5f
                        ? (1 - Mathf.Sqrt(1 - 4 * t * t)) / 2
                        : (Mathf.Sqrt(1 - (2 * t - 2) * (2 * t - 2)) + 1) / 2;

                // С отскоком (Back)
                case EaseType.EaseInBack:
                    return t * t * (2.70158f * t - 1.70158f);
                case EaseType.EaseOutBack:
                    return 1 + (--t) * t * (2.70158f * t + 1.70158f);
                case EaseType.EaseInOutBack:
                    var c1 = 1.70158f;
                    var c2 = c1 * 1.525f;
                    return t < 0.5f
                        ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
                        : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;

                // Упругие (Elastic)
                case EaseType.EaseInElastic:
                    if (t == 0) return 0;
                    if (Mathf.Approximately(t, 1)) return 1;
                    return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * (2 * Mathf.PI) / 3);
                case EaseType.EaseOutElastic:
                    if (t == 0) return 0;
                    if (Mathf.Approximately(t, 1)) return 1;
                    return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * (2 * Mathf.PI) / 3) + 1;
                case EaseType.EaseInOutElastic:
                    if (t == 0) return 0;
                    if (Mathf.Approximately(t, 1)) return 1;
                    return t < 0.5f
                        ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.PI) / 4.5f)) / 2
                        : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.PI) / 4.5f)) / 2 + 1;

                // Прыгающие (Bounce)
                case EaseType.EaseInBounce:
                    return 1 - EaseOutBounce(1 - t);
                case EaseType.EaseOutBounce:
                    return EaseOutBounce(t);
                case EaseType.EaseInOutBounce:
                    return t < 0.5f ? (1 - EaseOutBounce(1 - 2 * t)) / 2 : (1 + EaseOutBounce(2 * t - 1)) / 2;

                default:
                    return t;
            }
        }


        public enum EaseType
        {
            /// <summary>Равномерная скорость от начала до конца. ╱</summary>
            Linear,

            /// <summary>Медленный старт, плавное ускорение по синусоиде. ╰―</summary>
            EaseInSine,

            /// <summary>Быстрый старт, плавное торможение по синусоиде. ―╯</summary>
            EaseOutSine,

            /// <summary>Медленный старт и конец, быстрая середина (S-кривая, синус). ╰S╯</summary>
            EaseInOutSine,

            /// <summary>Медленный старт, квадратичное ускорение. ╰―</summary>
            EaseInQuad,

            /// <summary>Быстрый старт, квадратичное торможение. ―╯</summary>
            EaseOutQuad,

            /// <summary>Медленный старт и конец, квадратичная S-кривая. ╰S╯</summary>
            EaseInOutQuad,

            /// <summary>Медленный старт, кубическое ускорение — резче чем Quad. ╰—</summary>
            EaseInCubic,

            /// <summary>Быстрый старт, кубическое торможение — резче чем Quad. —╯</summary>
            EaseOutCubic,

            /// <summary>Медленный старт и конец, кубическая S-кривая. ╰S╯</summary>
            EaseInOutCubic,

            /// <summary>Очень медленный старт, резкое ускорение четвёртой степени. ╰—</summary>
            EaseInQuart,

            /// <summary>Очень быстрый старт, резкое торможение четвёртой степени. —╯</summary>
            EaseOutQuart,

            /// <summary>Медленный старт и конец, крутая S-кривая четвёртой степени. ╰S╯</summary>
            EaseInOutQuart,

            /// <summary>Почти нулевой старт, экспоненциальный взрывной разгон. ╰——</summary>
            EaseInExpo,

            /// <summary>Мгновенный старт, экспоненциальное резкое торможение. ——╯</summary>
            EaseOutExpo,

            /// <summary>Медленный старт и конец, крутая экспоненциальная S-кривая. ╰S╯</summary>
            EaseInOutExpo,

            /// <summary>Медленный старт, ускорение по дуге окружности. ╰—</summary>
            EaseInCirc,

            /// <summary>Быстрый старт, торможение по дуге окружности. —╯</summary>
            EaseOutCirc,

            /// <summary>Медленный старт и конец, круговая S-кривая. ╰S╯</summary>
            EaseInOutCirc,

            /// <summary>Небольшой откат назад перед стартом, затем разгон. ↙╰—</summary>
            EaseInBack,

            /// <summary>Перелёт за целевое значение с возвратом обратно. —╮↗</summary>
            EaseOutBack,

            /// <summary>Откат на старте и перелёт на финише. ↙╰S╮↗</summary>
            EaseInOutBack,

            /// <summary>Упругие колебания в начале перед движением к цели. ~~~╰—</summary>
            EaseInElastic,

            /// <summary>Упругие колебания в конце после достижения цели. —╯~~~</summary>
            EaseOutElastic,

            /// <summary>Упругие колебания на старте и финише. ~~~╰S╯~~~</summary>
            EaseInOutElastic,

            /// <summary>Эффект отскока в начале (мяч отскакивает к старту). ↑↓↓↓╰—</summary>
            EaseInBounce,

            /// <summary>Эффект отскока в конце (мяч падает и отскакивает несколько раз). —╯↓↑↓</summary>
            EaseOutBounce,

            /// <summary>Отскок на старте и финише. ↑↓╰S╯↓↑</summary>
            EaseInOutBounce
        }
    }
}