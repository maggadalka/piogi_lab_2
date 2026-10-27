using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    public class Player
    {
        // Все данные скрыты (private set) — инкапсуляция.
        public int Lvl { get; private set; }
        public BigNumber Gold { get; private set; }
        public BigNumber Damage { get; private set; }
        public BigNumber UpgradeCost { get; private set; }

        // Модификаторы
        public double DamageModifier { get; private set; }
        public double UpgradeModifier { get; private set; }

        public Player()
        {
            Lvl = 1;
            Gold = new BigNumber(0);
            Damage = new BigNumber(1); // Базовый урон
            UpgradeCost = new BigNumber(10); // Стартовая цена улучшения

            DamageModifier = 1.2; // Урон растет на 20% за уровень
            UpgradeModifier = 1.5; // Цена растет в 1.5 раза
        }

        // Получение золота
        public void AddGold(BigNumber amount)
        {
            Gold = Gold + amount;
        }

        // Попытка улучшения
        public bool TryUpgrade()
        {
            if (Gold >= UpgradeCost)
            {
                Gold = Gold - UpgradeCost; 
                Lvl++; 

                Damage = Damage * 2;

                UpgradeCost = UpgradeCost * 2;

                return true;
            }

            return false; 
        }

    }



}
