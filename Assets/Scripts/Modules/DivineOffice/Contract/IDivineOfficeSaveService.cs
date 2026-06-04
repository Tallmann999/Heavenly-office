using System;

public interface IDivineOfficeSaveService
{
    void Save(DivineOfficeSaveData data);
    DivineOfficeSaveData Load();
    void ResetSave();
}
