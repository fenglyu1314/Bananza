## ADDED Requirements

### Requirement: Genre as Lighthearted 3D Mischief Sandbox

项目 SHALL 被定位为 3D 第三人称的捣蛋沙盒游戏，参考品类为《Untitled Goose Game》《A Short Hike》等"无压力探索 + 环境互动"作品。该定位 MUST 作为所有下游玩法决策的基线——任何与"竞技对抗""硬核动作""生存压力"等方向根本冲突的提案 MUST 通过修改本 requirement 的 change 才能进入项目。

#### Scenario: 某 change 提议加入 PvP 对抗模式

- **WHEN** 某玩法 change 的 proposal 中提议加入双人 PvP 对抗模式
- **THEN** 该 change MUST 被视为与本 requirement 冲突
- **AND** 在未先通过一个修改本 requirement 的 change 之前 MUST NOT 被合入

#### Scenario: 新功能需要判断是否符合项目定位

- **WHEN** 作者不确定某个具体玩法（例如"限时偷东西小游戏"）是否符合项目基调
- **THEN** 作者 MUST 回到本 requirement 与 `No Failure or Penalty Mechanics` 做对照
- **AND** 若冲突明显，MUST 放弃该玩法或调整形态使其不违反本基调

---

### Requirement: No Failure or Penalty Mechanics

游戏 MUST NOT 实现任何终局失败机制，具体包括：

- MUST NOT 存在"Game Over"状态或界面。
- MUST NOT 引入生命值、血量、耐力值等可被耗尽并导致玩家失败的资源。
- MUST NOT 引入会因超时导致任务失败、关卡回滚或玩家进度损失的全局计时器。
- MUST NOT 对"捣蛋行为"施加会累积并最终惩罚玩家的负面系统（如通缉值达到阈值后永久封锁区域）。

玩家在游戏中遇到的挫折 MUST 仅限于**局部、可立即恢复**的情景，例如被 NPC 夺回当前持有的道具、被 NPC 驱赶而暂时离开某区域。

#### Scenario: 某 change 提议加入"猴子血量"

- **WHEN** 某玩法 change 提议为主角引入血量并在血量归零时触发重生 / 回到起点
- **THEN** 该 change MUST 被视为与本 requirement 冲突并被拒绝
- **AND** 若作者确实希望引入该机制，MUST 先起草一份修改本 requirement 的 change

#### Scenario: 任务被设计为有时间限制

- **WHEN** 任务设计者试图为某条常规任务设置"30 秒内完成否则失败"的计时器
- **THEN** 该设计 MUST 被拒绝
- **AND** 若存在"时间感"的体验需求，MAY 改为环境表演（例如 NPC 只在特定时段出现），但 MUST NOT 构成玩家失败条件

---

### Requirement: NPC Pushback as Expressive Response

NPC 对玩家捣蛋行为的反应（夺回道具、驱赶、追赶）SHALL 被设计为"情景表演"，其目的 MUST 是让世界显得有反应、有个性，而非惩罚玩家。

NPC 的反应 MUST 满足：

- MUST 是**局部的**——仅限当前场景、当前时刻，不跨越场景或长期累积。
- MUST 是**可逃离的**——玩家跑开到一定距离后 NPC 会停止追赶或恢复原状态。
- MUST NOT 剥夺玩家的进度（如重置任务完成度、扣除已收集物品以外的内容）。
- MAY 暂时夺回玩家**手上当前这一件**道具，但 MUST 允许玩家再次尝试获取同类或同一道具。

#### Scenario: 玩家被 NPC 抓住手上的苹果

- **WHEN** 玩家在村庄偷了苹果后被村民追上并夺走
- **THEN** 该苹果 MUST 回到原位或被 NPC 拿回原摊位，且玩家 MUST 能再次靠近并再次尝试偷取
- **AND** 玩家 MUST NOT 因此次"失败"而受到任何持久化惩罚（进度、成就、解锁状态均不变）

#### Scenario: NPC 持续驱赶导致玩家无法游玩

