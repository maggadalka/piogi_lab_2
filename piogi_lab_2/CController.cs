using System;
using System.Collections.Generic;

namespace piogi_lab_2
{
    // Управляет бонусными сферами и активными бонусами.
    // (первая версия: пока только список активных бонусов)
    public class CController
    {
        // ВАЖНО: активные бонусы хранятся в одном списке, у каждого своё время
        List<CActiveBonus> activeBonuses;

        // откуда брать размер "мгновенного золота" (задаёт окно игры)
        Func<BigNumber> goldBonusProvider;

        public CController(Func<BigNumber> goldBonusProvider)
        {
            activeBonuses = new List<CActiveBonus>();
            this.goldBonusProvider = goldBonusProvider;
        }

        public IEnumerable<CActiveBonus> ActiveBonuses
        {
            get { return activeBonuses; }
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

        // обновление времени бонусов: вызывается из таймера окна
        public void update(double dt, Player player)
        {
            // идём с конца, чтобы безопасно удалять из списка
            for (int i = activeBonuses.Count - 1; i >= 0; i--)
            {
                if (activeBonuses[i].update(dt))
                {
                    activeBonuses[i].onEnd(player);
                    activeBonuses.RemoveAt(i);
                }
            }
        }
    }
}
