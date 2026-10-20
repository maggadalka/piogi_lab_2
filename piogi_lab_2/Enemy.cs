using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    public class Enemy
    {
        // Здоровье и награда скрыты от прямого изменения.
        public string Name { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber RewardGold { get; private set; }

        public Enemy(CEnemyTemplate template)
        {
            Name = template.Name;

            MaxHealth = new BigNumber(template.BaseLife);
            RewardGold = new BigNumber(template.BaseGold);

            CurrentHealth = new BigNumber(template.BaseLife);
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
            return CurrentHealth == new BigNumber(0) || CurrentHealth < new BigNumber(0);
        }
    }

}
