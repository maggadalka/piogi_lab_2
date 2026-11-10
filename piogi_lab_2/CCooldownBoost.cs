namespace piogi_lab_2
{
    // Временный бонус: перезарядка между кликами умножается на factor (0.5 = вдвое быстрее)
    public class CCooldownBoost : CActiveBonus
    {
        double factor;

        public CCooldownBoost(double factor = 0.5, double duration = 10)
            : base("Перезарядка x" + factor, duration)
        {
            this.factor = factor;
        }

        public override void onStart(Player player)
        {
            player.Cooldown.AddFactor(factor);
        }

        public override void onEnd(Player player)
        {
            player.Cooldown.RemoveFactor(factor);
        }
    }
}
