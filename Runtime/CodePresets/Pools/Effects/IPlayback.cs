namespace Libraries.DNV.CodePresets.Pools.Effects
{
    /// <summary>
    /// Реализуют только те эффекты, у которых есть собственная длительность.
    /// Живёт отдельно от PooledEffect, чтобы правило возврата не тянуло свой предикат в базовый тип семейства.
    /// </summary>
    public interface IPlayback
    {
        bool IsPlaying { get; }
    }
}
