using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveWorld 
{
    public static void SaveNeuralNetwork(NeuralNetwork network)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/NeuralNetwork.AI";
        Debug.Log(path);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, network);
        stream.Close();
    }

    public static NeuralNetwork ReadData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/NeuralNetwork.AI";
        if (File.Exists(path))
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            return Formatter.Deserialize(stream) as NeuralNetwork;
        }
        else
        {
            return null;
        }
    }
}
