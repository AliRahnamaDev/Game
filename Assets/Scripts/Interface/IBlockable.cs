public interface IBlockable
{
    bool IsBlocking();
    float GetDamageReductionMultiplier();
    float GetKnockbackReductionMultiplier();
}