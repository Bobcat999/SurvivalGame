﻿

public class LoadWorldCommand : BaseCommand
{
    public string worldName;

    public LoadWorldCommand(string worldName)
    {
        this.worldName = worldName;
    }


    public void execute()
    {
        WorldManager.instance.LoadWorld(worldName);
    }
}