using System;
using System.IO;
using UnityEngine;

public class DivineOfficeSaveService : IDivineOfficeSaveService
{
    private readonly string fileName = "divine_office_save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    public DivineOfficeSaveData Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new DivineOfficeSaveData();
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<DivineOfficeSaveData>(json) ?? new DivineOfficeSaveData();
        }
        catch (Exception)
        {
            return new DivineOfficeSaveData();
        }
    }

    public void Save(DivineOfficeSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save DivineOffice data: " + ex.Message);
        }
    }

    public void ResetSave()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to reset DivineOffice save: " + ex.Message);
        }
    }
}
