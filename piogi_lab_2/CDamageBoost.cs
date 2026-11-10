namespace piogi_lab_2
{
    // Временный бонус: урон умножается на multiplier
    public class CDamageBoost : CActiveBonus
    {
        int multiplier;

        public CDamageBoost(int multiplier = 2, double duration = 10)
            : base("Урон x" + multiplier, duration)
        {
            this.multiplier = multiplier;
        }

        public override void onStart(Player player)
        {
            player.AddDamageMultiplier(multiplier);
        }

        public override void onEnd(Player player)
        {
            player.RemoveDamageMultiplier(multiplier);
        }
    }
}
