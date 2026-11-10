using System;

namespace piogi_lab_2
{
    // Абстрактный класс временного бонуса. Живёт в списке контроллера,
    // у каждого бонуса своё время истечения, поэтому несколько бонусов
    // могут действовать одновременно.
    public abstract class CActiveBonus
    {
        double duration;   // полная длительность, сек
        double timeLeft;   // сколько осталось, сек

        public string Name { get; private set; }

        public double TimeLeft { get { return timeLeft; } }

        protected CActiveBonus(string name, double duration)
        {
            Name = name;
            this.duration = duration;
            timeLeft = duration;
        }

        // повторный подбор такого же бонуса продлевает время, а не накладывается второй раз
        public void refresh()
        {
            timeLeft = duration;
        }

        // уменьшение оставшегося времени; true — бонус закончился
        public bool update(double dt)
        {
            timeLeft -= dt;
            return timeLeft <= 0;
        }

        // включение эффекта
        public abstract void onStart(Player player);

        // выключение эффекта (когда время вышло)
        public abstract void onEnd(Player player);
    }
}
