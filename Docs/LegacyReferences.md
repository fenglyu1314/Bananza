
# 旧版 Monkey_Demo 项目复盘 — 可复用设计参考

> 目的：梳理旧 Demo 在"一边探索一边开发"中沉淀下来的、值得在新项目中保留或参考的设计。  
> 生成时间：2026-05-01  
> 源工程：`Monkey_Demo/` — 阶段 P1~P5 + P7/P8 任务编辑器 已实现；P6 攀爬 暂停。

---

## 一、项目整体评估

### 1.1 目录布局（建议继承）

```
Assets/
├─ Scripts/
│   ├─ Core/        # 全局服务（GameManager、Registry、Interactable）
│   ├─ Player/      # 猴子控制、输入
│   ├─ NPC/         # NPC 基类与具体类型
│   ├─ AI/          # (新版 AI 框架探索：Brain / Action / Stimulus)
│   ├─ Props/       # 道具抓取与投掷
│   ├─ Locations/   # 位置标记（任务用）
│   ├─ Tasks/       # 任务数据与节点运行时
│   ├─ Camera/      # 俯视角双人相机
│   └─ UI/          # 任务清单等
├─ Editor/
│   └─ TaskEditor/  # GraphView 可视化任务编辑器
└─ Data/
    ├─ Props/ | NPCs/ | Tasks/
```

**优点**：模块划分明确、职责单一；数据（ScriptableObject）与逻辑（MonoBehaviour）分离。新项目建议延续这个分层。

### 1.2 技术栈（建议继承）

| 项目 | 方案 |
|------|------|
| 引擎 | 团结引擎（Tuanjie Engine） |
| 渲染 | URP |
| 输入 | Unity Input System |
| 寻路 | NavMeshAgent |
| 数据 | ScriptableObject + `SerializeReference` |
| 节点编辑器 | UIElements GraphView |

### 1.3 主要问题（重写时应规避）

| 问题 | 表现 | 重写建议 |
|------|------|----------|
| **两套 NPC 架构并存** | `NPC/NPCBase + States/` 与 `AI/NPCBrain + Actions + Stimulus` 同时存在，互不衔接 | 只保留一套（推荐 AI/ 的 Brain + Stimulus 思路） |
| **输入系统硬编码** | `MonkeyController.ReadInput()` 中直接根据 `_playerIndex` 判定键盘/手柄 | 使用 `PlayerInputManager` + Input Actions 统一设备分配 |
| **单例 + 静态 Registry 混用** | `GameManager` 是 MonoBehaviour 单例，`InteractableRegistry` 是静态类；场景切换时清理时机不一致 | 统一通过一个 `ServiceLocator / GameContext` 管理；静态缓存必须在场景卸载前显式 `Clear()` |
| **任务条件历经三次重构** | 扁平 `conditions` → 条件树 `rootCondition` → 节点图 `TaskGraph`，旧字段残留 | 新项目**直接用节点图**，不保留扁平列表 |
| **FindObjectsByType 滥用** | `NPCStateDetector.CheckNPCState` 每次求值遍历场景所有 NPC | NPC 也走 `Registry` 统一注册查询 |
| **状态机的行为耦合在状态里** | `GuardPatrolState/GuardChaseState` 等散布在 `NPCGuard.cs` 中，共享数据通过 `NPCBase` 字段传递 | 数据驱动化：状态/转换规则放到 `NPCProfile` SO |
| **条件检测轮询式** | `TaskManager` 每 0.5s 全量求值所有任务的整张节点图 | 事件驱动 / 脏标记（物品 / NPC / 玩家事件触发条件重新求值） |

---

## 二、值得保留 / 参考的设计

以下每一条都给出"**为什么好**"与"**新项目怎么用**"。

### 2.1 ⭐ 类型驱动 + 运行时注册表（`Interactable` + `InteractableRegistry`）

**核心思想**：任务条件引用 SO 类型（PropData.typeId），场景实例在 `OnEnable` 自动注册到全局表；任务求值时按 typeId 查询。

