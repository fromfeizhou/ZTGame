using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharaPlayer
{
    //武器id
    int EquipId { get; set; }
    //翅膀id
    int WingId { get; set; }
    //头盔id
    int ArmetId { get; set; }
    //护甲id
    int ArmorId { get; set; }
    //鞋子id
    int ShoeId { get; set; }
    //时装头
    int FasionHeadId { get; set; }
    //时装衣服
    int FasionClothId { get; set; }
    //时装鞋子
    int FasionShoeId { get; set; }
    
    int PetId { get; set; }
    int MountId { get; set; }
    //变身id
    int TransId { get; set; }

    int ModelType { get; set; }

    void SetPlayerInfo(int equipId, int wingId, int armetId, int armorId, int shoeId,int petId,int mountId,
        int fasionHead,int fasionCloth,int fasionShoe,int transId);
   
}
