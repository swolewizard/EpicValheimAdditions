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
        private const string ModVersion = "1.6.7";
        private const string ModGUID = "Huntard.EpicValheimsAdditions";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        private AssetBundle assetBundle;
        private Harmony _harmony;


        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        private class Patch_ZNetScene_Awake
        {
            private static void Postfix(ZNetScene __instance)
            {
                __instance.m_prefabs.Find(p => p.name == "HugeRoot1").GetComponent<Destructible>().enabled = true;
            }
        }
        [HarmonyPatch(typeof(OfferingBowl), "Awake")]
        public static class AlterOfferBowlAwake
        {
            public static void Prefix(OfferingBowl __instance)
            {
                if (__instance == null) return;
                var FuelEffects = new EffectList.EffectData[2]
                    {
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("sfx_offering"),
                            m_enabled = true,
                            m_variant = -1,
                        },
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("vfx_offering"),
                            m_enabled = true,
                            m_variant = -1,
                        }
                    };
                var SpawnStart = new EffectList.EffectData[2]
                    {
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("sfx_prespawn"),
                            m_enabled = true,
                            m_variant = -1,
                        },
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("vfx_prespawn"),
                            m_enabled = true,
                            m_variant = -1,
                        }
                    };
                var SpawnDone = new EffectList.EffectData[2]
                    {
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("sfx_spawn"),
                            m_enabled = true,
                            m_variant = -1,
                        },
                        new EffectList.EffectData
                        {
                            m_prefab = ZNetScene.instance.GetPrefab("vfx_spawn"),
                            m_enabled = true,
                            m_variant = -1,
                        }
                    };
                if (GameObject.Find("StaminaGreydwarf(Clone)") != null)
                {
                    GameObject prefabgreydwarf = GameObject.Find("StaminaGreydwarf(Clone)").gameObject;
                    OfferingBowl componentInChildren4 = prefabgreydwarf.GetComponentInChildren<OfferingBowl>();
                    componentInChildren4.m_name = "Golden Greydwarf Miniboss";
                    componentInChildren4.m_bossPrefab = ZNetScene.instance.GetPrefab("Golden_Greydwarf_Miniboss").gameObject;
                    componentInChildren4.m_spawnBossDelay = 12;
                    componentInChildren4.m_spawnBossMaxDistance = 10;
                    componentInChildren4.m_spawnBossMaxYDistance = 9999;
                    componentInChildren4.m_spawnOffset = 0;
                    componentInChildren4.m_fuelAddedEffects.m_effectPrefabs = FuelEffects;
                    componentInChildren4.m_spawnBossStartEffects.m_effectPrefabs = SpawnStart;
                    componentInChildren4.m_spawnBossDoneffects.m_effectPrefabs = SpawnDone;
                }
                if (GameObject.Find("StaminaTroll(Clone)") != null)
                {
                    GameObject prefabTroll = GameObject.Find("StaminaTroll(Clone)").gameObject;
                    OfferingBowl componentInChildren5 = prefabTroll.GetComponentInChildren<OfferingBowl>();
                    componentInChildren5.m_name = "Golden Troll Miniboss";
                    componentInChildren5.m_bossPrefab = ZNetScene.instance.GetPrefab("Golden_Troll_Miniboss").gameObject;
                    componentInChildren5.m_spawnBossDelay = 12;
                    componentInChildren5.m_spawnBossMaxDistance = 10;
                    componentInChildren5.m_spawnBossMaxYDistance = 9999;
                    componentInChildren5.m_spawnOffset = 0;
                    componentInChildren5.m_fuelAddedEffects.m_effectPrefabs = FuelEffects;
                    componentInChildren5.m_spawnBossStartEffects.m_effectPrefabs = SpawnStart;
                    componentInChildren5.m_spawnBossDoneffects.m_effectPrefabs = SpawnDone;
                }
                if (GameObject.Find("StaminaWraith(Clone)") != null)
                {
                    GameObject prefabwraith = GameObject.Find("StaminaWraith(Clone)").gameObject;
                    OfferingBowl componentInChildren6 = prefabwraith.GetComponentInChildren<OfferingBowl>();
                    componentInChildren6.m_name = "Golden Wraith Miniboss";
                    componentInChildren6.m_bossPrefab = ZNetScene.instance.GetPrefab("Golden_Wraith_Miniboss").gameObject;
                    componentInChildren6.m_spawnBossDelay = 12;
                    componentInChildren6.m_spawnBossMaxDistance = 10;
                    componentInChildren6.m_spawnBossMaxYDistance = 9999;
                    componentInChildren6.m_spawnOffset = 0;
                    componentInChildren6.m_fuelAddedEffects.m_effectPrefabs = FuelEffects;
                    componentInChildren6.m_spawnBossStartEffects.m_effectPrefabs = SpawnStart;
                    componentInChildren6.m_spawnBossDoneffects.m_effectPrefabs = SpawnDone;
                }
            }
        }


        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);
            this.RegisterPrefabs();
            this.CreateCraftingPieces();
            this.CreateIngots_Scales_Ores();
            this.RegisterMiscItems();
            this.RegisterBossStuff();
            this.RegisterFrometalWeapons();
            this.RegisterFlametalWeapons();
            this.RegisterHeavymetalWeapons();
            this.AddVegetation();
            this.CustomConversions();
            this.LoadConfig();
            ZoneManager.OnVanillaLocationsAvailable += AddLocations;
            Core.Init();

        }

        private void RegisterPrefabs()
        {
            Jotunn.Logger.LogInfo("Loading...");
            assetBundle = AssetUtils.LoadAssetBundleFromResources("eva_assets", typeof(Core).Assembly);
            Jotunn.Logger.LogInfo("Loaded Prefabs");
        }

        internal static void Init()
        {
            ItemManager.OnItemsRegistered += ModDrops;
            ItemManager.OnItemsRegistered += Foods;
            ItemManager.OnItemsRegistered += ModThorsForgeLevel;
            ItemManager.OnItemsRegistered += ModAlchemyLevel;
        }

        private void AddLocations()
        {

            GameObject MistlandsBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("SvartalfrQueenAltar_New"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(MistlandsBossAltar, true, new LocationConfig
            {
                Biome = Heightmap.Biome.Mistlands,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 2000f,
                Unique = false,
                Quantity = 3,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;
            GameObject DeepNorthBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("JotunnAltar"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(DeepNorthBossAltar, true, new LocationConfig
            {
                Biome = Heightmap.Biome.DeepNorth,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 2000f,
                Unique = false,
                Quantity = 3,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;
            GameObject AshlandsBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("BlazingDamnedOneAltar"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(AshlandsBossAltar, true, new LocationConfig
            {
                Biome = Heightmap.Biome.AshLands,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 2000f,
                Unique = false,
                Quantity = 3,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;
            GameObject MistlandsBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_SvartalfrQueen"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(MistlandsBossRuneStone, true, new LocationConfig
            {
                Biome = Heightmap.Biome.Plains,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 350f,
                Unique = false,
                Quantity = 75,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;
            GameObject DeepNorthBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_Jotunn"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(DeepNorthBossRuneStone, true, new LocationConfig
            {
                Biome = Heightmap.Biome.Mistlands,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 350f,
                Unique = false,
                Quantity = 75,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;
            GameObject AshlandsBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_BlazingDamnedOne"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(AshlandsBossRuneStone, true, new LocationConfig
            {
                Biome = Heightmap.Biome.DeepNorth,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 350f,
                Unique = false,
                Quantity = 75,
                Priotized = true,
                ExteriorRadius = 12f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            })); ; ; ;



            Jotunn.Logger.LogInfo("Loaded Locations");

        }

        private void AddVegetation()
        {
            GameObject HeavymetalVein = assetBundle.LoadAsset<GameObject>("HeavymetalVein");
            CustomVegetation customVegetation = new CustomVegetation(HeavymetalVein, true, new VegetationConfig
            {
                Max = 2f,
                GroupSizeMin = 1,
                GroupSizeMax = 2,
                GroupRadius = 15f,
                BlockCheck = true,
                Biome = Heightmap.Biome.Mistlands,
                MinAltitude = 5f,
                MaxTilt = 20f
            });
            ZoneManager.Instance.AddCustomVegetation(customVegetation);
            GameObject FrometalVein = assetBundle.LoadAsset<GameObject>("FrometalVein_frac");
            CustomVegetation customVegetation1 = new CustomVegetation(FrometalVein, true, new VegetationConfig
            {
                Max = 1f,
                GroupSizeMin = 1,
                GroupSizeMax = 1,
                GroupRadius = 25f,
                BlockCheck = true,
                Biome = Heightmap.Biome.DeepNorth,
                MinAltitude = 5f,
                MaxTilt = 20f
            });
            ZoneManager.Instance.AddCustomVegetation(customVegetation1);
            GameObject BurningTree = assetBundle.LoadAsset<GameObject>("BurningTree");
            CustomVegetation customVegetation2 = new CustomVegetation(BurningTree, true, new VegetationConfig
            {
                Max = 2f,
                GroupSizeMin = 1,
                GroupSizeMax = 4,
                GroupRadius = 60f,
                BlockCheck = true,
                Biome = Heightmap.Biome.AshLands,
                MinAltitude = 5f,
                MaxTilt = 20f
            });
            ZoneManager.Instance.AddCustomVegetation(customVegetation2);

            Jotunn.Logger.LogInfo("Loaded Vegetation");
        }

        private static void ModDrops()
        {
            DropOnDestroyed prefab1a = PrefabManager.Cache.GetPrefab<DropOnDestroyed>("HugeRoot1");
            Destructible prefab1b = PrefabManager.Cache.GetPrefab<Destructible>("HugeRoot1");
            var item1 = ObjectDB.instance.GetItemPrefab("WorldTreeFragment");
            var RootDrop = new List<DropTable.DropData> {
                new DropTable.DropData {
                    m_item = item1,
                    m_stackMax = 7,
                    m_stackMin = 3,
                    m_weight = 1
                }
            };
            prefab1a.m_dropWhenDestroyed.m_drops = RootDrop;
            prefab1b.m_health = 500;
            prefab1b.m_damages.m_pickaxe = HitData.DamageModifier.Immune;
            prefab1b.m_damages.m_chop = HitData.DamageModifier.Normal;

            MineRock5 prefab2b = PrefabManager.Cache.GetPrefab<MineRock5>("ice_rock1_frac");
            var item2 = ObjectDB.instance.GetItemPrefab("PrimordialIce");
            var IceRockDrop = new List<DropTable.DropData> {
                new DropTable.DropData {
                    m_item = item2,
                    m_stackMax = 1,
                    m_stackMin = 1,
                    m_weight = 1
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
            prefab2b.m_dropItems.m_dropChance = 0.05f;

            Jotunn.Logger.LogInfo("Updated Drops");
            ItemManager.OnItemsRegistered -= ModDrops;
        }

        private static void Foods()
        {
            ItemDrop dandelion = PrefabManager.Cache.GetPrefab<ItemDrop>("Dandelion");
            dandelion.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Consumable;
            dandelion.m_itemData.m_shared.m_food = 5;
            dandelion.m_itemData.m_shared.m_foodStamina = 15;
            dandelion.m_itemData.m_shared.m_foodBurnTime = 900;
            dandelion.m_itemData.m_shared.m_foodRegen = 1;

            Jotunn.Logger.LogInfo("Updated Food");
            ItemManager.OnItemsRegistered -= Foods;
        }

        private void CreateCraftingPieces()
        {
            GameObject gameObject = this.assetBundle.LoadAsset<GameObject>("piece_alchemystation");
            Piece component = gameObject.GetComponent<Piece>();
            component.m_name = "Inscription Table";
            component.m_description = "Rune Crafting/Upgrading.";
            CraftingStation component2 = gameObject.GetComponent<CraftingStation>();
            component2.m_name = "Inscription Table";
            CustomPiece customPiece = new CustomPiece(gameObject, false, new PieceConfig
            {
                PieceTable = "_HammerPieceTable",
                CraftingStation = "piece_workbench",
                AllowedInDungeons = false,
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FineWood",
                                Amount = 15,
                                Recover = true
                        },
                        new RequirementConfig {
                            Item = "Coal",
                                Amount = 10,
                                Recover = true
                        },
                        new RequirementConfig {
                            Item = "Bronze",
                                Amount = 5,
                                Recover = true
                        }
                    }
            });
            PieceManager.Instance.AddPiece(customPiece);
            ///
            GameObject gameObject5 = this.assetBundle.LoadAsset<GameObject>("piece_thorsforge");
            CustomPiece customPiece5 = new CustomPiece(gameObject5, false, new PieceConfig
            {
                PieceTable = "_HammerPieceTable",
                AllowedInDungeons = false,
                Requirements = new RequirementConfig[] {

                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 20,
                                Recover = true
                        },
                        new RequirementConfig {
                            Item = "Thunderstone",
                                Amount = 10,
                                Recover = true
                        },
                        new RequirementConfig {
                            Item = "Tar",
                                Amount = 5,
                                Recover = true
                        },
                        new RequirementConfig {
                            Item = "YagluthDrop",
                                Amount = 1,
                                Recover = true
                        }
                    }
            });
            PieceManager.Instance.AddPiece(customPiece5);

            Jotunn.Logger.LogInfo("Loaded CraftingPieces");
        }

        private static void ModThorsForgeLevel()
        {
            string piece = "piece_thorsforge";
            StationExtension station = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext1").AddComponent<StationExtension>();
            station.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station.m_maxStationDistance = 50;
            station.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station1 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext2").AddComponent<StationExtension>();
            station1.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station1.m_maxStationDistance = 50;
            station1.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station2 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext3").AddComponent<StationExtension>();
            station2.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station2.m_maxStationDistance = 50;
            station2.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station3 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext4").AddComponent<StationExtension>();
            station3.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station3.m_maxStationDistance = 50;
            station3.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station4 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext5").AddComponent<StationExtension>();
            station4.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station4.m_maxStationDistance = 50;
            station4.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station5 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext6").AddComponent<StationExtension>();
            station5.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station5.m_maxStationDistance = 50;
            station5.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            Jotunn.Logger.LogInfo("Updated Thors Forge level");
            ItemManager.OnItemsRegistered -= ModThorsForgeLevel;
        }

        private static void ModAlchemyLevel()
        {
            string piece = "piece_alchemystation";
            StationExtension station = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext1").AddComponent<StationExtension>();
            station.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station.m_maxStationDistance = 50;
            station.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station1 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext2").AddComponent<StationExtension>();
            station1.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station1.m_maxStationDistance = 50;
            station1.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station2 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext3").AddComponent<StationExtension>();
            station2.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station2.m_maxStationDistance = 50;
            station2.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station3 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext4").AddComponent<StationExtension>();
            station3.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station3.m_maxStationDistance = 50;
            station3.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station4 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext5").AddComponent<StationExtension>();
            station4.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station4.m_maxStationDistance = 50;
            station4.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            StationExtension station5 = PrefabManager.Cache.GetPrefab<GameObject>("forge_ext6").AddComponent<StationExtension>();
            station5.m_craftingStation = PrefabManager.Cache.GetPrefab<CraftingStation>(piece);
            station5.m_maxStationDistance = 50;
            station5.m_connectionPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_ExtensionConnection");
            Jotunn.Logger.LogInfo("Updated Alchemystation level");
            ItemManager.OnItemsRegistered -= ModAlchemyLevel;

        }

        private void CustomConversions()
        {
            CustomItemConversion OreHeavymetal = new CustomItemConversion(new SmelterConversionConfig
            {
                Station = "blastfurnace",
                FromItem = "OreHeavymetal",
                ToItem = "HeavymetalBar"
            });
            ItemManager.Instance.AddItemConversion(OreHeavymetal);
            CustomItemConversion OreFrometal = new CustomItemConversion(new SmelterConversionConfig
            {
                Station = "blastfurnace",
                FromItem = "OreFrometal",
                ToItem = "FrometalBar"
            });
            ItemManager.Instance.AddItemConversion(OreFrometal);
        }

        private void CreateIngots_Scales_Ores()
        {

            GameObject gameObject3 = this.assetBundle.LoadAsset<GameObject>("HeavymetalBar");
            CustomItem customItem3 = new CustomItem(gameObject3, false);
            ItemManager.Instance.AddItem(customItem3);
            GameObject gameObject5 = this.assetBundle.LoadAsset<GameObject>("FrometalBar");
            CustomItem customItem5 = new CustomItem(gameObject5, false);
            ItemManager.Instance.AddItem(customItem5);



            GameObject gameObject4 = this.assetBundle.LoadAsset<GameObject>("Heavyscale");
            ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
            component4.m_itemData.m_dropPrefab = gameObject4;
            component4.m_itemData.m_shared.m_name = "Heavyscale";
            component4.m_itemData.m_shared.m_description = "A scale, which is quite heavy";
            CustomItem customItem4 = new CustomItem(gameObject4, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Iron",
                                Amount = 1
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem4);
            ///
            GameObject gameObject6 = this.assetBundle.LoadAsset<GameObject>("Drakescale");
            ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
            component6.m_itemData.m_dropPrefab = gameObject6;
            component6.m_itemData.m_shared.m_name = "Drakescale";
            component6.m_itemData.m_shared.m_description = "A frosty scale, cold to the touch";
            CustomItem customItem6 = new CustomItem(gameObject6, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Silver",
                                Amount = 1
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem6);
            ///
            GameObject gameObject8 = this.assetBundle.LoadAsset<GameObject>("Forgedscale");
            ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
            component8.m_itemData.m_dropPrefab = gameObject8;
            component8.m_itemData.m_shared.m_name = "Forgedscale";
            component8.m_itemData.m_shared.m_description = "A scale, forged by a master";
            CustomItem customItem8 = new CustomItem(gameObject8, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Copper",
                                Amount = 1
                        },
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem8);
            ///



            GameObject oreheavymetal = this.assetBundle.LoadAsset<GameObject>("OreHeavymetal");
            CustomItem oreheavymetal1 = new CustomItem(oreheavymetal, false);
            ItemManager.Instance.AddItem(oreheavymetal1);

            GameObject orefrometal = this.assetBundle.LoadAsset<GameObject>("OreFrometal");
            CustomItem orefrometal1 = new CustomItem(orefrometal, false);
            ItemManager.Instance.AddItem(orefrometal1);

            Jotunn.Logger.LogInfo("Loaded Ingots/scales/ore");
        }

        private void RegisterHeavymetalWeapons()
        {
            GameObject gameObject = this.assetBundle.LoadAsset<GameObject>("BowHeavymetal");
            ItemDrop component = gameObject.GetComponent<ItemDrop>();
            component.m_itemData.m_shared.m_maxDurability = 200;
            component.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem = new CustomItem(gameObject, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 14,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 25,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 14,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem);
            GameObject gameObject2 = this.assetBundle.LoadAsset<GameObject>("AtgeirHeavymetal");
            ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
            component2.m_itemData.m_shared.m_maxDurability = 200;
            component2.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem2 = new CustomItem(gameObject2, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 14,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 24,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 14,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem2);
            GameObject gameObject3 = this.assetBundle.LoadAsset<GameObject>("SledgeHeavymetal");
            ItemDrop component3 = gameObject3.GetComponent<ItemDrop>();
            component3.m_itemData.m_shared.m_maxDurability = 200;
            component3.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem3 = new CustomItem(gameObject3, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 14,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 28,
                                AmountPerLevel = 6
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 14,
                                AmountPerLevel = 5
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem3);
            GameObject gameObject4 = this.assetBundle.LoadAsset<GameObject>("BattleaxeHeavymetal");
            ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
            component4.m_itemData.m_shared.m_maxDurability = 200;
            component4.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem4 = new CustomItem(gameObject4, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 16,
                                AmountPerLevel = 6
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 30,
                                AmountPerLevel = 7
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 12,
                                AmountPerLevel = 6
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem4);
            GameObject gameObject5 = this.assetBundle.LoadAsset<GameObject>("SpearHeavymetal");
            ItemDrop component5 = gameObject5.GetComponent<ItemDrop>();
            component5.m_itemData.m_shared.m_maxDurability = 200;
            component5.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem5 = new CustomItem(gameObject5, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 8,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 16,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 6,
                                AmountPerLevel = 3
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem5);
            GameObject gameObject6 = this.assetBundle.LoadAsset<GameObject>("KnifeHeavymetal");
            ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
            component6.m_itemData.m_shared.m_maxDurability = 200;
            component6.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem6 = new CustomItem(gameObject6, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 8,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 12,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 3,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem6);
            GameObject gameObject7 = this.assetBundle.LoadAsset<GameObject>("MaceHeavymetal");
            ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
            component7.m_itemData.m_shared.m_maxDurability = 200;
            component7.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem7 = new CustomItem(gameObject7, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 10,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 20,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 10,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem7);
            GameObject gameObject8 = this.assetBundle.LoadAsset<GameObject>("GreatSwordHeavymetal");
            ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
            component8.m_itemData.m_shared.m_maxDurability = 200;
            component8.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem8 = new CustomItem(gameObject8, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 30,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 30,
                                AmountPerLevel = 15
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem8);
            GameObject gameObject9 = this.assetBundle.LoadAsset<GameObject>("SwordHeavymetal");
            ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
            component9.m_itemData.m_shared.m_maxDurability = 200;
            component9.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem9 = new CustomItem(gameObject9, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 10,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 15,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 12,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem9);
            GameObject gameObject10 = this.assetBundle.LoadAsset<GameObject>("ShieldHeavymetal");
            ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
            component10.m_itemData.m_shared.m_name = "Heavymetal Shield";
            component10.m_itemData.m_shared.m_maxDurability = 200;
            component10.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem10 = new CustomItem(gameObject10, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 10,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 18,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 10,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem10);
            GameObject gameObject11 = this.assetBundle.LoadAsset<GameObject>("ShieldHeavymetalTower");
            ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
            component11.m_itemData.m_shared.m_maxDurability = 200;
            component11.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem11 = new CustomItem(gameObject11, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 20,
                                AmountPerLevel = 8
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 25,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 8
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem11);
            GameObject gameObject12 = this.assetBundle.LoadAsset<GameObject>("AxeHeavymetal");
            ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
            component12.m_itemData.m_shared.m_maxDurability = 200;
            component12.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem12 = new CustomItem(gameObject12, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 20,
                                AmountPerLevel = 8
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 8
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem12);
            GameObject gameObject13 = this.assetBundle.LoadAsset<GameObject>("PickaxeHeavymetal");
            ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
            component13.m_itemData.m_shared.m_maxDurability = 200;
            component13.m_itemData.m_shared.m_durabilityPerLevel = 50;
            CustomItem customItem13 = new CustomItem(gameObject13, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 20,
                                AmountPerLevel = 8
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 8
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem13);
            Jotunn.Logger.LogInfo("Loaded HeavymetalWeapons");
        }

        private void RegisterFrometalWeapons()
        {
            GameObject gameObject = this.assetBundle.LoadAsset<GameObject>("BowFrometal");
            ItemDrop component = gameObject.GetComponent<ItemDrop>();
            component.m_itemData.m_shared.m_maxDurability = 250;
            component.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem = new CustomItem(gameObject, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 15,
                                AmountPerLevel = 7
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 2,
                                AmountPerLevel = 1
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem);
            GameObject gameObject2 = this.assetBundle.LoadAsset<GameObject>("AtgeirFrometal");
            ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
            component2.m_itemData.m_shared.m_maxDurability = 250;
            component2.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem2 = new CustomItem(gameObject2, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 15,
                                AmountPerLevel = 7
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem2);
            GameObject gameObject4 = this.assetBundle.LoadAsset<GameObject>("SledgeFrometal");
            ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
            component4.m_itemData.m_shared.m_maxDurability = 250;
            component4.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem4 = new CustomItem(gameObject4, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 15,
                                AmountPerLevel = 7
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 7,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem4);
            GameObject gameObject5 = this.assetBundle.LoadAsset<GameObject>("BattleaxeFrometal");
            ItemDrop component5 = gameObject5.GetComponent<ItemDrop>();
            component5.m_itemData.m_shared.m_maxDurability = 250;
            component5.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem5 = new CustomItem(gameObject5, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 15,
                                AmountPerLevel = 7
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem5);
            GameObject gameObject6 = this.assetBundle.LoadAsset<GameObject>("SpearFrometal");
            ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
            component6.m_itemData.m_shared.m_maxDurability = 250;
            component6.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem6 = new CustomItem(gameObject6, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem6);
            GameObject gameObject7 = this.assetBundle.LoadAsset<GameObject>("KnifeFrometal");
            ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
            component7.m_itemData.m_shared.m_maxDurability = 250;
            component7.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem7 = new CustomItem(gameObject7, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 8,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 2,
                                AmountPerLevel = 1
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem7);
            GameObject gameObject8 = this.assetBundle.LoadAsset<GameObject>("MaceFrometal");
            ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
            component8.m_itemData.m_shared.m_maxDurability = 250;
            component8.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem8 = new CustomItem(gameObject8, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem8);
            GameObject gameObject9 = this.assetBundle.LoadAsset<GameObject>("GreatSwordFrometal");
            ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
            component9.m_itemData.m_shared.m_maxDurability = 250;
            component9.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem9 = new CustomItem(gameObject9, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 30,
                                AmountPerLevel = 15
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem9);
            GameObject gameObject10 = this.assetBundle.LoadAsset<GameObject>("SwordFrometal");
            ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
            component10.m_itemData.m_shared.m_maxDurability = 250;
            component10.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem10 = new CustomItem(gameObject10, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem10);
            GameObject ShieldFrometal = this.assetBundle.LoadAsset<GameObject>("ShieldFrometal");
            ItemDrop ItemDrop = ShieldFrometal.GetComponent<ItemDrop>();
            ItemDrop.m_itemData.m_shared.m_name = "Frometal Shield";
            ItemDrop.m_itemData.m_shared.m_maxDurability = 250;
            ItemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem ShieldFrometalBM = new CustomItem(ShieldFrometal, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 8,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 5,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(ShieldFrometalBM);
            GameObject gameObject11 = this.assetBundle.LoadAsset<GameObject>("ShieldFrometalTower");
            ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
            component11.m_itemData.m_shared.m_name = "Frometal Tower Shield";
            component11.m_itemData.m_shared.m_description = "A Towershield made out of Frometal.";
            component11.m_itemData.m_shared.m_variants = 0;
            component11.m_itemData.m_shared.m_maxDurability = 250;
            component11.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem11 = new CustomItem(gameObject11, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 12,
                                AmountPerLevel = 6
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 7,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem11);
            GameObject gameObject12 = this.assetBundle.LoadAsset<GameObject>("AxeFrometal");
            ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
            component12.m_itemData.m_shared.m_maxDurability = 250;
            component12.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem12 = new CustomItem(gameObject12, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {

                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 2,
                                AmountPerLevel = 1
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem12);
            GameObject gameObject13 = this.assetBundle.LoadAsset<GameObject>("PickaxeFrometal");
            ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
            component13.m_itemData.m_shared.m_maxDurability = 250;
            component13.m_itemData.m_shared.m_durabilityPerLevel = 65;
            CustomItem customItem13 = new CustomItem(gameObject13, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {

                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 2,
                                AmountPerLevel = 1
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem13);
            Jotunn.Logger.LogInfo("Loaded FrometalWeapons");
        }

        private void RegisterFlametalWeapons()
        {
            GameObject gameObject = this.assetBundle.LoadAsset<GameObject>("BowFlametal");
            ItemDrop component = gameObject.GetComponent<ItemDrop>();
            component.m_itemData.m_shared.m_maxDurability = 300;
            component.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem = new CustomItem(gameObject, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem);
            GameObject gameObject2 = this.assetBundle.LoadAsset<GameObject>("AtgeirFlametal");
            ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
            component2.m_itemData.m_shared.m_maxDurability = 300;
            component2.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem2 = new CustomItem(gameObject2, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem2);
            GameObject gameObject4 = this.assetBundle.LoadAsset<GameObject>("SledgeFlametal");
            ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
            component4.m_itemData.m_shared.m_maxDurability = 300;
            component4.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem4 = new CustomItem(gameObject4, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 4,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem4);
            GameObject gameObject5 = this.assetBundle.LoadAsset<GameObject>("BattleaxeFlametal");
            ItemDrop component5 = gameObject5.GetComponent<ItemDrop>();
            component5.m_itemData.m_shared.m_maxDurability = 300;
            component5.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem5 = new CustomItem(gameObject5, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 6,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem5);
            GameObject gameObject6 = this.assetBundle.LoadAsset<GameObject>("SpearFlametal");
            ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
            component6.m_itemData.m_shared.m_maxDurability = 300;
            component6.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem6 = new CustomItem(gameObject6, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem6);
            GameObject gameObject7 = this.assetBundle.LoadAsset<GameObject>("KnifeFlametal");
            ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
            component7.m_itemData.m_shared.m_maxDurability = 300;
            component7.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem7 = new CustomItem(gameObject7, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 8,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem7);
            GameObject gameObject8 = this.assetBundle.LoadAsset<GameObject>("MaceFlametal");
            ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
            component8.m_itemData.m_shared.m_maxDurability = 300;
            component8.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem8 = new CustomItem(gameObject8, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem8);
            GameObject gameObject9 = this.assetBundle.LoadAsset<GameObject>("GreatSwordFlametal");
            ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
            component9.m_itemData.m_shared.m_maxDurability = 300;
            component9.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem9 = new CustomItem(gameObject9, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 30,
                                AmountPerLevel = 15
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 6,
                                AmountPerLevel = 3
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem9);
            GameObject gameObject10 = this.assetBundle.LoadAsset<GameObject>("SwordFlametal");
            ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
            component10.m_itemData.m_shared.m_maxDurability = 300;
            component10.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem10 = new CustomItem(gameObject10, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 10,
                                AmountPerLevel = 5
                        },
                        new RequirementConfig {
                            Item = "LoxPelt",
                                Amount = 3,
                                AmountPerLevel = 2
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem10);
            GameObject ShieldFlametal = this.assetBundle.LoadAsset<GameObject>("ShieldFlametal");
            ItemDrop ItemDrop = ShieldFlametal.GetComponent<ItemDrop>();
            ItemDrop.m_itemData.m_shared.m_name = "Flametal Shield";
            ItemDrop.m_itemData.m_shared.m_maxDurability = 300;
            ItemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem ShieldFlametalBM = new CustomItem(ShieldFlametal, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 8,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(ShieldFlametalBM);
            GameObject gameObject11 = this.assetBundle.LoadAsset<GameObject>("ShieldFlametalTower");
            ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
            component11.m_itemData.m_shared.m_name = "Flametal Tower Shield";
            component11.m_itemData.m_shared.m_description = "A Towershield made out of Flametal.";
            component11.m_itemData.m_shared.m_variants = 0;
            component11.m_itemData.m_shared.m_maxDurability = 300;
            component11.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem11 = new CustomItem(gameObject11, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 12,
                                AmountPerLevel = 4
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem11);
            GameObject gameObject12 = this.assetBundle.LoadAsset<GameObject>("AxeFlametal");
            ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
            component12.m_itemData.m_shared.m_maxDurability = 300;
            component12.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem12 = new CustomItem(gameObject12, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem12);
            GameObject gameObject13 = this.assetBundle.LoadAsset<GameObject>("PickaxeFlametal");
            ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
            component13.m_itemData.m_shared.m_maxDurability = 300;
            component13.m_itemData.m_shared.m_durabilityPerLevel = 75;
            CustomItem customItem13 = new CustomItem(gameObject13, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem13);
            Jotunn.Logger.LogInfo("Loaded FlametalWeapons");
        }

        private void RegisterBossStuff()
        {
            GameObject svartalfrqueenaltar = assetBundle.LoadAsset<GameObject>("SvartalfrQueenAltar_New");
            CustomPrefab svartalfrqueenaltar1 = new CustomPrefab(svartalfrqueenaltar, false);
            PrefabManager.Instance.AddPrefab(svartalfrqueenaltar1);

            GameObject jotunnaltar = assetBundle.LoadAsset<GameObject>("JotunnAltar");
            CustomPrefab jotunnaltar1 = new CustomPrefab(jotunnaltar, false);
            PrefabManager.Instance.AddPrefab(jotunnaltar1);

            GameObject blazingdamnedonealtar = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneAltar");
            CustomPrefab blazingdamnedonealtar1 = new CustomPrefab(blazingdamnedonealtar, false);
            PrefabManager.Instance.AddPrefab(blazingdamnedonealtar1);

            GameObject Vegvisir_SvartalfrQueen = assetBundle.LoadAsset<GameObject>("Vegvisir_SvartalfrQueen");
            CustomPrefab Vegvisir_SvartalfrQueen1 = new CustomPrefab(Vegvisir_SvartalfrQueen, false);
            PrefabManager.Instance.AddPrefab(Vegvisir_SvartalfrQueen1);

            GameObject Vegvisir_Jotunn = assetBundle.LoadAsset<GameObject>("Vegvisir_Jotunn");
            CustomPrefab Vegvisir_Jotunn1 = new CustomPrefab(Vegvisir_Jotunn, false);
            PrefabManager.Instance.AddPrefab(Vegvisir_Jotunn1);

            GameObject Vegvisir_BlazingDamnedOne = assetBundle.LoadAsset<GameObject>("Vegvisir_BlazingDamnedOne");
            CustomPrefab Vegvisir_BlazingDamnedOne1 = new CustomPrefab(Vegvisir_BlazingDamnedOne, false);
            PrefabManager.Instance.AddPrefab(Vegvisir_BlazingDamnedOne1);

            GameObject fenrirsheart = assetBundle.LoadAsset<GameObject>("FenrirsHeart");
            CustomItem FenrirsHeart = new CustomItem(fenrirsheart, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_alchemystation",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 20
                        },
                        new RequirementConfig {
                            Item = "BurningWorldTreeFragment",
                                Amount = 30
                        }
                    }
            });
            ItemManager.Instance.AddItem(FenrirsHeart);

            GameObject ymirsSoulEssence = assetBundle.LoadAsset<GameObject>("YmirsSoulEssence");
            CustomItem YmirsSoulEssence = new CustomItem(ymirsSoulEssence, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_alchemystation",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 20
                        },
                        new RequirementConfig {
                            Item = "PrimordialIce",
                                Amount = 10
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 30
                        }
                    }
            });
            ItemManager.Instance.AddItem(YmirsSoulEssence);

            GameObject cursedEffigy = assetBundle.LoadAsset<GameObject>("CursedEffigy");
            CustomItem CursedEffigy = new CustomItem(cursedEffigy, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_alchemystation",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "HeavymetalBar",
                                Amount = 20
                        },
                        new RequirementConfig {
                            Item = "AncientSeed",
                                Amount = 10
                        },
                        new RequirementConfig {
                            Item = "WorldTreeFragment",
                                Amount = 30
                        }
                    }
            });
            ItemManager.Instance.AddItem(CursedEffigy);


            GameObject Golden_Greydwarf_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Greydwarf_Miniboss");
            PrefabManager.Instance.AddPrefab(Golden_Greydwarf_Miniboss);
            GameObject Golden_Troll_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Troll_Miniboss");
            PrefabManager.Instance.AddPrefab(Golden_Troll_Miniboss);
            GameObject Golden_Wraith_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Wraith_Miniboss");
            PrefabManager.Instance.AddPrefab(Golden_Wraith_Miniboss);
            GameObject SvartalfrQueen = assetBundle.LoadAsset<GameObject>("SvartalfrQueen");
            CustomPrefab SvartalfrQueen1 = new CustomPrefab(SvartalfrQueen, true);
            PrefabManager.Instance.AddPrefab(SvartalfrQueen1);
            GameObject SvarTentaRoot = assetBundle.LoadAsset<GameObject>("SvarTentaRoot");
            CustomPrefab SvarTentaRoot1 = new CustomPrefab(SvarTentaRoot, true);
            PrefabManager.Instance.AddPrefab(SvarTentaRoot);
            GameObject JotunnBoss = assetBundle.LoadAsset<GameObject>("Jotunn");
            CustomPrefab JotunnBoss1 = new CustomPrefab(JotunnBoss, true);
            PrefabManager.Instance.AddPrefab(JotunnBoss1);
            GameObject BlazingDamnedOne = assetBundle.LoadAsset<GameObject>("BlazingDamnedOne");
            CustomPrefab BlazingDamnedOne1 = new CustomPrefab(BlazingDamnedOne, true);
            PrefabManager.Instance.AddPrefab(BlazingDamnedOne1);


            GameObject SvartalfrQueenGreatSword = assetBundle.LoadAsset<GameObject>("SvartalfrQueenGreatSword");
            CustomItem SvartalfrQueenGreatSword1 = new CustomItem(SvartalfrQueenGreatSword, false);
            ItemManager.Instance.AddItem(SvartalfrQueenGreatSword1);
            GameObject SvartalfrQueenBow = assetBundle.LoadAsset<GameObject>("SvartalfrQueenBow");
            CustomItem SvartalfrQueenBow1 = new CustomItem(SvartalfrQueenBow, false);
            ItemManager.Instance.AddItem(SvartalfrQueenBow1);
            GameObject SvartalfrQueenBowArrowStorm = assetBundle.LoadAsset<GameObject>("SvartalfrQueenBowArrowStorm");
            CustomItem SvartalfrQueenBowArrowStorm1 = new CustomItem(SvartalfrQueenBowArrowStorm, false);
            ItemManager.Instance.AddItem(SvartalfrQueenBowArrowStorm1);
            GameObject SvartalfrQueen_rootspawn = assetBundle.LoadAsset<GameObject>("SvartalfrQueen_rootspawn");
            CustomItem SvartalfrQueen_rootspawn1 = new CustomItem(SvartalfrQueen_rootspawn, false);
            ItemManager.Instance.AddItem(SvartalfrQueen_rootspawn1);

            GameObject Jotunn_Groundslam = assetBundle.LoadAsset<GameObject>("Jotunn_Groundslam");
            CustomItem Jotunn_Groundslam1 = new CustomItem(Jotunn_Groundslam, false);
            ItemManager.Instance.AddItem(Jotunn_Groundslam1);
            GameObject Jotunn_Groundslam2 = assetBundle.LoadAsset<GameObject>("Jotunn_Groundslam2");
            CustomItem Jotunn_Groundslam21 = new CustomItem(Jotunn_Groundslam2, false);
            ItemManager.Instance.AddItem(Jotunn_Groundslam21);
            GameObject Jotunn_Shoot = assetBundle.LoadAsset<GameObject>("Jotunn_Shoot");
            CustomItem Jotunn_Shoot1 = new CustomItem(Jotunn_Shoot, false);
            ItemManager.Instance.AddItem(Jotunn_Shoot1);

            GameObject BlazingDamnedOneMace = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneMace");
            CustomItem BlazingDamnedOneMace1 = new CustomItem(BlazingDamnedOneMace, false);
            ItemManager.Instance.AddItem(BlazingDamnedOneMace1);
            GameObject BlazingDamnedOneMacetwo = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneMace2");
            CustomItem BlazingDamnedOneMace2 = new CustomItem(BlazingDamnedOneMacetwo, false);
            ItemManager.Instance.AddItem(BlazingDamnedOneMace2);
            GameObject BlazingDamnedOneMacethree = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneMace3");
            CustomItem BlazingDamnedOneMace3 = new CustomItem(BlazingDamnedOneMacethree, false);
            ItemManager.Instance.AddItem(BlazingDamnedOneMace3);
            GameObject BlazingDamnedOneMacefour = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneMace4");
            CustomItem BlazingDamnedOneMace4 = new CustomItem(BlazingDamnedOneMacefour, false);
            ItemManager.Instance.AddItem(BlazingDamnedOneMace4);
            GameObject BlazingDamnedOneShield = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneShield");
            CustomItem BlazingDamnedOneShield1 = new CustomItem(BlazingDamnedOneShield, false);
            ItemManager.Instance.AddItem(BlazingDamnedOneShield1);
            GameObject EVA_BlazingHelm = assetBundle.LoadAsset<GameObject>("EVA_BlazingHelm");
            CustomItem EVA_BlazingHelm1 = new CustomItem(EVA_BlazingHelm, false);
            ItemManager.Instance.AddItem(EVA_BlazingHelm1);
            GameObject EVA_BlazingChest = assetBundle.LoadAsset<GameObject>("EVA_BlazingChest");
            CustomItem EVA_BlazingChest1 = new CustomItem(EVA_BlazingChest, false);
            ItemManager.Instance.AddItem(EVA_BlazingChest1);
            GameObject EVA_BlazingBoots = assetBundle.LoadAsset<GameObject>("EVA_BlazingBoots");
            CustomItem EVA_BlazingBoots1 = new CustomItem(EVA_BlazingBoots, false);
            ItemManager.Instance.AddItem(EVA_BlazingBoots1);
            GameObject Blazing_Nova = assetBundle.LoadAsset<GameObject>("Blazing_Nova");
            CustomItem Blazing_Nova1 = new CustomItem(Blazing_Nova, false);
            ItemManager.Instance.AddItem(Blazing_Nova1);
            GameObject Blazing_Meteors = assetBundle.LoadAsset<GameObject>("Blazing_Meteors");
            CustomItem Blazing_Meteors1 = new CustomItem(Blazing_Meteors, false);
            ItemManager.Instance.AddItem(Blazing_Meteors1);
            GameObject Blazing_Shoot = assetBundle.LoadAsset<GameObject>("Blazing_Shoot");
            CustomItem Blazing_Shoot1 = new CustomItem(Blazing_Shoot, false);
            ItemManager.Instance.AddItem(Blazing_Shoot1);


            GameObject TrophySvartalfrQueen = assetBundle.LoadAsset<GameObject>("TrophySvartalfrQueen");
            CustomItem TrophySvartalfrQueen1 = new CustomItem(TrophySvartalfrQueen, false);
            ItemManager.Instance.AddItem(TrophySvartalfrQueen1);
            GameObject TrophyJotunn = assetBundle.LoadAsset<GameObject>("TrophyJotunn");
            CustomItem TrophyJotunn1 = new CustomItem(TrophyJotunn, false);
            ItemManager.Instance.AddItem(TrophyJotunn1);
            GameObject TrophyBlazingDamnedOne = assetBundle.LoadAsset<GameObject>("TrophyBlazingDamnedOne");
            CustomItem TrophyBlazingDamnedOne1 = new CustomItem(TrophyBlazingDamnedOne, false);
            ItemManager.Instance.AddItem(TrophyBlazingDamnedOne1);

            Jotunn.Logger.LogInfo("Loaded BossStuff");
        }

        private void RegisterMiscItems()
        {
            GameObject errordrop = assetBundle.LoadAsset<GameObject>("ErrorDrop");
            CustomItem ErrorDrop = new CustomItem(errordrop, false);
            ItemManager.Instance.AddItem(ErrorDrop);

            GameObject darkfrometal = assetBundle.LoadAsset<GameObject>("FrostInfusedDarkMetal");
            CustomItem darkfrometal1 = new CustomItem(darkfrometal, false);
            ItemManager.Instance.AddItem(darkfrometal1);

            GameObject burningfragment = assetBundle.LoadAsset<GameObject>("BurningWorldTreeFragment");
            CustomItem burningfragment1 = new CustomItem(burningfragment, false);
            ItemManager.Instance.AddItem(burningfragment1);

            GameObject TreeFragment = assetBundle.LoadAsset<GameObject>("WorldTreeFragment");
            CustomItem CursedWorldTreeFragment = new CustomItem(TreeFragment, false);
            ItemManager.Instance.AddItem(CursedWorldTreeFragment);

            GameObject primice = assetBundle.LoadAsset<GameObject>("PrimordialIce");
            CustomItem Primice = new CustomItem(primice, false);
            ItemManager.Instance.AddItem(Primice);

            Jotunn.Logger.LogInfo("Loaded MiscItems");
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                GenerateConfigFile();
                Jotunn.Logger.LogInfo("Generated new config");
                return;
            }
            RegisterConfigValues();

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                GenerateConfigFile();

                Jotunn.Logger.LogInfo("Updated config");
            }

        }

        private void GenerateConfigFile()
        {

            var bossConfigs = new List<BossConfig>();

            // SvartalFrQueen
            var SvartalfrQueenConfig = new BossConfig();
            var SvartalfrQueenPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("SvartalfrQueen");
            SvartalfrQueenConfig.BossPrefabName = "SvartalfrQueen";
            SvartalfrQueenConfig.Health = (int)SvartalfrQueenPrefab.m_health;

            SvartalfrQueenConfig.Attacks = new List<CustomAttack>();
            var svartalfrQueenStandardAttacks = new List<string> { "SvartalfrQueenGreatSword", "SvartalfrQueenBow" };
            AddDefaultAttacks(SvartalfrQueenConfig, svartalfrQueenStandardAttacks);
            var SvartalfrQueenRootspawn = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueen_rootspawn");
            SvartalfrQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfrQueenRootspawn.name,
                AttackAnimation = SvartalfrQueenRootspawn.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackInterval
            });
            var SvartalfrQueenRootspawn2 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvarTentaRoot_attack");
            SvartalfrQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfrQueenRootspawn2.name,
                AiAttackRange = SvartalfrQueenRootspawn2.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfrQueenRootspawn2.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfrQueenRootspawn2.m_itemData.m_shared.m_aiAttackInterval,
                Damages = SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages
            });
            var SvartalfrQueenBowArrowStorm = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenBowArrowStorm");
            SvartalfrQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfrQueenBowArrowStorm.name,
                AttackAnimation = SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackInterval
            });
            var bow_projectile_svar = PrefabManager.Cache.GetPrefab<Projectile>("bow_projectile_svar");
            SvartalfrQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = bow_projectile_svar.name,
                Damages = bow_projectile_svar.m_damage
            });

            bossConfigs.Add(SvartalfrQueenConfig);

            // Jotunn
            var JotunnConfig = new BossConfig();
            var JotunnPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("Jotunn");
            JotunnConfig.BossPrefabName = "Jotunn";
            JotunnConfig.Health = (int)JotunnPrefab.m_health;

            JotunnConfig.Attacks = new List<CustomAttack>();
            var jotunnStandardAttacks = new List<string> { "Jotunn_Groundslam", "Jotunn_Groundslam2", "Jotunn_Shoot" };
            AddDefaultAttacks(JotunnConfig, jotunnStandardAttacks);
            bossConfigs.Add(JotunnConfig);

            // BlazingDamnedOne
            var BlazingDamnedOneConfig = new BossConfig();
            var BlazingDamnedOne = PrefabManager.Cache.GetPrefab<Humanoid>("BlazingDamnedOne");
            BlazingDamnedOneConfig.BossPrefabName = "BlazingDamnedOne";
            BlazingDamnedOneConfig.Health = (int)BlazingDamnedOne.m_health;

            BlazingDamnedOneConfig.Attacks = new List<CustomAttack>();
            var blazingDamnedStandardAttacks = new List<string> { "BlazingDamnedOneMace", "BlazingDamnedOneMace2", "BlazingDamnedOneMace3", "BlazingDamnedOneMace4", "Blazing_Nova", "Blazing_Shoot" };
            AddDefaultAttacks(BlazingDamnedOneConfig, blazingDamnedStandardAttacks);
            var Blazing_Meteors = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Meteors");
            BlazingDamnedOneConfig.Attacks.Add(new CustomAttack
            {
                AttackName = Blazing_Meteors.name,
                AttackAnimation = Blazing_Meteors.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = Blazing_Meteors.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = Blazing_Meteors.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = Blazing_Meteors.m_itemData.m_shared.m_aiAttackInterval
            });
            var projectile_meteor_blazing = PrefabManager.Cache.GetPrefab<Projectile>("projectile_meteor_blazing");
            BlazingDamnedOneConfig.Attacks.Add(new CustomAttack
            {
                AttackName = projectile_meteor_blazing.name,
                Damages = projectile_meteor_blazing.m_damage
            });
            bossConfigs.Add(BlazingDamnedOneConfig);

            var jsonText = JsonMapper.ToJson(bossConfigs);
            File.WriteAllText(configPath, jsonText);

        }

        private void AddDefaultAttacks(BossConfig config, List<string> attackNames)
        {
            foreach (var attackName in attackNames)
            {
                var attack = PrefabManager.Cache.GetPrefab<ItemDrop>(attackName);
                config.Attacks.Add(new CustomAttack
                {
                    AttackName = attack.name,
                    AttackAnimation = attack.m_itemData.m_shared.m_attack.m_attackAnimation,
                    AiAttackRange = attack.m_itemData.m_shared.m_aiAttackRange,
                    AiAttackRangeMin = attack.m_itemData.m_shared.m_aiAttackRangeMin,
                    AiAttackInterval = attack.m_itemData.m_shared.m_aiAttackInterval,
                    Damages = attack.m_itemData.m_shared.m_damages
                });
            }
        }


        private void RegisterConfigValues()
        {
            var bossConfigs = GetJson();

            foreach (var config in bossConfigs)
            {
                try
                {
                    PrefabManager.Cache.GetPrefab<Humanoid>(config.BossPrefabName).m_health = config.Health;

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
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.BossPrefabName} failed. {e.Message} {e.StackTrace}");
                }
            }

            Jotunn.Logger.LogInfo("Loaded configs");

        }

        internal static List<BossConfig> GetJson()
        {
            Jotunn.Logger.LogDebug($"Attempting to load config file from path {configPath}");
            var jsonText = AssetUtils.LoadText(configPath);
            Jotunn.Logger.LogDebug("File found. Attempting to deserialize...");
            var bossconfigs = JsonMapper.ToObject<List<BossConfig>>(jsonText);
            return bossconfigs;
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

}