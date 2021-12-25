using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private string _username;
    
    public int Id => _id;
    public string Username => _username;

    public void Init(int id, string username)
    {
        _id = id;
        _username = username;
    }
}
