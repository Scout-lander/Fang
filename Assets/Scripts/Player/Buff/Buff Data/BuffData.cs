using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff Data", menuName = "Charactor/Charactor Buff Data", order = 1)]
public class BuffData : ScriptableObject
{
    public string name;
    public CharacterData.Stats modifier;

}