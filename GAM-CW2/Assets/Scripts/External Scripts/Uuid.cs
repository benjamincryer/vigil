//NOTE: Don't forget to generate a new UUID when you instantiate an object!

[System.Serializable]
public class Uuid
{
    public string uuid;

    public static Uuid Generate()
    {
        Uuid newUuid = new Uuid { uuid = System.Guid.NewGuid().ToString() };
        return newUuid;
    }
}