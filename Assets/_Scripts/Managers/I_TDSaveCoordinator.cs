public interface I_TDSaveCoordinator
{
    public abstract void InitFromData(SaveData saveData);
    public abstract void AddToSaveData(ref SaveData saveData);
}

public interface I_TDBulidingSaveCoordinator
{
    public abstract void InitFromData(SaveData.TowerSaveData towerSaveData);
    public abstract void AddToSaveData(ref SaveData saveData);
}