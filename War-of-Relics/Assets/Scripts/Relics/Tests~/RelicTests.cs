// Unity 없이 실행하는 유물 시스템 테스트 (폴더 이름이 ~로 끝나 Unity는 이 폴더를 무시)
// 실행: mcs -out:relic-tests.exe ../Core/*.cs RelicTests.cs && mono relic-tests.exe
using System;
using System.Linq;
using WarOfRelics;

static class RelicTests
{
    static int fails;
    static void Ok(bool c, string m) { Console.WriteLine((c ? "ok   " : "FAIL ") + m); if (!c) fails++; }
    static bool Near(float a, float b) => Math.Abs(a - b) < 1e-4f;

    static int Main()
    {
        var baseStats = new RelicStats { attack = 20, defense = 10, maxHp = 100 };
        var l = new RelicLoadout();
        int changes = 0; l.Changed += () => changes++;

        Ok(RelicDatabase.All.Select(r => r.Id).Distinct().Count() == RelicDatabase.All.Count, "유물 id 중복 없음");
        Ok(l.Collect(RelicDatabase.BaekjeSword) && !l.Collect(RelicDatabase.BaekjeSword), "같은 유물은 한 번만 보관");
        Ok(l.Total(baseStats).attack == 20, "장착 전에는 기본 능력치");

        l.Equip("baekje-sword");
        Ok(l.Total(baseStats).attack == 35 && l.Has(RelicAbility.ComboAttack), "백제의 장검: 공격 +15, 연속공격");

        try { l.Equip("celadon-gloves"); Ok(false, "없는 유물 장착은 거부"); }
        catch (InvalidOperationException) { Ok(true, "없는 유물 장착은 거부"); }

        l.Collect(RelicDatabase.HeavenlyNecklace); l.Equip("heavenly-necklace");
        var t = l.Total(baseStats);
        Ok(t.defense == 15 && l.ActiveSetBonuses().Count() == 1, "삼국의 유산 2세트: 방어 +5");

        l.Collect(RelicDatabase.CeladonGloves); l.Equip("celadon-gloves");
        t = l.Total(baseStats);
        Ok(t.attack == 45 && t.maxHp == 150 && l.ActiveSetBonuses().Count() == 2, "3세트: 공격 +10, 체력 +20 (장갑 +30 포함)");
        Ok(Near(t.counterChance, 0.10f) && Near(t.magicResist, 0.15f) && Near(t.cooldownReduction, 0.10f), "장갑·목걸이 확률 능력치");

        Ok(l.Unequip(RelicSlot.Necklace) == RelicDatabase.HeavenlyNecklace && l.ActiveSetBonuses().Count() == 1, "해제하면 세트 단계가 내려감");
        Ok(l.Unequip(RelicSlot.Necklace) == null, "빈 부위 해제는 null");

        var other = new Relic("test-sword", "시험용 검", "조선", RelicSlot.Weapon, new RelicStats { attack = 1 });
        l.Collect(other);
        Ok(l.Equip("test-sword") == RelicDatabase.BaekjeSword && !l.Has(RelicAbility.ComboAttack), "같은 부위 교체 시 이전 유물 반환·능력 해제");

        var greedy = new RelicStats { evasion = 0.9f, cooldownReduction = 0.9f, maxHp = -50 }.Clamped();
        Ok(Near(greedy.evasion, 0.6f) && Near(greedy.cooldownReduction, 0.4f) && greedy.maxHp == 1, "확률·감소율 상한, 체력 최소 1");

        var (owned, equipped) = l.Save();
        var restored = RelicLoadout.Load(owned.Concat(new[] { "removed-in-patch" }), equipped,
            id => id == "test-sword" ? other : RelicDatabase.Find(id));
        Ok(restored.Owned.Count == 4 && restored.Total(baseStats).attack == l.Total(baseStats).attack, "저장·복원 일치, 없어진 유물 id는 무시");

        Ok(changes > 0, "변경 이벤트 발생");
        Console.WriteLine(fails == 0 ? "ALL PASS" : $"FAILURES {fails}");
        return fails;
    }
}
