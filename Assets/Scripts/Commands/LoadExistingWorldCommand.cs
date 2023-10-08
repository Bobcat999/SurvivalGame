using System.Collections;
using UnityEngine;

public class LoadExistingWorldCommand : LoadWorldCommand
{

    public LoadExistingWorldCommand(string worldName)
    {
        this.worldName = worldName;
    }

    public override void execute()
    {

    }

}