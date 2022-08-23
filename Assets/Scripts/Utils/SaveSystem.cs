using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public const string saveFileName = "2048.remake.Save";

    public static void SavePlayer (PlayerManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + saveFileName;
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerProfile profile = new PlayerProfile(player);

        formatter.Serialize(stream, profile);

        stream.Close();

        Debug.Log("Savedddd");
    }


    public static PlayerProfile LoadPlayer()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerProfile profile = formatter.Deserialize(stream) as PlayerProfile;
            stream.Close();

            return profile;
        }
        else
        {
            Debug.Log("Save file not found in" + path);
            return null;
        }
    }
}
