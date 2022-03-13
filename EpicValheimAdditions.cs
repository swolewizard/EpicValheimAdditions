using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using System.IO;
using System.Linq;

namespace EpicValheimAdditions
{
    [BepInPlugin("Huntard.EpicValheimsAdditions", "Epic Valheims Additions - by Huntard", "1.6.5")]
    [BepInDependency("com.jotunn.jotunn", BepInDependency.DependencyFlags.HardDependency)]
    public class Core : BaseUnityPlugin
    {
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
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "Huntard.EpicValheimsAdditions");
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
            this.Config();
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
            PrefabManager.Instance.AddPrefab(SvartalfrQueen);
            GameObject SvarTentaRoot = assetBundle.LoadAsset<GameObject>("SvarTentaRoot");
            PrefabManager.Instance.AddPrefab(SvarTentaRoot);
            GameObject JotunnBoss = assetBundle.LoadAsset<GameObject>("Jotunn");
            PrefabManager.Instance.AddPrefab(JotunnBoss);
            GameObject BlazingDamnedOne = assetBundle.LoadAsset<GameObject>("BlazingDamnedOne");
            PrefabManager.Instance.AddPrefab(BlazingDamnedOne);


            GameObject TrophySvartalfrQueen = assetBundle.LoadAsset<GameObject>("TrophySvartalfrQueen");
            CustomItem TrophySvartalfrQueen1 = new CustomItem(TrophySvartalfrQueen, false);
            ItemManager.Instance.AddItem(TrophySvartalfrQueen1);
            GameObject TrophyJotunn = assetBundle.LoadAsset<GameObject>("TrophyJotunn");
            CustomItem TrophyJotunn1 = new CustomItem(TrophyJotunn, false);
            ItemManager.Instance.AddItem(TrophyJotunn1);
            GameObject TrophyBlazingDamnedOne = assetBundle.LoadAsset<GameObject>("TrophyBlazingDamnedOne");
            CustomItem TrophyBlazingDamnedOne1 = new CustomItem(TrophyBlazingDamnedOne, false);
            ItemManager.Instance.AddItem(TrophyBlazingDamnedOne1);

            GameObject SvartalfrQueenGreatSword = assetBundle.LoadAsset<GameObject>("SvartalfrQueenGreatSword");
            CustomItem SvartalfrQueenGreatSword1 = new CustomItem(SvartalfrQueenGreatSword, false);
            ItemManager.Instance.AddItem(SvartalfrQueenGreatSword1);
            GameObject SvartalfrQueenBow = assetBundle.LoadAsset<GameObject>("SvartalfrQueenBow");
            CustomItem SvartalfrQueenBow1 = new CustomItem(SvartalfrQueenBow, false);
            ItemManager.Instance.AddItem(SvartalfrQueenBow1);
            GameObject SvartalfrQueenBowArrowStorm = assetBundle.LoadAsset<GameObject>("SvartalfrQueenBowArrowStorm");
            CustomItem SvartalfrQueenBowArrowStorm1 = new CustomItem(SvartalfrQueenBowArrowStorm, false);
            ItemManager.Instance.AddItem(SvartalfrQueenBowArrowStorm1);

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

        private void Config()
        {
            ConfigSvartalfrQueen();
            ConfigJotunn();
            ConfigBlazingDamnedOne();
            RegisterConfigValues();

            Jotunn.Logger.LogInfo("Loaded Configs");
        }

        private void ConfigSvartalfrQueen()
        {
            base.Config.SaveOnConfigSet = true;

            Humanoid SvartalfrQueen = PrefabManager.Cache.GetPrefab<Humanoid>("SvartalfrQueen");
            int Health = (int)SvartalfrQueen.m_health;
            this.SvartalfrQueenBossHealth = base.Config.Bind<int>("(1a) SvartalfrQueen Boss", "Health", Health, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));


