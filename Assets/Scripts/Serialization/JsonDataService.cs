using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//So apparently what I need to do is have one big game data object and
//each member is a slightly smaller container for character information, party information, and map information
//Hopefully I can make it static and it won't throw a fit

public class JsonDataService : IDataService
{
    private const string KEY = "Crocodile=";
    private const string IV = "v==";

    //Calculate save performance by first storing a long DateTime.Now.Ticks then subtract that from the DateTime.Now.Ticks when it is done saving
    //Then take that value and divide it by TimeSpan.TicksPerMillisecond
    //I should be able to repurpose this code and produce a test save written in readable Json
    public bool SaveData<T>(string relativePath, T data, bool encrypted)
    {
        string path = Application.persistentDataPath + relativePath;

        try
        {
            //Create new file at location
            if (File.Exists(path)) { File.Delete(path); }
            using FileStream stream = File.Create(path);

            //Encrypt the file
            if (encrypted)
            {
                WriteEncryptedData(data, stream);
            }

            //Close stream and write to the file
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"This went wrong: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    //Apparently I can generate a key and IV by printing [Convert.ToBase64String(aesProvider.Key)] and [Convert.ToBase64String(aesProvider.IV)]
    void WriteEncryptedData<T>(T data, FileStream stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
    }

    //I'm going to need to see how this reserializes multiple objects instead of just one. There's also that data binding thing I might want to look into
    public T LoadData<T>(string relativePath, bool encrypted)
    {
        string path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path))
        {
            Debug.LogError("File doesn't exist");
            throw new FileNotFoundException($"{path} does not exist");
        }

        try
        {
            T data;
            if (encrypted)
            {
                data = LoadEncryptedData<T>(path); 
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return data;
        }
        catch(Exception e)
        {
            Debug.LogError($"This went wrong: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    private T LoadEncryptedData<T>(string path)
    {
        byte[] fileBytes = File.ReadAllBytes(path);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read);
        using StreamReader reader = new StreamReader(cryptoStream);

        string result = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(result);
    }
}
