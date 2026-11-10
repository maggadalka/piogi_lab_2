using System;
using System.Collections.Generic;
using System.Windows;

namespace piogi_lab_2
{
    // Управляет бонусными сферами и активными бонусами
    public class CController
    {
        // один общий список для всех видов сфер (полиморфизм)
        List<CCollectable> objects;

        // активные бонусы хранятся отдельно, у каждого своё время
        List<CActiveBonus> activeBonuses;

        double spawnRate;   // время между появлением сфер, сек
        double time;        // время с момента создания последней сферы
        Random rng;

        double minLifetime, maxLifetime;
        double minSpriteSize, maxSpriteSize;

        Size sceneSize;

        // откуда брать размер "мгновенного золота" (задаёт окно игры)
        Func<BigNumber> goldBonusProvider;

        // окно подписывается и само добавляет/удаляет фигуры на Canvas
        public event Action<CCollectable> ObjectSpawned;
        public event Action<CCollectable> ObjectRemoved;

        public CController(double spawnRate, Size sceneSize, Func<BigNumber> goldBonusProvider)
        {
            rng = new Random();
            objects = new List<CCollectable>();
            activeBonuses = new List<CActiveBonus>();

            this.spawnRate = spawnRate;
            time = 0;
            this.sceneSize = sceneSize;
            this.goldBonusProvider = goldBonusProvider;

            minLifetime = 3;
            maxLifetime = 6;
            minSpriteSize = 40;
            maxSpriteSize = 70;
        }

        public IEnumerable<CActiveBonus> ActiveBonuses
        {
            get { return activeBonuses; }
        }

        public void setSceneSize(Size size)
        {
            sceneSize = size;
        }

        public BigNumber getGoldBonus()
        {
            return goldBonusProvider();
        }

        // включение бонуса; такой же бонус уже активен — просто продлеваем
        public void addActiveBonus(CActiveBonus bonus, Player player)
        {
            foreach (CActiveBonus existing in activeBonuses)
            {
                if (existing.GetType() == bonus.GetType())
                {
                    existing.refresh();
                    return;
                }
            }

            bonus.onStart(player);
            activeBonuses.Add(bonus);
        }

        // создание новой сферы случайного вида
        void spawn()
        {
            double size = minSpriteSize + rng.NextDouble() * (maxSpriteSize - minSpriteSize);
            double lifetime = minLifetime + rng.NextDouble() * (maxLifetime - minLifetime);
            Point pos = new Point(
                rng.NextDouble() * Math.Max(1, sceneSize.Width - size),
                rng.NextDouble() * Math.Max(1, sceneSize.Height - size));

            CCollectable obj;
            double roll = rng.NextDouble();
            if (roll < 0.4)
                obj = new CGoldSphere(pos, size, lifetime);
            else if (roll < 0.7)
                obj = new CDamageSphere(pos, size, lifetime);
            else
                obj = new CCooldownSphere(pos, size, lifetime);

            objects.Add(obj);
            if (ObjectSpawned != null) ObjectSpawned(obj);
        }

        void remove(CCollectable obj)
        {
            objects.Remove(obj);
            if (ObjectRemoved != null) ObjectRemoved(obj);
        }

        // обновление состояния: вызывается из таймера окна
        public void update(double dt, Player player)
        {
            // появление новых сфер
            time += dt;
            if (time >= spawnRate)
            {
                time = 0;
                spawn();
            }

            // время жизни сфер (с конца, чтобы безопасно удалять)
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].updateLifetime(dt))
                    remove(objects[i]);
            }

            // время действия активных бонусов
            for (int i = activeBonuses.Count - 1; i >= 0; i--)
            {
                if (activeBonuses[i].update(dt))
                {
                    activeBonuses[i].onEnd(player);
                    activeBonuses.RemoveAt(i);
                }
            }
        }

        // клик по слою со сферами; true — собрана какая-то сфера
        public bool handleClick(Player player, Point mousePosition)
        {
            // с конца: верхняя (нарисованная последней) сфера проверяется первой
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                // полиморфизм: вызовется onClick реального вида сферы
                if (objects[i].onClick(player, this, mousePosition) == true)
                {
                    remove(objects[i]);
                    return true;
                }
            }
            return false;
        }
    }
}
