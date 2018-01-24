using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTSceneManager : Singleton<ZTSceneManager>
{

    public Dictionary<int,PlayerBattleInfo> CharaDic = null;
    public List<PlayerBattleInfo> CharaList = null;
    public Dictionary<int,GameObject> CharaViewDic = null;

    public PlayerBattleInfo MyPlayer = null;

    private GameObject _playerPrefab = null;
    public int SceneFrame = 0;
    //操作指令集合
    public Dictionary<int, List<FightCommandBase>> CommandDic;

    //初始化
    public override void Init()
    {
        CharaDic = new Dictionary<int, PlayerBattleInfo>();
        CharaViewDic = new Dictionary<int, GameObject>();
        CharaList = new List<PlayerBattleInfo>();
        SceneFrame = 0;

        OwnerControl.GetInstance().Init();
        //操作集合初始化
        CommandDic = new Dictionary<int, List<FightCommandBase>>();

        InitEvent();
    }

    public override void Destroy()
    {
        OwnerControl.GetInstance().Destroy();
        RemoveEvent();
    }

    //刷新对象
    public void Update()
    {
        if (SceneFrame > FightDefine.MaxFrame)
        {
            SceneFrame = 0;
        }
        SceneFrame++;
        //刷新玩家虚拟摇杆
        OwnerControl.GetInstance().Update();
        UpdateCommand();
        UpdatePlayer();
    }

    //命令刷新
    private void UpdateCommand()
    {
        foreach(int frame in CommandDic.Keys){
            if (FightDefine.CompareFrame(frame))
            {
                List<FightCommandBase> list = CommandDic[frame];
                for (int i = 0; i < list.Count; i++)
                {
                    DoCommand(list[i]);
                }
                list.Clear();
            }
        }
    }

    private void DoCommand(FightCommandBase command)
    {
        int battleId = command.BattleId;
        if (CharaDic.ContainsKey(battleId))
        {
            PlayerBattleInfo info = CharaDic[battleId] as PlayerBattleInfo;
            switch (command.CommandType)
            {
                case COMMAND_TYPE.MOVE:
                    info.MoveCommand(command as MoveCommand);
                    break;
                case COMMAND_TYPE.SKILL:
                    info.SkillCommand(command as SkillCommand);
                    break;
            }
        }
    }

    private void InitEvent()
    {
        SceneEvent.GetInstance().addEventListener(ScenePlayerEvents.ADD_PLAYER, OnAddPlayer);
        SceneEvent.GetInstance().addEventListener(ScenePlayerEvents.ADD_COMMAND, OnAddCommand);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(ScenePlayerEvents.ADD_PLAYER, OnAddPlayer);
        SceneEvent.GetInstance().removeEventListener(ScenePlayerEvents.ADD_COMMAND, OnAddCommand);
    }

    private void OnAddCommand(Notification data)
    {
        FightCommandBase command = data.param as FightCommandBase;
        int frame = command.Frame;
        if (!CommandDic.ContainsKey(frame)) CommandDic.Add(frame, new List<FightCommandBase>());
        CommandDic[frame].Add(command);
    }

   


    private void OnAddPlayer(Notification note)
    {
        int playerId = (int)note.param;
        CreatePlayer(playerId);
    }

    //创建玩家s
    private void CreatePlayer(int battleId)
    {
        //test
        if (!CharaDic.ContainsKey(battleId))
        {
            PlayerBattleInfo playerInfo = new PlayerBattleInfo(1,CHARA_TYPE.PLAYER);
            playerInfo.SetFightInfo(100);
            playerInfo.SetPlayerInfo();
            playerInfo.SetBattleInfo(battleId, battleId, new Vector3(CharaList.Count * 10, 0, CharaList.Count * 10));
            //test
            CharaDic.Add(battleId, playerInfo);
            CharaList.Add(playerInfo);
        }
      

        if (null != _playerPrefab)
        {
            PlayerBattleInfo info = CharaDic[battleId];
            GameObject gameObject = GameObject.Instantiate(_playerPrefab);
            gameObject.transform.localPosition = info.MovePos;
            gameObject.transform.parent = GameObject.Find("PlayerLayer").transform;
            CharaViewDic.Add(battleId, gameObject);
            gameObject.AddComponent<PlayerBattleActor>();
            gameObject.GetComponent<PlayerBattleActor>().SetInfo(info);

            if (battleId == 1)
            {
                MyPlayer = info;
                GameObject.Find("Main Camera").GetComponent<CameraFollow>().target = gameObject.transform;
                GameObject.Find("SkillJoystick").GetComponent<SkillArea>().player = gameObject;
            }
        }
        else
        {
            AssetManager.LoadAsset("Assets/Models/TmpCharacter/@fashi_1.prefab", LoadPrefabCom);
        }
    }

    private void LoadPrefabCom(Object target, string path)
    {
        _playerPrefab = target as GameObject;
        LaterCreatePlayer();
    }

    //延后创建玩家
    private void LaterCreatePlayer(){
        foreach (int key in CharaDic.Keys)
        {
            if (!CharaViewDic.ContainsKey(key))
            {
                CreatePlayer(key);
            }
        }
    }

    //移除玩家
    private void RemovePlayerById(int battleId)
    {
        int index = CharaList.FindIndex(delegate(PlayerBattleInfo player)
        {
            return player.BattleId == battleId;
        });

        //移除数据对象
        if (index != -1)
        {
            CharaList.RemoveAt(index);
        }
        if (CharaDic.ContainsKey(battleId))
        {
            CharaDic.Remove(battleId);
        }
        //移除场景对象
        if (CharaDic.ContainsKey(battleId))
        {
            GameObject.Destroy(CharaViewDic[battleId]);
            CharaViewDic.Remove(battleId);
        }
    }

    //刷新玩家
    private void UpdatePlayer()
    {
        //所有对象刷新
        for (int i = 0; i < CharaList.Count; i++)
        {
            CharaList[i].UpdateFrame();
        }
    }

}
