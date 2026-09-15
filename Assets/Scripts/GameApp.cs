using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp
{
    private static GameApp instance;

    public static GameApp Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameApp();
            }
            return instance;
        }
    }

    public static ControllerManager ControllerManager;

    public static ViewManager ViewManager;

    public static PlayerInputManager PlayerInputManager;

    public static MessageCenter MsgCenter;

    public static InteractManager InteractManager;

    public static CardTableManager CardTableManager;

    public static GameTurnManager GameTurnManager;

    public static CardManager CardManager;

    public static CommandManager CommandManager;

    public void Init()
    {
        ControllerManager = new ControllerManager();
        ViewManager = new ViewManager();
        PlayerInputManager = new PlayerInputManager();
        MsgCenter = new MessageCenter();

        InteractManager = new InteractManager();
        GameTurnManager = new GameTurnManager();
        CardManager = new CardManager();
        CardTableManager = new CardTableManager();
        CommandManager = new CommandManager();
    }

    public void Update(float dt)
    {
        PlayerInputManager.Update(dt);
        CommandManager.Update(dt);
    }
}
