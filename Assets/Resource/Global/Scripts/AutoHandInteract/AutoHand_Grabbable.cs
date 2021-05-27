using Autohand;

namespace AutoHandInteract
{
    public class AutoHand_Grabbable : Grabbable
    {
        /*
        instantGrab         =>      物品會自動將手放回去嗎-有利於節省姿勢，對較重的物品不利
        lockHandOnGrab      =>      將手鎖定到位
                                    抓取時手無法移動
        jointBreakForce     =>      打破fixedJoint所需的力量
                                    將其調高以禁用（可能引起抖動）
                                    理想值取決於手的質量和速度設置...對於10質量的手，請嘗試在1500-3000之間進行操作
        jointBreakTorque    =>      斷開固定接頭所需的扭矩
                                    將其調高以禁用（可能引起抖動）
                                    理想值取決於手的質量和速度設置



         */
    }
}

