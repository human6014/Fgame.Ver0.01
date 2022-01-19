using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}


[System.Serializable]
public class PlayerInfo
{
    public string nickName;
    public int actorNum;
    public int kill;
    public int death;
    public int tileNum;
    public double lifeTime;
    public PlayerInfo(string _nickName, int _actorNum, int _kill, int _death, int _tileNum, double _lifeTime)
    {
        nickName = _nickName;
        actorNum = _actorNum;
        kill = _kill;
        death = _death;
        tileNum = _tileNum;
        lifeTime = _lifeTime;
    }


}
