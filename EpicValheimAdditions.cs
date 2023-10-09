using BepInEx;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace EpicValheimsAdditions
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("com.jotunn.jotunn", BepInDependency.DependencyFlags.HardDependency)]
    public class Core : BaseUnityPlugin
    {
        private const string ModName = "Epic Valheims Additions - by Huntard";
        private const string ModVersion = "2.0.0";
        private const string ModGUID = "Huntard.EpicValheimsAdditions";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        public static string configPath2 = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}_Content.json");
        private AssetBundle assetBundle;
        private Harmony _harmony;
        private List<string> sfxAssetNames = new List<string>();

        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);
            this.RegisterPrefabs();
            this.LoadContentConfig();
            this.CreateCraftingPieces();
            this.RegisterMiscItems();
            this.RegisterAllPrefabs();
            this.RegisterBossStuff();
            this.RegisterMistlands();
            this.RegisterDeepnorth();
            this.RegisterAshlands();
            this.RegisterDeepAbyss();
            this.AddCreatures();
            this.CreateIngots_Scales();
            this.AddVegetation();
            this.LoadConfig();
            ZoneManager.OnVanillaLocationsAvailable += AddLocations;
            ZoneManager.OnVanillaLocationsAvailable += ModDrops;
            ZoneManager.OnVanillaLocationsAvailable += ModCraftingStationLevels;
            PrefabManager.OnVanillaPrefabsAvailable += FixSfx;
        }

        private void RegisterPrefabs()
        {
            Jotunn.Logger.LogInfo("Loading...");
            assetBundle = AssetUtils.LoadAssetBundleFromResources("eva_assets", typeof(Core).Assembly);
            Jotunn.Logger.LogInfo("Loaded Prefabs");
        }
        public void Start()
        {
            UpgradeWorld.Upgrade.Register("EVA_upgrade_mistlands", "Adds Mistlands locations and vegetations of mod EVA.", "locations_add SvartalfrQueenAltar_New,Vegvisir_SvartalfrQueen", "vegetation_reset HeavymetalVein start");

            UpgradeWorld.Upgrade.Register("EVA_upgrade_deepnorth", "Adds DeepNorth locations and vegetations of mod EVA.", "locations_add JotunnAltar,Vegvisir_Jotunn", "vegetation_reset FrometalVein_frac,EVA_Rock1,EVA_Rock2,EVA_Rock3," +
                "EVA_Rock4,EVA_Rock5,EVA_Rock6,EVA_Rock7,EVA_Rock8,EVA_Rock9,EVA_Rock10,EVA_Rock11,EVA_Rock12,EVA_Rock13,EVA_Tree1,EVA_Tree2,EVA_Tree3,EVA_Bush1,EVA_Bush2,Pickable_Snowgil_Shroom start");

            UpgradeWorld.Upgrade.Register("EVA_upgrade_ashlands", "Adds Ashlands locations and vegetations of mod EVA.", "locations_add BlazingDamnedOneAltar,Vegvisir_BlazingDamnedOne", "vegetation_reset EVA_SutRock1,EVA_SutRock2,EVA_SutRock3," +
                "EVA_SutRock4,EVA_SutRock5,EVA_SutRock6,EVA_SutRock7,EVA_SutRock8,EVA_SutRock9,EVA_SutRock10,EVA_SutRock11,EVA_BurningTree1,EVA_BurningTree2,EVA_BurningTree3,EVA_BurningBush1,EVA_BurningBush2,EVA_BurningBush3,Pickable_Emberbloom start");

            UpgradeWorld.Upgrade.Register("EVA_upgrade", "Adds all locations and vegetations of mod EVA.", "locations_add SvartalfrQueenAltar_New,Vegvisir_SvartalfrQueen,JotunnAltar,Vegvisir_Jotunn,BlazingDamnedOneAltar,Vegvisir_BlazingDamnedOne start",
                "vegetation_reset HeavymetalVein,FrometalVein_frac,EVA_Rock1,EVA_Rock2,EVA_Rock3,EVA_Rock4,EVA_Rock5,EVA_Rock6,EVA_Rock7,EVA_Rock8,EVA_Rock9,EVA_Rock10,EVA_Rock11,EVA_Rock12,EVA_Rock13,EVA_Tree1,EVA_Tree2,EVA_Tree3,EVA_Bush1," +
                "EVA_Bush2,Pickable_Snowgil_Shroom,EVA_SutRock1,EVA_SutRock2,EVA_SutRock3,EVA_SutRock4,EVA_SutRock5,EVA_SutRock6,EVA_SutRock7,EVA_SutRock8,EVA_SutRock9,EVA_SutRock10,EVA_SutRock11,EVA_BurningTree1,EVA_BurningTree2," +
                "EVA_BurningTree3,EVA_BurningBush1,EVA_BurningBush2,EVA_BurningBush3,Pickable_Emberbloom start");
        }

        private void AddCreatures()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Creatures.DeepNorthCreatures)
                    {
                        CreateCreatures("CrystalhideUrsar", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth, 15f, 3, "Crystalhide Ursar");
                        CreateCreatures("CrystalhideUrsar_Cub", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth, 5f, 1, "Crystalhide Ursar Cub");
                        CreateCreatures("FrostwingFae", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth, 15f, 4, "Frostwing Fae");
                    }
                    else
                    {
                        LoadAndRegisterAsset("CrystalhideUrsar", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("CrystalhideUrsar_Cub", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("FrostwingFae", CustomAssetType.Prefab);
                        Jotunn.Logger.LogInfo("EVA Deep North creatures are turned off");
                    }
                    if (config.Creatures.AshlandsCreatures)
                    {
                        CreateCreatures("MagmaSkarab", Character.Faction.Demon, Heightmap.Biome.AshLands, 15f, 4, "Magma Skarab");
                        CreateCreatures("InfernalMinotaur", Character.Faction.Demon, Heightmap.Biome.AshLands, 15f, 3, "Infernal Minotaur");
                    }
                    else
                    {
                        LoadAndRegisterAsset("MagmaSkarab", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("InfernalMinotaur", CustomAssetType.Prefab);
                        Jotunn.Logger.LogInfo("EVA Ashlands creatures are turned off");
                    }
                    if (config.Creatures.OceanCreatures)
                    {
                        CreateCreatures("DraugrMariner", Character.Faction.SeaMonsters, Heightmap.Biome.Ocean, 15f, 3, "Draugr Mariner", "Killed_HelDemon");
                    }
                    else
                    {
                        LoadAndRegisterAsset("DraugrMariner", CustomAssetType.Prefab);
                        Jotunn.Logger.LogInfo("EVA Ocean creatures are turned off");
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for Deep North:{config.Creatures.DeepNorthCreatures}, Ashlands:{config.Creatures.AshlandsCreatures}, Ocean:{config.Creatures.OceanCreatures} failed. {e.Message} {e.StackTrace}");
                }
            }
            Jotunn.Logger.LogInfo("Loaded Creatures");
        }

        private void CreateCreatures(string creaturePrefabName, Character.Faction faction, Heightmap.Biome biome, float spawnChance, int maxSpawned, string spawnName, string key = "")
        {
            var customCreaturePrefab = assetBundle.LoadAsset<GameObject>(creaturePrefabName);

            var customCreatureConfig = new CreatureConfig();
            customCreatureConfig.Name = spawnName;
            customCreatureConfig.Faction = faction;

            if(creaturePrefabName == "DraugrMariner")
            {
                customCreatureConfig.AddSpawnConfig(new SpawnConfig
                {
                    Name = spawnName,
                    SpawnChance = spawnChance,
                    MaxSpawned = maxSpawned,
                    Biome = biome,
                    RequiredGlobalKey = key,
                    MaxAltitude = -5,
                    MinAltitude = -100,
                    SpawnDistance = 50,
                    GroupRadius = 3,
                    MinGroupSize = 1,
                    MaxGroupSize = 1,
                    MinTilt = 0,
                    MaxTilt = 35,
                    SpawnInForest = true,
                    SpawnOutsideForest = true,
                    BiomeArea = Heightmap.BiomeArea.Median,
                    SpawnInterval = 500,
                    GroundOffset = 0.5f,
                    SpawnAtDay = false,
                    SpawnAtNight = true
                });
            }
            else
            {
                customCreatureConfig.AddSpawnConfig(new SpawnConfig
                {
                    Name = spawnName,
                    SpawnChance = spawnChance,
                    MaxSpawned = maxSpawned,
                    Biome = biome,
                    RequiredGlobalKey = key
                });
            }
            

            CreatureManager.Instance.AddCreature(new CustomCreature(customCreaturePrefab, true, customCreatureConfig));
        }

        private void AddLocations()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Locations.MistlandsLocations)
                    {
                        AddCustomLocation("SvartalfrQueenAltar_New", Heightmap.Biome.Mistlands, 1000f, 4000f, 3, 15f, false, 1f, true, false, true);
                        AddCustomLocation("Vegvisir_SvartalfrQueen", Heightmap.Biome.Mistlands, 1000f, 1000f, 35, 12f, false, 1f, true, false, true);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands locations are turned off");
                    }

                    if (config.Locations.DeepNorthLocations)
                    {
                        AddCustomLocation("JotunnAltar", Heightmap.Biome.DeepNorth, 1000f, 4000f, 3, 20f, false, 1f, true, false, true);
                        AddCustomLocation("Vegvisir_Jotunn", Heightmap.Biome.DeepNorth, 1000f, 1000f, 15, 6f, false, 1f, true, false, true);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth locations are turned off");
                    }

                    if (config.Locations.AshlandsLocations)
                    {
                        AddCustomLocation("BlazingDamnedOneAltar", Heightmap.Biome.AshLands, 1000f, 4000f, 3, 15f, false, 1f, true, false, true);
                        AddCustomLocation("Vegvisir_BlazingDamnedOne", Heightmap.Biome.AshLands, 1000f, 1000f, 15, 4f, false, 1f, true, false, true);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands locations are turned off");
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for Mistlands:{config.Locations.MistlandsLocations}, DeepNorth:{config.Locations.DeepNorthLocations}, Ashlands:{config.Locations.AshlandsLocations} failed. {e.Message} {e.StackTrace}");
                }
            }

            AddCustomLocation("StaminaGreydwarf", Heightmap.Biome.Meadows, 1000f, 150f, 15, 4f, false, 1f, true, false, true);
            AddCustomLocation("StaminaTroll", Heightmap.Biome.BlackForest, 8500f, 150f, 15, 4f, false, 1f, true, false, true);
            AddCustomLocation("StaminaWraith", Heightmap.Biome.Swamp, 8500f, 150f, 15, 4f, false, 1f, true, false, true);

            Jotunn.Logger.LogInfo("Loaded Locations");
        }

        private void AddCustomLocation(string prefabName, Heightmap.Biome biome, float maxAltitude, float minDistanceFromSimilar, int quantity, float exteriorRadius, bool randomRotation, float minAltitude, bool clearArea, bool unique, bool priotized)
        {
            GameObject locationContainer = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>(prefabName));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(locationContainer, true, new LocationConfig
            {
                Biome = biome,
                MaxAltitude = maxAltitude,
                MinDistanceFromSimilar = minDistanceFromSimilar,
                Unique = unique,
                Quantity = quantity,
                Priotized = priotized,
                ExteriorRadius = exteriorRadius,
                RandomRotation = randomRotation,
                MinAltitude = minAltitude,
                ClearArea = clearArea
            }));
        }

        private void AddVegetation()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Locations.MistlandsLocations)
                    {
                        AddCustomVegetation("HeavymetalVein", Heightmap.Biome.Mistlands, 3f, 1f, 1, 2, 12f, false, 0f, 1000f, 50f, true, false);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands locations are turned off");
                    }

                    if (config.Locations.DeepNorthLocations)
                    {
                        AddCustomVegetation("FrometalVein_frac", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 1, 25f, true, 5f, 1000f, 20f, true, false);

                        AddCustomVegetation("EVA_Rock1", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock2", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock3", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 1, 100f, true, 5f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock4", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock5", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock6", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock7", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock8", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 1, 100f, true, 5f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock9", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock10", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock11", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock12", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Rock13", Heightmap.Biome.DeepNorth, 1f, 1f, 1, 1, 50f, true, 1f, 1000f, 20f, true, false);

                        AddCustomVegetation("EVA_Tree1", Heightmap.Biome.DeepNorth, 2f, 1f, 1, 3, 50f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Tree2", Heightmap.Biome.DeepNorth, 2f, 1f, 1, 3, 50f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Tree3", Heightmap.Biome.DeepNorth, 2f, 1f, 1, 3, 50f, true, 1f, 1000f, 20f, true, false);

                        AddCustomVegetation("EVA_Bush1", Heightmap.Biome.DeepNorth, 3f, 1f, 2, 3, 4f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_Bush2", Heightmap.Biome.DeepNorth, 3f, 1f, 2, 3, 4f, true, 1f, 1000f, 20f, true, false);

                        AddCustomVegetation("Pickable_Snowgil_Shroom", Heightmap.Biome.DeepNorth, 4f, 1f, 2, 2, 70f, true, 1f, 1000f, 20f, true, false);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth locations are turned off");
                    }

                    if (config.Locations.AshlandsLocations)
                    {
                        AddCustomVegetation("EVA_SutRock1", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock2", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock3", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock4", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock5", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock6", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock7", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock8", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock9", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock10", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 5f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_SutRock11", Heightmap.Biome.AshLands, 1f, 1f, 1, 1, 50f, true, 1f, 1000f, 20f, true, false);

                        AddCustomVegetation("EVA_BurningTree1", Heightmap.Biome.AshLands, 1f, 1f, 1, 1, 35f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_BurningTree2", Heightmap.Biome.AshLands, 1f, 1f, 1, 2, 15, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_BurningTree3", Heightmap.Biome.AshLands, 1f, 1f, 1, 1, 65f, true, 1f, 1000f, 20f, true, false);
                        sfxAssetNames.AddRange(new List<string> { "EVA_BurningTree1", "EVA_BurningTree2", "EVA_BurningTree3" });

                        AddCustomVegetation("EVA_BurningBush1", Heightmap.Biome.AshLands, 3f, 1f, 2, 3, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_BurningBush2", Heightmap.Biome.AshLands, 3f, 1f, 2, 3, 10f, true, 1f, 1000f, 20f, true, false);
                        AddCustomVegetation("EVA_BurningBush3", Heightmap.Biome.AshLands, 3f, 1f, 2, 3, 10f, true, 1f, 1000f, 20f, true, false);

                        AddCustomVegetation("Pickable_Emberbloom", Heightmap.Biome.AshLands, 3f, 1f, 2, 3, 70f, true, 1f, 1000f, 20f, true, false);
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands locations are turned off");
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for Mistlands:{config.Locations.MistlandsLocations}, DeepNorth:{config.Locations.DeepNorthLocations}, Ashlands:{config.Locations.AshlandsLocations} failed. {e.Message} {e.StackTrace}");
                }
            }

            Jotunn.Logger.LogInfo("Loaded Vegetation");
        }

        private void AddCustomVegetation(string prefabName, Heightmap.Biome biome, float max, float min, int groupSizeMin, int groupSizeMax, float groupRadius, bool blockCheck, float minAltitude, float maxAltitude, float maxTilt, bool unique, bool priotized)
        {
            GameObject vegetationPrefab = assetBundle.LoadAsset<GameObject>(prefabName);
            CustomVegetation customVegetation = new CustomVegetation(vegetationPrefab, true, new VegetationConfig
            {
                Max = max,
                Min = min,
                GroupSizeMin = groupSizeMin,
                GroupSizeMax = groupSizeMax,
                GroupRadius = groupRadius,
                BlockCheck = blockCheck,
                Biome = biome,
                MinAltitude = minAltitude,
                MaxAltitude = maxAltitude,
                MaxTilt = maxTilt
            });
            ZoneManager.Instance.AddCustomVegetation(customVegetation);
        }

        private void ModDrops()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if(config.Locations.MistlandsLocations == false)
                    {
                        ItemDrop prefabpick = PrefabManager.Cache.GetPrefab<ItemDrop>("PickaxeBlackMetal");
                        prefabpick.m_itemData.m_shared.m_toolTier = 5;
                    }

                    if (config.Content.DeepNorth == true)
                    {
                        MineRock5 prefab2b = PrefabManager.Cache.GetPrefab<MineRock5>("ice_rock1_frac");
                        var item2 = ObjectDB.instance.GetItemPrefab("PrimordialIce");
                        var IceRockDrop = new List<DropTable.DropData> {
                        new DropTable.DropData {
                            m_item = item2,
                            m_stackMax = 2,
                            m_stackMin = 1,
                            m_weight = 2
                        }
                    };
                        prefab2b.m_dropItems.m_drops = IceRockDrop;
                        prefab2b.m_damageModifiers.m_blunt = HitData.DamageModifier.Normal;
                        prefab2b.m_damageModifiers.m_slash = HitData.DamageModifier.Immune;
                        prefab2b.m_damageModifiers.m_pierce = HitData.DamageModifier.Immune;
                        prefab2b.m_damageModifiers.m_lightning = HitData.DamageModifier.Normal;
                        prefab2b.m_minToolTier = 5;
                        prefab2b.m_dropItems.m_dropMin = 1;
                        prefab2b.m_dropItems.m_dropMax = 2;
                        prefab2b.m_dropItems.m_dropChance = 0.35f;
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Content.Mistlands},{config.Content.DeepNorth} failed. {e.Message} {e.StackTrace}");
                }
            }
        }

        private void CreateCraftingPieces()
        {
            LoadAndRegisterAsset("piece_alchemystation", CustomAssetType.Piece, pieceConfig: new PieceConfig
            {
                PieceTable = "_HammerPieceTable",
                CraftingStation = "piece_workbench",
                AllowedInDungeons = false,
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "FineWood",
                        Amount = 15,
                        Recover = true
                    },
                    new RequirementConfig
                    {
                        Item = "Coal",
                        Amount = 10,
                        Recover = true
                    },
                    new RequirementConfig
                    {
                        Item = "Bronze",
                        Amount = 5,
                        Recover = true
                    }
                }
            });

            LoadAndRegisterAsset("piece_thorsforge", CustomAssetType.Piece, pieceConfig: new PieceConfig
            {
                PieceTable = "_HammerPieceTable",
                AllowedInDungeons = false,
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "YggdrasilWood",
                        Amount = 20,
                        Recover = true
                    },
                    new RequirementConfig
                    {
                        Item = "Thunderstone",
                        Amount = 10,
                        Recover = true
                    },
                    new RequirementConfig
                    {
                        Item = "Tar",
                        Amount = 5,
                        Recover = true
                    },
                    new RequirementConfig
                    {
                        Item = "YagluthDrop",
                        Amount = 1,
                        Recover = true
                    }
                }
            });

            Jotunn.Logger.LogInfo("Loaded CraftingPieces");
        }

        private void ModCraftingStationLevel(string craftingStationPrefabName)
        {
            StationExtension[] stationExtensions = new StationExtension[6];
            string[] extensionNames = new string[] { "forge_ext1", "forge_ext2", "forge_ext3", "forge_ext4", "forge_ext5", "forge_ext6" };

            for (int i = 0; i < extensionNames.Length; i++)
            {
                StationExtension station = PrefabManager.Cache.GetPrefab<GameObject>(extensionNames[i]).AddComponent<StationExtension>();
                station.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(craftingStationPrefabName);
                station.m_maxStationDistance = 50;
                station.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
                stationExtensions[i] = station;
            }

            Jotunn.Logger.LogInfo($"Updated {craftingStationPrefabName} level");
        }

        private void ModCraftingStationLevels()
        {
            ModCraftingStationLevel("piece_thorsforge");
            ModCraftingStationLevel("piece_alchemystation");
        }

        private void CreateIngots_Scales()
        {
            LoadAndRegisterAsset("Heavyscale", CustomAssetType.Item);
            LoadAndRegisterAsset("Drakescale", CustomAssetType.Item);
            LoadAndRegisterAsset("Forgedscale", CustomAssetType.Item);
            LoadAndRegisterAsset("OreHeavymetal", CustomAssetType.Item);
            LoadAndRegisterAsset("HeavymetalBar", CustomAssetType.Item);
            LoadAndRegisterAsset("OreFrometal", CustomAssetType.Item);
            LoadAndRegisterAsset("FrometalBar", CustomAssetType.Item);
            LoadAndRegisterAsset("NjordsTearstone", CustomAssetType.Item);
            LoadAndRegisterAsset("DeepAbyssEssence", CustomAssetType.Item);

            Jotunn.Logger.LogInfo("Loaded Ingots/scales/ore");
        }

        private void RegisterBossStuff()
        {
            LoadAndRegisterAsset("Golden_Greydwarf_Miniboss", CustomAssetType.Prefab);
            LoadAndRegisterAsset("Golden_Troll_Miniboss", CustomAssetType.Prefab);
            LoadAndRegisterAsset("Golden_Wraith_Miniboss", CustomAssetType.Prefab);
            LoadAndRegisterAsset("Jotunn", CustomAssetType.Prefab);

            LoadAndRegisterAsset("Jotunn_Groundslam", CustomAssetType.Item);
            LoadAndRegisterAsset("Jotunn_Groundslam2", CustomAssetType.Item);
            LoadAndRegisterAsset("Jotunn_Shoot", CustomAssetType.Item);

            LoadAndRegisterAsset("HelDemon", CustomAssetType.Prefab);

            LoadAndRegisterAsset("BlazingMace", CustomAssetType.Item);
            LoadAndRegisterAsset("BlazingMace2", CustomAssetType.Item);
            LoadAndRegisterAsset("BlazingMace3", CustomAssetType.Item);
            LoadAndRegisterAsset("BlazingMace4", CustomAssetType.Item);
            LoadAndRegisterAsset("BlazingShield", CustomAssetType.Item);
            LoadAndRegisterAsset("EVA_BlazingHelm", CustomAssetType.Item);
            LoadAndRegisterAsset("EVA_BlazingChest", CustomAssetType.Item);
            LoadAndRegisterAsset("EVA_BlazingBoots", CustomAssetType.Item);
            LoadAndRegisterAsset("Blazing_Nova", CustomAssetType.Item);
            LoadAndRegisterAsset("Blazing_Meteors", CustomAssetType.Item);
            LoadAndRegisterAsset("Blazing_Shoot", CustomAssetType.Item);

            LoadAndRegisterAsset("SvartalfarQueen", CustomAssetType.Prefab);
            LoadAndRegisterAsset("SvarTentaRoot", CustomAssetType.Prefab);

            LoadAndRegisterAsset("SvartalfarQueenGreatSword", CustomAssetType.Item);
            LoadAndRegisterAsset("SvartalfarQueenBow", CustomAssetType.Item);
            LoadAndRegisterAsset("SvartalfarQueenBowArrowStorm", CustomAssetType.Item);
            LoadAndRegisterAsset("SvartalfarQueen_rootspawn", CustomAssetType.Item);

            LoadAndRegisterAsset("TrophyHelDemon", CustomAssetType.Item);
            LoadAndRegisterAsset("TrophySvartalfarQueen", CustomAssetType.Item);
            LoadAndRegisterAsset("TrophyJotunn", CustomAssetType.Item);

            LoadAndRegisterAsset("HeavymetalPickaxeHead", CustomAssetType.Item);
            LoadAndRegisterAsset("HeavymetalAxeHead", CustomAssetType.Item);
            LoadAndRegisterAsset("FrometalPickaxeHead", CustomAssetType.Item);
            LoadAndRegisterAsset("FrometalAxeHead", CustomAssetType.Item);
            LoadAndRegisterAsset("FlametalPickaxeHead", CustomAssetType.Item);
            LoadAndRegisterAsset("FlametalAxeHead", CustomAssetType.Item);

            Jotunn.Logger.LogInfo("Loaded BossStuff");
        }

        private void RegisterMiscItems()
        {
            LoadAndRegisterAsset("EVA_AdminFood", CustomAssetType.Item);

            Jotunn.Logger.LogInfo("Loaded MiscItems");
        }

        private void RegisterAllPrefabs()
        {
            List<string> Prefabnames = new List<string>
            {
                "Burning_Log:sfx",
                "Burning_Log_Half:sfx",
                "BurningStump:sfx",
                "FrozenStump",
                "Frozen_Log",
                "Frozen_Log_Half",
                "sfx_magic_ice_explosion",
                "Bear_Attack1",
                "Bear_Attack2",
                "Bear_Attack3",
                "Bear_Attack5",
                "Fairy_Attack1",
                "Fairy_Attack_Projectile",
                "Mariner_Attack1",
                "Mariner_Attack2",
                "Minotaur_Attack1",
                "Minotaur_Attack2",
                "Minotaur_Attack3",
                "Minotaur_Attack4_kick",
                "Skarab_Attack1",
                "Skarab_Attack2",
                "Skarab_Attack3",
                "Blazing_ragdoll",
                "FrostBear_Ragdoll",
                "Greydwarf_elite_ragdoll_gold",
                "Jotunn_ragdoll",
                "Minotaur_Ragdoll",
                "Skarab_Ragdoll",
                "SvartalfarQueen_Ragdoll",
                "Troll_ragdoll_gold",
                "vfx_burningtreecut",
                "vfx_burningtreecut_dead",
                "vfx_offering_ashlands",
                "lightningAOE:sfx",
                "sfx_death_Skarab",
                "sfx_minotaur_alerted",
                "sfx_minotaur_attack_hit",
                "vfx_generic_death 2",
                "sfx_minotaur_death",
                "vfx_fairy_hit",
                "sfx_death_fairy",
                "vfx_fairy_death",
                "sfx_fairy_idle",
                "sfx_fairy_alerted",
                "sfx_fairy_attack_launch",
                "vfx_FrostfireballHit",
                "sfx_bear_hit",
                "vfx_generic_death 1",
                "sfx_bear_death",
                "sfx_bear_alerted",
                "sfx_bear_idle",
                "vfx_mariner_death",
                "sfx_mariner_death",
                "sfx_Mariner_Alerted_Idle",
                "bow_projectile_svar",
                "spawn_arrows_svartalfr",
                "sfx_death_gold",
                "sfx_attack_hit_gold",
                "Greydwarf_elite_attack_gold",
                "troll_punch",
                "troll_throw",
                "troll_groundslam",
                "wraith_melee_gold",
                "vfx_frozentreecut",
                "bow_projectile_frometal",
                "bow_projectile_flametal",
                "bow_projectile_abyss",
                "frometalspear_projectile",
                "flametalspear_projectile",
                "heavymetalspear_projectile",
                "Trident_projectile",
                "fx_bear_pet",
                "sfx_bear_love",
                "Bear_Attack_cub",
                "FrostBear_Ragdoll_Cub",
                "vfx_ice_destroyed_fro",
                "lightningAOE_Abyss",
                "vfx_wraith_death_gold"
            };

            foreach (string Prefabname in Prefabnames)
            {
                LoadAndRegisterAsset(Prefabname, CustomAssetType.Prefab);
            }
        }

        private void FixSfx()
        {
            foreach (string assetName in sfxAssetNames)
            {
                AudioSource mixerGroupSFX = PrefabManager.Cache.GetPrefab<AudioSource>("sfx_arrow_hit");
                GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>(assetName);
                prefab.GetComponentInChildren<AudioSource>().outputAudioMixerGroup = mixerGroupSFX.outputAudioMixerGroup;
            }
        }

        private void RegisterMistlands()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Content.Mistlands == true)
                    {
                        CustomItemConversion OreHeavymetal = new CustomItemConversion(new SmelterConversionConfig
                        {
                            Station = "blastfurnace",
                            FromItem = "OreHeavymetal",
                            ToItem = "HeavymetalBar"
                        });
                        ItemManager.Instance.AddItemConversion(OreHeavymetal);

                        LoadAndRegisterAsset("SvartalfrQueenAltar_New", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("Vegvisir_SvartalfrQueen", CustomAssetType.Prefab);

                        LoadAndRegisterAsset("CursedEffigy", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_alchemystation",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 20
                                },
                                new RequirementConfig
                                {
                                    Item = "AncientSeed",
                                    Amount = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 30
                                }
                            }
                        });

                        LoadAndRegisterAsset("BowHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 14,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 25,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 14,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("AtgeirHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 14,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 24,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 14,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("SledgeHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 14,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 28,
                                    AmountPerLevel = 6
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 14,
                                    AmountPerLevel = 5
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("BattleaxeHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 16,
                                    AmountPerLevel = 6
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 30,
                                    AmountPerLevel = 7
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 12,
                                    AmountPerLevel = 6
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("SpearHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 8,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 16,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("KnifeHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 12,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("MaceHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 10,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 20,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 10,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("GreatSwordHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 30,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 30,
                                    AmountPerLevel = 15
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 15,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("SwordHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 10,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 15,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 12,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("ShieldHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 10,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 18,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 10,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("ShieldHeavymetalTower", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 20,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 25,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 15,
                                    AmountPerLevel = 8
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("AxeHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 5,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 15,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "HeavymetalAxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        LoadAndRegisterAsset("PickaxeHeavymetal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "HeavymetalBar",
                                    Amount = 5,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "LinenThread",
                                    Amount = 15,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "HeavymetalPickaxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 200;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        });

                        Jotunn.Logger.LogInfo("Loaded EVA Mistlands content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Content.Mistlands} failed. {e.Message} {e.StackTrace}");
                }
            }


        }

        private void RegisterDeepnorth()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Content.DeepNorth == true)
                    {
                        CustomItemConversion OreFrometal = new CustomItemConversion(new SmelterConversionConfig
                        {
                            Station = "blastfurnace",
                            FromItem = "OreFrometal",
                            ToItem = "FrometalBar"
                        });
                        ItemManager.Instance.AddItemConversion(OreFrometal);

                        LoadAndRegisterAsset("JotunnAltar", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("Vegvisir_Jotunn", CustomAssetType.Prefab);

                        LoadAndRegisterAsset("PrimordialIce", CustomAssetType.Item);
                        LoadAndRegisterAsset("YmirsSoulEssence", CustomAssetType.Item);
                        LoadAndRegisterAsset("UsarPelt", CustomAssetType.Item);
                        LoadAndRegisterAsset("FaeDust", CustomAssetType.Item);
                        LoadAndRegisterAsset("BearMeat", CustomAssetType.Item); 
                        LoadAndRegisterAsset("SnowgillShroom", CustomAssetType.Item); 

                        LoadAndRegisterAsset("ArrowFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 20,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Feathers",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Wood",
                                    Amount = 8
                                }
                            }
                        });
                        // Mage
                        LoadAndRegisterAsset("YggdrasilElixir", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FaeDust",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "SnowgillShroom",
                                    Amount = 6
                                }
                            }
                        }); 
                        // Health
                        LoadAndRegisterAsset("GjallarhornGumbo", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FaeDust",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BearMeat",
                                    Amount = 4
                                }
                            }
                        }); 
                        // Stam
                        LoadAndRegisterAsset("FrostbiteFuel", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FaeDust",
                                    Amount = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "BearMeat",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "SnowgillShroom",
                                    Amount = 2
                                }
                            }
                        });

                        LoadAndRegisterAsset("BowFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 15,
                                    AmountPerLevel = 7
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 2,
                                    AmountPerLevel = 1
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("AtgeirFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 15,
                                    AmountPerLevel = 7
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("SledgeFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 15,
                                    AmountPerLevel = 7
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 13,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("BattleaxeFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 15,
                                    AmountPerLevel = 7
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 10,
                                    AmountPerLevel = 6
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("SpearFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("KnifeFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 5,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("MaceFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("GreatSwordFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 30,
                                    AmountPerLevel = 15
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 15,
                                    AmountPerLevel = 8
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 10,
                                    AmountPerLevel = 6
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("SwordFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "YggdrasilWood",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("ShieldFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_name = "Frometal Shield";
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("ShieldFrometalTower", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 12,
                                    AmountPerLevel = 6
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 13,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_variants = 0;
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("AxeFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 5,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 5,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 2,
                                    AmountPerLevel = 1
                                },
                                new RequirementConfig
                                {
                                    Item = "FrometalAxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        LoadAndRegisterAsset("PickaxeFrometal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "FrometalBar",
                                    Amount = 5,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "PrimordialIce",
                                    Amount = 5,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "UsarPelt",
                                    Amount = 2,
                                    AmountPerLevel = 1
                                },
                                new RequirementConfig
                                {
                                    Item = "FrometalPickaxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 250;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        });

                        Jotunn.Logger.LogInfo("Loaded EVA DeepNorth content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Content.DeepNorth} failed. {e.Message} {e.StackTrace}");
                }
            }

        }

        private void RegisterAshlands()
        {
            var contentConfigs = GetJson<EVAConfiguration>(configPath2);

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Content.Ashlands == true)
                    {

                        LoadAndRegisterAsset("BlazingDamnedOneAltar", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("Vegvisir_BlazingDamnedOne", CustomAssetType.Prefab);
                        LoadAndRegisterAsset("BurningWorldTreeFragment", CustomAssetType.Item);
                        LoadAndRegisterAsset("MagmaGuck", CustomAssetType.Item);
                        LoadAndRegisterAsset("AshenHide", CustomAssetType.Item);
                        LoadAndRegisterAsset("MinotaurMeat", CustomAssetType.Item); 
                        LoadAndRegisterAsset("Emberbloom", CustomAssetType.Item);
                        LoadAndRegisterAsset("EmberbloomSeeds", CustomAssetType.Item);
                        LoadAndRegisterAsset("FenrirsHeart", CustomAssetType.Item);

                        LoadAndRegisterAsset("Sapling_Emberbloom", CustomAssetType.Piece, pieceConfig: new PieceConfig
                        {
                            PieceTable = "_CultivatorPieceTable",
                            AllowedInDungeons = false,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "EmberbloomSeeds",
                                    Amount = 1,
                                    Recover = false
                                }
                            }
                        });
                        LoadAndRegisterAsset("ArrowFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 20,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Feathers",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Wood",
                                    Amount = 8
                                }
                            }
                        });
                        // Mage 
                        LoadAndRegisterAsset("MoltenMagesMunch", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "MagmaGuck",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Emberbloom",
                                    Amount = 4
                                }
                            }
                        });
                        // Health
                        LoadAndRegisterAsset("AshenTitansFeast", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "MagmaGuck",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "MinotaurMeat",
                                    Amount = 4
                                }
                            }
                        });
                        // Stamina 
                        LoadAndRegisterAsset("EruptionElixir", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_cauldron",
                            MinStationLevel = 5,
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "MagmaGuck",
                                    Amount = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "MinotaurMeat",
                                    Amount = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "Emberbloom",
                                    Amount = 1
                                }
                            }
                        });


                        LoadAndRegisterAsset("BowFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("AtgeirFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("SledgeFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("BattleaxeFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 20,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 10,
                                    AmountPerLevel = 6
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("SpearFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("KnifeFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 8,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("MaceFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("GreatSwordFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 30,
                                    AmountPerLevel = 15
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 10,
                                    AmountPerLevel = 6
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("SwordFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("ShieldFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 6,
                                    AmountPerLevel = 3
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_name = "Flametal Shield";
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("ShieldFlametalTower", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 12,
                                    AmountPerLevel = 4
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 10,
                                    AmountPerLevel = 5
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 8,
                                    AmountPerLevel = 4
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_variants = 0;
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("AxeFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 5,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "FlametalAxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        LoadAndRegisterAsset("PickaxeFlametal", CustomAssetType.Item, new ItemConfig
                        {
                            Amount = 1,
                            CraftingStation = "piece_thorsforge",
                            Requirements = new RequirementConfig[]
                            {
                                new RequirementConfig
                                {
                                    Item = "Flametal",
                                    Amount = 5,
                                    AmountPerLevel = 10
                                },
                                new RequirementConfig
                                {
                                    Item = "AshenHide",
                                    Amount = 3,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "BurningWorldTreeFragment",
                                    Amount = 4,
                                    AmountPerLevel = 2
                                },
                                new RequirementConfig
                                {
                                    Item = "FlametalPickaxeHead",
                                    Amount = 1
                                }
                            }
                        },
                        (itemDrop) =>
                        {
                            itemDrop.m_itemData.m_shared.m_maxDurability = 300;
                            itemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        });

                        Jotunn.Logger.LogInfo("Loaded EVA Ashlands content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Content.Ashlands} failed. {e.Message} {e.StackTrace}");
                }
            }


        }

        private void RegisterDeepAbyss()
        {
            CustomItemConversion njordsTearstone = new CustomItemConversion(new SmelterConversionConfig
            {
                Station = "blastfurnace",
                FromItem = "NjordsTearstone",
                ToItem = "DeepAbyssEssence"
            });
            ItemManager.Instance.AddItemConversion(njordsTearstone);

            LoadAndRegisterAsset("ArrowAbyss", CustomAssetType.Item, new ItemConfig
            {
                Amount = 20,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "DeepAbyssEssence",
                        Amount = 2
                    },
                    new RequirementConfig
                    {
                        Item = "Feathers",
                        Amount = 2
                    },
                    new RequirementConfig
                    {
                        Item = "Wood",
                        Amount = 8
                    }
                }
            });

            LoadAndRegisterAsset("TridentDeepAbyss", CustomAssetType.Item, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "DeepAbyssEssence",
                        Amount = 20,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "YggdrasilWood",
                        Amount = 8,
                        AmountPerLevel = 4
                    }
                }
            });

            LoadAndRegisterAsset("BowDeepAbyss", CustomAssetType.Item, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "DeepAbyssEssence",
                        Amount = 20,
                        AmountPerLevel = 10
                    },
                    new RequirementConfig
                    {
                        Item = "YggdrasilWood",
                        Amount = 8,
                        AmountPerLevel = 4
                    }
                }
            });

            LoadAndRegisterAsset("GreatSwordDeepAbyss", CustomAssetType.Item, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Item = "DeepAbyssEssence",
                        Amount = 60,
                        AmountPerLevel = 40
                    },
                    new RequirementConfig
                    {
                        Item = "YggdrasilWood",
                        Amount = 40,
                        AmountPerLevel = 20
                    }
                }
            });


            Jotunn.Logger.LogInfo("Loaded DeepAbyssWeapons");
        }

        private void LoadAndRegisterAsset(string assetName, CustomAssetType assetType, ItemConfig itemConfig = null, Action<ItemDrop> customizeItemAction = null, PieceConfig pieceConfig = null)
        {
            if (assetName.Contains("sfx"))
            {
                if (assetName.EndsWith(":sfx"))
                {
                    assetName = assetName.Substring(0, assetName.Length - ":sfx".Length);
                }
                sfxAssetNames.Add(assetName);
            }

            GameObject asset = assetBundle.LoadAsset<GameObject>(assetName);
            switch (assetType)
            {
                case CustomAssetType.Prefab:
                    CustomPrefab customPrefab = new CustomPrefab(asset, true);
                    PrefabManager.Instance.AddPrefab(customPrefab);
                    break;
                case CustomAssetType.Item:
                    CustomItem customItem;
                    if (itemConfig != null)
                    {
                        customItem = new CustomItem(asset, true, itemConfig);
                    }
                    else
                    {
                        customItem = new CustomItem(asset, true);
                    }
                    ItemManager.Instance.AddItem(customItem);

                    if (customizeItemAction != null)
                    {
                        GameObject itemGameObject = customItem.ItemPrefab;
                        ItemDrop itemDropComponent = itemGameObject.GetComponent<ItemDrop>();
                        if (itemDropComponent != null)
                        {
                            customizeItemAction(itemDropComponent);
                        }
                    }
                    break;
                case CustomAssetType.Piece:
                    if (pieceConfig != null)
                    {
                        CustomPiece customPiece = new CustomPiece(asset, true, pieceConfig);
                        PieceManager.Instance.AddPiece(customPiece);
                    }
                    break;
                // Add more cases for future custom asset types
                // case CustomAssetType.OtherType:
                //     // Handle other custom asset type registration here
                //     break;
                default:
                    break;
            }
        }

        public enum CustomAssetType
        {
            Prefab,
            Item,
            Piece
            // Add more custom asset types here for future additions
            // OtherType,
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                GenerateConfigFile();
                Jotunn.Logger.LogInfo("Generated new configs");
                return;
            }
            RegisterConfigValues();

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                GenerateConfigFile();

                Jotunn.Logger.LogInfo("Updated configs");
            }
        }

        private void GenerateConfigFile()
        {
            var bossConfigs = new List<BossConfig>();

            // Define boss data in a dictionary
            var bossData = new Dictionary<string, List<string>>
            {
                { "SvartalfarQueen", new List<string> { "SvartalfarQueenGreatSword", "SvartalfarQueenBow", "SvartalfarQueen_rootspawn", "SvarTentaRoot_attack", "SvartalfarQueenBowArrowStorm", "bow_projectile_svar" } },
                { "Jotunn", new List<string> { "Jotunn_Groundslam", "Jotunn_Groundslam2", "Jotunn_Shoot" } },
                { "HelDemon", new List<string> { "BlazingMace", "BlazingMace2", "BlazingMace3", "BlazingMace4", "Blazing_Nova", "Blazing_Shoot", "Blazing_Meteors", "projectile_meteor_blazing" } },
                { "Golden_Greydwarf_Miniboss", new List<string> { "Greydwarf_elite_attack_gold" } },
                { "Golden_Troll_Miniboss", new List<string> { "troll_punch", "troll_groundslam", "troll_throw" } },
                { "Golden_Wraith_Miniboss", new List<string> { "wraith_melee_gold" } },
                { "FrostwingFae", new List<string> { "Fairy_Attack1", "Fairy_Attack_Projectile" } },
                { "CrystalhideUrsar", new List<string> { "Bear_Attack1", "Bear_Attack2", "Bear_Attack3", "Bear_Attack5" } },
                { "CrystalhideUrsar_Cub", new List<string> { "Bear_Attack_cub" } },
                { "InfernalMinotaur", new List<string> { "Minotaur_Attack1", "Minotaur_Attack2", "Minotaur_Attack3", "Minotaur_Attack4_kick" } },
                { "MagmaSkarab", new List<string> { "Skarab_Attack1", "Skarab_Attack2", "Skarab_Attack3" } },
                { "DraugrMariner", new List<string> { "Mariner_Attack1", "Mariner_Attack2" } }
            };

            foreach (var bossEntry in bossData)
            {
                var bossName = bossEntry.Key;
                var bossConfig = new BossConfig();
                var bossPrefab = PrefabManager.Cache.GetPrefab<Humanoid>(bossName);
                bossConfig.BossPrefabName = bossName;
                bossConfig.Health = (int)bossPrefab.m_health;

                bossConfig.Attacks = new List<CustomAttack>();
                AddCustomAttacks(bossConfig, bossEntry.Value);

                bossConfigs.Add(bossConfig);
            }

            var jsonText = JsonMapper.ToJson(bossConfigs);
            File.WriteAllText(configPath, jsonText);
        }

        private void AddCustomAttacks(BossConfig config, List<string> attackNames)
        {
            foreach (var attackName in attackNames)
            {
                if (attackName.ToLower().Contains("projectile"))
                {
                    var attackPrefab = PrefabManager.Cache.GetPrefab<Projectile>(attackName);
                    var customAttack = new CustomAttack
                    {
                        AttackName = attackName,
                        Damages = attackPrefab.m_damage
                    };
                    config.Attacks.Add(customAttack);
                }
                else
                {
                    var attackPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>(attackName);

                    if (attackPrefab != null)
                    {
                        var customAttack = new CustomAttack
                        {
                            AttackName = attackName,
                            AttackAnimation = attackPrefab.m_itemData.m_shared.m_attack.m_attackAnimation,
                            AiAttackRange = attackPrefab.m_itemData.m_shared.m_aiAttackRange,
                            AiAttackRangeMin = attackPrefab.m_itemData.m_shared.m_aiAttackRangeMin,
                            AiAttackInterval = attackPrefab.m_itemData.m_shared.m_aiAttackInterval,
                            Damages = attackPrefab.m_itemData.m_shared.m_damages
                        };

                        config.Attacks.Add(customAttack);
                    }
                    else
                    {
                        Jotunn.Logger.LogError($"Loading config for {attackName} failed. This attack was either deleted or renamed.");
                    }
                }
            }
        }

        private void RegisterConfigValues()
        {
            var bossConfigs = GetJson<BossConfig>(configPath);

            foreach (var config in bossConfigs)
            {
                UpdateBossHealth(config);
                UpdateBossAttacks(config);
            }

            Jotunn.Logger.LogInfo("Loaded configs");
        }

        private void UpdateBossHealth(BossConfig config)
        {
            try
            {
                var bossPrefab = PrefabManager.Cache.GetPrefab<Humanoid>(config.BossPrefabName);
                bossPrefab.m_health = config.Health;
            }
            catch (Exception e)
            {
                Jotunn.Logger.LogError($"Loading config for {config.BossPrefabName} failed. {e.Message} {e.StackTrace}");
            }
        }

        private void UpdateBossAttacks(BossConfig config)
        {
            foreach (var attack in config.Attacks)
            {
                try
                {
                    if (attack.AttackName.ToLower().Contains("projectile"))
                    {
                        var projectile = PrefabManager.Cache.GetPrefab<Projectile>(attack.AttackName);
                        projectile.m_damage = attack.Damages.Value;
                    }
                    else
                    {
                        var itemDrop = PrefabManager.Cache.GetPrefab<ItemDrop>(attack.AttackName);
                        if (attack.Damages != null)
                        {
                            itemDrop.m_itemData.m_shared.m_damages = attack.Damages.Value;
                        }
                        if (attack.AttackAnimation != null)
                        {
                            itemDrop.m_itemData.m_shared.m_attack.m_attackAnimation = attack.AttackAnimation;
                        }
                        itemDrop.m_itemData.m_shared.m_aiAttackRange = attack.AiAttackRange;
                        itemDrop.m_itemData.m_shared.m_aiAttackRangeMin = attack.AiAttackRangeMin;
                        itemDrop.m_itemData.m_shared.m_aiAttackInterval = attack.AiAttackInterval;
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {attack.AttackName} failed. This attack was either deleted or renamed. {e.Message} {e.StackTrace}");
                }
            }
        }

        private bool IsExpectedFormat(OldConfiguration oldConfig)
        {
            return oldConfig != null &&
                   oldConfig.Description != null;
        }

        private void LoadContentConfig()
        {
            EVAConfiguration contentconfig = GetDefaultEVAConfiguration();

            if (!File.Exists(configPath2))
            {
                Jotunn.Logger.LogInfo("Generated Content Configuration");
                SaveEVAConfiguration(contentconfig);
                return;
            }

            var jsonText = File.ReadAllText(configPath2);
            var oldConfigs = JsonMapper.ToObject<List<OldConfiguration>>(jsonText);

            if (oldConfigs == null || oldConfigs.Count == 0)
            {
                Jotunn.Logger.LogInfo("Generated Content Configuration");
                SaveEVAConfiguration(contentconfig);
                return;
            }

            var oldConfig = oldConfigs[0];
            if (!IsExpectedFormat(oldConfig))
            {
                Jotunn.Logger.LogInfo("Loaded Content Configuration");
                return;
            }

            contentconfig = new EVAConfiguration
            {
                Content = new ContentConfiguration
                {
                    Mistlands = oldConfig.Mistlands,
                    DeepNorth = oldConfig.DeepNorth,
                    Ashlands = oldConfig.Ashlands
                },
                Locations = new LocationConfiguration
                {
                    MistlandsLocations = oldConfig.MistlandsLocations,
                    DeepNorthLocations = oldConfig.DeepNorthLocations,
                    AshlandsLocations = oldConfig.AshlandsLocations
                },
                Creatures = new CreatureConfiguration
                {
                    DeepNorthCreatures = true,
                    AshlandsCreatures = true,
                    OceanCreatures = true
                },
                Content_Description = contentDescription
            };

            Jotunn.Logger.LogInfo("Updated old Content Configuration");
            SaveEVAConfiguration(contentconfig);
        }

        private void SaveEVAConfiguration(EVAConfiguration config)
        {
            var evaconfig = new List<EVAConfiguration>
            {
                config
            };
            var jsonText = JsonMapper.ToJson(evaconfig);
            File.WriteAllText(configPath2, jsonText);
        }

        private EVAConfiguration GetDefaultEVAConfiguration()
        {
            return new EVAConfiguration
            {
                Content = new ContentConfiguration
                {
                    Mistlands = false,
                    DeepNorth = true,
                    Ashlands = true
                },
                Locations = new LocationConfiguration
                {
                    MistlandsLocations = false,
                    DeepNorthLocations = true,
                    AshlandsLocations = true
                },
                Creatures = new CreatureConfiguration
                {
                    DeepNorthCreatures = true,
                    AshlandsCreatures = true,
                    OceanCreatures = true
                },
                Content_Description = contentDescription
            };
        }

        string contentDescription =
                        "This JSON configuration controls various aspects of in-game content and features. " +
                        "Adjust the boolean values to enable or disable specific content, locations, and creatures. " +
                        "Customize your in-game experience using the following sections:" +
                        "- 'Content': Toggle to enable or disable general content such as Weapons, Tools, Resources, Crafting tables, and Boss summons." +
                        "- 'Locations': Manage the presence of in-game locations, including Altars, Bosses, Vegvisirs, Ores, and Vegetation." +
                        "- 'Creatures': Control the spawning of custom creatures in different biomes, including the ocean biome." +
                        " **Note:** Modifying these settings can lead to warnings and errors in-game. Proceed with caution and only make changes if you understand their implications.";

        internal static List<T> GetJson<T>(string configPath)
        {
            Jotunn.Logger.LogDebug($"Attempting to load config file from path {configPath}");
            var jsonText = AssetUtils.LoadText(configPath);
            Jotunn.Logger.LogDebug("File found. Attempting to deserialize...");
            var configs = JsonMapper.ToObject<List<T>>(jsonText);
            return configs;
        }
    }

    [Serializable]
    public class BossConfig
    {
        public string BossPrefabName { get; set; }
        public int Health { get; set; }
        public List<CustomAttack> Attacks { get; set; }
    }

    [Serializable]
    public class CustomAttack
    {
        public string AttackName { get; set; }
        public string AttackAnimation { get; set; }
        public float AiAttackRange { get; set; }
        public float AiAttackRangeMin { get; set; }
        public float AiAttackInterval { get; set; }
        public HitData.DamageTypes? Damages { get; set; }
    }

    [Serializable]
    public class EVAConfiguration
    {
        public string Content_Description { get; set; }
        public ContentConfiguration Content { get; set; }
        public LocationConfiguration Locations { get; set; }
        public CreatureConfiguration Creatures { get; set; }
    }
    [Serializable]
    public class ContentConfiguration
    {
        public bool Mistlands { get; set; }
        public bool DeepNorth { get; set; }
        public bool Ashlands { get; set; }
    }
    [Serializable]
    public class LocationConfiguration
    {
        public bool MistlandsLocations { get; set; }
        public bool DeepNorthLocations { get; set; }
        public bool AshlandsLocations { get; set; }
    }

    [Serializable]
    public class CreatureConfiguration
    {
        public bool DeepNorthCreatures { get; set; }
        public bool AshlandsCreatures { get; set; }
        public bool OceanCreatures { get; set; }
    }
    [Serializable]
    public class OldConfiguration
    {
        public string Description { get; set; }
        public bool MistlandsLocations { get; set; }
        public bool Mistlands { get; set; }
        public bool DeepNorthLocations { get; set; }
        public bool DeepNorth { get; set; }
        public bool AshlandsLocations { get; set; }
        public bool Ashlands { get; set; }
    }
}