- **WHEN** 某个 NPC 驱赶行为的实现让玩家在离开区域后仍被追赶超过若干秒
- **THEN** 该实现 MUST 被视为违反"局部性"约束
- **AND** 实现方 MUST 调整驱赶逻辑，使得 NPC 在玩家离开其关注范围后恢复常态

---

### Requirement: No Narrative Dependency

游戏 MUST NOT 依赖线性剧情推进关卡或角色弧线。关卡主题的差异（例如景区 → 村庄 → 加油站 → 城市场景）SHALL 被解读为"场景切换"而非"情节发展"。

- 游戏 MUST NOT 引入强制过场动画（cutscene）、对白树或角色发展线作为解锁条件。
- 游戏 MUST NOT 将"情绪弧线"（例如主角的心境变化）作为机制层的显式状态。
- 文案性内容（NPC 台词、招牌文字）MAY 存在并具备幽默或氛围价值，但 MUST NOT 作为玩家理解玩法的前提。

#### Scenario: 某 change 提议引入过场动画

- **WHEN** 某 change 提议在关卡切换时强制播放 20 秒过场动画以交代剧情
- **THEN** 该 change MUST 被视为与本 requirement 冲突并被拒绝

#### Scenario: 玩家跳过所有 NPC 对白后仍能完成游戏

- **WHEN** 玩家从不阅读任何 NPC 对话气泡或招牌文字
- **THEN** 玩家 MUST 仍能通过环境线索与任务提示完成所有常规任务并解锁所有关卡

---

### Requirement: Local 1-2 Player Shared-Screen Co-op

游戏 SHALL 支持本地 1 人或 2 人模式，且 2 人模式 MUST 为**共享屏幕**（同一视窗内显示两只猴子），MUST NOT 采用分屏形式。

- 2 人模式 MUST 通过两个手柄输入，不要求键盘玩法。
- 1 人模式下第二只猴子 MUST 不存在于游戏世界中（非 AI 托管，非隐身）。
- 两名玩家在功能、能力、可交互道具范围上 SHALL 完全对等。
- 进入 / 退出第二玩家的流程由后续 `player-character` 或类似 capability 规定。

#### Scenario: 单人玩家开始游戏

- **WHEN** 玩家以单手柄启动游戏并选择单人模式
- **THEN** 游戏世界 MUST 只存在 1 只由该玩家操控的猴子
- **AND** MUST NOT 存在任何由 AI 托管或静止的第二只猴子

#### Scenario: 双人玩家启动游戏

- **WHEN** 两名玩家以双手柄启动游戏并选择双人模式
- **THEN** 两只猴子 MUST 同时存在于同一屏幕的同一关卡中
- **AND** 游戏画面 MUST 保持单个视口，MUST NOT 切换为上下或左右分屏

---

### Requirement: Identical Player Characters

1P 与 2P 所控制的猴子 SHALL 在机制能力上完全一致，仅在美术层面（颜色、饰品等）可被区分。

- 两只猴子 MUST 使用同一套动作集（移动、抓取、投掷）。
- 两只猴子 MUST 遵循同一套交互规则（可拾起的道具、可触发的任务、NPC 识别机制）。
- 任一玩家完成的常规任务、隐藏任务、收集成就 SHALL 共享进度，MUST NOT 分别计算。

#### Scenario: 2P 完成了某个隐藏任务

- **WHEN** 在双人模式下，2P 触发了某个隐藏任务的完成条件
- **THEN** 该隐藏任务 MUST 在共享的任务列表 / 成就簿中显示为已完成
- **AND** 1P 与 2P 切换身份后 MUST NOT 再次被要求完成该任务

#### Scenario: 某 change 提议让 2P 只能爬、不能扔

- **WHEN** 某 change 提议为 2P 引入与 1P 不同的能力组合
- **THEN** 该 change MUST 被视为与本 requirement 冲突
- **AND** MUST 先通过一份修改本 requirement 的 change 才能实现

---

