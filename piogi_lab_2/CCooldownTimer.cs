using System;

namespace piogi_lab_2
{
    // Отсчёт времени до следующего доступного клика.
    // Не привязан к игроку, поэтому его можно использовать и для других задач.
    public class CCooldownTimer
    {
        double baseDuration;   // базовая длительность перезарядки, сек (её улучшают за золото)
        double factor;         // временный множитель от бонусов (0.5 = перезарядка вдвое короче)
        double elapsed;        // сколько прошло с момента последнего клика, сек
        double minDuration;    // ниже этого значения базовую перезарядку улучшить нельзя

        public CCooldownTimer(double duration, double minDuration = 0.1)
        {
            baseDuration = duration;
            this.minDuration = minDuration;
            factor = 1.0;
            elapsed = Duration; // в начале игры клик сразу доступен
        }

        // текущая длительность с учётом бонусов
        public double Duration { get { return baseDuration * factor; } }

        // можно ли ещё улучшать перезарядку
        public bool CanReduce { get { return baseDuration - 0.05 >= minDuration - 1e-9; } }

        // 0..1 — для полоски прогресса
        public double Progress { get { return Math.Min(1.0, elapsed / Duration); } }

        public bool IsReady { get { return elapsed >= Duration; } }

        public void Update(double dt)
        {
            if (elapsed < Duration)
                elapsed += dt;
        }

        // вызывается после успешного клика
        public void Reset()
        {
            elapsed = 0;
        }

        // постоянное улучшение (за золото)
        public void Reduce(double amount)
        {
            baseDuration = Math.Max(minDuration, baseDuration - amount);
        }

        // временный множитель (бонусные сферы): включение и выключение
        public void AddFactor(double f)
        {
            factor *= f;
        }

        public void RemoveFactor(double f)
        {
            factor /= f;
        }
    }
}
