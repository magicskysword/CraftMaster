# [English Readme Click Here](ReadmeEn.md)

# Mod介绍

该Mod为开拓者：正义之怒添加了制造奇物的功能，Mod目前处于开发阶段，可能Bug频出，遇到问题欢迎提Issue反馈。

# Mod安装

1. 下载并安装UnityModManager（简称UMM）
2. 下载该Mod
3. 使用UnityModManager或ModFinder安装本Mod
4. 启动游戏，读取存档，在游戏中按Ctrl+F10打开UMM界面
5. 找到CraftMaster的Mod界面，使用Mod功能

# 风险提示

该Mod使用的部分功能在游戏存档中加入了新数据

移除本Mod有极大可能造成坏档

且该Mod目前处于开发阶段，可能不是很稳定，请及时备份你的存档。

# 制造规则

在该Mod中，制造普通物品或魔法物品时，有两个关键指标：制造点数与制造DC

制造点数为制造该物品需要达成的点数，点数达成后，物品制造完成。

制造DC为制造该物品时需要进行的检定，游戏中每隔1小时会进行1次制造检定，根据检定结果会增加制造点数进度。

DC检定结果每比制造DC高5，制造点数进度便额外增加1点

## 普通装备

制造点数为 10

制造DC 为 10

## 装备附魔

制造点数为 10 + 新增附魔点数 ^ 2 * 10

制造DC为 20 + 新增附魔点数 * 5

## 魔杖制造

制造点数为 10 + 施法者等级 * 5 + 法术环数 * 10

制造DC为 20 + 施法者等级 * 2 + 法术环数 * 2

## 相关专长

### 普通专长

| 专长名称           | 专长前置          | 专长效果                                                                                                                                                                                                                                                                                                     |
| ------------------ | ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 制造魔法武器与防具 | 施法者等级达到5级 | 你能制造魔法盔甲，盾牌和武器。<br />为一件武器、一套盔甲或盾牌附魔需要每小时进行一次制造检定(DC)，检定通过后增加1点制造进度，检定结果每比检定难度高5，便额外获得1点制造进度。<br />附魔需要进行的检定次数为 10 + 新增附魔点数 ^ 2 * 10<br />附魔的检定DC难度为 20 + 新增附魔点数 * 5                       |
| 制造魔杖           | 施法者等级达到5级 | 你能制造任何你知道的4环或以下法术的魔杖。<br />制造一柄魔杖需要每小时进行一次制造检定(DC)，检定通过后增加1点制造进度，检定结果每比检定难度高5，便额外获得1点制造进度。<br />制造魔杖需要进行的检定次数为 10 + 施法者等级 * 5 + 法术环数 * 10<br />制造魔杖的检定DC难度为 20 + 施法者等级 * 2 + 法术环数 * 2 |

### 神话专长

| 专长名称         | 专长前置 | 专长效果                                                                                        |
| ---------------- | -------- | ----------------------------------------------------------------------------------------------- |
| 制造魔杖（神话） | 制造魔杖 | 你制造魔杖的技巧是如此的卓越，你可以制造4环以上的魔杖，同时为魔杖额外添加等于你的神话等级的DC。 |

### 神话能力

| 专长名称 | 专长前置 | 专长效果                                                                                                       |
| -------- | -------- | -------------------------------------------------------------------------------------------------------------- |
| 炉火纯青 | 无       | 你制造魔法物品的能力已臻化境，制造魔法物品时，成功通过DC检定后额外获得等于一半神话等级（向上取整）的制造点数。 |