### Requirement: Hard Distance Cap Between Co-op Players

在双人共享屏幕模式下，两名玩家的世界坐标距离 MUST 存在一个**硬性上限**。当距离达到该上限时，试图进一步远离另一名玩家的一方 MUST 无法继续移动（其移动输入在"远离"方向上被忽略或置零）。

- 距离上限的具体数值 SHALL 由 `player-character` 或 `camera-system` capability 规定。
- 实现 MAY 提供视觉或音频反馈（例如画面边缘变色、轻微震动、音效提示），但 MUST NOT 通过"传送靠近""强制拉拽"等方式干预玩家坐标。
- 本 requirement 仅约束"硬限制"这一形态；未来如需升级为橡皮筋式软限制，MUST 通过修改本 requirement 的 change。

#### Scenario: 玩家 A 试图跑离玩家 B 过远

- **WHEN** 玩家 A 持续向远离玩家 B 的方向推摇杆，直到两者距离达到硬上限
- **THEN** 玩家 A 在该方向上的移动 MUST 被停止
- **AND** 玩家 A 在其他方向上的移动 MUST 不受影响
- **AND** 玩家 A 与 B 均 MUST NOT 被系统强制传送或拉拽以缩短距离

#### Scenario: 单人模式下距离限制

- **WHEN** 玩家以单人模式游玩
- **THEN** 本距离限制 MUST 不生效，单人玩家可在关卡内任意移动

---

### Requirement: No Netcode Abstractions in Current Scope

本阶段的玩法实现 SHALL NOT 为未来的网络联机能力预留接口层或抽象层。具体地：

- MUST NOT 强制所有玩家输入经过类似 `IPlayerInputSource` 的抽象接口。
- MUST NOT 强制状态变更走可序列化的 `Command` / `Intent` 模式。
- MUST NOT 为"随机数种子同步"等网络需求留下实现位。

未来若决定引入联机，MUST 通过一个独立的 change 统一重构；任何此类重构 MUST 也相应修改本 requirement。

#### Scenario: 某 change 提议引入 `IPlayerInputSource` 接口以"为未来联机预留"

- **WHEN** 某 change 在 proposal 中给出理由"为未来网络同步预留"
- **THEN** 该理由 MUST 被视为与本 requirement 冲突
- **AND** 该接口 MUST NOT 作为"预留"动机被合入；若有其他独立动机（如测试替身），MAY 以该动机单独立项

#### Scenario: 未来确实决定开发联机

- **WHEN** 项目未来决定引入网络联机能力
- **THEN** 团队 MUST 起草一份独立的 change（例如 `introduce-netcode`）
- **AND** 该 change MUST 在 MODIFIED / REMOVED Requirements 段中处理本 requirement

---

### Requirement: Grab and Throw as Sole Core Actions

本阶段玩家角色的核心动作集 SHALL 限定为：**移动**、**抓取**、**投掷**。任何其他动作（攀爬、跳跃挂边、二段跳、变身、咆哮、潜行等）MUST NOT 进入本阶段实现范围。

未来增量动作 MUST 通过独立 change 引入，且 MUST 在 `design.md` 中说明为何现在才加入、以及与本 requirement 的兼容方式。

#### Scenario: 某 change 提议加入攀爬

- **WHEN** 某 change 提议为猴子加入沿墙攀爬能力
- **THEN** 该 change MUST 显式声明它扩展了本 requirement
- **AND** MUST 在 MODIFIED Requirements 段中更新本 requirement 的动作集

#### Scenario: 基础跳跃是否属于核心动作

- **WHEN** 实现方不确定"站立起跳"是否属于"移动"的一部分
- **THEN** 实现方 MUST 将决定记录在 `player-character` capability 的相应 requirement 中
- **AND** 若需要引入可跨越障碍的"战术性跳跃"（例如二段跳、扑墙），MUST 按上一 scenario 的流程扩展本 requirement

---

### Requirement: Single-Item Carrying Constraint

