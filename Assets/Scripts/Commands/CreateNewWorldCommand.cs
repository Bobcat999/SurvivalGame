using System.Collections;
using UnityEngine;

public class CreateNewWorldCommand : BaseCommand
{
    string worldName;
    int seed;

    public CreateNewWorldCommand(string worldName, int seed)
    {
        this.worldName = worldName;
        this.seed = seed;
    }

    public void execute()
    {
        WorldManager.instance.CreateNewWorld(worldName, seed);
    }
}