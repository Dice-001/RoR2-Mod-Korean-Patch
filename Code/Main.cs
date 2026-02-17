using BepInEx;
using System.IO;
using System.Collections.Generic;

[BepInPlugin("com.dice.ss2kopatch", "RoR2 Mod Korean Patch", "1.0.0")]
public class RoR2ModKoreanPatch : BaseUnityPlugin
{
    public void Awake()
    {
        CopyKoFilesToMods();
    }

    private void CopyKoFilesToMods()
    {
        //한글 파일 폴더 경로
        string baseLanguagePath = Path.Combine(Path.GetDirectoryName(Info.Location), "language");
        if (!Directory.Exists(baseLanguagePath))
        {
            Logger.LogWarning("language 폴더를 찾을 수 없습니다: " + baseLanguagePath);
            return;
        }

        string pluginsPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Info.Location), ".."));

        // 모드별 타겟 설정
        Dictionary<string, (string myFolder, string targetFolder, string fileExt)> modTargets =
            new Dictionary<string, (string, string, string)>()
        {// { "한패 대상 모드의 경로", ("한패 모드의 언어 폴더", "대상 모드의 언어 폴더 위치", "*.파일 확장명") }
            { "TeamMoonstorm-Starstorm2", ("Starstorm2", "languages/ko", "*.json") },
            { "Risky_Sleeps-ClassicItemsReturns/ClassicItemsReturns", ("RiskyClassicItems", "language/ko", "*.json") },
            { "William758-ZetAspects/ZetAspects", ("ZetAspects", "lang", "*.zetlang") },
            { "EnforcerGang-Enforcer", ("Enforcer", "Language/ko", "*.txt") },
            { "EnforcerGang-Rocket/EnforcerGang-Rocket", ("Rocket", "language/ko", "*.txt") },
            { "EnforcerGang-Pilot/EnforcerGang-Pilot", ("Pilot", "language/ko", "*.txt") },
            { "EnforcerGang-SniperClassic/SniperClassic", ("SniperClassic", "language/ko", "*.txt") },
            { "EnforcerGang-Direseeker/EnforcerGang-Direseeker", ("Direseeker", "language/ko", "*.txt") },
            { "EnforcerGang-HAND_OVERCLOCKED/HAND_Overclocked", ("HANDOVERCLOCKED", "language/ko", "*.txt") },
            { "Moffein-TeammateRevival/KosmosisDire-TeammateRevival/Localization", ("TeammateRevival", "Languages/ko", "*.json") },
            { "Wolfo-WLoopWeather/LoopWeather", ("WLoopWeather", "Languages/ko", "*.json") },
        };

        foreach (var mod in modTargets)
        {
            string modPath = Path.Combine(pluginsPath, mod.Key);
            if (!Directory.Exists(modPath))
            {
                Logger.LogWarning($"모드 폴더를 찾을 수 없습니다: {mod.Key}");
                continue;
            }

            string myKoPath = Path.Combine(baseLanguagePath, mod.Value.myFolder);
            if (!Directory.Exists(myKoPath))
            {
                Logger.LogWarning($"내 패치 KO 폴더가 없습니다: {myKoPath}");
                continue;
            }

            string targetKoPath = Path.Combine(modPath, mod.Value.targetFolder);
            Directory.CreateDirectory(targetKoPath);

            foreach (var file in Directory.GetFiles(myKoPath, mod.Value.fileExt))
            {
                string dest = Path.Combine(targetKoPath, Path.GetFileName(file));

                // 내용 비교 후 다를 때만 복사 (최적화 용도)
                if (File.Exists(dest))
                {
                    string srcContent = File.ReadAllText(file);
                    string destContent = File.ReadAllText(dest);

                    if (srcContent == destContent)
                    {
                        Logger.LogInfo($"[{mod.Key}] 동일 파일, 복사하지 않음: {Path.GetFileName(file)}");
                        continue;
                    }
                }

                File.Copy(file, dest, true);
                Logger.LogInfo($"[{mod.Key}] 복사/덮어쓰기 완료: {Path.GetFileName(file)}");
            }
        }

        Logger.LogInfo("모드별 파일 복사 완료");
    }
}
