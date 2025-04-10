using System;

public class WorldHandler
{
    public IWorld CurrentWorld { get; private set; } = new WorldOne();
}

public interface IWorld
{

}

public class WorldOne : IWorld
{
    int worldWidth  = 32;
    int worldHeight = 32;
}