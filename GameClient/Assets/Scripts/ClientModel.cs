using UnityEngine;

public class ClientModel : MonoBehaviour
{
    public int Id => _id;
    public string Username => _username;

    private int _id;
    private string _username;
    
    public void Init(int id, string username)
    {
        _id = id;
        _username = username;
    }
}
