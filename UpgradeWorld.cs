using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace UpgradeWorld;

public class CommandRegistration
{
    public string name = "";
    public string description = "";
    public string[] commands = new string[0];

    public void AddCommand()
    {
        new Console.ConsoleCommand(name, description, (args) =>
        {
            foreach (var command in commands)
                args.Context.TryRunCommand(command);
        });
    }
}

[HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
public static class Upgrade
{
    private static List<CommandRegistration> registrations = new();

    public const string GUID = "upgrade_world";
    public static void Register(string name, string description, params string[] commands)
    {
        if (!Chainloader.PluginInfos.ContainsKey(GUID)) return;
        registrations.Add(new() { name = name, description = description, commands = commands });
    }

    static void Postfix()
    {
        foreach (var registration in registrations)
            registration.AddCommand();
    }
}