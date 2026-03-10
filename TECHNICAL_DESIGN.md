# Merlin —— 技术设计文档

> 本文档对游戏中采用的主要技术方案进行较为详细的说明，供团队内部参考与学习交流。

---

## 目录

1. [等距地图风格与 Tilemap 系统](#1-等距地图风格与-tilemap-系统)
2. [程序化地图与敌人生成](#2-程序化地图与敌人生成)
3. [存档与地图持久化](#3-存档与地图持久化)
4. [模块化法术组件与子弹类继承设计](#4-模块化法术组件与子弹类继承设计)
5. [伤害判定与状态系统](#5-伤害判定与状态系统)
6. [项目局限性说明](#6-项目局限性说明)

---

## 1. 等距地图风格与 Tilemap 系统

游戏采用 2D 俯视角（Top-down）风格，世界通过 Unity 内置的 **Tilemap** 系统构建，具有以下特点：

- **双层 Tilemap 架构**：地图分为主地形层（`main_tilemap`）和装饰层（`tilemap2`），主层承载地面与水体瓦片，装饰层渲染水草、岩石等植被特效，两者叠加呈现层次感。
- **丰富的瓦片集**：瓦片资源按类型分组预加载，包括土地/泥土（28 张）、草地（17 张）、水体/雪地/沙漠（36 张）、植被装饰（25 张）及树木（5 张），瓦片类型编号与对应 `Tile` 资源在 `Resources/Tiles/` 下统一管理。
- **`map_info` 三维数组**：地图信息以 `int[mapWidth×2, mapHeight×2, 2]` 存储，第三维分别记录瓦片类型（1=泥土，2=水，3=草地，4=装饰）和瓦片在对应集合中的索引。逻辑分辨率翻倍是为了在过渡边缘处可以额外插入随机装饰瓦片。
- **Cinemachine 平滑缩放**：`MapGenerate` 绑定 `CinemachineVirtualCamera`，监听鼠标滚轮输入，通过 `Mathf.SmoothDamp` 对正交尺寸做平滑过渡，实现顺畅的镜头缩放（范围 3～10）。

---

## 2. 程序化地图与敌人生成

### 2.1 多倍频柏林噪声（Noise.cs）

地形高低分布由 `Noise.GenerateNoiseMap()` 生成，其核心思路是将多个不同频率/振幅的柏林噪声叠加（分形噪声），从而同时呈现大尺度地形轮廓与细节纹理。

关键参数：

| 参数 | 含义 |
|------|------|
| `seed` | 随机种子，决定本次地图的唯一形态 |
| `octaves` | 叠加层数（度），层数越多细节越丰富 |
| `persistance` | 持久度（振幅衰减比例），控制高频层的权重 |
| `lacunarity` | 间隙度（频率增长比例），控制每层频率倍增量 |
| `offset` | 采样偏移，用于无缝衔接多块地图 |

输出为归一化到 `[0, 1]` 区间的二维浮点数组，通过 `Mathf.InverseLerp` 线性映射到全局最大/最小噪声高度。

每一度引入独立的随机偏移 `octaveOffsets[i]`（以 `seed` 为随机源），防止多度叠加后产生重复的对称花纹。

### 2.2 四气候地形生成（MapGenerate.cs）

`MapGenerate` 根据 `TheGlobalManager.Climate()` 返回的气候编号决定调用哪套地形生成逻辑：

| 气候编号 | 地形主题 | 对应方法 |
|--------|--------|--------|
| 4（默认）| 草地/森林 | `GenerateMap_1` |
| 2 | 稀疏草原 | `GenerateMap_2` |
| 1 | 雪地 | `GenerateMap_3` |
| 3 | 沙漠 | `GenerateMap_4` |

以草地地形（`GenerateMap_1`）为例，噪声值阈值分段策略如下：

```
噪声值 ≤ 0.10         → 草地边缘（浅水/泥岸过渡）
噪声值 ∈ (0.10, 0.15] → 草丛过渡带（随机变体瓦片）
噪声值 ∈ (0.15, 0.80] → 主体草地
噪声值 > 0.80          → 水体
```

水体边缘额外进行随机装饰——概率性在水岸格子的第二层（`map_info2`）插入水草（`water_plant0/1`），使海岸线视觉更自然。

所有 map_info 赋值完成后，统一遍历执行 `Tilemap.SetTile()` 批量写入，两层分别渲染。

### 2.3 按气候/难度生成敌人

**GameManagerBehavior.Start()** 在每次进入未访问场景时，根据气候类型 Instantiate 对应预制体：

- 气候 1（雪地）：生成 `WaterMonster` + `AerMonster`，并额外刷出两个 `Boss_0`
- 气候 2（草原）：生成 `FireMonster` + `LittleMonster_1`
- 气候 3/4（沙漠/森林）：对应其他敌人组合

**难度随深度递增**：`TheGlobalManager.GetdDifficult()` 以当前坐标到原点的曼哈顿距离 $d = |x| + |y|$ 为依据，分四档返回基础敌人数量（4/6/8/10），使玩家越走越偏、遭遇的敌人越来越多。

**EnemySpawner**（协程生成器）则在战斗场景内以固定时间间隔（`spawnInterval`）通过 `SpawnRoutine` 协程持续刷新敌人，同时检测生成点与玩家之间的最小距离约束（`minDistanceFromPlayer`），避免敌人直接刷在玩家身上。

---

## 3. 存档与地图持久化

### 3.1 数据结构

存档使用两个可序列化类型：

**`SingleScene`（单张地图快照）**

```csharp
[Serializable]
public struct SingleScene {
    public int   seed;         // 地图生成种子
    public float moisture;     // 湿度
    public float temperature;  // 温度
    public bool  safe;         // 是否已通关（安全地带）
}
```

**`Save`（全局存档对象）**

```csharp
[Serializable]
public class Save {
    public Dictionary<Tuple<int,int>, SingleScene> GlobalMapInfoTree; // 坐标 → 场景数据
    public Tuple<int, int>            LastSafeScene;   // 最近安全点坐标
    public Tuple<int,int,int,int>     ElementNum;      // 四元素持有量（火/水/土/风）
}
```

`GlobalMapInfoTree` 本质上是**世界坐标字典**：每次进入一张新地图，就以 `(x, y)` 为 key 存入 `SingleScene`，相同坐标再次访问时直接读取，不重新生成地图，从而实现"地图持久化"。

### 3.2 序列化与读写

`TheGlobalManager` 使用 C# 内置的 `BinaryFormatter` 进行二进制序列化：

- **保存**：`BinaryFormatter.Serialize(fileStream, thissave)` → `Application.dataPath/Saves/save0.txt`
- **读取**：`BinaryFormatter.Deserialize(fileStream)` → 反序列化为 `Save` 对象

> `TheGlobalManager` 使用 `DontDestroyOnLoad` 保持跨场景存活，是全局唯一的状态中枢。

### 3.3 气候演进与跨场景导航

玩家通过移动到场景边缘触发 `ButtontoNewMap.GotoNewMap()`，根据玩家在场景中的象限（四个方向）调用 `TheGlobalManager.EnterScene(direction, newSeed)`。

每次移动时温湿度会根据方向微调：

| 方向 | X 坐标 | Y 坐标 | 温度变化 | 湿度变化 |
|------|--------|--------|--------|--------|
| 上（1） | +0 | +1 | −0.5 | +0.5 |
| 左（2） | −1 | +0 | −0.5 | −0.5 |
| 下（3） | +0 | −1 | +0.5 | −0.5 |
| 右（4） | +1 | +0 | +0.5 | +0.5 |

`Climate()` 方法再根据当前 `temperature` 与 `moisture` 四象限判断气候类型，形成随移动方向自然演变的气候带分布。

`EnterScene()` 的返回值同样作为"是否为新地图"的判断依据：若目标坐标已在字典中，则直接加载 `WarScene`（复活）而不重刷敌人；若为新坐标，则触发全流程的 `LoadScene` → 重新生成地图和敌人。

---

## 4. 模块化法术组件与子弹类继承设计

这是本游戏最具设计亮点的模块，通过接口 + 抽象基类 + 组合（而非纯继承）实现了"子弹实体"与"法术行为"的完全解耦，赋予了系统极强的扩展性与组合自由度。

### 4.1 整体架构一览

```
ILogicalOperators（接口）
  └── Emit(bb)   — 发射时回调
  └── Process(bb)— 每帧回调
  └── Hit(c, bb) — 碰撞时回调
  └── Blast()    — 消散时回调

SpellComponentBase（抽象类）
  ├── SpellEntity（抽象类）  ← 定义子弹的"是什么"
  │     ├── SpellEntity_Ignisbullet     火元素子弹
  │     ├── SpellEntity_Aquabullet      水元素子弹
  │     ├── SpellEntity_Terrabullet     土元素子弹
  │     ├── SpellEntity_Aerbullet       风元素子弹
  │     ├── SpellEntity_Thunder         雷元素子弹
  │     ├── SpellEntity_IgnisbulletPlus 强化火子弹
  │     ├── SpellEntity_AerbulletPlus   强化风子弹
  │     ├── SpellEntity_AquabulletPlus  强化水子弹
  │     └── SpellEntity_Bigger         放大子弹
  └── SpellLogical（抽象类）← 定义子弹的"做什么"（修饰符）
        ├── SpellLogical_double    双弹
        ├── SpellLogical_bubble    气泡散弹（7连）
        └── SpellLogical_through   穿透

BulletBase（MonoBehaviour）← 场景中子弹的生命周期
  └── EntityBullet              实际飞行子弹
        ├── SpellEntity bullet_main_entity  （主实体）
        └── LogicalOperators[] operators     （修饰符列表）
```

### 4.2 两个核心抽象类的职责分工

**SpellEntity** —— 子弹的"静态定义"

- 负责描述一类子弹的固有属性：贴图路径 `Sprite_Location`、基础飞行速度 `basespeed`、血量伤害 `Blood_dec_`、防御削减 `Defence_dec_`、发射间隔 `intervaltime`。
- 实现 `LogicalOperators` 接口，提供默认的 `Emit`（角度旋转对齐方向向量）、`Process`（空实现，子类可覆盖实现曲线飞行）、`Hit`（空实现，子类覆盖以施加元素伤害和状态）。
- 每种元素子弹的具体逻辑仅需关注"命中后做什么"，例如 `SpellEntity_Ignisbullet.Hit()` 中通过 `enemy.Status.CurrentBlood -= Blood_dec * (1 - enemy.Status.CurrentMagicDfense[1])` 计算扣血，并设置 `Burn` 灼烧状态。

**SpellLogical** —— 子弹的"行为修饰符"

- 不携带任何视觉属性，仅关注在特定生命周期节点改变子弹行为。
- 所有方法提供空默认实现，子类仅需覆盖感兴趣的回调。
- 典型实现：
  - `SpellLogical_double.Emit()`：在主弹发射时额外通过 `TestBulletManager.TBM.CreateANewBullet2()` 创建一颗副弹。
  - `SpellLogical_bubble.Emit()`：同理射出 7 颗带随机偏移方向的副弹。
  - `SpellLogical_through.Emit()`：将 `BulletBase.collidetime++`，令子弹可穿透多个敌人。
  - `SpellLogical_bubble.Process()`：每帧对速度施加 `Speed *= (1 - 0.008f)` 的衰减，模拟气泡缓慢漂浮效果。

### 4.3 EntityBullet —— 运行时的组合容器

`EntityBullet` 是场景中实际存在的 `MonoBehaviour`，由静态工厂方法 `EntityBullet.Create()` 在运行时动态创建 `GameObject` 并附加所有组件。关键字段：

```csharp
public SpellEntity       bullet_main_entity;  // 唯一主实体（决定子弹类型）
public LogicalOperators[] operators;           // 0~N 个修饰符（可叠加）
public int               logical_num;          // 有效修饰符数量
public bool              copy;                 // 是否为副弹（防止无限递归派生）
```

生命周期调用顺序（完全对称于接口定义）：

```
Start()  → entity.Emit(this)  → foreach operator: op.Emit(this)
Update() → entity.Process(this) → foreach operator: op.Process(this)
OnTriggerEnter2D() → entity.Hit(col, this) → foreach operator: op.Hit(col, this)
Destroy  → entity.Blast()     → ...
```

### 4.4 法术配置的反射机制

UI 层（背包/法术槽）通过 `Magicscript`（ScriptableObject）配置法术，其中字段 `TypeName` 存储对应 C# 类名字符串，`IsBullet` / `IsComponent` 区分子弹与修饰符类型。

`Boxselected` 格子组件持有 `Type SpellType`（C# 反射类型对象）。`GameManagerBehavior` 在玩家"确认装备"时调用 `TestBulletManager.sequence_set()`，遍历法术槽读取每个格子的 `SpellType`：

- 若 `IsBullet == true`：通过 `Activator.CreateInstance(SpellType)` 实例化为 `SpellEntity` 并设为主实体。
- 若 `IsComponent == true`：将 `SpellType` 存入 `operators_type[]` 数组。

每次创建子弹时，`setoperators()` 再次通过 `Activator.CreateInstance(operators_type[i])` 批量反射实例化修饰符数组，注入到 `EntityBullet` 中。

这一机制的优势在于：**添加新法术只需新增一个继承 `SpellEntity` 或 `SpellLogical` 的类，再创建对应的 `Magicscript` 资产，无需修改任何管理器或工厂代码**，扩展成本极低。

### 4.5 副弹 copy 机制防无限递归

`double` / `bubble` 等修饰符在 `Emit()` 中会调用 `CreateANewBullet2()` 创建副弹，副弹的 `copy` 字段设为 `true`。修饰符在执行时优先判断 `if(eb.copy == false)`，保证副弹不再递归派生新的副弹。

### 4.6 如何新增一个子弹或法术修饰符

> 以下为框架层面的快速概览，完整的步骤（含代码模板）请参阅 **[Scripts/LTY/HOW_TO_ADD_SPELL.md](Merlin/Assets/Scripts/LTY/HOW_TO_ADD_SPELL.md)**。

新增一种**子弹实体**或**逻辑修饰符**总共涉及三个步骤：

**① 创建 C# 类**

- 新增子弹：继承 `SpellEntity`，在构造函数中赋值 `Sprite_Location`、`basespeed`、`Blood_dec_` 等属性；覆盖抽象属性 `Blood_dec` / `Defence_dec`；按需覆盖 `Hit()` 以实现元素伤害和状态施加。
- 新增修饰符：继承 `SpellLogical`，仅覆盖所需的 `Emit()` / `Process()` / `Hit()` 回调，其余保持默认空实现。

**② 放置贴图资源**

子弹贴图（PNG 格式）需存放在 `Resources/Textures/Bullets/` 目录下，文件名需与构造函数中 `Sprite_Location` 字段赋值保持一致。修饰符无需独立贴图，在 UI 中共用 `Magicscript` 资产指定的图标。

**③ 创建 Magicscript ScriptableObject 资产**

在 `Assets/Scripts/ZhangShengqi/ObjectableObject/` 目录下右键 → *Create → ScriptableObjects → ScriptableObject*（实际上定义于`Assets/Scripts/ZhangShengqi/ObjectableObject/MagicScripts.cs）`，创建新资产并填写：

| 字段 | 说明 |
|------|------|
| `TypeName` | 填写与 C# 类名**完全一致**的字符串，供 `Activator.CreateInstance` 反射 |
| `IsBullet` | 如果是子弹类型则置为 `true` |
| `IsComponent` | 如果是法术组件则置为 `true` |
| `ElementInfo[4]` | 四元素消耗量（火/水/土/风），决定获取法术的代价 |
| `NameInfo` / `FuncInfo` | 名称与功能说明，显示在背包 UI 中 |
| `sprite` | 显示在游戏中的图标 Sprite |

完成后将资产拖入对应的背包面板槽位，系统即可在运行时通过反射自动识别并组装新法术，无需修改任何已有管理器代码。

### 4.7 现存问题

尽管该设计具有较强的扩展性，在实际开发中也暴露出若干值得注意的缺陷：

**1. 性能问题——缺少享元模式与对象池**

每次发射子弹都会调用 `new GameObject()`、`AddComponent<>()` 以及 `Resources.Load<Sprite>()` 来动态构建子弹实例，且销毁时直接调用 `Destroy(gameObject)`。在散弹（bubble，7 发/次）或高频连射的场景下，频繁的 Instantiate/Destroy 会产生卡顿。要解决这些问题，好方法是引入**对象池**来复用 `EntityBullet` 实例，并用**享元模式**共享同类子弹的 `SpellEntity` 只读数据部分（可能需要对于那四个共有方法进行重新设计，使其只包含共有部分，各个具体法术的私有部分另想办法引入其中），避免每颗子弹各自持有独立的 `SpellEntity` 实例。

**2. 加入“奇特”的法术逻辑可能需要侵入式地修改基类**

`SpellLogical` 的设计假设所有修饰符只需通过接口回调影响已存在的 `EntityBullet`，但在实际实现中，像`double` 或 `bubble` 这种"产生副弹"的需求要求修饰符能够主动再创建新子弹。为此，不得不在 `EntityBullet` 中额外引入 `copy` 标志位，并在 `TestBulletManager` 暴露 `CreateANewBullet2()` 方法。这些修改都是很不得不的，对原始接口契约来说是破坏。每次遇到类似的非寻常逻辑，开发者就可能面临修改多个基类或公共接口的风险。

**3. 法术链不支持跨场景存档持久化**

当前存档系统（`Save` 和 `TheGlobalManager`）仅持久化世界地图的坐标、气候参数和四元素数量。玩家在背包 UI 中拼装好的法术链（主弹实体 + 修饰符列表）仅保存在 `TestBulletManager` 的运行时内存中，切换场景或退出游戏后会完全丢失，玩家每次进入战斗场景都需要重新配置法术。要彻底修复这一问题，或许我们需要将法术链的类名序列化进 `Save` 结构，并在场景加载完成后恢复 `TestBulletManager` 的状态。

**4. 法术链偶然出现 Bug 导致无法释放子弹**

在某些操作序列下，`TestBulletManager` 中 `main_bullet_entity` 为 `null` 或 `operators_type` 数组状态不一致，导致 `EntityBullet.Create()` 中的 `setoperators()` 返回异常，子弹无法正常生成。由于缺少统一的法术链合法性校验（即没有在 `sequence_set()` 完成时断言主实体非空），该 bug 难以稳定复现，我们在最后的测试阶段发现了这个问题，但是没有时间排查原因并修改。

---

## 5. 伤害判定与状态系统

### 5.1 Entity 层级

`Entity`持有着两个核心引用：

- `EntityProperty Status`：组件，在运行时管理实时属性状态。
- `CharacterStatusConfig ConcreteInformation`：ScriptableObject，存储角色的初始值。

`Entity.Readconfig()` 在 `Start()` 中调用，每种实体（Entity）的属性（EntityProperty）设计值写入 `Status`，由此使每种怪物/玩家的数值独立配置，无需改代码。

### 5.2 EntityProperty —— 属性与状态管理

`EntityProperty` 存储角色完整的战斗属性：

| 属性 | 说明 |
|------|------|
| `CurrentBlood` | 当前血量 |
| `CurrentSpeed` | 当前移动速度（状态异常时会被临时修改） |
| `CurrentFrontDefense` | 前向物理护甲 |
| `CurrentBehindDefense` | 背向物理护甲 |
| `CurrentMagicDfense[5]` | 法术抗性（0=通用，1=火，2=水，3=风，4=土） |
| `CurrentSituation` | `(situation枚举, 剩余时间)` 元组，记录当前异常状态 |

每帧调用 `UpdataAll()` 统一刷新血条、速度、防御、法抗和状态图标 UI，状态剩余时间归零后自动清除状态并还原速度。
当然，这些属性很多未使用，不过提供了多样法术效果的可能性。

### 5.3 10 种异常状态

```csharp
public enum situation {
    Nothing,      // 无异常
    Toxic,        // 中毒（持续掉血）
    Burn,         // 灼烧（火元素命中触发）
    Wet,          // 潮湿（水元素命中触发）
    Frozen,       // 冰冻（降低移动速度）
    Easyhurt,     // 易伤（受伤加成）
    Magicshield,  // 法抗护盾
    Physicshield, // 物理护盾
    Fast,         // 迅捷
    Death         // 死亡（血量归零后自动设置）
}
```

状态由子弹的 `Hit()` 方法施加，例如：
- `SpellEntity_Ignisbullet.Hit()` 在血量扣减后设置 `Burn` 状态。
- `SpellEntity_Aquabullet.Hit()` 将 `CurrentSpeed *= 0.5f` 并设置 `Frozen` 状态。

### 5.4 元素抗性计算

实际伤害公式统一为：

$$\text{实际伤害} = \text{Blood\_dec} \times (1 - \text{CurrentMagicDfense}[\text{元素Index}])$$

法抗值为 `[0, 1]` 区间的浮点数，0 表示无抗性（满伤），1 表示完全免疫。不同怪物具备差异化的元素弱点/抗性，驱动玩家有策略地选择对应元素子弹。

---

## 6. 项目局限性说明

Merlin 是四人团队在**约一周时间**内为西安交通大学课程 COMP561405 赶制的汇报项目。受限于时间进度与团队的技术积累，项目在若干方面存在明显不足，在此坦诚列出，供后续迭代或类似项目参考。

### 6.1 框架完备但内容稀少

法术组件系统提供了极为便捷的扩展接口——新增一种法术仅需一个 C# 类和一个 ScriptableObject 资产。然而由于时间限制，最终实装的子弹种类（5 种基础 + 4 种强化）和修饰符种类（3 种）远未能体现设计所预设的"无限组合"的愿景。框架的价值在实际游戏体验中难以充分展示。

### 6.2 数值设计不成熟

子弹伤害、速度、发射间隔等参数均为开发过程中凭感觉手动赋值，缺乏系统性的数值规划。部分子弹（如强化风弹 `AerbulletPlus`）穿透且帧伤，在设计时认为极其强力，但实际使用时由于散射太高、距离较近，而表现极弱。四元素的获取量与消耗量之间的平衡性亦未经充分测试，玩家在中后期可能面临资源过剩或枯竭两种极端状态。

### 6.3 UI 界面过于简陋

背包/法术槽 UI 缺乏足够清晰的引导文字和视觉反馈，新玩家难以自行摸清法术组合的规则。登录界面虽提供了文字说明，但核心的"拼装法术"流程始终没有配套教程或提示系统，造成较高的上手门槛。

### 6.4 代码结构与命名混乱

四位团队成员各自维护一个以姓名命名的文件夹（`LTY/`、`Lyb_Scripts/`、`ZhangShengqi*/`、`Yzh_Scripts/`），代码风格与命名规范各成体系。项目结构在第一天就基本确定，后续因时间紧迫未能重构，导致以下问题积累：

- **跨文件夹的大规模修改**：玩家控制器 `MerlinWalk`（位于 `Yzh_Scripts/`）与战斗管理器 `GameManagerBehavior`（位于 `ZhangShengqi/`）承担了大量跨模块的功能，频繁成为其他成员修改的"公共区域"，产生了多处职责混合。
- **命名与功能脱节**：部分脚本的命名沿用了最初的占位名称，未能反映后续功能扩展后的实际职责，例如 `TestBulletManager` 由测试脚本演变为生产环境的核心法术管理器，但名称未随之更新。
- **模块边界模糊**：伤害判定相关代码同时分散在 `Lyb_Scripts/WarScene/EntityAndProperty/`（属性与状态）和各 `SpellEntity_*.Hit()` 方法（元素逻辑）中，逻辑上没什么问题，但需要对代码有充分理解，增加了定位 bug 的难度。

这些问题是在快节奏的课程项目中难以完全规避的，但从我们的经历中能够获得警示：**在多人协作中，哪怕是简单项目，早期约定好统一的命名规范、模块划分和代码审查流程极其重要，先不要急着写代码。其次，难免在实现过程中会有新想法，那么一开始设计时就应该充分考虑可扩展性。最后，一定在开发过程中（或者在项目刚刚告一段落时）写好文档和代码注释，一方面记录下修改方便合作工作，另一方面为了以后的重构和迭代做好铺垫，避免像我一样在项目结束的一年后，花费大量功夫阅读代码、补充注释和写文档。**。

---

*文档版本：v0.1（初版），对应游戏版本 Merlin v0.1.0*