在任一时刻，每只猴子 MUST 最多只能持有一件道具。

- MUST NOT 存在背包、库存、收纳格等允许同时携带多件道具的容器。
- 拾取新道具时，若已持有道具，实现 MUST 要求玩家**先主动扔下或投掷当前道具**，MUST NOT 自动替换。
- 本约束 MUST 对 1P 与 2P 分别独立生效；2 人协作时，两只猴子合计 MAY 同时持有两件道具，但不得"把道具递给对方"造成单只猴子持有两件。

#### Scenario: 玩家已持有道具 A，靠近道具 B

- **WHEN** 玩家角色手上正持有苹果 A 并靠近桌上的苹果 B
- **THEN** 系统 MUST NOT 自动把苹果 A 替换为 B
- **AND** 玩家 MUST 需要先触发"投掷/扔下"动作，使手上为空后才能拾起 B

#### Scenario: 某 change 提议"嘴巴可额外叼一个小物"

- **WHEN** 某 change 提议让猴子额外用嘴叼一个小物以增加携带量
- **THEN** 该提议 MUST 被视为与本 requirement 冲突
- **AND** MUST 先通过修改本 requirement 的 change 才能实现

---

### Requirement: Progressive Unlock with Free Backtracking

游戏流程 SHALL 采用"初次线性解锁 + 解锁后自由穿梭"的双阶段结构：

- 玩家首次游玩时，关卡之间的通道 MUST 初始处于封闭状态，MUST 通过完成常规任务或触发特定条件才能解锁。
- 一旦某通道被解锁，玩家 MUST 能够在任意时刻通过该通道在相邻关卡间**双向穿梭**；已访问的关卡 MUST 对玩家持续开放，MUST NOT 因剧情进度或其他原因被再次锁死。
- 后期 MAY 存在可被进一步解锁的"捷径"，以缩短跨关卡移动时间；捷径的存在与条件由 `world-structure` capability 规定。
- 具体关卡数量、拓扑、通道阻断物形态由 `world-structure` capability 规定，不属于本 spec 范畴。

#### Scenario: 玩家完成景区任务后进入村庄

- **WHEN** 玩家在初始关卡完成常规任务并解锁了通往下一关卡的通道
- **THEN** 该通道 MUST 持续保持可双向通行，玩家 MUST 能随时回到初始关卡继续探索

#### Scenario: 某 change 提议"过关后前关自动封闭"

- **WHEN** 某 change 提议在玩家进入下一关卡后自动封闭上一关卡
- **THEN** 该 change MUST 被视为与本 requirement 冲突并被拒绝

---

### Requirement: Persistent World State Across Visits

玩家已解锁并离开的关卡，其世界状态 MUST 在玩家再次返回时保持与离开时一致。具体地：

- 玩家拿走的道具 MUST 仍然不在原位（除非游戏有明确的"刷新"机制且该机制由独立 requirement 规定）。
- 玩家造成的环境变化（被推倒的桌子、被砸碎的花瓶）MUST 在返回后保持该变化状态。
- NPC 对玩家过往行为的"记忆"是否延续由 `npc-ai` capability 决定，但**世界物体的物理状态** MUST 保持一致。
- 具体持久化实现（内存常驻 / 序列化存档 / `Registry` 设计）属于 `world-state-persistence` 或等价 capability，不属于本 spec 范畴。

#### Scenario: 玩家在景区拿走一朵花后去村庄再返回

- **WHEN** 玩家在景区拾起一朵花并走到村庄，之后再返回景区
- **THEN** 原位置 MUST 仍然没有那朵花（除非存在独立 requirement 定义的刷新规则）

#### Scenario: 玩家打碎村庄花瓶后去加油站再返回

- **WHEN** 玩家在村庄砸碎一个花瓶，前往加油站完成任务后返回村庄
- **THEN** 该花瓶 MUST 仍然处于被砸碎状态，MUST NOT 被重置为完整

---

### Requirement: Three-Tier Objective System

游戏 SHALL 同时提供三类目标，三者并存且互不包含：

