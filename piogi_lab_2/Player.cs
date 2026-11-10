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

        // Перезарядка между кликами по врагу
        public CCooldownTimer Cooldown { get; private set; }
        public BigNumber CooldownUpgradeCost { get; private set; }

        // Временный множитель урона от бонусных сфер (1 = бонуса нет)
        public int DamageMultiplier { get; private set; }

        // Урон с учётом активных бонусов — именно он наносится врагу
        public BigNumber EffectiveDamage
        {
            get { return Damage * DamageMultiplier; }
        }

        public Player()
        {
            Lvl = 1;
            Gold = new BigNumber(0);
            Damage = new BigNumber(1); // Базовый урон
            UpgradeCost = new BigNumber(10); // Стартовая цена улучшения

            DamageModifier = 1.2; // Урон растет на 20% за уровень
            UpgradeModifier = 1.5; // Цена растет в 1.5 раза

            Cooldown = new CCooldownTimer(0.5);          // 0.5 сек между кликами
            CooldownUpgradeCost = new BigNumber(20);
            DamageMultiplier = 1;
        }

        // Обновление времени (вызывается из таймера окна)
        public void update(double dt)
        {
            Cooldown.Update(dt);
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

        // Попытка улучшить перезарядку за золото (по той же схеме, что TryUpgrade)
        public bool TryUpgradeCooldown()
        {
            if (!Cooldown.CanReduce)
                return false;

            if (Gold >= CooldownUpgradeCost)
            {
                Gold = Gold - CooldownUpgradeCost;
                Cooldown.Reduce(0.05);
                CooldownUpgradeCost = CooldownUpgradeCost * 2;
                return true;
            }

            return false;
        }

        // Временные множители урона (включаются/выключаются бонусами)
        public void AddDamageMultiplier(int m)
        {
            DamageMultiplier *= m;
        }

        public void RemoveDamageMultiplier(int m)
        {
            DamageMultiplier /= m;
        }

    }



}
