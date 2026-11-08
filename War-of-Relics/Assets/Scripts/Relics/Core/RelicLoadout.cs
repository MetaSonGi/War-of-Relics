using System;
using System.Collections.Generic;
using System.Linq;

namespace WarOfRelics
{
    /// <summary>
    /// 유물 수집(인벤토리)과 장착. 부위마다 하나만 장착하며, 같은 부위에 장착하면 기존 유물은 인벤토리로 돌아간다.
    /// 최종 능력치 = 기본 능력치 + 장착 유물 + 충족한 세트 효과 (상한 적용).
    /// </summary>
    public sealed class RelicLoadout
    {
        readonly List<Relic> owned = new List<Relic>();
        readonly Dictionary<RelicSlot, Relic> equipped = new Dictionary<RelicSlot, Relic>();
        readonly IReadOnlyList<RelicSetBonus> setBonuses;

        public event Action Changed;

        public RelicLoadout(IReadOnlyList<RelicSetBonus> setBonuses = null)
        {
            this.setBonuses = setBonuses ?? RelicDatabase.SetBonuses;
        }

        public IReadOnlyList<Relic> Owned => owned;
        public IReadOnlyDictionary<RelicSlot, Relic> Equipped => equipped;

        /// <summary>유물 획득. 같은 유물은 한 번만 보관한다.</summary>
        public bool Collect(Relic relic)
        {
            if (relic == null || owned.Any(r => r.Id == relic.Id)) return false;
            owned.Add(relic);
            Changed?.Invoke();
            return true;
        }

        /// <summary>장착하고, 그 부위에 있던 유물(없으면 null)을 돌려준다.</summary>
        public Relic Equip(string relicId)
        {
            var relic = owned.FirstOrDefault(r => r.Id == relicId)
                ?? throw new InvalidOperationException($"가지고 있지 않은 유물입니다: {relicId}");
            equipped.TryGetValue(relic.Slot, out var previous);
            if (previous == relic) return null;
            equipped[relic.Slot] = relic;
            Changed?.Invoke();
            return previous;
        }

        public Relic Unequip(RelicSlot slot)
        {
            if (!equipped.TryGetValue(slot, out var relic)) return null;
            equipped.Remove(slot);
            Changed?.Invoke();
            return relic;
        }

        public bool Has(RelicAbility ability) => equipped.Values.Any(r => (r.Abilities & ability) != 0);

        public IEnumerable<RelicSetBonus> ActiveSetBonuses()
        {
            var counts = equipped.Values.Where(r => r.SetId != null).GroupBy(r => r.SetId).ToDictionary(g => g.Key, g => g.Count());
            return setBonuses.Where(b => counts.TryGetValue(b.SetId, out var n) && n >= b.RequiredPieces);
        }

        public RelicStats Total(RelicStats baseStats)
        {
            var total = baseStats;
            foreach (var r in equipped.Values) total += r.Bonus;
            foreach (var b in ActiveSetBonuses()) total += b.Bonus;
            return total.Clamped();
        }

        /// <summary>저장용: 보유·장착 유물 id 목록</summary>
        public (string[] owned, string[] equipped) Save() =>
            (owned.Select(r => r.Id).ToArray(), equipped.Values.Select(r => r.Id).ToArray());

        /// <summary>저장 데이터 복원. 데이터베이스에 없는 id는 무시한다(버전 변경 대비).</summary>
        public static RelicLoadout Load(IEnumerable<string> ownedIds, IEnumerable<string> equippedIds, Func<string, Relic> find = null)
        {
            find = find ?? RelicDatabase.Find;
            var l = new RelicLoadout();
            foreach (var id in ownedIds ?? Enumerable.Empty<string>()) l.Collect(find(id));
            foreach (var id in equippedIds ?? Enumerable.Empty<string>())
                if (l.owned.Any(r => r.Id == id)) l.Equip(id);
            return l;
        }
    }
}
