using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    public class Enemy
    {
        // Здоровье и награда скрыты от прямого изменения
        public string Name { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber RewardGold { get; private set; }

        public Enemy(CEnemyTemplate template, int wave)
        {
            Name = template.Name;

            // Базовое здоровье из редактора
            long baseHealth = template.BaseLife;

            // Прирост
            long healthBonus = (long)(baseHealth * 0.4 * (wave - 1));

            long finalHealth = baseHealth + healthBonus;
            if (finalHealth < 1) finalHealth = 1; 

            MaxHealth = new BigNumber(finalHealth);
            CurrentHealth = new BigNumber(finalHealth);

            long baseGold = template.BaseGold;
            long goldBonus = (long)(baseGold * 0.3 * (wave - 1)); 
            long finalGold = baseGold + goldBonus;

            RewardGold = new BigNumber(finalGold);
        }

        public void TakeDamage(BigNumber damage)
        {
            CurrentHealth = CurrentHealth - damage;

            if (CurrentHealth < new BigNumber(0))
            {
                CurrentHealth = new BigNumber(0); 
            }
        }

        public bool IsDead()
        {
            BigNumber zero = new BigNumber(0);

            return CurrentHealth <= zero;
        }
    }

}
