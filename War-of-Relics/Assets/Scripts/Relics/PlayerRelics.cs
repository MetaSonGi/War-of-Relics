using UnityEngine;

namespace WarOfRelics
{
    /// <summary>
    /// 플레이어 오브젝트에 붙이는 유물 컴포넌트. 기본 능력치와 장착 상태를 들고 있고,
    /// 다른 스크립트(이동·전투)는 CurrentStats를 읽어 사용한다. 저장은 PlayerPrefs(JSON).
    /// </summary>
    public class PlayerRelics : MonoBehaviour
    {
        [Header("기본 능력치 (이방원)")]
        public RelicStats baseStats = new RelicStats { attack = 20, defense = 10, maxHp = 100 };

        [Tooltip("시작할 때 지급할 유물 id (테스트용)")]
        public string[] startingRelics = { "baekje-sword" };

        const string SaveKey = "war-of-relics.loadout";

        public RelicLoadout Loadout { get; private set; }
        public RelicStats CurrentStats { get; private set; }

        [System.Serializable] class SaveData { public string[] owned; public string[] equipped; }

        void Awake()
        {
            var json = PlayerPrefs.GetString(SaveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                var data = JsonUtility.FromJson<SaveData>(json);
                Loadout = RelicLoadout.Load(data.owned, data.equipped);
            }
            else
            {
                Loadout = new RelicLoadout();
                foreach (var id in startingRelics)
                {
                    var relic = RelicDatabase.Find(id);
                    if (relic != null && Loadout.Collect(relic)) Loadout.Equip(id);
                }
            }
            Loadout.Changed += Recalculate;
            Recalculate();
        }

        void OnDestroy()
        {
            if (Loadout != null) Loadout.Changed -= Recalculate;
        }

        void Recalculate()
        {
            CurrentStats = Loadout.Total(baseStats);
            Debug.Log($"[유물] {CurrentStats}");
        }

        /// <summary>던전에서 유물을 주웠을 때 호출</summary>
        public void PickUp(string relicId, bool autoEquip = true)
        {
            var relic = RelicDatabase.Find(relicId);
            if (relic == null) { Debug.LogWarning($"알 수 없는 유물: {relicId}"); return; }
            if (Loadout.Collect(relic) && autoEquip && !Loadout.Equipped.ContainsKey(relic.Slot)) Loadout.Equip(relicId);
        }

        public void Save()
        {
            var (owned, equipped) = Loadout.Save();
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(new SaveData { owned = owned, equipped = equipped }));
            PlayerPrefs.Save();
        }
    }
}
