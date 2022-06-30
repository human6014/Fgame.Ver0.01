using Photon.Pun;
public class BulletCase : MonoBehaviourPunCallbacks
{
    void Start() => Destroy(gameObject, 3);
}