            ItemDrop SvartalfrQueenSword = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenGreatSword");
            int SwordDamage = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_damage;
            int SwordBlunt = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_blunt;
            int SwordSlash = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_slash;
            int SwordPierce = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_pierce;
            int SwordChop = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_chop;
            int SwordPickaxe = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_pickaxe;
            int SwordFire = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_fire;
            int SwordFrost = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_frost;
            int SwordLightning = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_lightning;
            int SwordPoison = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_poison;
            int SwordSpirit = (int)SvartalfrQueenSword.m_itemData.m_shared.m_damages.m_spirit;
            int SwordRange = (int)SvartalfrQueenSword.m_itemData.m_shared.m_aiAttackRange;
            int SwordRangeMin = (int)SvartalfrQueenSword.m_itemData.m_shared.m_aiAttackRangeMin;
            int SwordInterval = (int)SvartalfrQueenSword.m_itemData.m_shared.m_aiAttackInterval;
            this.SvartalfrQueenAttack1Damage = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Damage", SwordDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.SvartalfrQueenAttack1Blunt = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Blunt", SwordBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.SvartalfrQueenAttack1Slash = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Slash", SwordSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.SvartalfrQueenAttack1Pierce = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Pierce", SwordPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.SvartalfrQueenAttack1Chop = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Chop", SwordChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.SvartalfrQueenAttack1Pickaxe = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Pickaxe", SwordPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.SvartalfrQueenAttack1Fire = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Fire", SwordFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.SvartalfrQueenAttack1Frost = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Frost", SwordFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.SvartalfrQueenAttack1Lightning = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Lightning", SwordLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.SvartalfrQueenAttack1Poison = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Poison", SwordPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.SvartalfrQueenAttack1Spirit = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "Spirit", SwordSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.SvartalfrQueenAttack1Range = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "AI Attack range max", SwordRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.SvartalfrQueenAttack1RangeMin = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "AI Attack range min", SwordRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.SvartalfrQueenAttack1Interval = base.Config.Bind<int>("(1b) SvartalfrQueen Attack: SvartalfrQueenGreatSword", "AI Attack Interval", SwordInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop SvartalfrQueenBow = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenBow");
            int BowDamage = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_damage;
            int BowBlunt = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_blunt;
            int BowSlash = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_slash;
            int BowPierce = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_pierce;
            int BowChop = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_chop;
            int BowPickaxe = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_pickaxe;
            int BowFire = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_fire;
            int BowFrost = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_frost;
            int BowLightning = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_lightning;
            int BowPoison = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_poison;
            int BowSpirit = (int)SvartalfrQueenBow.m_itemData.m_shared.m_damages.m_spirit;
            int BowRange = (int)SvartalfrQueenBow.m_itemData.m_shared.m_aiAttackRange;
            int BowRangeMin = (int)SvartalfrQueenBow.m_itemData.m_shared.m_aiAttackRangeMin;
            int BowInterval = (int)SvartalfrQueenBow.m_itemData.m_shared.m_aiAttackInterval;
            this.SvartalfrQueenAttack2Damage = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Damage", BowDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.SvartalfrQueenAttack2Blunt = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Blunt", BowBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.SvartalfrQueenAttack2Slash = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Slash", BowSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.SvartalfrQueenAttack2Pierce = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Pierce", BowPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.SvartalfrQueenAttack2Chop = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Chop", BowChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.SvartalfrQueenAttack2Pickaxe = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Pickaxe", BowPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.SvartalfrQueenAttack2Fire = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Fire", BowFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.SvartalfrQueenAttack2Frost = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Frost", BowFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.SvartalfrQueenAttack2Lightning = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Lightning", BowLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.SvartalfrQueenAttack2Poison = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Poison", BowPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.SvartalfrQueenAttack2Spirit = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "Spirit", BowSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.SvartalfrQueenAttack2Range = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "AI Attack range max", BowRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.SvartalfrQueenAttack2RangeMin = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "AI Attack range min", BowRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.SvartalfrQueenAttack2Interval = base.Config.Bind<int>("(1c) SvartalfrQueen Attack: SvartalfrQueenBow", "AI Attack Interval", BowInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop SvartalfrQueenRootspawn = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueen_rootspawn");
            int RootRange = (int)SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackRange;
            int RootRangeMin = (int)SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackRangeMin;
            int RootInterval = (int)SvartalfrQueenRootspawn.m_itemData.m_shared.m_aiAttackInterval;
            ItemDrop SvartalfrQueenRootspawn2 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvarTentaRoot_attack");
            int RootDamage = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_damage;
            int RootBlunt = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_blunt;
            int RootSlash = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_slash;
            int RootPierce = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_pierce;
            int RootChop = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_chop;
            int RootPickaxe = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_pickaxe;
            int RootFire = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_fire;
            int RootFrost = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_frost;
            int RootLightning = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_lightning;
            int RootPoison = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_poison;
            int RootSpirit = (int)SvartalfrQueenRootspawn2.m_itemData.m_shared.m_damages.m_spirit;


            this.SvartalfrQueenAttack3Damage = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Damage", RootDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.SvartalfrQueenAttack3Blunt = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Blunt", RootBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.SvartalfrQueenAttack3Slash = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Slash", RootSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.SvartalfrQueenAttack3Pierce = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Pierce", RootPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.SvartalfrQueenAttack3Chop = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Chop", RootChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.SvartalfrQueenAttack3Pickaxe = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Pickaxe", RootPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.SvartalfrQueenAttack3Fire = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Fire", RootFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.SvartalfrQueenAttack3Frost = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Frost", RootFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.SvartalfrQueenAttack3Lightning = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Lightning", RootLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.SvartalfrQueenAttack3Poison = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Poison", RootPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.SvartalfrQueenAttack3Spirit = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "Spirit", RootSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.SvartalfrQueenAttack3Range = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "AI Attack range max", RootRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.SvartalfrQueenAttack3RangeMin = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "AI Attack range min", RootRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.SvartalfrQueenAttack3Interval = base.Config.Bind<int>("(1d) SvartalfrQueen Attack: SvartalfrQueen_rootspawn", "AI Attack Interval", RootInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));


            ItemDrop SvartalfrQueenBowArrowStorm = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenBowArrowStorm");
            int Bow2Range = (int)SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRange;
            int Bow2RangeMin = (int)SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRangeMin;
            int Bow2Interval = (int)SvartalfrQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackInterval;
            Projectile bow_projectile_svar = PrefabManager.Cache.GetPrefab<Projectile>("bow_projectile_svar");
            int Bow2Damage = (int)bow_projectile_svar.m_damage.m_damage;
            int Bow2Blunt = (int)bow_projectile_svar.m_damage.m_blunt;
            int Bow2Slash = (int)bow_projectile_svar.m_damage.m_slash;
            int Bow2Pierce = (int)bow_projectile_svar.m_damage.m_pierce;
            int Bow2Chop = (int)bow_projectile_svar.m_damage.m_chop;
            int Bow2Pickaxe = (int)bow_projectile_svar.m_damage.m_pickaxe;
            int Bow2Fire = (int)bow_projectile_svar.m_damage.m_fire;
            int Bow2Frost = (int)bow_projectile_svar.m_damage.m_frost;
            int Bow2Lightning = (int)bow_projectile_svar.m_damage.m_lightning;
            int Bow2Poison = (int)bow_projectile_svar.m_damage.m_poison;
            int Bow2Spirit = (int)bow_projectile_svar.m_damage.m_spirit;



            this.SvartalfrQueenAttack4Damage = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Damage", Bow2Damage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.SvartalfrQueenAttack4Blunt = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Blunt", Bow2Blunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.SvartalfrQueenAttack4Slash = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Slash", Bow2Slash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.SvartalfrQueenAttack4Pierce = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Pierce", Bow2Pierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.SvartalfrQueenAttack4Chop = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Chop", Bow2Chop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.SvartalfrQueenAttack4Pickaxe = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Pickaxe", Bow2Pickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.SvartalfrQueenAttack4Fire = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Fire", Bow2Fire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.SvartalfrQueenAttack4Frost = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Frost", Bow2Frost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.SvartalfrQueenAttack4Lightning = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Lightning", Bow2Lightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.SvartalfrQueenAttack4Poison = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Poison", Bow2Poison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.SvartalfrQueenAttack4Spirit = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "Spirit", Bow2Spirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.SvartalfrQueenAttack4Range = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "AI Attack range max", Bow2Range, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.SvartalfrQueenAttack4RangeMin = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "AI Attack range min", Bow2RangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.SvartalfrQueenAttack4Interval = base.Config.Bind<int>("(1e) SvartalfrQueen Attack: SvartalfrQueenBowArrowStorm", "AI Attack Interval", Bow2Interval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));
        }

        private void ConfigJotunn()
        {
            Humanoid Jotunn = PrefabManager.Cache.GetPrefab<Humanoid>("Jotunn");
            int Health = (int)Jotunn.m_health;
            this.JotunnBossHealth = base.Config.Bind<int>("(2a) Jotunn Boss", "Health", Health, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));


            ItemDrop Jotunn_Groundslam = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Groundslam");
            int SlamDamage = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_damage;
            int SlamBlunt = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_blunt;
            int SlamSlash = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_slash;
            int SlamPierce = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_pierce;
            int SlamChop = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_chop;
            int SlamPickaxe = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_pickaxe;
            int SlamFire = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_fire;
            int SlamFrost = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_frost;
            int SlamLightning = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_lightning;
            int SlamPoison = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_poison;
            int SlamSpirit = (int)Jotunn_Groundslam.m_itemData.m_shared.m_damages.m_spirit;
            int SlamRange = (int)Jotunn_Groundslam.m_itemData.m_shared.m_aiAttackRange;
            int SlamRangeMin = (int)Jotunn_Groundslam.m_itemData.m_shared.m_aiAttackRangeMin;
            int SlamInterval = (int)Jotunn_Groundslam.m_itemData.m_shared.m_aiAttackInterval;
            this.JotunnAttack1Damage = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Damage", SlamDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.JotunnAttack1Blunt = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Blunt", SlamBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.JotunnAttack1Slash = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Slash", SlamSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.JotunnAttack1Pierce = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Pierce", SlamPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.JotunnAttack1Chop = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Chop", SlamChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.JotunnAttack1Pickaxe = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Pickaxe", SlamPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.JotunnAttack1Fire = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Fire", SlamFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.JotunnAttack1Frost = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Frost", SlamFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.JotunnAttack1Lightning = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Lightning", SlamLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.JotunnAttack1Poison = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Poison", SlamPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.JotunnAttack1Spirit = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "Spirit", SlamSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.JotunnAttack1Range = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "AI Attack range max", SlamRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.JotunnAttack1RangeMin = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "AI Attack range min", SlamRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.JotunnAttack1Interval = base.Config.Bind<int>("(2b) Jotunn Attack: Jotunn_Groundslam", "AI Attack Interval", SlamInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop Jotunn_GroundSlam2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Groundslam2");
            int Slam2Damage = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_damage;
            int Slam2Blunt = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_blunt;
            int Slam2Slash = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_slash;
            int Slam2Pierce = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_pierce;
            int Slam2Chop = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_chop;
            int Slam2Pickaxe = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_pickaxe;
            int Slam2Fire = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_fire;
            int Slam2Frost = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_frost;
            int Slam2Lightning = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_lightning;
            int Slam2Poison = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_poison;
            int Slam2Spirit = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_damages.m_spirit;
            int Slam2Range = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_aiAttackRange;
            int Slam2RangeMin = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_aiAttackRangeMin;
            int Slam2Interval = (int)Jotunn_GroundSlam2.m_itemData.m_shared.m_aiAttackInterval;
            this.JotunnAttack2Damage = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Damage", Slam2Damage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.JotunnAttack2Blunt = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Blunt", Slam2Blunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.JotunnAttack2Slash = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Slash", Slam2Slash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.JotunnAttack2Pierce = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Pierce", Slam2Pierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.JotunnAttack2Chop = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Chop", Slam2Chop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.JotunnAttack2Pickaxe = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Pickaxe", Slam2Pickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.JotunnAttack2Fire = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Fire", Slam2Fire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.JotunnAttack2Frost = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Frost", Slam2Frost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.JotunnAttack2Lightning = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Lightning", Slam2Lightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.JotunnAttack2Poison = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Poison", Slam2Poison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.JotunnAttack2Spirit = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "Spirit", Slam2Spirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.JotunnAttack2Range = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "AI Attack range max", Slam2Range, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.JotunnAttack2RangeMin = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "AI Attack range min", Slam2RangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.JotunnAttack2Interval = base.Config.Bind<int>("(2c) Jotunn Attack: Jotunn_GroundSlam2", "AI Attack Interval", Slam2Interval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop Jotunn_Shoot = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Shoot");
            int ShootDamage = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_damage;
            int ShootBlunt = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_blunt;
            int ShootSlash = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_slash;
            int ShootPierce = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_pierce;
            int ShootChop = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_chop;
            int ShootPickaxe = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_pickaxe;
            int ShootFire = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_fire;
            int ShootFrost = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_frost;
            int ShootLightning = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_lightning;
            int ShootPoison = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_poison;
            int ShootSpirit = (int)Jotunn_Shoot.m_itemData.m_shared.m_damages.m_spirit;
            int ShootRange = (int)Jotunn_Shoot.m_itemData.m_shared.m_aiAttackRange;
            int ShootRangeMin = (int)Jotunn_Shoot.m_itemData.m_shared.m_aiAttackRangeMin;
            int ShootInterval = (int)Jotunn_Shoot.m_itemData.m_shared.m_aiAttackInterval;
            this.JotunnAttack3Damage = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Damage", ShootDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.JotunnAttack3Blunt = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Blunt", ShootBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.JotunnAttack3Slash = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Slash", ShootSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.JotunnAttack3Pierce = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Pierce", ShootPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.JotunnAttack3Chop = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Chop", ShootChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.JotunnAttack3Pickaxe = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Pickaxe", ShootPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.JotunnAttack3Fire = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Fire", ShootFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.JotunnAttack3Frost = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Frost", ShootFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.JotunnAttack3Lightning = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Lightning", ShootLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.JotunnAttack3Poison = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Poison", ShootPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.JotunnAttack3Spirit = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "Spirit", ShootSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.JotunnAttack3Range = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "AI Attack range max", ShootRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.JotunnAttack3RangeMin = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "AI Attack range min", ShootRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.JotunnAttack3Interval = base.Config.Bind<int>("(2d) Jotunn Attack: Jotunn_Shoot", "AI Attack Interval", ShootInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));
        }

        private void ConfigBlazingDamnedOne()
        {
            Humanoid BlazingDamnedOne = PrefabManager.Cache.GetPrefab<Humanoid>("BlazingDamnedOne");
            int Health = (int)BlazingDamnedOne.m_health;
            this.BlazingDamnedOneBossHealth = base.Config.Bind<int>("(3a) BlazingDamnedOne Boss", "Health", Health, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));


            ItemDrop BlazingDamnedOneMace = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace");
            int MaceDamage = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_damage;
            int MaceBlunt = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_blunt;
            int MaceSlash = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_slash;
            int MacePierce = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_pierce;
            int MaceChop = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_chop;
            int MacePickaxe = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_pickaxe;
            int MaceFire = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_fire;
            int MaceFrost = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_frost;
            int MaceLightning = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_lightning;
            int MacePoison = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_poison;
            int MaceSpirit = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_damages.m_spirit;
            int MaceRange = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_aiAttackRange;
            int MaceRangeMin = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_aiAttackRangeMin;
            int MaceInterval = (int)BlazingDamnedOneMace.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack1Damage = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Damage", MaceDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack1Blunt = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Blunt", MaceBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack1Slash = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Slash", MaceSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack1Pierce = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Pierce", MacePierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack1Chop = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Chop", MaceChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack1Pickaxe = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Pickaxe", MacePickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack1Fire = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Fire", MaceFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack1Frost = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Frost", MaceFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack1Lightning = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Lightning", MaceLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack1Poison = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Poison", MacePoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack1Spirit = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "Spirit", MaceSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack1Range = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "AI Attack range max", MaceRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack1RangeMin = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "AI Attack range min", MaceRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack1Interval = base.Config.Bind<int>("(3b) BlazingDamnedOne Attack: BlazingDamnedOneMace(swing_axe2)", "AI Attack Interval", MaceInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop BlazingDamnedOneMace2 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace2");
            int Mace2Damage = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_damage;
            int Mace2Blunt = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_blunt;
            int Mace2Slash = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_slash;
            int Mace2Pierce = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_pierce;
            int Mace2Chop = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_chop;
            int Mace2Pickaxe = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_pickaxe;
            int Mace2Fire = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_fire;
            int Mace2Frost = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_frost;
            int Mace2Lightning = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_lightning;
            int Mace2Poison = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_poison;
            int Mace2Spirit = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_damages.m_spirit;
            int Mace2Range = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_aiAttackRange;
            int Mace2RangeMin = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_aiAttackRangeMin;
            int Mace2Interval = (int)BlazingDamnedOneMace2.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack2Damage = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Damage", Mace2Damage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack2Blunt = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Blunt", Mace2Blunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack2Slash = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Slash", Mace2Slash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack2Pierce = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Pierce", Mace2Pierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack2Chop = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Chop", Mace2Chop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack2Pickaxe = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Pickaxe", Mace2Pickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack2Fire = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Fire", Mace2Fire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack2Frost = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Frost", Mace2Frost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack2Lightning = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Lightning", Mace2Lightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack2Poison = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Poison", Mace2Poison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack2Spirit = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "Spirit", Mace2Spirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack2Range = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "AI Attack range max", Mace2Range, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack2RangeMin = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "AI Attack range min", Mace2RangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack2Interval = base.Config.Bind<int>("(3c) BlazingDamnedOne Attack: BlazingDamnedOneMace2(battleaxe_attack1)", "AI Attack Interval", Mace2Interval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop BlazingDamnedOneMace3 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace3");
            int Mace3Damage = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_damage;
            int Mace3Blunt = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_blunt;
            int Mace3Slash = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_slash;
            int Mace3Pierce = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_pierce;
            int Mace3Chop = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_chop;
            int Mace3Pickaxe = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_pickaxe;
            int Mace3Fire = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_fire;
            int Mace3Frost = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_frost;
            int Mace3Lightning = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_lightning;
            int Mace3Poison = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_poison;
            int Mace3Spirit = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_damages.m_spirit;
            int Mace3Range = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_aiAttackRange;
            int Mace3RangeMin = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_aiAttackRangeMin;
            int Mace3Interval = (int)BlazingDamnedOneMace3.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack3Damage = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Damage", Mace3Damage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack3Blunt = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Blunt", Mace3Blunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack3Slash = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Slash", Mace3Slash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack3Pierce = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Pierce", Mace3Pierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack3Chop = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Chop", Mace3Chop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack3Pickaxe = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Pickaxe", Mace3Pickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack3Fire = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Fire", Mace3Fire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack3Frost = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Frost", Mace3Frost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack3Lightning = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Lightning", Mace3Lightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack3Poison = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Poison", Mace3Poison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack3Spirit = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "Spirit", Mace3Spirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack3Range = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "AI Attack range max", Mace3Range, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack3RangeMin = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "AI Attack range min", Mace3RangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack3Interval = base.Config.Bind<int>("(3d) BlazingDamnedOne Attack: BlazingDamnedOneMace3(jump_attack)", "AI Attack Interval", Mace3Interval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop BlazingDamnedOneMace4 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace4");
            int Mace4Damage = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_damage;
            int Mace4Blunt = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_blunt;
            int Mace4Slash = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_slash;
            int Mace4Pierce = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_pierce;
            int Mace4Chop = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_chop;
            int Mace4Pickaxe = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_pickaxe;
            int Mace4Fire = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_fire;
            int Mace4Frost = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_frost;
            int Mace4Lightning = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_lightning;
            int Mace4Poison = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_poison;
            int Mace4Spirit = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_damages.m_spirit;
            int Mace4Range = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_aiAttackRange;
            int Mace4RangeMin = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_aiAttackRangeMin;
            int Mace4Interval = (int)BlazingDamnedOneMace4.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack4Damage = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Damage", Mace4Damage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack4Blunt = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Blunt", Mace4Blunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack4Slash = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Slash", Mace4Slash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack4Pierce = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Pierce", Mace4Pierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack4Chop = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Chop", Mace4Chop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack4Pickaxe = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Pickaxe", Mace4Pickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack4Fire = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Fire", Mace4Fire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack4Frost = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Frost", Mace4Frost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack4Lightning = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Lightning", Mace4Lightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack4Poison = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Poison", Mace4Poison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack4Spirit = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "Spirit", Mace4Spirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack4Range = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "AI Attack range max", Mace4Range, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack4RangeMin = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "AI Attack range min", Mace4RangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack4Interval = base.Config.Bind<int>("(3e) BlazingDamnedOne Attack: BlazingDamnedOneMace4(swing_sledge)", "AI Attack Interval", Mace4Interval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop Blazing_Nova = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Nova");
            int NovaDamage = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_damage;
            int NovaBlunt = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_blunt;
            int NovaSlash = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_slash;
            int NovaPierce = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_pierce;
            int NovaChop = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_chop;
            int NovaPickaxe = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_pickaxe;
            int NovaFire = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_fire;
            int NovaFrost = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_frost;
            int NovaLightning = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_lightning;
            int NovaPoison = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_poison;
            int NovaSpirit = (int)Blazing_Nova.m_itemData.m_shared.m_damages.m_spirit;
            int NovaRange = (int)Blazing_Nova.m_itemData.m_shared.m_aiAttackRange;
            int NovaRangeMin = (int)Blazing_Nova.m_itemData.m_shared.m_aiAttackRangeMin;
            int NovaInterval = (int)Blazing_Nova.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack5Damage = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Damage", NovaDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack5Blunt = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Blunt", NovaBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack5Slash = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Slash", NovaSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack5Pierce = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Pierce", NovaPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack5Chop = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Chop", NovaChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack5Pickaxe = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Pickaxe", NovaPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack5Fire = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Fire", NovaFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack5Frost = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Frost", NovaFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack5Lightning = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Lightning", NovaLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack5Poison = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Poison", NovaPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack5Spirit = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "Spirit", NovaSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack5Range = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "AI Attack range max", NovaRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack5RangeMin = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "AI Attack range min", NovaRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack5Interval = base.Config.Bind<int>("(3f) BlazingDamnedOne Attack: Blazing_Nova", "AI Attack Interval", NovaInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop Blazing_Shoot = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Shoot");
            int ShootDamage = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_damage;
            int ShootBlunt = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_blunt;
            int ShootSlash = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_slash;
            int ShootPierce = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_pierce;
            int ShootChop = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_chop;
            int ShootPickaxe = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_pickaxe;
            int ShootFire = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_fire;
            int ShootFrost = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_frost;
            int ShootLightning = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_lightning;
            int ShootPoison = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_poison;
            int ShootSpirit = (int)Blazing_Shoot.m_itemData.m_shared.m_damages.m_spirit;
            int ShootRange = (int)Blazing_Shoot.m_itemData.m_shared.m_aiAttackRange;
            int ShootRangeMin = (int)Blazing_Shoot.m_itemData.m_shared.m_aiAttackRangeMin;
            int ShootInterval = (int)Blazing_Shoot.m_itemData.m_shared.m_aiAttackInterval;
            this.BlazingDamnedOneAttack6Damage = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Damage", ShootDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack6Blunt = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Blunt", ShootBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack6Slash = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Slash", ShootSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack6Pierce = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Pierce", ShootPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack6Chop = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Chop", ShootChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack6Pickaxe = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Pickaxe", ShootPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack6Fire = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Fire", ShootFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack6Frost = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Frost", ShootFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack6Lightning = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Lightning", ShootLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack6Poison = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Poison", ShootPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack6Spirit = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "Spirit", ShootSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack6Range = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "AI Attack range max", ShootRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack6RangeMin = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "AI Attack range min", ShootRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack6Interval = base.Config.Bind<int>("(3g) BlazingDamnedOne Attack: Blazing_Shoot", "AI Attack Interval", ShootInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));

            ItemDrop Blazing_Meteors = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Meteors");
            int MeteorsRange = (int)Blazing_Meteors.m_itemData.m_shared.m_aiAttackRange;
            int MeteorsRangeMin = (int)Blazing_Meteors.m_itemData.m_shared.m_aiAttackRangeMin;
            int MeteorsInterval = (int)Blazing_Meteors.m_itemData.m_shared.m_aiAttackInterval;
            Projectile projectile_meteor_blazing = PrefabManager.Cache.GetPrefab<Projectile>("projectile_meteor_blazing");
            int MeteorsDamage = (int)projectile_meteor_blazing.m_damage.m_damage;
            int MeteorsBlunt = (int)projectile_meteor_blazing.m_damage.m_blunt;
            int MeteorsSlash = (int)projectile_meteor_blazing.m_damage.m_slash;
            int MeteorsPierce = (int)projectile_meteor_blazing.m_damage.m_pierce;
            int MeteorsChop = (int)projectile_meteor_blazing.m_damage.m_chop;
            int MeteorsPickaxe = (int)projectile_meteor_blazing.m_damage.m_pickaxe;
            int MeteorsFire = (int)projectile_meteor_blazing.m_damage.m_fire;
            int MeteorsFrost = (int)projectile_meteor_blazing.m_damage.m_frost;
            int MeteorsLightning = (int)projectile_meteor_blazing.m_damage.m_lightning;
            int MeteorsPoison = (int)projectile_meteor_blazing.m_damage.m_poison;
            int MeteorsSpirit = (int)projectile_meteor_blazing.m_damage.m_spirit;
            this.BlazingDamnedOneAttack7Damage = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Damage", MeteorsDamage, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 14
                }
            }));
            this.BlazingDamnedOneAttack7Blunt = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Blunt", MeteorsBlunt, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 13
                }
            }));
            this.BlazingDamnedOneAttack7Slash = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Slash", MeteorsSlash, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 12
                }
            }));
            this.BlazingDamnedOneAttack7Pierce = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Pierce", MeteorsPierce, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 11
                }
            }));
            this.BlazingDamnedOneAttack7Chop = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Chop", MeteorsChop, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 10
                }
            }));
            this.BlazingDamnedOneAttack7Pickaxe = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Pickaxe", MeteorsPickaxe, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 9
                }
            }));
            this.BlazingDamnedOneAttack7Fire = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Fire", MeteorsFire, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 8
                }
            }));
            this.BlazingDamnedOneAttack7Frost = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Frost", MeteorsFrost, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 7
                }
            }));
            this.BlazingDamnedOneAttack7Lightning = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Lightning", MeteorsLightning, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 6
                }
            }));
            this.BlazingDamnedOneAttack7Poison = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Poison", MeteorsPoison, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 5
                }
            }));
            this.BlazingDamnedOneAttack7Spirit = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "Spirit", MeteorsSpirit, new ConfigDescription("", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 4
                }
            }));
            this.BlazingDamnedOneAttack7Range = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "AI Attack range max", MeteorsRange, new ConfigDescription("Max distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 3
                }
            }));
            this.BlazingDamnedOneAttack7RangeMin = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "AI Attack range min", MeteorsRangeMin, new ConfigDescription("Min distance from the player the attack starts", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 2
                }
            }));
            this.BlazingDamnedOneAttack7Interval = base.Config.Bind<int>("(3h) BlazingDamnedOne Attack: Blazing_Meteors", "AI Attack Interval", MeteorsInterval, new ConfigDescription("Interval between attacks", null, new object[]
            {
                new ConfigurationManagerAttributes
                {
                    IsAdminOnly = true,
                    Order = 1
                }
            }));
        }

        private void RegisterConfigValues()
        {
            Humanoid SvartalfrQueenHealth = PrefabManager.Cache.GetPrefab<Humanoid>("SvartalfrQueen");
            SvartalfrQueenHealth.m_health = SvartalfrQueenBossHealth.Value;

            ItemDrop SvartalfrQueenAttack1 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenGreatSword");
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_damage = SvartalfrQueenAttack1Damage.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_blunt = SvartalfrQueenAttack1Blunt.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_slash = SvartalfrQueenAttack1Slash.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_pierce = SvartalfrQueenAttack1Pierce.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_chop = SvartalfrQueenAttack1Chop.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_pickaxe = SvartalfrQueenAttack1Pickaxe.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_fire = SvartalfrQueenAttack1Fire.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_frost = SvartalfrQueenAttack1Frost.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_lightning = SvartalfrQueenAttack1Lightning.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_poison = SvartalfrQueenAttack1Poison.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_damages.m_spirit = SvartalfrQueenAttack1Spirit.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_aiAttackRange = SvartalfrQueenAttack1Range.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_aiAttackRangeMin = SvartalfrQueenAttack1RangeMin.Value;
            SvartalfrQueenAttack1.m_itemData.m_shared.m_aiAttackInterval = SvartalfrQueenAttack1Interval.Value;

            ItemDrop SvartalfrQueenAttack2 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenBow");
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_damage = SvartalfrQueenAttack2Damage.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_blunt = SvartalfrQueenAttack2Blunt.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_slash = SvartalfrQueenAttack2Slash.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_pierce = SvartalfrQueenAttack2Pierce.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_chop = SvartalfrQueenAttack2Chop.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_pickaxe = SvartalfrQueenAttack2Pickaxe.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_fire = SvartalfrQueenAttack2Fire.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_frost = SvartalfrQueenAttack2Frost.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_lightning = SvartalfrQueenAttack2Lightning.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_poison = SvartalfrQueenAttack2Poison.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_damages.m_spirit = SvartalfrQueenAttack2Spirit.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_aiAttackRange = SvartalfrQueenAttack2Range.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_aiAttackRangeMin = SvartalfrQueenAttack2RangeMin.Value;
            SvartalfrQueenAttack2.m_itemData.m_shared.m_aiAttackInterval = SvartalfrQueenAttack2Interval.Value;

            ItemDrop SvartalfrQueenAttack3 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueen_rootspawn");
            ItemDrop SvartalfrQueenAttack3a = PrefabManager.Cache.GetPrefab<ItemDrop>("SvarTentaRoot_attack");
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_damage = SvartalfrQueenAttack3Damage.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_blunt = SvartalfrQueenAttack3Blunt.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_slash = SvartalfrQueenAttack3Slash.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_pierce = SvartalfrQueenAttack3Pierce.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_chop = SvartalfrQueenAttack3Chop.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_pickaxe = SvartalfrQueenAttack3Pickaxe.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_fire = SvartalfrQueenAttack3Fire.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_frost = SvartalfrQueenAttack3Frost.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_lightning = SvartalfrQueenAttack3Lightning.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_poison = SvartalfrQueenAttack3Poison.Value;
            SvartalfrQueenAttack3a.m_itemData.m_shared.m_damages.m_spirit = SvartalfrQueenAttack3Spirit.Value;
            SvartalfrQueenAttack3.m_itemData.m_shared.m_aiAttackRange = SvartalfrQueenAttack3Range.Value;
            SvartalfrQueenAttack3.m_itemData.m_shared.m_aiAttackRangeMin = SvartalfrQueenAttack3RangeMin.Value;
            SvartalfrQueenAttack3.m_itemData.m_shared.m_aiAttackInterval = SvartalfrQueenAttack3Interval.Value;

            ItemDrop SvartalfrQueenAttack4 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfrQueenBowArrowStorm");
            Projectile SvartalfrQueenAttack4a = PrefabManager.Cache.GetPrefab<Projectile>("bow_projectile_svar");
            SvartalfrQueenAttack4a.m_damage.m_damage = SvartalfrQueenAttack4Damage.Value;
            SvartalfrQueenAttack4a.m_damage.m_blunt = SvartalfrQueenAttack4Blunt.Value;
            SvartalfrQueenAttack4a.m_damage.m_slash = SvartalfrQueenAttack4Slash.Value;
            SvartalfrQueenAttack4a.m_damage.m_pierce = SvartalfrQueenAttack4Pierce.Value;
            SvartalfrQueenAttack4a.m_damage.m_chop = SvartalfrQueenAttack4Chop.Value;
            SvartalfrQueenAttack4a.m_damage.m_pickaxe = SvartalfrQueenAttack4Pickaxe.Value;
            SvartalfrQueenAttack4a.m_damage.m_fire = SvartalfrQueenAttack4Fire.Value;
            SvartalfrQueenAttack4a.m_damage.m_frost = SvartalfrQueenAttack4Frost.Value;
            SvartalfrQueenAttack4a.m_damage.m_lightning = SvartalfrQueenAttack4Lightning.Value;
            SvartalfrQueenAttack4a.m_damage.m_poison = SvartalfrQueenAttack4Poison.Value;
            SvartalfrQueenAttack4a.m_damage.m_spirit = SvartalfrQueenAttack4Spirit.Value;
            SvartalfrQueenAttack4.m_itemData.m_shared.m_aiAttackRange = SvartalfrQueenAttack4Range.Value;
            SvartalfrQueenAttack4.m_itemData.m_shared.m_aiAttackRangeMin = SvartalfrQueenAttack4RangeMin.Value;
            SvartalfrQueenAttack4.m_itemData.m_shared.m_aiAttackInterval = SvartalfrQueenAttack4Interval.Value;



            Humanoid JotunnHealth = PrefabManager.Cache.GetPrefab<Humanoid>("Jotunn");
            JotunnHealth.m_health = JotunnBossHealth.Value;

            ItemDrop JotunnAttack1 = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Groundslam");
            JotunnAttack1.m_itemData.m_shared.m_damages.m_damage = JotunnAttack1Damage.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_blunt = JotunnAttack1Blunt.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_slash = JotunnAttack1Slash.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_pierce = JotunnAttack1Pierce.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_chop = JotunnAttack1Chop.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_pickaxe = JotunnAttack1Pickaxe.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_fire = JotunnAttack1Fire.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_frost = JotunnAttack1Frost.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_lightning = JotunnAttack1Lightning.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_poison = JotunnAttack1Poison.Value;
            JotunnAttack1.m_itemData.m_shared.m_damages.m_spirit = JotunnAttack1Spirit.Value;
            JotunnAttack1.m_itemData.m_shared.m_aiAttackRange = JotunnAttack1Range.Value;
            JotunnAttack1.m_itemData.m_shared.m_aiAttackRangeMin = JotunnAttack1RangeMin.Value;
            JotunnAttack1.m_itemData.m_shared.m_aiAttackInterval = JotunnAttack1Interval.Value;

            ItemDrop JotunnAttack2 = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Groundslam2");
            JotunnAttack2.m_itemData.m_shared.m_damages.m_damage = JotunnAttack2Damage.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_blunt = JotunnAttack2Blunt.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_slash = JotunnAttack2Slash.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_pierce = JotunnAttack2Pierce.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_chop = JotunnAttack2Chop.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_pickaxe = JotunnAttack2Pickaxe.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_fire = JotunnAttack2Fire.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_frost = JotunnAttack2Frost.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_lightning = JotunnAttack2Lightning.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_poison = JotunnAttack2Poison.Value;
            JotunnAttack2.m_itemData.m_shared.m_damages.m_spirit = JotunnAttack2Spirit.Value;
            JotunnAttack2.m_itemData.m_shared.m_aiAttackRange = JotunnAttack2Range.Value;
            JotunnAttack2.m_itemData.m_shared.m_aiAttackRangeMin = JotunnAttack2RangeMin.Value;
            JotunnAttack2.m_itemData.m_shared.m_aiAttackInterval = JotunnAttack2Interval.Value;

            ItemDrop JotunnAttack3 = PrefabManager.Cache.GetPrefab<ItemDrop>("Jotunn_Shoot");
            JotunnAttack3.m_itemData.m_shared.m_damages.m_damage = JotunnAttack3Damage.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_blunt = JotunnAttack3Blunt.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_slash = JotunnAttack3Slash.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_pierce = JotunnAttack3Pierce.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_chop = JotunnAttack3Chop.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_pickaxe = JotunnAttack3Pickaxe.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_fire = JotunnAttack3Fire.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_frost = JotunnAttack3Frost.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_lightning = JotunnAttack3Lightning.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_poison = JotunnAttack3Poison.Value;
            JotunnAttack3.m_itemData.m_shared.m_damages.m_spirit = JotunnAttack3Spirit.Value;
            JotunnAttack3.m_itemData.m_shared.m_aiAttackRange = JotunnAttack3Range.Value;
            JotunnAttack3.m_itemData.m_shared.m_aiAttackRangeMin = JotunnAttack3RangeMin.Value;
            JotunnAttack3.m_itemData.m_shared.m_aiAttackInterval = JotunnAttack3Interval.Value;


            Humanoid BlazingDamnedOneHealth = PrefabManager.Cache.GetPrefab<Humanoid>("BlazingDamnedOne");
            BlazingDamnedOneHealth.m_health = BlazingDamnedOneBossHealth.Value;

            ItemDrop BlazingDamnedOneAttack1 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace");
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack1Damage.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack1Blunt.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack1Slash.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack1Pierce.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack1Chop.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack1Pickaxe.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack1Fire.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack1Frost.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack1Lightning.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack1Poison.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack1Spirit.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack1Range.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack1RangeMin.Value;
            BlazingDamnedOneAttack1.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack1Interval.Value;


            ItemDrop BlazingDamnedOneAttack2 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace2");
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack2Damage.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack2Blunt.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack2Slash.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack2Pierce.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack2Chop.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack2Pickaxe.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack2Fire.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack2Frost.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack2Lightning.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack2Poison.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack2Spirit.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack2Range.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack2RangeMin.Value;
            BlazingDamnedOneAttack2.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack2Interval.Value;

            ItemDrop BlazingDamnedOneAttack3 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace3");
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack3Damage.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack3Blunt.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack3Slash.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack3Pierce.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack3Chop.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack3Pickaxe.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack3Fire.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack3Frost.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack3Lightning.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack3Poison.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack3Spirit.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack3Range.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack3RangeMin.Value;
            BlazingDamnedOneAttack3.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack3Interval.Value;

            ItemDrop BlazingDamnedOneAttack4 = PrefabManager.Cache.GetPrefab<ItemDrop>("BlazingDamnedOneMace4");
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack4Damage.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack4Blunt.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack4Slash.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack4Pierce.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack4Chop.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack4Pickaxe.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack4Fire.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack4Frost.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack4Lightning.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack4Poison.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack4Spirit.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack4Range.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack4RangeMin.Value;
            BlazingDamnedOneAttack4.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack4Interval.Value;

            ItemDrop BlazingDamnedOneAttack5 = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Nova");
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack5Damage.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack5Blunt.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack5Slash.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack5Pierce.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack5Chop.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack5Pickaxe.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack5Fire.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack5Frost.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack5Lightning.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack5Poison.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack5Spirit.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack5Range.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack5RangeMin.Value;
            BlazingDamnedOneAttack5.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack5Interval.Value;

            ItemDrop BlazingDamnedOneAttack6 = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Shoot");
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_damage = BlazingDamnedOneAttack6Damage.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_blunt = BlazingDamnedOneAttack6Blunt.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_slash = BlazingDamnedOneAttack6Slash.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_pierce = BlazingDamnedOneAttack6Pierce.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_chop = BlazingDamnedOneAttack6Chop.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_pickaxe = BlazingDamnedOneAttack6Pickaxe.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_fire = BlazingDamnedOneAttack6Fire.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_frost = BlazingDamnedOneAttack6Frost.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_lightning = BlazingDamnedOneAttack6Lightning.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_poison = BlazingDamnedOneAttack6Poison.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_damages.m_spirit = BlazingDamnedOneAttack6Spirit.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack6Range.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack6RangeMin.Value;
            BlazingDamnedOneAttack6.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack6Interval.Value;

            ItemDrop BlazingDamnedOneAttack7 = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Meteors");
            Projectile BlazingDamnedOneAttack7a = PrefabManager.Cache.GetPrefab<Projectile>("projectile_meteor_blazing");
            BlazingDamnedOneAttack7a.m_damage.m_damage = BlazingDamnedOneAttack7Damage.Value;
            BlazingDamnedOneAttack7a.m_damage.m_blunt = BlazingDamnedOneAttack7Blunt.Value;
            BlazingDamnedOneAttack7a.m_damage.m_slash = BlazingDamnedOneAttack7Slash.Value;
            BlazingDamnedOneAttack7a.m_damage.m_pierce = BlazingDamnedOneAttack7Pierce.Value;
            BlazingDamnedOneAttack7a.m_damage.m_chop = BlazingDamnedOneAttack7Chop.Value;
            BlazingDamnedOneAttack7a.m_damage.m_pickaxe = BlazingDamnedOneAttack7Pickaxe.Value;
            BlazingDamnedOneAttack7a.m_damage.m_fire = BlazingDamnedOneAttack7Fire.Value;
            BlazingDamnedOneAttack7a.m_damage.m_frost = BlazingDamnedOneAttack7Frost.Value;
            BlazingDamnedOneAttack7a.m_damage.m_lightning = BlazingDamnedOneAttack7Lightning.Value;
            BlazingDamnedOneAttack7a.m_damage.m_poison = BlazingDamnedOneAttack7Poison.Value;
            BlazingDamnedOneAttack7a.m_damage.m_spirit = BlazingDamnedOneAttack7Spirit.Value;
            BlazingDamnedOneAttack7.m_itemData.m_shared.m_aiAttackRange = BlazingDamnedOneAttack7Range.Value;
            BlazingDamnedOneAttack7.m_itemData.m_shared.m_aiAttackRangeMin = BlazingDamnedOneAttack7RangeMin.Value;
            BlazingDamnedOneAttack7.m_itemData.m_shared.m_aiAttackInterval = BlazingDamnedOneAttack7Interval.Value;





        }

        public ConfigEntry<int> SvartalfrQueenBossHealth;

        public ConfigEntry<int> SvartalfrQueenAttack1Damage;
        public ConfigEntry<int> SvartalfrQueenAttack1Blunt;
        public ConfigEntry<int> SvartalfrQueenAttack1Slash;
        public ConfigEntry<int> SvartalfrQueenAttack1Pierce;
        public ConfigEntry<int> SvartalfrQueenAttack1Chop;
        public ConfigEntry<int> SvartalfrQueenAttack1Pickaxe;
        public ConfigEntry<int> SvartalfrQueenAttack1Fire;
        public ConfigEntry<int> SvartalfrQueenAttack1Frost;
        public ConfigEntry<int> SvartalfrQueenAttack1Lightning;
        public ConfigEntry<int> SvartalfrQueenAttack1Poison;
        public ConfigEntry<int> SvartalfrQueenAttack1Spirit;
        public ConfigEntry<int> SvartalfrQueenAttack1Range;
        public ConfigEntry<int> SvartalfrQueenAttack1RangeMin;
        public ConfigEntry<int> SvartalfrQueenAttack1Interval;

        public ConfigEntry<int> SvartalfrQueenAttack2Damage;
        public ConfigEntry<int> SvartalfrQueenAttack2Blunt;
        public ConfigEntry<int> SvartalfrQueenAttack2Slash;
        public ConfigEntry<int> SvartalfrQueenAttack2Pierce;
        public ConfigEntry<int> SvartalfrQueenAttack2Chop;
        public ConfigEntry<int> SvartalfrQueenAttack2Pickaxe;
        public ConfigEntry<int> SvartalfrQueenAttack2Fire;
        public ConfigEntry<int> SvartalfrQueenAttack2Frost;
        public ConfigEntry<int> SvartalfrQueenAttack2Lightning;
        public ConfigEntry<int> SvartalfrQueenAttack2Poison;
        public ConfigEntry<int> SvartalfrQueenAttack2Spirit;
        public ConfigEntry<int> SvartalfrQueenAttack2Range;
        public ConfigEntry<int> SvartalfrQueenAttack2RangeMin;
        public ConfigEntry<int> SvartalfrQueenAttack2Interval;

        public ConfigEntry<int> SvartalfrQueenAttack3Damage;
        public ConfigEntry<int> SvartalfrQueenAttack3Blunt;
        public ConfigEntry<int> SvartalfrQueenAttack3Slash;
        public ConfigEntry<int> SvartalfrQueenAttack3Pierce;
        public ConfigEntry<int> SvartalfrQueenAttack3Chop;
        public ConfigEntry<int> SvartalfrQueenAttack3Pickaxe;
        public ConfigEntry<int> SvartalfrQueenAttack3Fire;
        public ConfigEntry<int> SvartalfrQueenAttack3Frost;
        public ConfigEntry<int> SvartalfrQueenAttack3Lightning;
        public ConfigEntry<int> SvartalfrQueenAttack3Poison;
        public ConfigEntry<int> SvartalfrQueenAttack3Spirit;
        public ConfigEntry<int> SvartalfrQueenAttack3Range;
        public ConfigEntry<int> SvartalfrQueenAttack3RangeMin;
        public ConfigEntry<int> SvartalfrQueenAttack3Interval;

        public ConfigEntry<int> SvartalfrQueenAttack4Damage;
        public ConfigEntry<int> SvartalfrQueenAttack4Blunt;
        public ConfigEntry<int> SvartalfrQueenAttack4Slash;
        public ConfigEntry<int> SvartalfrQueenAttack4Pierce;
        public ConfigEntry<int> SvartalfrQueenAttack4Chop;
        public ConfigEntry<int> SvartalfrQueenAttack4Pickaxe;
        public ConfigEntry<int> SvartalfrQueenAttack4Fire;
        public ConfigEntry<int> SvartalfrQueenAttack4Frost;
        public ConfigEntry<int> SvartalfrQueenAttack4Lightning;
        public ConfigEntry<int> SvartalfrQueenAttack4Poison;
        public ConfigEntry<int> SvartalfrQueenAttack4Spirit;
        public ConfigEntry<int> SvartalfrQueenAttack4Range;
        public ConfigEntry<int> SvartalfrQueenAttack4RangeMin;
        public ConfigEntry<int> SvartalfrQueenAttack4Interval;

        public ConfigEntry<int> JotunnBossHealth;

        public ConfigEntry<int> JotunnAttack1Damage;
        public ConfigEntry<int> JotunnAttack1Blunt;
        public ConfigEntry<int> JotunnAttack1Slash;
        public ConfigEntry<int> JotunnAttack1Pierce;
        public ConfigEntry<int> JotunnAttack1Chop;
        public ConfigEntry<int> JotunnAttack1Pickaxe;
        public ConfigEntry<int> JotunnAttack1Fire;
        public ConfigEntry<int> JotunnAttack1Frost;
        public ConfigEntry<int> JotunnAttack1Lightning;
        public ConfigEntry<int> JotunnAttack1Poison;
        public ConfigEntry<int> JotunnAttack1Spirit;
        public ConfigEntry<int> JotunnAttack1Range;
        public ConfigEntry<int> JotunnAttack1RangeMin;
        public ConfigEntry<int> JotunnAttack1Interval;

        public ConfigEntry<int> JotunnAttack2Damage;
        public ConfigEntry<int> JotunnAttack2Blunt;
        public ConfigEntry<int> JotunnAttack2Slash;
        public ConfigEntry<int> JotunnAttack2Pierce;
        public ConfigEntry<int> JotunnAttack2Chop;
        public ConfigEntry<int> JotunnAttack2Pickaxe;
        public ConfigEntry<int> JotunnAttack2Fire;
        public ConfigEntry<int> JotunnAttack2Frost;
        public ConfigEntry<int> JotunnAttack2Lightning;
        public ConfigEntry<int> JotunnAttack2Poison;
        public ConfigEntry<int> JotunnAttack2Spirit;
        public ConfigEntry<int> JotunnAttack2Range;
        public ConfigEntry<int> JotunnAttack2RangeMin;
        public ConfigEntry<int> JotunnAttack2Interval;

        public ConfigEntry<int> JotunnAttack3Damage;
        public ConfigEntry<int> JotunnAttack3Blunt;
        public ConfigEntry<int> JotunnAttack3Slash;
        public ConfigEntry<int> JotunnAttack3Pierce;
        public ConfigEntry<int> JotunnAttack3Chop;
        public ConfigEntry<int> JotunnAttack3Pickaxe;
        public ConfigEntry<int> JotunnAttack3Fire;
        public ConfigEntry<int> JotunnAttack3Frost;
        public ConfigEntry<int> JotunnAttack3Lightning;
        public ConfigEntry<int> JotunnAttack3Poison;
        public ConfigEntry<int> JotunnAttack3Spirit;
        public ConfigEntry<int> JotunnAttack3Range;
        public ConfigEntry<int> JotunnAttack3RangeMin;
        public ConfigEntry<int> JotunnAttack3Interval;

        public ConfigEntry<int> BlazingDamnedOneBossHealth;

        public ConfigEntry<int> BlazingDamnedOneAttack1Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack1Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack1Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack1Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack1Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack1Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack1Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack1Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack1Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack1Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack1Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack1Range;
        public ConfigEntry<int> BlazingDamnedOneAttack1RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack1Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack2Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack2Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack2Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack2Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack2Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack2Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack2Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack2Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack2Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack2Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack2Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack2Range;
        public ConfigEntry<int> BlazingDamnedOneAttack2RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack2Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack3Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack3Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack3Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack3Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack3Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack3Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack3Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack3Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack3Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack3Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack3Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack3Range;
        public ConfigEntry<int> BlazingDamnedOneAttack3RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack3Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack4Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack4Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack4Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack4Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack4Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack4Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack4Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack4Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack4Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack4Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack4Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack4Range;
        public ConfigEntry<int> BlazingDamnedOneAttack4RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack4Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack5Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack5Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack5Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack5Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack5Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack5Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack5Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack5Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack5Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack5Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack5Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack5Range;
        public ConfigEntry<int> BlazingDamnedOneAttack5RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack5Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack6Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack6Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack6Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack6Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack6Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack6Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack6Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack6Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack6Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack6Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack6Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack6Range;
        public ConfigEntry<int> BlazingDamnedOneAttack6RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack6Interval;

        public ConfigEntry<int> BlazingDamnedOneAttack7Damage;
        public ConfigEntry<int> BlazingDamnedOneAttack7Blunt;
        public ConfigEntry<int> BlazingDamnedOneAttack7Slash;
        public ConfigEntry<int> BlazingDamnedOneAttack7Pierce;
        public ConfigEntry<int> BlazingDamnedOneAttack7Chop;
        public ConfigEntry<int> BlazingDamnedOneAttack7Pickaxe;
        public ConfigEntry<int> BlazingDamnedOneAttack7Fire;
        public ConfigEntry<int> BlazingDamnedOneAttack7Frost;
        public ConfigEntry<int> BlazingDamnedOneAttack7Lightning;
        public ConfigEntry<int> BlazingDamnedOneAttack7Poison;
        public ConfigEntry<int> BlazingDamnedOneAttack7Spirit;
        public ConfigEntry<int> BlazingDamnedOneAttack7Range;
        public ConfigEntry<int> BlazingDamnedOneAttack7RangeMin;
        public ConfigEntry<int> BlazingDamnedOneAttack7Interval;


        private AssetBundle assetBundle;
    }
}