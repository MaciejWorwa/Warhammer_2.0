using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellStats : MonoBehaviour
{
    public int Spell_S = 3; // siła zaklęcia
    public int PowerRequired = 6; // wymagany poziom mocy
    public float SpellRange = 8f; // zasięg zaklęcia

    public float AreaSize = 1; // wielkość obszaru objętego działaniem zaklęcia

    public int CastDuration; // czas rzucania zaklęcia

    public bool OffensiveSpell; // określa, czy zaklęcie jest ofensywne (może byc rzucane tylko na przeciwników)

    public bool IgnoreArmor; // Określa, czy zaklęcie ofensywne ignoruje pancerz
}
