using System;
using Mining;
using TheSTAR.Data;
using TheSTAR.GUI;
using TheSTAR.GUI.Screens;
using TheSTAR.Input;
using UnityEngine;
using World;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameWorld world;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InputController input;
    [SerializeField] private DropItemsContainer drop;
    [SerializeField] private ItemsController items;
    [SerializeField] private DataController data;
    [SerializeField] private TransactionsController transactions;
    [SerializeField] private GuiController gui;

    [Space] [SerializeField] private float startGameDelay = 0.5f;

    public event Action OnStartGameEvent;

    /// <summary>
    /// Main logic entry point
    /// </summary>
    private void Start()
    {
        Init();
        Invoke(nameof(StartGame), startGameDelay);
    }

    private void StartGame()
    {
        OnStartGameEvent?.Invoke();
    }

    private void Init()
    {
        items.Init();
        world.Init(drop);
        cameraController.FocusTo(world.CurrentPlayer);
        
        gui.Init(out var trs);
        var gameScreen = gui.FindScreen<GameScreen>();
        gameScreen.Init(items);
        
        input.Init(gameScreen.JoystickContainer, world.CurrentPlayer);
        transactions.Init(trs, data);
        drop.Init(transactions, items, world.CurrentPlayer, world.CurrentPlayer, world.CurrentPlayer.StopInteract);
    }
}