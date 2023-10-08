using System.Collections;
using UnityEngine;

public class LoadNewWorldCommand : LoadWorldCommand
{

    public LoadNewWorldCommand(string worldName)
    {
        this.worldName = worldName;
    }

    public override void execute()
    {

    }
}