using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaPlayerInfo : CharaFightInfo, ICharaPlayer
{
    //**===================接口实现=======================**//
    //武器id
    public int EquipId { get; set; }
    //翅膀id
    public int WingId { get; set; }
    //头盔id
    public int ArmetId { get; set; }
    //护甲id
    public int ArmorId { get; set; }
    //鞋子id
    public int ShoeId { get; set; }
    //时装头
    public int FasionHeadId { get; set; }
    //时装衣服
    public int FasionClothId { get; set; }
    //时装鞋子
    public int FasionShoeId { get; set; }

    public int PetId { get; set; }
    public int MountId { get; set; }
    //变身id
    public int TransId { get; set; }

    public int ModelType { get; set; }


    public void SetPlayerInfo(int equipId = 0, int wingId = 0, int armetId = 0, int armorId = 0, int shoeId = 0, int petId = 0, int mountId = 0,
        int fasionHead = 0, int fasionCloth = 0, int fasionShoe = 0, int transId = 0)
    {
        EquipId = equipId;
        WingId = wingId;
        ArmetId = armetId;
        ArmorId = armorId;
        ShoeId = shoeId;
        FasionHeadId = fasionHead;
        FasionClothId = fasionCloth;
        FasionShoeId = fasionShoe;

        TransId = transId;
    }

    //**===================接口实现=======================**//
    public CharaPlayerInfo(int animaId, CHARA_TYPE type)
        : base(animaId, CHARA_TYPE.PLAYER)
    {
        
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