**关键文件**：
- [Interactable.cs](Monkey_Demo/Assets/Scripts/Core/Interactable.cs)
- [InteractableRegistry.cs](Monkey_Demo/Assets/Scripts/Core/InteractableRegistry.cs)
- [LocationMarker.cs](Monkey_Demo/Assets/Scripts/Locations/LocationMarker.cs)

**为什么好**：
1. 彻底解耦了 SO（磁盘资产）和场景对象（无法直接序列化引用）。
2. 策划在 Prefab 上配置 PropData，地编只管拖放，无需改脚本。
3. Registry 提供 `GetAllOfType / CountMatch / AnyMatch` 等查询 API，覆盖 90% 任务检测需求。

**新项目建议**：
- 直接移植，但扩展为**支持层级查询**：PropData 有 `baseType` 字段可做类型继承（例："所有食物"、"所有帽子"）。
- 统一所有"可被任务引用的对象"基类——**NPC、SafeZone、Prop、Location、Interactable Trigger 全走同一个注册机制**（旧项目 SafeZone、LocationMarker 已经这样做了，NPC 没有，这是漏洞）。

### 2.2 ⭐ 道具数据结构 `PropData`

参考 [PropData.cs](Monkey_Demo/Assets/Scripts/Props/PropData.cs)。字段设计完整度很高，可以几乎原样保留：

```
typeId / category / tags / baseType / isUnique  (标识与分类)
weight / throwForce / throwAngle              (物理)
holdOffset / holdRotation                     (抓取姿态)
canBeThrown / canBePlaced / destroyOnImpact   (行为标志)
grabSound / throwSound / impactSound          (音效)
```

**保留**：这套字段结构。  
**可改进**：把音效改成 `AudioClipRef`（Addressable）或引入 `AudioEventSO`，避免直接硬引用。

### 2.3 ⭐ 任务节点图系统（TaskGraph / TaskNodeBase / 端口定义）

**关键文件**：
- [TaskGraph.cs](Monkey_Demo/Assets/Scripts/Tasks/Nodes/TaskGraph.cs) — 运行时求值器
- [TaskNodeBase.cs](Monkey_Demo/Assets/Scripts/Tasks/Nodes/TaskNodeBase.cs) — 节点基类
- [PortDefinition.cs](Monkey_Demo/Assets/Scripts/Tasks/Nodes/PortDefinition.cs) — 类型化端口
- `DetectorNodes / LogicNodes / MathNodes / FlowNodes / CompareNodes / ValueNodes` — 节点库
- [TaskNodeGraphView.cs](Monkey_Demo/Assets/Editor/TaskEditor/TaskNodeGraphView.cs) — GraphView 画布

**为什么好**：
1. 参考 UE 蓝图的"**值节点 → 比较 → 逻辑 → 检测 → 完成**"管线，可以让策划不写代码完成绝大多数任务逻辑。
2. 端口类型（Bool/Int/Float/String/Object）做了类型匹配校验。
3. `TaskGraph.Evaluate` 是"拉取式求值"：从输出节点向上递归，设计极简。
4. `[SerializeReference]` 解决了多态节点的序列化。
5. 支持便签（StickyNote）和注释框（Group），策划友好。

**新项目建议**（强烈保留）：
- 核心节点分类可直接复用：`Value / Compare / Math / Logic / Flow / Detector`。
- 但 **Evaluate 的轮询调用需要优化**：改成"事件驱动"——Registry 在数据变化时发信号，任务只对受影响的节点重新求值。
- 节点图存储格式已经稳定（`TaskGraph` + `[SerializeReference] nodes` + `NodeConnection`），可直接迁移现有 Task 资产。

### 2.4 ⭐ 安全区（`SafeZone`）+ 驱赶系统（`RepelSystem`）

**关键文件**：
- [SafeZone.cs](Monkey_Demo/Assets/Scripts/Core/SafeZone.cs)
- [RepelSystem.cs](Monkey_Demo/Assets/Scripts/NPC/RepelSystem.cs)
- `MonkeyController` 中的 `StartBeingRepelled / HandleFleeMovement`

