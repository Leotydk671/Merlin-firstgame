# 如何新增一个子弹或法术修饰符

> 本文档详细说明在 Merlin 项目中扩展法术系统的完整步骤，适用于新增**子弹实体**（`SpellEntity` 子类）或**逻辑修饰符**（`SpellLogical` 子类）。
>
> 框架概述请参阅 [TECHNICAL_DESIGN.md § 4.6](../../../../../TECHNICAL_DESIGN.md)。

---

## 目录

1. [新增子弹实体](#1-新增子弹实体)
2. [新增逻辑修饰符](#2-新增逻辑修饰符)
3. [创建 Magicscript ScriptableObject 资产](#3-创建-magicscript-scriptableobject-资产)
4. [放置贴图资源](#4-放置贴图资源)
5. [接入背包 UI](#5-接入背包-ui)
6. [完整示例：新增一个"毒素子弹"](#6-完整示例新增一个毒素子弹)
7. [常见问题](#7-常见问题)

---

## 1. 新增子弹实体

### 1.1 文件位置

在 `Assets/Scripts/LTY/` 目录下新建 C# 脚本，命名规范为：

```
SpellEntity_<名称>.cs
```

例如：`SpellEntity_Toxicbullet.cs`

### 1.2 继承关系

```csharp
public class SpellEntity_Toxicbullet : SpellEntity, LogicalOperators
{
    // ...
}
```

必须同时继承 `SpellEntity` 并实现 `LogicalOperators` 接口（`SpellEntity` 已声明实现该接口，子类只需覆盖相关方法）。

### 1.3 构造函数——设置子弹属性

在无参构造函数中完成所有属性初始化：

```csharp
public SpellEntity_Toxicbullet()
{
    Sprite_Location = "Textures/Bullets/Toxicbullet"; // 与贴图路径对应
    entity_types    = Entity_Type.Bullet;
    basespeed       = 0.08f;   // 飞行速度
    Blood_dec_      = 6.0f;    // 基础伤害
    Defence_dec_    = 0f;
    intervaltime    = 0.4f;    // 发射间隔（秒）
}
```

### 1.4 必须实现的抽象属性

`SpellEntity` 要求子类实现 `Blood_dec` 和 `Defence_dec` 属性：

```csharp
public override float Blood_dec
{
    get => Blood_dec_;
    set => Blood_dec_ = value;
}

public override float Defence_dec
{
    get => Defence_dec_;
    set => Defence_dec_ = value;
}
```

直接转发到父类已声明的 `Blood_dec_` / `Defence_dec_` 字段即可，无需额外逻辑。

### 1.5 覆盖生命周期回调

根据需要覆盖以下方法（全部均有父类默认实现，不需要的可不写）：

| 方法 | 触发时机 | 典型用途 |
|------|---------|---------|
| `Emit(BulletBase bb)` | 子弹 `Start()` 时 | 调整散射参数、存活时间、初始速度 |
| `Process(BulletBase bb)` | 每帧 `Update()` | 实时修改速度/方向，实现曲线弹道 |
| `Hit(Collider2D col, BulletBase bb)` | 碰到敌人触发器 | 扣血、施加状态、控制是否销毁子弹 |
| `Blast()` | 子弹被销毁时 | 播放特效或者其它有趣的功能（当前版本暂未深度使用） |

**命中逻辑示例**——施加毒素状态：

```csharp
public override void Hit(Collider2D col, BulletBase bb)
{
    Enemy enemy = col.gameObject.GetComponent<Enemy>();
    if (enemy != null)
    {
        // 扣血（索引 0 = 通用法抗）
        enemy.Status.CurrentBlood -= Blood_dec * (1 - enemy.Status.CurrentMagicDfense[0]);
        // 施加毒素状态（仅在无状态或状态已结束时覆盖）
        if (enemy.Status.CurrentSituation.Item1 == EntityProperty.situation.Nothing
            || enemy.Status.CurrentSituation.Item2 <= 0)
        {
            enemy.Status.CurrentSituation.Item1 = EntityProperty.situation.Toxic;
        }
        bb.Destroy_bullet();
    }
}
```

> **元素法抗索引约定**：`CurrentMagicDfense[0]` = 通用，`[1]` = 火，`[2]` = 水，`[3]` = 风，`[4]` = 土。
> 请根据子弹元素属性选择对应索引。

---

## 2. 新增逻辑修饰符

### 2.1 文件位置与命名

在 `Assets/Scripts/LTY/` 目录下新建脚本，命名规范：

```
SpellLogical_<名称>.cs
```

例如：`SpellLogical_homing.cs`（追踪修饰符）

### 2.2 继承关系

```csharp
public class SpellLogical_homing : SpellLogical, LogicalOperators
{
    // ...
}
```

`SpellLogical` 已为所有接口方法提供空的默认实现，子类**只需覆盖感兴趣的回调**，无需全部实现。

### 2.3 构造函数

修饰符通常不需要特殊初始化，父类 `SpellLogical()` 构造函数已自动将 `spelltype` 设置为 `SpellComponentType.Logical`。

```csharp
// 如无特殊字段，可不写构造函数，使用编译器默认构造即可
```

### 2.4 覆盖回调

**在 Emit 阶段修改子弹参数**：

```csharp
public override void Emit(BulletBase bb)
{
    if (bb is EntityBullet eb)
    {
        eb.alive_distance_time = 3.0f; // 延长存活时间
        eb.Speed *= 0.8f;              // 降低初速度
    }
}
```

**在 Process 阶段实现追踪效果**（注意性能，每帧执行）：

```csharp
public override void Process(BulletBase bb)
{
    if (bb is EntityBullet eb)
    {
        // 每帧将子弹方向缓慢转向最近敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;
        // ... 追踪逻辑
    }
}
```

**在 Emit 中派生副弹**（参考 `SpellLogical_double`，注意必须检查 `eb.copy == false`）：

```csharp
public override void Emit(BulletBase bb)
{
    if (bb is EntityBullet eb && eb.copy == false)
    {
        if (Time.time - TestBulletManager.last_bullet_time < eb.intervaltime)
            return;
        TestBulletManager.TBM.CreateANewBullet2(
            eb.original_up + (Vector3)UnityEngine.Random.insideUnitCircle,
            PlayerController.EntityMerlin.gameObject.transform.position);
    }
}
```

> **重要**：派生副弹时务必判断 `eb.copy == false`，否则副弹会无限递归生成副弹。详见 [TECHNICAL_DESIGN.md § 4.5](../../../../../TECHNICAL_DESIGN.md)。

---

## 3. 创建 Magicscript ScriptableObject 资产

每个子弹或修饰符都需要一个对应的 `Magicscript` 资产，作为 UI 与 C# 类型之间的桥梁。

### 3.1 创建资产

在 Unity Editor 中，右键点击 `Assets/Scripts/ZhangShengqi/ObjectableObject/` 目录：

```
右键 → Create → ScriptableObjects → ScriptableObject
```

重命名资产（建议与 C# 类名保持一致，例如 `SpellToxic.asset`）。

### 3.2 填写字段

在 Inspector 中填写以下字段：

| 字段 | 填写内容 | 说明 |
|------|---------|------|
| `TypeName` | `SpellEntity_Toxicbullet` | **必须与 C# 类名完全一致**，区分大小写 |
| `IsBullet` | ✅ `true` | 子弹类型 |
| `IsComponent` | ☐ `false` | |
| `ElementInfo[0]` | 消耗火元素量 | 例如 `2` |
| `ElementInfo[1]` | 消耗水元素量 | 例如 `1` |
| `ElementInfo[2]` | 消耗土元素量 | 例如 `0` |
| `ElementInfo[3]` | 消耗风元素量 | 例如 `0` |
| `NameInfo` | `毒素弹` | 显示在背包中的名称 |
| `FuncInfo` | `命中施加中毒状态` | 功能说明 |
| `sprite` | 拖入图标 Sprite | 背包 UI 中显示的图标 |

对于**修饰符**类型，将 `IsComponent` 置为 `true`、`IsBullet` 置为 `false`。

---

## 4. 放置贴图资源

### 4.1 子弹飞行贴图

将子弹的飞行外观 png 图片放置在：

```
Assets/Resources/Textures/Bullets/<文件名>.png
```

文件名需与构造函数中 `Sprite_Location` 字段赋值的字符串**完全一致**（不含扩展名），例如：

```csharp
Sprite_Location = "Textures/Bullets/Toxicbullet";
// 对应贴图路径：Assets/Resources/Textures/Bullets/Toxicbullet.png
```

### 4.2 导入设置建议

- **Texture Type**：Sprite (2D and UI)
- **Pixels Per Unit**：建议与场景其他子弹贴图保持一致（可参考现有 Ignisbullet 的设置）
- 如果子弹贴图显示比例异常，可在 `Sprite_per_unit.cs`（Editor 工具）中批量调整

### 4.3 背包图标贴图

图标 Sprite 放置位置无硬性要求，通常与其他图标放在一起（`Assets/Scripts/ZhangShengqi/ObjectableObject/` 目录下已有多个 PNG 示例），在 `Magicscript` 资产的 `sprite` 字段中拖入引用即可。

---

## 5. 接入背包 UI

完成以上步骤后，需要将新资产链接到背包面板。

1. 在 Unity Editor 中打开 `WarScene` 场景。
2. 找到背包 UI 的面板 → `ElementPanelBehavior` 组件（挂载在对应 Panel GameObject 上）。
3. 在 `ElementPanelBehavior` 的 Inspector 中，将新建的 `Magicscript` 资产拖入对应的法术槽引用。
4. 运行时玩家点击背包中的图标后，`ElementPanelBehavior` 会读取资产的 `TypeName` 并通过 `Boxselected.SpellType = Type.GetType(TypeName)` 注册到格子中，后续由 `TestBulletManager.sequence_set()` 统一反射装配。

---

## 6. 完整示例：新增一个"毒素子弹"

**Step 1**：在 `Assets/Scripts/LTY/` 下新建 `SpellEntity_Toxicbullet.cs`：

```csharp
using UnityEngine;

// 毒素子弹：命中后施加 Toxic 状态，造成持续伤害
public class SpellEntity_Toxicbullet : SpellEntity, LogicalOperators
{
    public SpellEntity_Toxicbullet()
    {
        Sprite_Location = "Textures/Bullets/Toxicbullet";
        entity_types    = Entity_Type.Bullet;
        basespeed       = 0.08f;
        Blood_dec_      = 6.0f;
        Defence_dec_    = 0f;
        intervaltime    = 0.4f;
    }

    public override float Blood_dec
    {
        get => Blood_dec_;
        set => Blood_dec_ = value;
    }

    public override float Defence_dec
    {
        get => Defence_dec_;
        set => Defence_dec_ = value;
    }

    public override void Hit(Collider2D col, BulletBase bb)
    {
        Enemy enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Status.CurrentBlood -= Blood_dec * (1 - enemy.Status.CurrentMagicDfense[0]);
            if (enemy.Status.CurrentSituation.Item1 == EntityProperty.situation.Nothing
                || enemy.Status.CurrentSituation.Item2 <= 0)
            {
                enemy.Status.CurrentSituation.Item1 = EntityProperty.situation.Toxic;
            }
            bb.Destroy_bullet();
        }
    }
}
```

**Step 2**：将 `Toxicbullet.png` 放入 `Assets/Resources/Textures/Bullets/`。

**Step 3**：创建 `SpellToxic.asset`，填写 `TypeName = "SpellEntity_Toxicbullet"`，`IsBullet = true`，元素消耗与图标按需填写。

**Step 4**：在背包面板中将 `SpellToxic.asset` 链接到一个法术槽。

完成，无需修改任何其他现有代码。

---

## 7. 常见问题

**Q：子弹无法生成，控制台报 `NullReferenceException`**

A：检查 `Magicscript` 资产中的 `TypeName` 是否与 C# 类名**完全一致**（包括大小写）。`Activator.CreateInstance` 对类名是大小写敏感的。

**Q：子弹生成但贴图显示为错误的 Sprite 或粉色方块**

A：确认 png 文件路径与 `Sprite_Location` 字段一致，且文件位于 `Assets/Resources/` 下（必须在 Resources 文件夹内才能通过 `Resources.Load` 加载）。

**Q：修饰符没有效果**

A：确认 `Magicscript` 资产的 `IsComponent` 已设为 `true` 且 `IsBullet` 为 `false`；同时确认资产已正确拖入背包面板槽位，且玩家在游戏中点击了"确认"按钮触发 `TestBulletManager.sequence_set()`。

**Q：派生副弹的修饰符触发了无限递归**

A：在 `Emit()` 中派生副弹时，必须先判断 `if (bb is EntityBullet eb && eb.copy == false)`，参见[第 2.4 节](#24-覆盖回调)。

**Q：法术切换后旧法术的修饰符仍在生效**

A：这是已知 bug（见 TECHNICAL_DESIGN.md § 4.7.4），`TestBulletManager.extension_num` 未在每次 `sequence_set()` 前清零。临时解法：重新打开背包重新确认配置。

---

*文档版本：v0.1，对应游戏版本 Merlin v0.1.0*
