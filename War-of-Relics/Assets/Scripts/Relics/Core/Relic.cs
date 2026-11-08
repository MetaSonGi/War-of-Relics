using System;

namespace WarOfRelics
{
    public enum RelicSlot { Weapon, Necklace, Gloves, Headband }

    [Flags]
    public enum RelicAbility { None = 0, ComboAttack = 1 }

    /// <summary>유물 정의 (변하지 않는 데이터)</summary>
    public sealed class Relic
    {
        public string Id { get; }
        public string Name { get; }
        public string Origin { get; }      // 백제·신라·고려·조선
        public RelicSlot Slot { get; }
        public RelicStats Bonus { get; }
        public RelicAbility Abilities { get; }
        public string SetId { get; }

        public Relic(string id, string name, string origin, RelicSlot slot, RelicStats bonus, RelicAbility abilities = RelicAbility.None, string setId = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("유물 id가 필요합니다", nameof(id));
            Id = id; Name = name; Origin = origin; Slot = slot; Bonus = bonus; Abilities = abilities; SetId = setId;
        }

        public override string ToString() => $"{Name} ({Origin}, {Slot})";
    }

    /// <summary>세트 효과: 같은 세트 유물을 requiredPieces개 이상 장착하면 bonus 적용</summary>
    public sealed class RelicSetBonus
    {
        public string SetId { get; }
        public string Name { get; }
        public int RequiredPieces { get; }
        public RelicStats Bonus { get; }

        public RelicSetBonus(string setId, string name, int requiredPieces, RelicStats bonus)
        {
            SetId = setId; Name = name; RequiredPieces = requiredPieces; Bonus = bonus;
        }
    }
}