**为什么好**：
- 用"**逃向最近安全区**"替换了原作"押送到出口"的繁琐逻辑，手感更直接、代码更简洁。
- `SafeZone.FindNearest` + `IsInAnySafeZone` 作为静态查询 API，设计非常利落。
- 支持**安全区优先级**，方便关卡设计多层逃跑策略。

**保留要点**：这是"**被抓机制**"的核心玩法框架，新项目基本可以原样继承。

### 2.5 刺激-响应 AI 框架（Stimulus / NPCBrain / Action）

**关键文件**：
- [Stimulus.cs](Monkey_Demo/Assets/Scripts/AI/Core/Stimulus.cs) — 刺激数据
- [StimulusManager.cs](Monkey_Demo/Assets/Scripts/AI/Core/StimulusManager.cs) — 按优先级排序
- [NPCBrain.cs](Monkey_Demo/Assets/Scripts/AI/Core/NPCBrain.cs) — 决策中心
- [NPCProfile.cs](Monkey_Demo/Assets/Scripts/AI/Configs/NPCProfile.cs) — 数据配置 SO
- `AI/Actions/` — IdleAction / ChaseAction / WatchAction / WorkAction

**为什么好**：
1. 方向对了：用 **"感知-决策-行为"三层** 替代硬编码的 FSM。
2. `NPCProfile` SO 把参数、刺激响应、日常任务都数据化，策划能配置不同 NPC 类型。
3. `Stimulus` 带 `priority / duration / source`，天然支持"多个刺激竞争"。
4. `Action` 有 `CanBeInterrupted / Priority / IsComplete`，框架扩展性好。

**新项目建议**：
- **保留这套架构并成为唯一 AI 系统**（替换掉旧的 `NPCBase + INPCState` 体系）。
- `StimulusType` 枚举改为**可扩展的 ID/SO**，避免枚举膨胀。
- 刺激的**发布端**（谁抛出 Stimulus）旧项目还没做完，需要新项目统一：比如"物品被偷"应由 `GrabSystem` 在抓取时广播。
- 引入 **感知半径 / 视线 / 听觉** 分层过滤（现在只按 `responseRange`）。

### 2.6 双人俯视角相机（`CoopCameraController`）

**关键文件**：[CoopCameraController.cs](Monkey_Demo/Assets/Scripts/Camera/CoopCameraController.cs)

**为什么好**：
- "跟随中点 + 按距离动态拉高" 是 Untitled Goose Game 类游戏的标配。
- `GameManager.RegisterPlayer` 自动把玩家注册到相机，解耦得很好。

**保留要点**：沿用接口设计。细节可以加入**边界吸附**、**敌人警戒时镜头轻微缩放**等增强。

### 2.7 GraphView 编辑器方案 & 节点位置持久化

旧项目的 `TaskEditorWindow` 三栏布局 + `TaskNodeGraphView` 用的是 Unity **UIElements + GraphView** 原生 API（不是第三方插件），并把节点位置/连线/便签/注释框都保存到 `TaskGraph` 内部。

**保留要点**：
- 架构可复用。
- 但 `TaskNodeGraphView.cs` 已经 **57KB / 超 1500 行**，太臃肿，说明编辑器一直在小步迭代加功能。新项目开始前应先设计清晰的**子模块划分**（例如把"节点创建菜单、连线校验、分组、便签"拆成独立管理器）。

### 2.8 任务数据的三层组织

```
TaskListData (关卡一张列表)
    ├─ mainTasks:   List<TaskData>
    ├─ sideTasks:   List<TaskData>
    └─ hiddenTasks: List<TaskData>   (靠 prerequisites + unlocksOnComplete 解锁)

TaskData
    ├─ displayName / description / taskType
    ├─ taskGraph: TaskGraph        (节点图条件)
    ├─ prerequisites / unlocksOnComplete
    └─ onTaskComplete: UnityEvent
```

这套结构支持"主线/支线/隐藏"三类任务，任务间依赖关系清晰。**保留**。

### 2.9 开发规范与流程（`.augment-guidelines`）

