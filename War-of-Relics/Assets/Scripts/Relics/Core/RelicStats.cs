using System;

namespace WarOfRelics
{
    /// <summary>캐릭터·유물 능력치. 퍼센트 값은 0.10 = 10%.</summary>
    [Serializable]
    public struct RelicStats
    {
        public int attack;
        public int defense;
        public int maxHp;
        public float moveSpeed;          // 퍼센트 증가
        public float evasion;            // 회피율
        public float cooldownReduction;  // 스킬 쿨다운 감소
        public float magicResist;        // 마법 저항
        public float counterChance;      // 반격 확률

        public static RelicStats operator +(RelicStats a, RelicStats b) => new RelicStats
        {
            attack = a.attack + b.attack,
            defense = a.defense + b.defense,
            maxHp = a.maxHp + b.maxHp,
            moveSpeed = a.moveSpeed + b.moveSpeed,
            evasion = a.evasion + b.evasion,
            cooldownReduction = a.cooldownReduction + b.cooldownReduction,
            magicResist = a.magicResist + b.magicResist,
            counterChance = a.counterChance + b.counterChance,
        };

        /// <summary>확률·감소율은 게임이 깨지지 않도록 상한을 둔다.</summary>
        public RelicStats Clamped() => new RelicStats
        {
            attack = attack, defense = defense, maxHp = Math.Max(1, maxHp),
            moveSpeed = Math.Min(moveSpeed, 0.5f),
            evasion = Math.Min(evasion, 0.6f),
            cooldownReduction = Math.Min(cooldownReduction, 0.4f),
            magicResist = Math.Min(magicResist, 0.75f),
            counterChance = Math.Min(counterChance, 0.5f),
        };

        public override string ToString() =>
            $"공격 {attack} · 방어 {defense} · 체력 {maxHp} · 이동 +{moveSpeed:P0} · 회피 {evasion:P0} · 쿨감 {cooldownReduction:P0} · 마저 {magicResist:P0} · 반격 {counterChance:P0}";
    }
}
