# Unity 入门：场景、脚本与物理实验

## 目录

- [1. 基本术语](#terms)
- [2. 基本创建流程](#creation)
- [3. 脚本与 Unity 的交互](#scripts)
- [4. 第一次实验：重力与碰撞](#experiment)
- [5. 常见问题](#troubleshooting)

<a id="terms"></a>

## 1. 基本术语

| 术语 | 含义 | 示例 |
| --- | --- | --- |
| Project（工程） | 包含资源、代码、包和项目设置的工作目录 | 当前 Unity 工程 |
| Scene（场景） | 保存场景中物体及其布局、组件配置的资源 | 一个物理实验房间，文件为 `.unity` |
| GameObject（游戏物体） | 场景中的对象，通过组件获得功能 | 方块、相机、灯光、空物体 |
| Component（组件） | 挂在物体上的数据与功能 | Transform、Rigidbody、自定义脚本 |
| Transform（变换） | 保存位置、旋转、缩放；支持父子关系 | 将方块放在地面上方 |
| Prefab（预制体） | 可复用的物体模板，包含组件及子物体 | 同一箱子模板生成多个箱子实例 |
| Asset（资源） | Project 中的文件或资源对象 | 场景、脚本、材质、Prefab |
| Mesh Filter / Mesh Renderer | 分别提供网格、负责将网格渲染出来 | 方块的形状与显示 |
| Collider（碰撞体） | 定义物理碰撞形状 | Box Collider、地面的 Mesh Collider |
| Rigidbody（刚体） | 让物体参与刚体运动模拟 | 受到重力、力、冲量影响的箱子 |
| Material（材质） | 控制物体的视觉外观 | 颜色、金属感、粗糙程度 |

常用窗口：

| 窗口 | 用途 |
| --- | --- |
| Hierarchy | 查看当前已加载场景中的物体及父子关系 |
| Scene | 编辑场景，可自由观察 |
| Game | 查看游戏相机输出的画面 |
| Inspector | 查看、修改选中对象的组件和参数 |
| Project | 管理资源文件 |
| Console | 查看日志、警告、编译和运行错误 |

要点：

- GameObject 是对象，Component 提供功能；C# 脚本可以成为自定义组件。
- 场景中的 Prefab 实例与 Project 中的 Prefab 资源有联系；实例修改可形成 Override，应用后才能更新模板。
- Transform 的 `position` 是世界坐标，`localPosition` 是相对父物体的坐标；Inspector 中的位置通常显示局部坐标。
- Unity 3D 默认 Y 轴向上；物理实验通常按 1 单位约等于 1 米设置尺度。

<a id="creation"></a>
## 2. 基本创建流程

### 2.1 创建并保存场景

1. 在 Project 的 `Assets` 中创建 `Scenes`、`Scripts`、`Prefabs` 文件夹。
2. 选择 **File → New Scene**，选择基础 3D 场景模板，保留相机与灯光。
3. 选择 **File → Save As**，保存到 `Assets/Scenes/PhysicsRoom.unity`。

### 2.2 创建物体

在 Hierarchy 空白处右键，选择 **3D Object**：

| 类型 | 命名 | Position | Rotation | Scale |
| --- | --- | --- | --- | --- |
| Plane | Ground | `(0, 0, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |
| Cube | Box | `(0, 2, 0)` | `(0, 0, 0)` | `(1, 1, 1)` |

选中物体后，在 Inspector 中设置数值，通过 **Add Component** 添加组件。

观察方式：选中 Box，在 Scene 窗口按 `F` 聚焦。需要 Game 画面时，将 Main Camera 设为 Position `(0, 3, -7)`、Rotation `(15, 0, 0)`。

### 2.3 创建 Prefab

1. 将 Hierarchy 中的 Box 拖到 `Assets/Prefabs`。
2. 从 Project 中将该 Prefab 拖回 Hierarchy，生成新实例。
3. 将新实例移到其他位置，避免物体重叠。

### 2.4 保存与运行

- `Ctrl+S` 保存场景；脚本在代码编辑器中单独保存。
- 点击顶部 **Play** 开始运行，再次点击退出。
- Play 中对场景物体的常规修改，退出后通常会还原；需要保留的配置在退出 Play 后修改并保存。

<a id="scripts"></a>
## 3. 脚本与 Unity 的交互

### 3.1 从代码到组件

```text
编写并保存 .cs 文件
    → Unity 编译
    → 将脚本挂到 GameObject，形成组件实例
    → 在 Inspector 配置参数与对象引用
    → 运行时 Unity 调用生命周期和事件方法
    → 脚本读取或修改物体、组件的状态
```

### 3.2 基本调用流程

普通启用组件的初始化过程可记为：`Awake → OnEnable → Start`，随后进入反复更新。

| 方法 | 调用时机 | 常见用途 |
| --- | --- | --- |
| `Awake()` | 实例初始化时；非激活物体可能延迟到激活时 | 获取组件引用 |
| `OnEnable()` | 对象激活且组件启用时，可重复调用 | 注册事件 |
| `Start()` | 第一次更新前，满足启用条件时执行一次 | 设置初始行为 |
| `Update()` | 每个渲染帧调用 | 输入、普通逻辑、非物理动画 |
| `FixedUpdate()` | 按固定模拟时间间隔调用 | 持续施力等物理逻辑 |
| `LateUpdate()` | 每帧 Update 之后 | 相机跟随 |
| `OnCollisionEnter(Collision other)` | 开始发生碰撞接触时 | 获取碰撞对象、输出日志 |
| `OnDisable()` | 组件禁用或对象停用时 | 注销事件 |

`FixedUpdate` 每个渲染帧可能执行零次、一次或多次，不是严格的“Update 后调用一次”。不同物体的同名方法也不应默认按固定先后顺序执行。参见 [Unity 生命周期说明](https://docs.unity3d.com/6000.0/Documentation/Manual/ExecutionOrder.html)。

### 3.3 基本代码：移动方块

创建 `Assets/Scripts/MoveBox.cs`：

```csharp
using UnityEngine;

public class MoveBox : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
```

将脚本拖到 Box 上，运行后观察：

| 代码 | 作用 |
| --- | --- |
| `using UnityEngine` | 使用 Unity 提供的类型 |
| `: MonoBehaviour` | 声明为可挂载的脚本组件 |
| `[SerializeField]` | 将私有字段交给 Unity 序列化，并显示在 Inspector |
| `Vector3.right` | 世界坐标正 X 方向 `(1, 0, 0)` |
| `speed * Time.deltaTime` | 速度乘经过时间，得到本帧位移 |

`speed` 为 `1` 时向右移动，为 `-1` 时向左移动。这里没有动态 Rigidbody，位置由脚本直接控制。

<a id="experiment"></a>
## 4. 第一次实验：重力与碰撞

### 4.1 准备物体

使用第 2 节的 Ground 和 Box，实验时只保留一个 Box。

| 对象 | 必要配置 |
| --- | --- |
| Ground | 保留 Collider；取消 Is Trigger；不需要 Rigidbody |
| Box | 保留 Box Collider；取消 Is Trigger；添加 Rigidbody |
| Box 的 Rigidbody | Is Kinematic 关闭；Position 约束不勾选；初始无运动 |

### 4.3 逐项实验

每次都先退出 Play、清空 Console，再设置参数并重新运行；Box 起点保持 `(0, 2, 0)`。

| 实验 | Gravity Enabled | Initial Impulse | 其他设置 | 预期现象 |
| --- | --- | --- | --- | --- |
| A：重力下落 | 开 | `(0, 0, 0)` | 地面 Collider 开 | 方块落到地面，输出碰撞日志 |
| B：关闭重力 | 关 | `(0, 0, 0)` | 地面 Collider 开 | 方块停在空中；未接触地面，不产生接触日志 |
| C：无重力但有运动 | 关 | `(0, -2, 0)` | 地面 Collider 开 | 冲量使方块向下运动，接触地面并输出日志 |
| D：移除地面碰撞 | 开 | `(0, 0, 0)` | 地面 Collider 关 | 方块穿过地面，不产生与地面的碰撞日志 |

实验结论：

- 无重力不等于不能运动；已有速度或外力仍可让刚体运动。
- 无重力不等于没有碰撞；Collider 与刚体配置决定物理接触。
- 看得见的表面不一定能挡住物体，渲染与碰撞分别由不同组件负责。

### 4.4 完成检查

- [ ] 能创建并保存场景、创建物体和 Prefab。
- [ ] 能挂载脚本，在 Inspector 修改参数。
- [ ] 能说明 `Awake`、`Start`、`Update`、`FixedUpdate` 的基本区别。
- [ ] 能复现 A—D，并解释各自现象。
- [ ] 保存场景与脚本，保留相应 `.meta` 文件。

<a id="troubleshooting"></a>
## 5. 常见问题

| 现象 | 优先检查 |
| --- | --- |
| 脚本无法挂载 | Console 是否有编译错误；文件名是否与类名一致；是否继承 MonoBehaviour |
| 修改代码默认值没有效果 | 已有组件可能保存了 Inspector 值，直接检查并修改该值 |
| 方块不下落 | 是否进入 Play；是否有 Rigidbody；Gravity Enabled 是否打开；Is Kinematic 或位置约束是否开启 |
| 方块穿过地面 | 双方 Collider 是否启用；Is Trigger 是否关闭；是否仍有脚本直接修改位置 |
| 没有碰撞日志 | 是否真实接触；脚本是否挂在 Box；是否混用了 2D 与 3D 组件；双方至少一方是否为动态 Rigidbody |
| 运行中修改 Gravity Enabled 没变化 | 本实验只在 Start 读取参数，需停止后修改再运行 |
| 停止运行后物体回到原位 | 场景物体的常规运行时修改会还原，在编辑模式设置并保存初始状态 |