1. **常规任务（Regular Tasks）**：在任务列表中**显性展示**，完成后 MAY 触发通道解锁或可见奖励；每关 SHOULD 有若干条常规任务（具体数量由 `world-structure` 或 `task-system` capability 规定）。
2. **隐藏任务（Hidden Tasks）**：完成前 MUST NOT 在任何 UI 中提示其存在（包括无进度条、无计数、无"？"占位）；完成后 SHALL 出现在任务列表 / 成就簿中作为可见条目。
3. **全局收集成就（Global Collection Achievements）**：跨多个关卡达成的目标（例如"从每个关卡各带一件特定物品到指定位置"）；其完成需要借助 `Progressive Unlock with Free Backtracking` 定义的回访能力。

本 requirement 仅规定三层目标的**存在性与可见性策略**；具体配置机制（TaskGraph 节点、触发器、奖励发放）由 `task-system` capability 规定。

#### Scenario: 玩家首次进入关卡

- **WHEN** 玩家首次进入某关卡并打开任务列表
- **THEN** 该关卡的所有常规任务 MUST 以条目形式显示
- **AND** 该关卡的所有隐藏任务 MUST NOT 以任何形式显示（无计数、无占位符）

#### Scenario: 玩家无意中完成了一个隐藏任务

- **WHEN** 玩家在捣蛋过程中无意触发了某个隐藏任务的完成条件
- **THEN** 该隐藏任务 MUST 在任务列表或成就簿中新增为"已完成"条目
- **AND** 任何此前未完成的隐藏任务 MUST 仍然完全不可见

#### Scenario: 全局收集成就需要跨关卡回访

- **WHEN** 某全局收集成就要求玩家"从四个关卡各带一朵花到指定地点"
- **THEN** 该成就 MUST 能通过 `Progressive Unlock with Free Backtracking` 定义的回访能力在多次往返中完成
- **AND** 玩家 MUST NOT 因路径限制而被剥夺完成该成就的可能性

---

### Requirement: Task List Visibility Policy

任务可见性 MUST 遵循以下规则：

- 常规任务 SHALL 在任务列表中始终可见，无论是否已完成。
- 隐藏任务 MUST 在完成前完全不可见——不允许进度条、分母（"1/4"）、问号占位、模糊描述等任何形式的暗示。
- 隐藏任务 SHALL 在完成后显示为已完成条目，供玩家回顾所做的事。
- 全局收集成就 MAY 采用上述任一策略，具体由 `task-system` capability 规定。

#### Scenario: 任务列表的默认视图

- **WHEN** 玩家打开任务列表
- **THEN** MUST 见到所有当前关卡的常规任务
- **AND** MUST 见到所有已完成的隐藏任务（如果有）
- **AND** MUST NOT 见到任何尚未完成的隐藏任务的任何信息

#### Scenario: 某 change 提议"为隐藏任务加进度提示"

- **WHEN** 某 change 提议在 UI 上加入"已发现隐藏任务 3/5"的提示
- **THEN** 该 change MUST 被视为与本 requirement 冲突并被拒绝

---

### Requirement: Lighthearted Visual and Audio Tone

项目的视觉与音频基调 SHALL 保持**简约、明亮、卡通**，MUST NOT 走向写实、黑暗、恐怖、血腥或压抑的方向。

- 视觉 MUST 让玩家在第一眼就感受到"轻松可爱"；色彩 SHOULD 偏明亮、饱和度适中。
- 音频（BGM、环境音、NPC 语音/拟声）MUST 与视觉基调一致，MUST NOT 出现惊吓、压迫、惊悚类音效作为常态元素。
- 本 requirement 仅锁定**语义层基调**，不规定具体技术规格（面数上限、光照方案、配乐风格）——这些由 `art-direction` 或 `audio-direction` 等 capability 规定。

#### Scenario: 美术方案评审

- **WHEN** 团队评审首版关卡美术方案
- **THEN** 评审者 MUST 检查整体观感是否"简约明亮卡通"，若出现写实或阴暗倾向 MUST 要求返工

