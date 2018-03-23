%% coding: latin-1
-record(sys_Equipment, {
                        id, %装备ID
                        type, %类型
                        professional, %职业
                        sex, %性别
                        superposition, %叠加
                        hp, %生命
                        damage_min, %最小攻击
                        damage_max, %最大攻击
                        defense, %防御
                        attack_speed, %攻速
                        life_back, %生命恢复
                        energy_max, %能量上限
                        lucky, %幸运值
                        curse, %诅咒值
                        move, %移动
                        wound_deeper, %伤害加成
                        damage_reduction, %伤害减免
                        skills_cd %技能冷却
}).
