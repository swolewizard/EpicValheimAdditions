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
        private const string ModVersion = "1.9.3";
        private const string ModGUID = "Huntard.EpicValheimsAdditions";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        public static string configPath2 = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}_Content.json");
        private AssetBundle assetBundle;
        private Harmony _harmony;

        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);
            this.RegisterPrefabs();
            this.LoadContentConfig();
            this.CreateCraftingPieces();
            this.RegisterMiscItems();
            this.RegisterBossStuff();
            this.RegisterMistlands();
            this.RegisterDeepnorth();
            this.RegisterAshlands();
            this.CreateIngots_Scales();
            this.RegisterDeepAbyssWeapons();
            this.AddVegetation();
            this.LoadConfig();
            ZoneManager.OnVanillaLocationsAvailable += AddLocations;
            ZoneManager.OnVanillaLocationsAvailable += ModDrops;
            ZoneManager.OnVanillaLocationsAvailable += ModThorsForgeLevel;
            ZoneManager.OnVanillaLocationsAvailable += ModAlchemyLevel;

        }

        private void RegisterPrefabs()
        {
            Jotunn.Logger.LogInfo("Loading...");
            assetBundle = AssetUtils.LoadAssetBundleFromResources("eva_assets", typeof(Core).Assembly);
            Jotunn.Logger.LogInfo("Loaded Prefabs");
        }

        private void AddLocations()
        {
            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.MistlandsLocations == true)
                    {
                        GameObject MistlandsBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("SvartalfrQueenAltar_New"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(MistlandsBossAltar, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.Mistlands,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 4000f,
                            Unique = false,
                            Quantity = 3,
                            Priotized = true,
                            ExteriorRadius = 15f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                        GameObject MistlandsBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_SvartalfrQueen"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(MistlandsBossRuneStone, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.Mistlands,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 1000f,
                            Unique = false,
                            Quantity = 35,
                            Priotized = true,
                            ExteriorRadius = 12f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands locations are turned off");
                    }
                    if (config.DeepNorthLocations == true)
                    {
                        GameObject DeepNorthBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("JotunnAltar"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(DeepNorthBossAltar, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.DeepNorth,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 4000f,
                            Unique = false,
                            Quantity = 3,
                            Priotized = true,
                            ExteriorRadius = 20f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                        GameObject DeepNorthBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_Jotunn"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(DeepNorthBossRuneStone, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.DeepNorth,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 1000f,
                            Unique = false,
                            Quantity = 15,
                            Priotized = true,
                            ExteriorRadius = 6f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth locations are turned off");
                    }
                    if (config.AshlandsLocations == true)
                    {
                        GameObject AshlandsBossAltar = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("BlazingDamnedOneAltar"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(AshlandsBossAltar, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.AshLands,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 4000f,
                            Unique = false,
                            Quantity = 3,
                            Priotized = true,
                            ExteriorRadius = 15f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                        GameObject AshlandsBossRuneStone = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("Vegvisir_BlazingDamnedOne"));
                        ZoneManager.Instance.AddCustomLocation(new CustomLocation(AshlandsBossRuneStone, true, new LocationConfig
                        {
                            Biome = Heightmap.Biome.AshLands,
                            MaxAltitude = 1000f,
                            MinDistanceFromSimilar = 1000f,
                            Unique = false,
                            Quantity = 15,
                            Priotized = true,
                            ExteriorRadius = 4f,
                            RandomRotation = false,
                            MinAltitude = 1f,
                            ClearArea = true
                        }));
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands locations are turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.MistlandsLocations},{config.DeepNorthLocations},{config.AshlandsLocations} failed. {e.Message} {e.StackTrace}");
                }
            }

            GameObject GoldenGreydwarfalter = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("StaminaGreydwarf"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(GoldenGreydwarfalter, true, new LocationConfig
            {
                Biome = Heightmap.Biome.Meadows,
                MaxAltitude = 1000f,
                MinDistanceFromSimilar = 150f,
                Unique = false,
                Quantity = 15,
                Priotized = true,
                ExteriorRadius = 4f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            }));

            GameObject GoldenTrollalter = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("StaminaTroll"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(GoldenTrollalter, true, new LocationConfig
            {
                Biome = Heightmap.Biome.BlackForest,
                MaxAltitude = 8500f,
                MinDistanceFromSimilar = 150f,
                Unique = false,
                Quantity = 15,
                Priotized = true,
                ExteriorRadius = 4f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            }));

            GameObject GoldenWraithalter = ZoneManager.Instance.CreateLocationContainer(assetBundle.LoadAsset<GameObject>("StaminaWraith"));
            ZoneManager.Instance.AddCustomLocation(new CustomLocation(GoldenWraithalter, true, new LocationConfig
            {
                Biome = Heightmap.Biome.Meadows,
                MaxAltitude = 8500f,
                MinDistanceFromSimilar = 150f,
                Unique = false,
                Quantity = 15,
                Priotized = true,
                ExteriorRadius = 4f,
                RandomRotation = false,
                MinAltitude = 1f,
                ClearArea = true
            }));


            Jotunn.Logger.LogInfo("Loaded Locations");

        }

        private void AddVegetation()
        {
            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.MistlandsLocations == true)
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
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands locations are turned off");
                    }
                    if (config.DeepNorthLocations == true)
                    {
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
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth locations are turned off");
                    }
                    if (config.AshlandsLocations == true)
                    {
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
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands locations are turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.MistlandsLocations},{config.DeepNorthLocations},{config.AshlandsLocations} failed. {e.Message} {e.StackTrace}");
                }
            }


            Jotunn.Logger.LogInfo("Loaded Vegetation");
        }

        private void ModDrops()
        {
            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if(config.MistlandsLocations == false)
                    {
                        ItemDrop prefabpick = PrefabManager.Cache.GetPrefab<ItemDrop>("PickaxeBlackMetal");
                        prefabpick.m_itemData.m_shared.m_toolTier = 5;
                    }

                    if (config.DeepNorth == true)
                    {
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
                    }
                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Mistlands},{config.DeepNorth} failed. {e.Message} {e.StackTrace}");
                }
            }
        }

        private void CreateCraftingPieces()
        {
            GameObject gameObject = assetBundle.LoadAsset<GameObject>("piece_alchemystation");
            Piece component = gameObject.GetComponent<Piece>();
            component.m_name = "Inscription Table";
            component.m_description = "Rune Crafting/Upgrading.";
            CraftingStation component2 = gameObject.GetComponent<CraftingStation>();
            component2.m_name = "Inscription Table";
            CustomPiece customPiece = new CustomPiece(gameObject, true, new PieceConfig
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
            GameObject gameObject5 = assetBundle.LoadAsset<GameObject>("piece_thorsforge");
            CustomPiece customPiece5 = new CustomPiece(gameObject5, true, new PieceConfig
            {
                PieceTable = "_HammerPieceTable",
                AllowedInDungeons = false,
                Requirements = new RequirementConfig[] {

                        new RequirementConfig {
                            Item = "YggdrasilWood",
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

        private void ModThorsForgeLevel()
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
        }

        private void ModAlchemyLevel()
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

        }

        private void CreateIngots_Scales()
        {

            GameObject gameObject4 = assetBundle.LoadAsset<GameObject>("Heavyscale");
            ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
            component4.m_itemData.m_dropPrefab = gameObject4;
            component4.m_itemData.m_shared.m_name = "Heavyscale";
            component4.m_itemData.m_shared.m_description = "A scale, which is quite heavy";
            CustomItem customItem4 = new CustomItem(gameObject4, true);
            ItemManager.Instance.AddItem(customItem4);

            GameObject gameObject6 = assetBundle.LoadAsset<GameObject>("Drakescale");
            ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
            component6.m_itemData.m_dropPrefab = gameObject6;
            component6.m_itemData.m_shared.m_name = "Drakescale";
            component6.m_itemData.m_shared.m_description = "A frosty scale, cold to the touch";
            CustomItem customItem6 = new CustomItem(gameObject6, true);
            ItemManager.Instance.AddItem(customItem6);

            GameObject gameObject8 = assetBundle.LoadAsset<GameObject>("Forgedscale");
            ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
            component8.m_itemData.m_dropPrefab = gameObject8;
            component8.m_itemData.m_shared.m_name = "Forgedscale";
            component8.m_itemData.m_shared.m_description = "A scale, forged by a master";
            CustomItem customItem8 = new CustomItem(gameObject8, true);
            ItemManager.Instance.AddItem(customItem8);

            GameObject oreheavymetal = assetBundle.LoadAsset<GameObject>("OreHeavymetal");
            CustomItem oreheavymetal1 = new CustomItem(oreheavymetal, true);
            ItemManager.Instance.AddItem(oreheavymetal1);

            GameObject gameObject33 = assetBundle.LoadAsset<GameObject>("HeavymetalBar");
            CustomItem customItem33 = new CustomItem(gameObject33, true);
            ItemManager.Instance.AddItem(customItem33);

            GameObject orefrometal = assetBundle.LoadAsset<GameObject>("OreFrometal");
            CustomItem orefrometal1 = new CustomItem(orefrometal, true);
            ItemManager.Instance.AddItem(orefrometal1);

            GameObject gameObject5 = assetBundle.LoadAsset<GameObject>("FrometalBar");
            CustomItem customItem5 = new CustomItem(gameObject5, true);
            ItemManager.Instance.AddItem(customItem5);

            GameObject gameObject2 = assetBundle.LoadAsset<GameObject>("DeepAbyssEssence");
            CustomItem customItem2 = new CustomItem(gameObject2, true, new ItemConfig
            {
                Amount = 5,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "FrometalBar",
                                Amount = 5
                        },
                        new RequirementConfig {
                            Item = "Flametal",
                                Amount = 5
                        },
                        new RequirementConfig {
                            Item = "TrophyHelDemon",
                                Amount = 1
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem2);

            Jotunn.Logger.LogInfo("Loaded Ingots/scales/ore");
        }

        private void RegisterDeepAbyssWeapons()
        {
            GameObject gameObject = assetBundle.LoadAsset<GameObject>("TridentDeepAbyss");
            CustomItem customItem = new CustomItem(gameObject, true, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "DeepAbyssEssence",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "YggdrasilWood",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem);

            GameObject gameObject2 = assetBundle.LoadAsset<GameObject>("BowDeepAbyss");
            CustomItem customItem2 = new CustomItem(gameObject2, true, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "DeepAbyssEssence",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "YggdrasilWood",
                                Amount = 8,
                                AmountPerLevel = 4
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem2);

            GameObject gameObject3 = assetBundle.LoadAsset<GameObject>("GreatSwordDeepAbyss");
            CustomItem customItem3 = new CustomItem(gameObject3, true, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_thorsforge",
                Requirements = new RequirementConfig[] {
                        new RequirementConfig {
                            Item = "DeepAbyssEssence",
                                Amount = 60,
                                AmountPerLevel = 40
                        },
                        new RequirementConfig {
                            Item = "YggdrasilWood",
                                Amount = 40,
                                AmountPerLevel = 20
                        }
                    }
            });
            ItemManager.Instance.AddItem(customItem3);
            Jotunn.Logger.LogInfo("Loaded DeepAbyssWeapons");
        }

        private void RegisterBossStuff()
        {
            GameObject Golden_Greydwarf_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Greydwarf_Miniboss");
            CustomPrefab Golden_Greydwarf_Miniboss1 = new CustomPrefab(Golden_Greydwarf_Miniboss, true);
            PrefabManager.Instance.AddPrefab(Golden_Greydwarf_Miniboss1);
            GameObject Golden_Troll_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Troll_Miniboss");
            CustomPrefab Golden_Troll_Miniboss1 = new CustomPrefab(Golden_Troll_Miniboss, true);
            PrefabManager.Instance.AddPrefab(Golden_Troll_Miniboss1);
            GameObject Golden_Wraith_Miniboss = assetBundle.LoadAsset<GameObject>("Golden_Wraith_Miniboss");
            CustomPrefab Golden_Wraith_Miniboss1 = new CustomPrefab(Golden_Wraith_Miniboss, true);
            PrefabManager.Instance.AddPrefab(Golden_Wraith_Miniboss1);
            GameObject JotunnBoss = assetBundle.LoadAsset<GameObject>("Jotunn");
            CustomPrefab JotunnBoss1 = new CustomPrefab(JotunnBoss, true);
            PrefabManager.Instance.AddPrefab(JotunnBoss1);

            GameObject Jotunn_Groundslam = assetBundle.LoadAsset<GameObject>("Jotunn_Groundslam");
            CustomItem Jotunn_Groundslam1 = new CustomItem(Jotunn_Groundslam, true);
            ItemManager.Instance.AddItem(Jotunn_Groundslam1);
            GameObject Jotunn_Groundslam2 = assetBundle.LoadAsset<GameObject>("Jotunn_Groundslam2");
            CustomItem Jotunn_Groundslam21 = new CustomItem(Jotunn_Groundslam2, true);
            ItemManager.Instance.AddItem(Jotunn_Groundslam21);
            GameObject Jotunn_Shoot = assetBundle.LoadAsset<GameObject>("Jotunn_Shoot");
            CustomItem Jotunn_Shoot1 = new CustomItem(Jotunn_Shoot, true);
            ItemManager.Instance.AddItem(Jotunn_Shoot1);

            GameObject HelDemon = assetBundle.LoadAsset<GameObject>("HelDemon");
            CustomPrefab HelDemon1 = new CustomPrefab(HelDemon, true);
            PrefabManager.Instance.AddPrefab(HelDemon1);

            GameObject BlazingMace = assetBundle.LoadAsset<GameObject>("BlazingMace");
            CustomItem BlazingMace1 = new CustomItem(BlazingMace, true);
            ItemManager.Instance.AddItem(BlazingMace1);
            GameObject BlazingMacetwo = assetBundle.LoadAsset<GameObject>("BlazingMace2");
            CustomItem BlazingMace2 = new CustomItem(BlazingMacetwo, true);
            ItemManager.Instance.AddItem(BlazingMace2);
            GameObject BlazingMacethree = assetBundle.LoadAsset<GameObject>("BlazingMace3");
            CustomItem BlazingMace3 = new CustomItem(BlazingMacethree, true);
            ItemManager.Instance.AddItem(BlazingMace3);
            GameObject BlazingMacefour = assetBundle.LoadAsset<GameObject>("BlazingMace4");
            CustomItem BlazingMace4 = new CustomItem(BlazingMacefour, true);
            ItemManager.Instance.AddItem(BlazingMace4);
            GameObject BlazingShield = assetBundle.LoadAsset<GameObject>("BlazingShield");
            CustomItem BlazingShield1 = new CustomItem(BlazingShield, true);
            ItemManager.Instance.AddItem(BlazingShield1);
            GameObject EVA_BlazingHelm = assetBundle.LoadAsset<GameObject>("EVA_BlazingHelm");
            CustomItem EVA_BlazingHelm1 = new CustomItem(EVA_BlazingHelm, true);
            ItemManager.Instance.AddItem(EVA_BlazingHelm1);
            GameObject EVA_BlazingChest = assetBundle.LoadAsset<GameObject>("EVA_BlazingChest");
            CustomItem EVA_BlazingChest1 = new CustomItem(EVA_BlazingChest, true);
            ItemManager.Instance.AddItem(EVA_BlazingChest1);
            GameObject EVA_BlazingBoots = assetBundle.LoadAsset<GameObject>("EVA_BlazingBoots");
            CustomItem EVA_BlazingBoots1 = new CustomItem(EVA_BlazingBoots, true);
            ItemManager.Instance.AddItem(EVA_BlazingBoots1);
            GameObject Blazing_Nova = assetBundle.LoadAsset<GameObject>("Blazing_Nova");
            CustomItem Blazing_Nova1 = new CustomItem(Blazing_Nova, true);
            ItemManager.Instance.AddItem(Blazing_Nova1);
            GameObject Blazing_Meteors = assetBundle.LoadAsset<GameObject>("Blazing_Meteors");
            CustomItem Blazing_Meteors1 = new CustomItem(Blazing_Meteors, true);
            ItemManager.Instance.AddItem(Blazing_Meteors1);
            GameObject Blazing_Shoot = assetBundle.LoadAsset<GameObject>("Blazing_Shoot");
            CustomItem Blazing_Shoot1 = new CustomItem(Blazing_Shoot, true);
            ItemManager.Instance.AddItem(Blazing_Shoot1);

            GameObject SvartalfarQueen = assetBundle.LoadAsset<GameObject>("SvartalfarQueen");
            CustomPrefab SvartalfarQueen1 = new CustomPrefab(SvartalfarQueen, true);
            PrefabManager.Instance.AddPrefab(SvartalfarQueen1);
            GameObject SvarTentaRoot = assetBundle.LoadAsset<GameObject>("SvarTentaRoot");
            CustomPrefab SvarTentaRoot1 = new CustomPrefab(SvarTentaRoot, true);
            PrefabManager.Instance.AddPrefab(SvarTentaRoot1);

            GameObject SvartalfarQueenGreatSword = assetBundle.LoadAsset<GameObject>("SvartalfarQueenGreatSword");
            CustomItem SvartalfarQueenGreatSword1 = new CustomItem(SvartalfarQueenGreatSword, true);
            ItemManager.Instance.AddItem(SvartalfarQueenGreatSword1);
            GameObject SvartalfarQueenBow = assetBundle.LoadAsset<GameObject>("SvartalfarQueenBow");
            CustomItem SvartalfarQueenBow1 = new CustomItem(SvartalfarQueenBow, true);
            ItemManager.Instance.AddItem(SvartalfarQueenBow1);
            GameObject SvartalfarQueenBowArrowStorm = assetBundle.LoadAsset<GameObject>("SvartalfarQueenBowArrowStorm");
            CustomItem SvartalfarQueenBowArrowStorm1 = new CustomItem(SvartalfarQueenBowArrowStorm, true);
            ItemManager.Instance.AddItem(SvartalfarQueenBowArrowStorm1);
            GameObject SvartalfarQueen_rootspawn = assetBundle.LoadAsset<GameObject>("SvartalfarQueen_rootspawn");
            CustomItem SvartalfarQueen_rootspawn1 = new CustomItem(SvartalfarQueen_rootspawn, true);
            ItemManager.Instance.AddItem(SvartalfarQueen_rootspawn1);

            GameObject TrophyHelDemon = assetBundle.LoadAsset<GameObject>("TrophyHelDemon");
            CustomItem TrophyHelDemon1 = new CustomItem(TrophyHelDemon, true);
            ItemManager.Instance.AddItem(TrophyHelDemon1);

            GameObject TrophySvartalfarQueen = assetBundle.LoadAsset<GameObject>("TrophySvartalfarQueen");
            CustomItem TrophySvartalfarQueen1 = new CustomItem(TrophySvartalfarQueen, true);
            ItemManager.Instance.AddItem(TrophySvartalfarQueen1);

            GameObject TrophyJotunn = assetBundle.LoadAsset<GameObject>("TrophyJotunn");
            CustomItem TrophyJotunn1 = new CustomItem(TrophyJotunn, true);
            ItemManager.Instance.AddItem(TrophyJotunn1);

            Jotunn.Logger.LogInfo("Loaded BossStuff");
        }

        private void RegisterMiscItems()
        {
            GameObject errordrop = assetBundle.LoadAsset<GameObject>("ErrorDrop");
            CustomItem ErrorDrop = new CustomItem(errordrop, true);
            ItemManager.Instance.AddItem(ErrorDrop);

            GameObject EVA_AdminFood = assetBundle.LoadAsset<GameObject>("EVA_AdminFood");
            CustomItem EVA_AdminFood1 = new CustomItem(EVA_AdminFood, true);
            ItemManager.Instance.AddItem(EVA_AdminFood1);

            Jotunn.Logger.LogInfo("Loaded MiscItems");
        }

        private void RegisterMistlands()
        {
            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Mistlands == true)
                    {


                        CustomItemConversion OreHeavymetal = new CustomItemConversion(new SmelterConversionConfig
                        {
                            Station = "blastfurnace",
                            FromItem = "OreHeavymetal",
                            ToItem = "HeavymetalBar"
                        });
                        ItemManager.Instance.AddItemConversion(OreHeavymetal);

                        GameObject gameObject = assetBundle.LoadAsset<GameObject>("BowHeavymetal");
                        ItemDrop component = gameObject.GetComponent<ItemDrop>();
                        component.m_itemData.m_shared.m_maxDurability = 200;
                        component.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        component.m_itemData.m_shared.m_attack.m_bowDraw = true;
                        component.m_itemData.m_shared.m_attack.m_drawAnimationState = "bow_aim";
                        component.m_itemData.m_shared.m_attack.m_drawDurationMin = 2.3f;
                        component.m_itemData.m_shared.m_attack.m_drawStaminaDrain = 9;
                        CustomItem customItem = new CustomItem(gameObject, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject2 = assetBundle.LoadAsset<GameObject>("AtgeirHeavymetal");
                        ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
                        component2.m_itemData.m_shared.m_maxDurability = 200;
                        component2.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem2 = new CustomItem(gameObject2, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject3 = assetBundle.LoadAsset<GameObject>("SledgeHeavymetal");
                        ItemDrop component3 = gameObject3.GetComponent<ItemDrop>();
                        component3.m_itemData.m_shared.m_maxDurability = 200;
                        component3.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem3 = new CustomItem(gameObject3, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject4 = assetBundle.LoadAsset<GameObject>("BattleaxeHeavymetal");
                        ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
                        component4.m_itemData.m_shared.m_maxDurability = 200;
                        component4.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem4 = new CustomItem(gameObject4, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject5 = assetBundle.LoadAsset<GameObject>("SpearHeavymetal");
                        ItemDrop component5 = gameObject5.GetComponent<ItemDrop>();
                        component5.m_itemData.m_shared.m_maxDurability = 200;
                        component5.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem5 = new CustomItem(gameObject5, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject6 = assetBundle.LoadAsset<GameObject>("KnifeHeavymetal");
                        ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
                        component6.m_itemData.m_shared.m_maxDurability = 200;
                        component6.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem6 = new CustomItem(gameObject6, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject7 = assetBundle.LoadAsset<GameObject>("MaceHeavymetal");
                        ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
                        component7.m_itemData.m_shared.m_maxDurability = 200;
                        component7.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem7 = new CustomItem(gameObject7, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject8 = assetBundle.LoadAsset<GameObject>("GreatSwordHeavymetal");
                        ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
                        component8.m_itemData.m_shared.m_maxDurability = 200;
                        component8.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem8 = new CustomItem(gameObject8, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject9 = assetBundle.LoadAsset<GameObject>("SwordHeavymetal");
                        ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
                        component9.m_itemData.m_shared.m_maxDurability = 200;
                        component9.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem9 = new CustomItem(gameObject9, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject10 = assetBundle.LoadAsset<GameObject>("ShieldHeavymetal");
                        ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
                        component10.m_itemData.m_shared.m_name = "Heavymetal Shield";
                        component10.m_itemData.m_shared.m_maxDurability = 200;
                        component10.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem10 = new CustomItem(gameObject10, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject11 = assetBundle.LoadAsset<GameObject>("ShieldHeavymetalTower");
                        ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
                        component11.m_itemData.m_shared.m_maxDurability = 200;
                        component11.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem11 = new CustomItem(gameObject11, true, new ItemConfig
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
                            Item = "YggdrasilWood",
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
                        GameObject gameObject12 = assetBundle.LoadAsset<GameObject>("AxeHeavymetal");
                        ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
                        component12.m_itemData.m_shared.m_maxDurability = 200;
                        component12.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem12 = new CustomItem(gameObject12, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 8
                        },
                        new RequirementConfig {
                            Item = "TrophySvartalfarQueen",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem12);
                        GameObject gameObject13 = assetBundle.LoadAsset<GameObject>("PickaxeHeavymetal");
                        ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
                        component13.m_itemData.m_shared.m_maxDurability = 200;
                        component13.m_itemData.m_shared.m_durabilityPerLevel = 50;
                        CustomItem customItem13 = new CustomItem(gameObject13, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 20,
                                AmountPerLevel = 10
                        },
                        new RequirementConfig {
                            Item = "LinenThread",
                                Amount = 15,
                                AmountPerLevel = 8
                        },
                        new RequirementConfig {
                            Item = "TrophySvartalfarQueen",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem13);

                        GameObject svartalfrqueenaltar = assetBundle.LoadAsset<GameObject>("SvartalfrQueenAltar_New");
                        CustomPrefab svartalfrqueenaltar1 = new CustomPrefab(svartalfrqueenaltar, true);
                        PrefabManager.Instance.AddPrefab(svartalfrqueenaltar1);

                        GameObject Vegvisir_SvartalfrQueen = assetBundle.LoadAsset<GameObject>("Vegvisir_SvartalfrQueen");
                        CustomPrefab Vegvisir_SvartalfrQueen1 = new CustomPrefab(Vegvisir_SvartalfrQueen, true);
                        PrefabManager.Instance.AddPrefab(Vegvisir_SvartalfrQueen1);

                        GameObject cursedEffigy = assetBundle.LoadAsset<GameObject>("CursedEffigy");
                        CustomItem CursedEffigy = new CustomItem(cursedEffigy, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 30
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(CursedEffigy);

                        GameObject darkfrometal = assetBundle.LoadAsset<GameObject>("FrostInfusedDarkMetal");
                        CustomItem darkfrometal1 = new CustomItem(darkfrometal, true);
                        ItemManager.Instance.AddItem(darkfrometal1);

                        Jotunn.Logger.LogInfo("Loaded EVA Mistlands content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Mistlands content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Mistlands} failed. {e.Message} {e.StackTrace}");
                }
            }


        }

        private void RegisterDeepnorth()
        {

            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.DeepNorth == true)
                    {


                        CustomItemConversion OreFrometal = new CustomItemConversion(new SmelterConversionConfig
                        {
                            Station = "blastfurnace",
                            FromItem = "OreFrometal",
                            ToItem = "FrometalBar"
                        });
                        ItemManager.Instance.AddItemConversion(OreFrometal);

                        GameObject gameObject = assetBundle.LoadAsset<GameObject>("BowFrometal");
                        ItemDrop component = gameObject.GetComponent<ItemDrop>();
                        component.m_itemData.m_shared.m_maxDurability = 250;
                        component.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        component.m_itemData.m_shared.m_attack.m_bowDraw = true;
                        component.m_itemData.m_shared.m_attack.m_drawAnimationState = "bow_aim";
                        component.m_itemData.m_shared.m_attack.m_drawDurationMin = 2.1f;
                        component.m_itemData.m_shared.m_attack.m_drawStaminaDrain = 8;
                        CustomItem customItem = new CustomItem(gameObject, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem);
                        GameObject gameObject2 = assetBundle.LoadAsset<GameObject>("AtgeirFrometal");
                        ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
                        component2.m_itemData.m_shared.m_maxDurability = 250;
                        component2.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem2 = new CustomItem(gameObject2, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem2);
                        GameObject gameObject4 = assetBundle.LoadAsset<GameObject>("SledgeFrometal");
                        ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
                        component4.m_itemData.m_shared.m_maxDurability = 250;
                        component4.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem4 = new CustomItem(gameObject4, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem4);
                        GameObject gameObject55 = assetBundle.LoadAsset<GameObject>("BattleaxeFrometal");
                        ItemDrop component55 = gameObject55.GetComponent<ItemDrop>();
                        component55.m_itemData.m_shared.m_maxDurability = 250;
                        component55.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem55 = new CustomItem(gameObject55, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem55);
                        GameObject gameObject6 = assetBundle.LoadAsset<GameObject>("SpearFrometal");
                        ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
                        component6.m_itemData.m_shared.m_maxDurability = 250;
                        component6.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem6 = new CustomItem(gameObject6, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem6);
                        GameObject gameObject7 = assetBundle.LoadAsset<GameObject>("KnifeFrometal");
                        ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
                        component7.m_itemData.m_shared.m_maxDurability = 250;
                        component7.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem7 = new CustomItem(gameObject7, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem7);
                        GameObject gameObject8 = assetBundle.LoadAsset<GameObject>("MaceFrometal");
                        ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
                        component8.m_itemData.m_shared.m_maxDurability = 250;
                        component8.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem8 = new CustomItem(gameObject8, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem8);
                        GameObject gameObject9 = assetBundle.LoadAsset<GameObject>("GreatSwordFrometal");
                        ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
                        component9.m_itemData.m_shared.m_maxDurability = 250;
                        component9.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem9 = new CustomItem(gameObject9, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 10,
                                AmountPerLevel = 6
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem9);
                        GameObject gameObject10 = assetBundle.LoadAsset<GameObject>("SwordFrometal");
                        ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
                        component10.m_itemData.m_shared.m_maxDurability = 250;
                        component10.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem10 = new CustomItem(gameObject10, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem10);
                        GameObject ShieldFrometal = assetBundle.LoadAsset<GameObject>("ShieldFrometal");
                        ItemDrop ItemDrop = ShieldFrometal.GetComponent<ItemDrop>();
                        ItemDrop.m_itemData.m_shared.m_name = "Frometal Shield";
                        ItemDrop.m_itemData.m_shared.m_maxDurability = 250;
                        ItemDrop.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem ShieldFrometalBM = new CustomItem(ShieldFrometal, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(ShieldFrometalBM);
                        GameObject gameObject11 = assetBundle.LoadAsset<GameObject>("ShieldFrometalTower");
                        ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
                        component11.m_itemData.m_shared.m_name = "Frometal Tower Shield";
                        component11.m_itemData.m_shared.m_description = "A Towershield made out of Frometal.";
                        component11.m_itemData.m_shared.m_variants = 0;
                        component11.m_itemData.m_shared.m_maxDurability = 250;
                        component11.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem11 = new CustomItem(gameObject11, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 4,
                                AmountPerLevel = 2
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem11);
                        GameObject gameObject12 = assetBundle.LoadAsset<GameObject>("AxeFrometal");
                        ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
                        component12.m_itemData.m_shared.m_maxDurability = 250;
                        component12.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem12 = new CustomItem(gameObject12, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 2,
                                AmountPerLevel = 1
                        },
                        new RequirementConfig {
                            Item = "TrophyJotunn",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem12);
                        GameObject gameObject13 = assetBundle.LoadAsset<GameObject>("PickaxeFrometal");
                        ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
                        component13.m_itemData.m_shared.m_maxDurability = 250;
                        component13.m_itemData.m_shared.m_durabilityPerLevel = 65;
                        CustomItem customItem13 = new CustomItem(gameObject13, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 2,
                                AmountPerLevel = 1
                        },
                        new RequirementConfig {
                            Item = "TrophyJotunn",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem13);

                        GameObject jotunnaltar = assetBundle.LoadAsset<GameObject>("JotunnAltar");
                        CustomPrefab jotunnaltar1 = new CustomPrefab(jotunnaltar, true);
                        PrefabManager.Instance.AddPrefab(jotunnaltar1);

                        GameObject Vegvisir_Jotunn = assetBundle.LoadAsset<GameObject>("Vegvisir_Jotunn");
                        CustomPrefab Vegvisir_Jotunn1 = new CustomPrefab(Vegvisir_Jotunn, true);
                        PrefabManager.Instance.AddPrefab(Vegvisir_Jotunn1);

                        GameObject ymirsSoulEssence = assetBundle.LoadAsset<GameObject>("YmirsSoulEssence");
                        CustomItem YmirsSoulEssence = new CustomItem(ymirsSoulEssence, true, new ItemConfig
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
                            Item = "YggdrasilWood",
                                Amount = 30
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(YmirsSoulEssence);

                        GameObject primice = assetBundle.LoadAsset<GameObject>("PrimordialIce");
                        CustomItem Primice = new CustomItem(primice, true);
                        ItemManager.Instance.AddItem(Primice);

                        Jotunn.Logger.LogInfo("Loaded EVA DeepNorth content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA DeepNorth content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.DeepNorth} failed. {e.Message} {e.StackTrace}");
                }
            }

        }

        private void RegisterAshlands()
        {
            var contentConfigs = GetJson2();

            foreach (var config in contentConfigs)
            {
                try
                {
                    if (config.Ashlands == true)
                    {
                        GameObject gameObject = assetBundle.LoadAsset<GameObject>("BowFlametal");
                        ItemDrop component = gameObject.GetComponent<ItemDrop>();
                        component.m_itemData.m_shared.m_maxDurability = 300;
                        component.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        component.m_itemData.m_shared.m_attack.m_bowDraw = true;
                        component.m_itemData.m_shared.m_attack.m_drawAnimationState = "bow_aim";
                        component.m_itemData.m_shared.m_attack.m_drawDurationMin = 1.9f;
                        component.m_itemData.m_shared.m_attack.m_drawStaminaDrain = 7;
                        CustomItem customItem = new CustomItem(gameObject, true, new ItemConfig
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
                        GameObject gameObject2 = assetBundle.LoadAsset<GameObject>("AtgeirFlametal");
                        ItemDrop component2 = gameObject2.GetComponent<ItemDrop>();
                        component2.m_itemData.m_shared.m_maxDurability = 300;
                        component2.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem2 = new CustomItem(gameObject2, true, new ItemConfig
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
                        GameObject gameObject4 = assetBundle.LoadAsset<GameObject>("SledgeFlametal");
                        ItemDrop component4 = gameObject4.GetComponent<ItemDrop>();
                        component4.m_itemData.m_shared.m_maxDurability = 300;
                        component4.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem4 = new CustomItem(gameObject4, true, new ItemConfig
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
                        GameObject gameObject5 = assetBundle.LoadAsset<GameObject>("BattleaxeFlametal");
                        ItemDrop component5 = gameObject5.GetComponent<ItemDrop>();
                        component5.m_itemData.m_shared.m_maxDurability = 300;
                        component5.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem5 = new CustomItem(gameObject5, true, new ItemConfig
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
                        GameObject gameObject6 = assetBundle.LoadAsset<GameObject>("SpearFlametal");
                        ItemDrop component6 = gameObject6.GetComponent<ItemDrop>();
                        component6.m_itemData.m_shared.m_maxDurability = 300;
                        component6.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem6 = new CustomItem(gameObject6, true, new ItemConfig
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
                        GameObject gameObject7 = assetBundle.LoadAsset<GameObject>("KnifeFlametal");
                        ItemDrop component7 = gameObject7.GetComponent<ItemDrop>();
                        component7.m_itemData.m_shared.m_maxDurability = 300;
                        component7.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem7 = new CustomItem(gameObject7, true, new ItemConfig
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
                        GameObject gameObject8 = assetBundle.LoadAsset<GameObject>("MaceFlametal");
                        ItemDrop component8 = gameObject8.GetComponent<ItemDrop>();
                        component8.m_itemData.m_shared.m_maxDurability = 300;
                        component8.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem8 = new CustomItem(gameObject8, true, new ItemConfig
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
                        GameObject gameObject9 = assetBundle.LoadAsset<GameObject>("GreatSwordFlametal");
                        ItemDrop component9 = gameObject9.GetComponent<ItemDrop>();
                        component9.m_itemData.m_shared.m_maxDurability = 300;
                        component9.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem9 = new CustomItem(gameObject9, true, new ItemConfig
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
                        GameObject gameObject10 = assetBundle.LoadAsset<GameObject>("SwordFlametal");
                        ItemDrop component10 = gameObject10.GetComponent<ItemDrop>();
                        component10.m_itemData.m_shared.m_maxDurability = 300;
                        component10.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem10 = new CustomItem(gameObject10, true, new ItemConfig
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
                        GameObject ShieldFlametal = assetBundle.LoadAsset<GameObject>("ShieldFlametal");
                        ItemDrop ItemDrop = ShieldFlametal.GetComponent<ItemDrop>();
                        ItemDrop.m_itemData.m_shared.m_name = "Flametal Shield";
                        ItemDrop.m_itemData.m_shared.m_maxDurability = 300;
                        ItemDrop.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem ShieldFlametalBM = new CustomItem(ShieldFlametal, true, new ItemConfig
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
                        GameObject gameObject11 = assetBundle.LoadAsset<GameObject>("ShieldFlametalTower");
                        ItemDrop component11 = gameObject11.GetComponent<ItemDrop>();
                        component11.m_itemData.m_shared.m_name = "Flametal Tower Shield";
                        component11.m_itemData.m_shared.m_description = "A Towershield made out of Flametal.";
                        component11.m_itemData.m_shared.m_variants = 0;
                        component11.m_itemData.m_shared.m_maxDurability = 300;
                        component11.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem11 = new CustomItem(gameObject11, true, new ItemConfig
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
                        GameObject gameObject12 = assetBundle.LoadAsset<GameObject>("AxeFlametal");
                        ItemDrop component12 = gameObject12.GetComponent<ItemDrop>();
                        component12.m_itemData.m_shared.m_maxDurability = 300;
                        component12.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem12 = new CustomItem(gameObject12, true, new ItemConfig
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
                        },
                        new RequirementConfig {
                            Item = "TrophyHelDemon",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem12);
                        GameObject gameObject13 = assetBundle.LoadAsset<GameObject>("PickaxeFlametal");
                        ItemDrop component13 = gameObject13.GetComponent<ItemDrop>();
                        component13.m_itemData.m_shared.m_maxDurability = 300;
                        component13.m_itemData.m_shared.m_durabilityPerLevel = 75;
                        CustomItem customItem13 = new CustomItem(gameObject13, true, new ItemConfig
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
                        },
                        new RequirementConfig {
                            Item = "TrophyHelDemon",
                                Amount = 1
                        }
                    }
                        });
                        ItemManager.Instance.AddItem(customItem13);

                        GameObject blazingdamnedonealtar = assetBundle.LoadAsset<GameObject>("BlazingDamnedOneAltar");
                        CustomPrefab blazingdamnedonealtar1 = new CustomPrefab(blazingdamnedonealtar, true);
                        PrefabManager.Instance.AddPrefab(blazingdamnedonealtar1);

                        GameObject Vegvisir_BlazingDamnedOne = assetBundle.LoadAsset<GameObject>("Vegvisir_BlazingDamnedOne");
                        CustomPrefab Vegvisir_BlazingDamnedOne1 = new CustomPrefab(Vegvisir_BlazingDamnedOne, true);
                        PrefabManager.Instance.AddPrefab(Vegvisir_BlazingDamnedOne1);

                        GameObject burningfragment = assetBundle.LoadAsset<GameObject>("BurningWorldTreeFragment");
                        CustomItem burningfragment1 = new CustomItem(burningfragment, true);
                        ItemManager.Instance.AddItem(burningfragment1);

                        GameObject fenrirsheart = assetBundle.LoadAsset<GameObject>("FenrirsHeart");
                        CustomItem FenrirsHeart = new CustomItem(fenrirsheart, true, new ItemConfig
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

                        

                        Jotunn.Logger.LogInfo("Loaded EVA Ashlands content");
                    }
                    else
                    {
                        Jotunn.Logger.LogInfo("EVA Ashlands content is turned off");
                    }

                }
                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.Ashlands} failed. {e.Message} {e.StackTrace}");
                }
            }


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

        private void LoadContentConfig()
        {
            if (!File.Exists(configPath2))
            {
                GenerateConfigFile2();
                Jotunn.Logger.LogInfo("Generated Content Configuration");
                return;
            }
        }

        private void GenerateConfigFile()
        {
            var bossConfigs = new List<BossConfig>();

            // SvartalfarQueen
            var SvartalfarQueenConfig = new BossConfig();
            var SvartalfarQueenPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("SvartalfarQueen");
            SvartalfarQueenConfig.BossPrefabName = "SvartalfarQueen";
            SvartalfarQueenConfig.Health = (int)SvartalfarQueenPrefab.m_health;

            SvartalfarQueenConfig.Attacks = new List<CustomAttack>();
            var SvartalfarQueenStandardAttacks = new List<string> { "SvartalfarQueenGreatSword", "SvartalfarQueenBow" };
            AddDefaultAttacks(SvartalfarQueenConfig, SvartalfarQueenStandardAttacks);
            var SvartalfarQueenRootspawn = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfarQueen_rootspawn");
            SvartalfarQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfarQueenRootspawn.name,
                AttackAnimation = SvartalfarQueenRootspawn.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = SvartalfarQueenRootspawn.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfarQueenRootspawn.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfarQueenRootspawn.m_itemData.m_shared.m_aiAttackInterval
            });
            var SvartalfarQueenRootspawn2 = PrefabManager.Cache.GetPrefab<ItemDrop>("SvarTentaRoot_attack");
            SvartalfarQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfarQueenRootspawn2.name,
                AiAttackRange = SvartalfarQueenRootspawn2.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfarQueenRootspawn2.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfarQueenRootspawn2.m_itemData.m_shared.m_aiAttackInterval,
                Damages = SvartalfarQueenRootspawn2.m_itemData.m_shared.m_damages
            });
            var SvartalfarQueenBowArrowStorm = PrefabManager.Cache.GetPrefab<ItemDrop>("SvartalfarQueenBowArrowStorm");
            SvartalfarQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = SvartalfarQueenBowArrowStorm.name,
                AttackAnimation = SvartalfarQueenBowArrowStorm.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = SvartalfarQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = SvartalfarQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = SvartalfarQueenBowArrowStorm.m_itemData.m_shared.m_aiAttackInterval
            });
            var bow_projectile_svar = PrefabManager.Cache.GetPrefab<Projectile>("bow_projectile_svar");
            SvartalfarQueenConfig.Attacks.Add(new CustomAttack
            {
                AttackName = bow_projectile_svar.name,
                Damages = bow_projectile_svar.m_damage
            });

            bossConfigs.Add(SvartalfarQueenConfig);

            // Jotunn
            var JotunnConfig = new BossConfig();
            var JotunnPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("Jotunn");
            JotunnConfig.BossPrefabName = "Jotunn";
            JotunnConfig.Health = (int)JotunnPrefab.m_health;

            JotunnConfig.Attacks = new List<CustomAttack>();
            var jotunnStandardAttacks = new List<string> { "Jotunn_Groundslam", "Jotunn_Groundslam2", "Jotunn_Shoot" };
            AddDefaultAttacks(JotunnConfig, jotunnStandardAttacks);
            bossConfigs.Add(JotunnConfig);

            // HelDemon
            var HelDemonConfig = new BossConfig();
            var HelDemon = PrefabManager.Cache.GetPrefab<Humanoid>("HelDemon");
            HelDemonConfig.BossPrefabName = "HelDemon";
            HelDemonConfig.Health = (int)HelDemon.m_health;

            HelDemonConfig.Attacks = new List<CustomAttack>();
            var blazingDamnedStandardAttacks = new List<string> { "BlazingMace", "BlazingMace2", "BlazingMace3", "BlazingMace4", "Blazing_Nova", "Blazing_Shoot" };
            AddDefaultAttacks(HelDemonConfig, blazingDamnedStandardAttacks);
            var Blazing_Meteors = PrefabManager.Cache.GetPrefab<ItemDrop>("Blazing_Meteors");
            HelDemonConfig.Attacks.Add(new CustomAttack
            {
                AttackName = Blazing_Meteors.name,
                AttackAnimation = Blazing_Meteors.m_itemData.m_shared.m_attack.m_attackAnimation,
                AiAttackRange = Blazing_Meteors.m_itemData.m_shared.m_aiAttackRange,
                AiAttackRangeMin = Blazing_Meteors.m_itemData.m_shared.m_aiAttackRangeMin,
                AiAttackInterval = Blazing_Meteors.m_itemData.m_shared.m_aiAttackInterval
            });
            var projectile_meteor_blazing = PrefabManager.Cache.GetPrefab<Projectile>("projectile_meteor_blazing");
            HelDemonConfig.Attacks.Add(new CustomAttack
            {
                AttackName = projectile_meteor_blazing.name,
                Damages = projectile_meteor_blazing.m_damage
            });
            bossConfigs.Add(HelDemonConfig);

            // Golden_Greydwarf_Miniboss
            var GoldenGreydwarfConfig = new BossConfig();
            var GoldenGreydwarfPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("Golden_Greydwarf_Miniboss");
            GoldenGreydwarfConfig.BossPrefabName = "Golden_Greydwarf_Miniboss";
            GoldenGreydwarfConfig.Health = (int)GoldenGreydwarfPrefab.m_health;

            GoldenGreydwarfConfig.Attacks = new List<CustomAttack>();
            var GoldenGreydwarfStandardAttacks = new List<string> { "Greydwarf_elite_attack_gold" };
            AddDefaultAttacks(GoldenGreydwarfConfig, GoldenGreydwarfStandardAttacks);

            bossConfigs.Add(GoldenGreydwarfConfig);

            // Golden_Troll_Miniboss
            var GoldenTrollConfig = new BossConfig();
            var GoldenTrollPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("Golden_Troll_Miniboss");
            GoldenTrollConfig.BossPrefabName = "Golden_Troll_Miniboss";
            GoldenTrollConfig.Health = (int)GoldenTrollPrefab.m_health;

            GoldenTrollConfig.Attacks = new List<CustomAttack>();
            var GoldenTrollStandardAttacks = new List<string> { "troll_punch", "troll_groundslam", "troll_throw" };
            AddDefaultAttacks(GoldenTrollConfig, GoldenTrollStandardAttacks);

            bossConfigs.Add(GoldenTrollConfig);

            // Golden_Wraith_Miniboss
            var GoldenWraithConfig = new BossConfig();
            var GoldenWraithPrefab = PrefabManager.Cache.GetPrefab<Humanoid>("Golden_Wraith_Miniboss");
            GoldenWraithConfig.BossPrefabName = "Golden_Wraith_Miniboss";
            GoldenWraithConfig.Health = (int)GoldenWraithPrefab.m_health;

            GoldenWraithConfig.Attacks = new List<CustomAttack>();
            var GoldenWraithStandardAttacks = new List<string> { "wraith_melee_gold" };
            AddDefaultAttacks(GoldenWraithConfig, GoldenWraithStandardAttacks);

            bossConfigs.Add(GoldenWraithConfig);

            var jsonText = JsonMapper.ToJson(bossConfigs);
            File.WriteAllText(configPath, jsonText);

        }

        private void GenerateConfigFile2()
        {
            var evaconfig = new List<EVAConfiguration>();

            var contentconfig = new EVAConfiguration();
            contentconfig.Description = "Locations contain (Altars/Bosses/Vegvisirs/Ores/Trees), Mistlands/Deepnorth/Ashlands contain (Weapons/Tools/Resources/Crafting tables/Boss summons). Having just the locations toggled true, wouldn't work progression wise.";
            contentconfig.MistlandsLocations = false;
            contentconfig.Mistlands = false;
            contentconfig.DeepNorthLocations = true;
            contentconfig.DeepNorth = true;
            contentconfig.AshlandsLocations = true;
            contentconfig.Ashlands = true;
            evaconfig.Add(contentconfig);

            var jsonText2 = JsonMapper.ToJson(evaconfig);
            File.WriteAllText(configPath2, jsonText2);
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

        internal static List<EVAConfiguration> GetJson2()
        {
            Jotunn.Logger.LogDebug($"Attempting to load config file from path {configPath2}");
            var jsonText2 = AssetUtils.LoadText(configPath2);
            Jotunn.Logger.LogDebug("File found. Attempting to deserialize...");
            var evaconfiguration = JsonMapper.ToObject<List<EVAConfiguration>>(jsonText2);
            return evaconfiguration;
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
        public string Description { get; set; }
        public bool MistlandsLocations { get; set; }
        public bool Mistlands { get; set; }
        public bool DeepNorthLocations { get; set; }
        public bool DeepNorth { get; set; }
        public bool AshlandsLocations { get; set; }
        public bool Ashlands { get; set; }

    }
}