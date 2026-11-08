using System.Collections.Generic;

namespace WarOfRelics
{
    /// <summary>README의 주요 유물 예시. 수치는 초기 밸런스 값이며 기획에 맞게 조정한다.</summary>
    public static class RelicDatabase
    {
        public const string ThreeKingdomsSet = "three-kingdoms";

        public static readonly Relic BaekjeSword = new Relic("baekje-sword", "백제의 장검", "백제", RelicSlot.Weapon,
            new RelicStats { attack = 15 }, RelicAbility.ComboAttack, ThreeKingdomsSet);

        public static readonly Relic HeavenlyNecklace = new Relic("heavenly-necklace", "천상의 목걸이", "신라", RelicSlot.Necklace,
            new RelicStats { cooldownReduction = 0.10f, magicResist = 0.15f }, setId: ThreeKingdomsSet);

        public static readonly Relic CeladonGloves = new Relic("celadon-gloves", "고려청자 장갑", "고려", RelicSlot.Gloves,
            new RelicStats { maxHp = 30, counterChance = 0.10f }, setId: ThreeKingdomsSet);

        public static readonly Relic HwarangHeadband = new Relic("hwarang-headband", "화랑의 머리띠", "신라", RelicSlot.Headband,
            new RelicStats { moveSpeed = 0.10f, evasion = 0.08f });

        public static readonly IReadOnlyList<Relic> All = new[] { BaekjeSword, HeavenlyNecklace, CeladonGloves, HwarangHeadband };

        public static readonly IReadOnlyList<RelicSetBonus> SetBonuses = new[]
        {
            new RelicSetBonus(ThreeKingdomsSet, "삼국의 유산 (2세트)", 2, new RelicStats { defense = 5 }),
            new RelicSetBonus(ThreeKingdomsSet, "삼국의 유산 (3세트)", 3, new RelicStats { attack = 10, maxHp = 20 }),
        };

        public static Relic Find(string id)
        {
            foreach (var r in All) if (r.Id == id) return r;
            return null;
        }
    }
}
