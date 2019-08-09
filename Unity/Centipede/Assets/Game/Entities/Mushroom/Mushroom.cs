public class Mushroom : BoardEntityBase
{
    public MushroomVisual Visual;

    public override GameConstants.CellType GetCellType()
    {
        return GameConstants.CellType.Mushroom;
    }

    public void ApplyDamage(float damage)
    {
    }
}