#### Scenario: 音效库选择

- **WHEN** 为 NPC 被惊吓的反应挑选音效
- **THEN** 应选用**夸张卡通化**的惊呼（例如上升滑音、喜剧性 sting），MUST NOT 选用写实的尖叫或惊悚音效

---

### Requirement: Short-Form Complete Experience

项目 SHALL 以"短篇完整作品"为目标形态，预期首次通关主线流程时长在 **3-5 小时**量级，并包含多个主题关卡。

- MUST 在项目 scope 中同时涵盖"垂直切片打磨"与"全量关卡铺设"两个阶段，MUST NOT 永远停留在单关卡 Demo 状态。
- MUST NOT 扩展为 10 小时以上的长篇作品，除非通过一份专门的 change 修改本 requirement。
- 具体关卡数量、每关规模由 `world-structure` capability 规定。

#### Scenario: 某 change 提议加入"无尽模式"

- **WHEN** 某 change 提议加入可无限游玩的 roguelike 无尽模式
- **THEN** 该 change MUST 被视为与本 requirement 的"短篇完整"定位冲突
- **AND** 仅允许作为独立可选模式，且 MUST 在 design 中说明为何不稀释主线资源

#### Scenario: 估算项目范围

- **WHEN** 作者为某 change 估算工作量
- **THEN** 作者 MUST 以"3-5 小时主线"为参照，避免把单关卡做成需要数小时才能通关的大型沙盒

---

### Requirement: Modern China Thematic Setting

游戏世界观 SHALL 以"**现代中国**"日常场景为主题（例如景区、村庄、公路沿线、城市街区等）。

- 各关卡的具体地点、布局、NPC 设定属于 `world-structure` capability 范畴，不属于本 spec。
- 本 requirement 仅约束**题材大类**：关卡 MUST NOT 设定为奇幻王国、科幻太空、西方中世纪等与"现代中国"主题明显冲突的世界观，除非通过修改本 requirement 的 change。
- 涉及真实地名、真实品牌、真实人物的内容 MUST 采用虚构化名或明显艺术加工，以规避权利风险。

#### Scenario: 关卡原型命名

- **WHEN** 作者为某景区关卡命名
- **THEN** 该名称 MUST NOT 直接使用真实地名（例如"峨眉山"）
- **AND** SHOULD 采用虚构化名（例如"峨×山""青猿岭"）或明显卡通化的称呼

#### Scenario: 某 change 提议把某关卡改为"外星基地"

- **WHEN** 某 change 提议将一个关卡重设定为科幻外星基地
- **THEN** 该提议 MUST 被视为与本 requirement 冲突
- **AND** MUST 先通过修改本 requirement 的 change 才能进入项目

---

### Requirement: Single-Machine Delivery Scope

本阶段的设计与实现 SHALL 以"本地单机游玩"为**唯一**目标交付形态。

- MUST NOT 引入网络联机、云存档、在线排行榜、社交分享、成就服务器同步、远程配置下发等任何需要网络连接才能正常运行的功能。
- 游戏 MUST 在首次安装后、无任何网络连接的情况下完整可玩。
- 未来若计划引入任一上述能力，MUST 通过独立 change 引入，且该 change MUST 在 MODIFIED Requirements 段中相应修改本 requirement。

#### Scenario: 玩家在无网络环境下游玩

- **WHEN** 玩家在完全离线的 PC 上启动本游戏
- **THEN** 游戏 MUST 能完整启动、完整游玩、正常保存进度
- **AND** MUST NOT 因无网络而出现任何功能缺失或错误提示

#### Scenario: 某 change 提议加入"Steam 成就同步"

- **WHEN** 某 change 提议在后端引入 Steamworks 成就上传
- **THEN** 该 change MUST 被视为与本 requirement 冲突
- **AND** MUST 先通过修改本 requirement 的 change 才能引入，且必须证明无网络时游戏仍可正常游玩