[.augment-guidelines](.augment-guidelines) 里定义好的命名、中英混用、团结引擎兼容等规则，**原样保留**。

---

## 三、重写时的结构性建议

基于上面的复盘，给新项目几条**顶层架构**建议：

### 3.1 统一 Registry 架构

```
IRegistrable (interface)
    ↑ 实现
  ┌───────────┬────────────┬──────────┬──────────┐
 Prop      NPC        Location   SafeZone   Player (?)

GameRegistry (统一查询入口)
    ├─ ByType<T>(typeId)
    ├─ OnAdded<T> / OnRemoved<T>  (事件)
    └─ OnStateChanged<T>          (NPC 进入新状态、物品被抓等)
```

这样任务节点 / AI 感知 / UI 都走同一套查询接口。

### 3.2 AI 单轨制

只保留 **Brain + Stimulus + Action** 一条线，彻底删掉 `NPCBase + INPCState + NPCGuard.cs` 里那堆 state 类。守卫的"巡逻-警戒-追逐-搜索-返回"应通过一组 `Action` 组合实现，行为优先级用 Utility / priority 值控制。

### 3.3 事件驱动任务检测

```
Stimulus 发布   ─┐
Prop 状态变化  ─┼─> EventBus ─> TaskManager 重新求值相关 Task
NPC 状态变化   ─┘
```

放弃 0.5s 轮询整图。节点图保留，但 `TaskGraph.Evaluate()` 只在**依赖的数据源发生变化**时触发。

### 3.4 输入重构

用 `PlayerInputManager` + `PlayerInput` 组件 + Input Actions，支持：
- 动态玩家加入/离开
- 自动设备分配（键盘和任一手柄各一个玩家）
- 按键重映射

从 `MonkeyController.ReadInput()` 的硬编码彻底解放。

### 3.5 数据驱动的 NPC 配置

`NPCProfile` 继续扩展：
```
NPCProfile
  ├─ 基础参数 (速度、感知)
  ├─ 刺激响应表 (StimulusType → ResponseType + 优先级)
  ├─ 日常任务 (定点定时的工作)
  └─ 巡逻路径 ID (配合 Scene 里用 PatrolPathData SO 存点位)
```

让"村民 / 摊贩 / 守卫 / 老人 / 小孩" 变成**5 个 SO 配置**，不再是 5 个继承自 NPCBase 的子类。

---

## 四、迁移清单（把旧资产带到新项目）

可以直接复制/移植：

| 旧资源 | 建议做法 |
|--------|----------|
| `Assets/Suriyun/`（猴子模型/动画） | 直接复制 |
| `Assets/Akishaqs/` | 直接复制 |
| `Assets/PolygonFarm/` | 按需复制（场景搭建素材） |
| `Settings/URP-*.asset` | 直接复制 |
| `Data/MonkeyInputActions.inputactions` | 复用并扩展 |
| `Data/Props/*.asset` | 迁移，typeId 保留兼容 |
| `Data/Tasks/*.asset` | 迁移（TaskGraph 结构不变） |
| `Docs/` 其他文档 | 作为需求/规则参考 |

**需要重写**（不建议迁移代码）：
- 所有 `NPC/States/*.cs` 及 `NPC/Types/*.cs`
- `MonkeyController.cs` 的输入部分
- `TaskManager.cs` 的轮询逻辑

---

## 五、一页纸结论

> 旧 Demo 最大的资产是 **"数据层 + 编辑器层"**：
> - `PropData / LocationData / TaskData / NPCProfile / TaskGraph` 的 SO 设计已相当成熟；
> - GraphView 任务编辑器能让策划直接配置逻辑；
> - 类型驱动的 Registry 解决了 SO 无法引用场景实例的痛点。
>
> 旧 Demo 最大的负担是 **"运行时层"**：
> - NPC 有两套架构；
> - 任务检测是轮询；
> - 输入硬编码。
>
> **重写策略**：数据层/编辑器层 **保留并打磨**，运行时层 **按事件驱动 + Brain/Stimulus 方案重构**。
