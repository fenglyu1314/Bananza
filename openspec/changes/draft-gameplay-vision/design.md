## Context

Bananza 是一个基于团结引擎（Tuanjie）的单机 3D 小游戏项目，当前状态：

- 仓库已建立目录布局规范（`project-layout`）与文档语言规范（`documentation-conventions`）。
- `Assets/Scripts/` 下仅有两个基线 `.asmdef`（`Bananza.Runtime` / `Bananza.Editor`），尚无任何玩法代码。
- 存在一份对旧项目的复盘文档 [LegacyReferences.md](../../../Docs/LegacyReferences.md)，记录了可继承的旧 Demo 资产（`GrabSystem`、`PropData`/`PropRegistry`、TaskGraph 编辑器、`NPCBrain` + Stimulus AI、`SafeZone`/`Repel`、共享屏幕相机）与应规避的坑（场景切换 `Registry` 未清理、两套 NPC 架构并存、攀爬未完成）。

项目团队规模为单人（用户本人）为主，目标为轻量可完成的短篇作品。在正式开始任何玩法模块实现之前，**缺乏一份顶层参照文档**——所有具体设计讨论都会不断回到"我们究竟在做什么游戏"的元问题。本 change 的目的正是把已经通过多轮 explore 讨论确定的 17 条顶层决策固化为规范。

## Goals / Non-Goals

**Goals:**

- 建立 capability `gameplay-vision`，作为项目玩法侧的**单一事实源**，为未来所有具体能力 change 提供引用锚点。
- 将多轮讨论中达成的 17 条顶层决策翻译为 RFC 2119 语义的 requirement，每条可被后续 change 明确引用（例如 "本 change 实现 `gameplay-vision` 的 Requirement: Grab and Throw as Sole Core Actions"）。
- 明确**不属于本 spec**的内容（关卡拓扑、相机算法、美术细节、具体任务配置机制），避免单个 capability 膨胀。
- 为下游文档（`README.md`、`AGENTS.md`）提供可回链的权威源。

**Non-Goals:**

- 不定义任何实现细节（C# 类结构、组件挂载、数据 schema）——这些属于后续各自 capability 的 spec。
- 不描述具体关卡（景区/村庄/加油站/酒吧的内容、尺度、NPC 配置）——留给 `world-structure` 或类似 capability。
- 不规定相机具体算法（视野插值、平滑曲线、跟随偏移）——留给 `player-character` 或 `camera-system`。
- 不规定美术风格的具体数值（面数上限、色彩范围、光照方案）——留给 `art-direction`。
- 不为未来可能引入的网络联机留 API 层预留——用户已明确接受"未来要联机时重构本地实现"。
- 不产出任何 C# 代码或资源。

## Decisions

本 change 的核心产出是一份 spec，因此"决策"即对 17 条顶层玩法选择的记录。下面按主题分组，逐条写明**选择**、**理由**与**曾考虑的替代方案**。spec 文件中每条 requirement 都与本节一条或多条决策对应。

### D1. 品类定位：3D 捣蛋沙盒 Goose-like

- **选择**：3D 第三人称捣蛋沙盒，参考品类为《Untitled Goose Game》《A Short Hike》。
- **理由**：与旧 Demo 资产高度契合（`GrabSystem`、`NPCBrain`、`SafeZone` 皆为此类玩法服务），且该品类在单人小团队开发下可行性高。
- **替代方案**：派对竞技（`Gang Beasts`/`Fall Guys`）——旧资产几乎不适用；动作平台（`Donkey Kong Country`）——关卡工作量过大；工作模拟（`Overcooked`）——偏离"捣蛋"核心。
- **对应 requirement**：`Genre as Lighthearted 3D Mischief Sandbox`。

### D2. 无失败 / 无惩罚 / 无压力基调

- **选择**：游戏 MUST NOT 包含任何终局失败机制（Game Over、生命值、限时任务、存档点回滚）。NPC 夺回道具或驱赶玩家仅为**情景表演**，玩家自由逃离即可继续。
- **理由**：用户明确希望"玩家可以随便探索、随便捣乱、没有心理负担"。这一基调锁死了大量后续系统选择（无血量、无计时、无 Game Over UI），能显著简化实现。
- **替代方案**：引入"被抓三次回关卡起点"等软惩罚——被用户显式否决。
- **对应 requirement**：`No Failure or Penalty Mechanics`、`NPC Pushback as Expressive Response`。

### D3. 不承载叙事

- **选择**：游戏 MUST NOT 依赖线性故事推进关卡，场景变化仅作为视觉与任务主题的切换，不代表角色弧线或情绪转变。
- **理由**：用户明确"不需要讲故事"。避免为了叙事强行加入对白系统、过场动画等高成本模块。
- **替代方案**：做一条"猴子下山"的情绪弧线（从野生到被文明吞噬）——在 explore 阶段曾被讨论，最终被用户否决。
- **对应 requirement**：`No Narrative Dependency`。

### D4. 玩家模式：本地 1-2 人手柄共享屏幕

- **选择**：游戏 SHALL 支持本地 1 人或 2 人模式；2 人时两名玩家共享同一屏幕；1 人时第二只猴子直接不存在（非 AI 托管）。
- **理由**：用户明确"初期项目尽量简单，先做本地双人"，且不希望第二玩家由 AI 托管（会引入额外 AI 工作量）。
- **替代方案**：分屏——性能与 UI 复杂度翻倍；强制双人——提高用户门槛；AI 托管 2P——增加额外 AI 负担。
- **对应 requirement**：`Local 1-2 Player Shared-Screen Co-op`。

### D5. 玩家距离硬限制

- **选择**：当两名玩家在共享屏幕中的距离达到上限时，**推离摄像机中心的一方 SHALL 无法继续远离**（输入被吞），直到另一方靠近。超距时 MAY 通过视觉/音效反馈提示。
- **理由**：硬限制实现最简（一个距离判断 + 输入过滤），符合"初期尽量简单"。用户接受"后续如体验不佳再优化为橡皮筋"。
- **替代方案**：橡皮筋软限制（阻力递增）——实现稍复杂；分屏——已被 D4 排除。
- **对应 requirement**：`Hard Distance Cap Between Co-op Players`。

### D6. 暂不为网络联机做架构预留

- **选择**：本阶段的代码实现 SHALL NOT 为"未来网络联机"留抽象层（例如强制的 `PlayerInputSource` 接口、`Command` 模式、随机数种子同步）。真正要做联机时 MUST 通过一次独立的重构 change 引入。
- **理由**：用户明确"未来考虑加入"，但未承诺一定做；用户又同时要求"初期尽量简单"。过早引入抽象层会抬高整个项目心智负担。
- **替代方案**：立即用 NGO / Mirror；做出输入抽象但不同步状态——都被用户明确否决为"过度设计"。
- **对应 requirement**：`No Netcode Abstractions in Current Scope`。
- **风险与缓解**：未来接联机成本相对方案 B 更高——接受，因为"未来是否联机"本身不确定。

### D7. 主角：两只基本相同的猴子

- **选择**：1P 与 2P 的猴子 SHALL 在能力上完全一致，仅在美术上以颜色/饰品区分。
- **理由**：用户确认"两个玩家的猴子都是差不多的"。对称设计最简，双人体验无上手门槛。
- **替代方案**：功能差异化（一只手大一只手小、一只能爬一只能跳）——开发量暴涨，被延后到未来增量。
- **对应 requirement**：`Identical Player Characters`。

### D8. 核心动作：移动 + 单手单道具的抓取与投掷

- **选择**：本阶段玩家核心动作 SHALL 限定为移动、抓取、投掷；任何时刻猴子 MUST 最多只能持有一个道具；其他动作（攀爬、变身、嘶吼等）一律延后到未来增量 change。
- **理由**：用户明确"核心动作依然还是以抓取投掷，其他功能后面再考虑添加"。单道具限制产生"取舍"的玩法张力——要拿新东西必须先扔掉当前的。
- **替代方案**：背包系统（多槽位/无限）——用户明确否决；嘴里能多叼一个小物——在 explore 阶段被讨论过，最终用户选择更严格的单道具。
- **对应 requirement**：`Grab and Throw as Sole Core Actions`、`Single-Item Carrying Constraint`。

### D9. 关卡结构：初线性解锁，解锁后自由穿梭 + 捷径

- **选择**：游戏开始时关卡间的通道 SHALL 由常规任务解锁；一旦解锁，玩家 SHALL 可以自由往返于所有已访问的关卡，后期 MAY 出现捷径进一步缩短路径。
- **理由**：用户明确"各个关卡之间可以随便穿梭，甚至在各个通道之间的限制被解除后还有捷径"。这让跨关卡收集任务（D11）可行，也避免"错过支线就过了"的焦虑。
- **替代方案**：纯线性（过关即锁前关）——与 D11 冲突；完全开放（初始就能去任何地方）——削弱解锁的乐趣。
- **对应 requirement**：`Progressive Unlock with Free Backtracking`。
- **注**：具体关卡数量与拓扑放在 `world-structure` capability。

### D10. 关卡常驻策略（本 spec 只约束"可自由穿梭"，不约束加载策略）

- **选择**：本 spec **不规定**关卡采用"全部常驻"还是"按需加载"。仅约束**可观察行为**：玩家无论何时返回已解锁关卡，其世界状态 MUST 与离开时一致（见 D12）。
- **理由**：加载策略属于实现细节，应由 `world-structure` 或 `world-loading` capability 决定；本 spec 保持抽象层级纯净。
- **替代方案**：在本 spec 直接写死"全部常驻"——会让 spec 与实现选择耦合，日后需要调整时要改本 spec。
- **对应 requirement**：无专门条目；见 `Persistent World State Across Visits`。

### D11. 三类目标：常规任务 / 隐藏任务 / 全局收集

- **选择**：游戏 SHALL 并存三类目标：
  1. **常规任务**：任务列表显性展示，完成后 MAY 解锁通道。
  2. **隐藏任务**：仅在玩家完成后才在任务列表 / 成就簿中显示，玩家探索或捣蛋时被动触发。
  3. **全局收集成就**：跨关卡道具类目标（例如"从每个关卡各取一朵花放到指定位置"），需要在通道解锁后进行回访。
- **理由**：用户明确这三层结构，并强调隐藏任务"完成才显示"是其关键体验点。三类目标共同支撑 3-5 小时的短篇时长与回访价值。
- **替代方案**：只做常规任务——缺少惊喜；隐藏任务有进度提示——用户明确否决。
- **对应 requirement**：`Three-Tier Objective System`。

### D12. 跨关卡世界状态持久化（观察要求）

- **选择**：已访问过的关卡，其世界状态（被拿走的道具是否还在、被破坏的物体是否仍破坏、NPC 的位置与记忆）MUST 在玩家离开与返回之间保持一致。
- **理由**：D9 的自由穿梭 + D11 的全局收集成就要求玩家能在多关卡之间搬运道具，必须有稳定的跨关状态语义，否则"偷花带到下一关"等核心玩法不可能。
- **替代方案**：关卡状态每次进入都重置——直接破坏 D11 的全局收集。
- **对应 requirement**：`Persistent World State Across Visits`。
- **注**：具体实现机制（`PropRegistry` 扩展 / `WorldStateRegistry` / 存档序列化）留给后续 capability。

### D13. 任务列表 UI 的显示策略

- **选择**：常规任务 SHALL 在任务列表中始终可见；隐藏任务 MUST NOT 在完成前以任何形式提示（无进度条、无计数、无"？"条目）；完成后 SHALL 出现在列表或图鉴中。
- **理由**：用户明确选择此策略。保护"发现的惊喜"这一核心体验。
- **替代方案**：完全不展示（哪怕完成了）——削弱成就感；进度提示——被显式否决。
- **对应 requirement**：`Task List Visibility Policy`。

### D14. 美术风格：Low-poly 简约风（本 spec 只锚定"基调等级"）

- **选择**：游戏整体 SHALL 保持**简约、明亮、卡通**的视觉与音频基调，避免写实、黑暗、恐怖等相反风格。具体技术路线（Low-poly、面数上限、光照方案）由 `art-direction` capability 规定。
- **理由**：明亮卡通与 D2（无压力）直接一致——如果画面黑暗或写实，NPC 抓猴子的场景容易让玩家产生"被抓很恐怖"的负面联想。本 spec 锁定**语义层**，不涉入**技术层**。
- **替代方案**：写实向——与基调冲突；像素风——用户未选择。
- **对应 requirement**：`Lighthearted Visual and Audio Tone`。

### D15. 项目规模：短篇完整（约 3-5 小时总时长）

- **选择**：游戏 SHALL 定位为"短篇完整作品"，预期总流程时长在 3-5 小时量级，包含多个主题关卡。
- **理由**：用户明确选择"B. 短篇完整（四关都有，但每关不大）"。这个规模既能让玩法有变化，又不致失控。
- **替代方案**：垂直切片 Demo——不能达成完整作品目标；长篇——单人开发不现实。
- **对应 requirement**：`Short-Form Complete Experience`。
- **注**：具体关卡数量、单关规模属于 `world-structure`。

### D16. 现代中国题材（本 spec 只锚定"题材大类"，不锚定具体地点）

- **选择**：游戏世界观 SHALL 为"**现代中国**"日常场景，具体地点（景区、村庄、加油站、酒吧等）由 `world-structure` capability 具体化，且 MUST 采用**虚构化名**以规避真实地名/品牌的权利风险。
- **理由**：用户给出的初步路线（"峨眉山风格景区 → 村庄 → 加油站 → 城市酒吧"）是现代中国题材的典型切片。本 spec 锁定题材大类，让具体地点未来可调整。
- **替代方案**：奇幻（香蕉王国）/ 纯都市——都被用户的"现代中国"覆盖范围排除。
- **对应 requirement**：`Modern China Thematic Setting`。

### D17. 本阶段以本地游玩为唯一交付形态

- **选择**：本阶段的所有设计与实现 SHALL 以"本地游玩"为唯一目标形态；任何涉及联机、云存档、排行榜、社交的内容 MUST NOT 进入当前 scope，除非通过独立 change 引入。
- **理由**：D6 的延伸——把边界写死以避免 scope 漂移。
- **替代方案**：为 Steam 云存档等预留接口——被 D6 一并否决。
- **对应 requirement**：`Single-Machine Delivery Scope`。

---

### 拓扑示意：本 spec 与未来 capability 的关系

```
openspec/specs/
├── project-layout/            （已有）
├── documentation-conventions/ （已有）
└── gameplay-vision/           （本 change 新增 —— 只锚定"是什么"）
                │
                │  被引用
                ▼
    未来各自独立的 change 与 capability：
    ├── world-structure          （关卡数量、拓扑、过渡通道、捷径）
    ├── player-character         （猴子控制器、相机算法、距离限制实现）
    ├── grab-throw-system        （抓取投掷的具体系统）
    ├── task-system              （任务图、解锁、触发检测）
    ├── npc-ai                   （NPCBrain + Stimulus 迁移）
    ├── art-direction            （Low-poly 技术规格）
    └── world-state-persistence  （跨关卡状态存储机制）
```

## Risks / Trade-offs

- **[风险] 决策快照会过时** → 项目方向在开发中不可避免会演化。缓解：任何演化通过标准的 MODIFIED / REMOVED Requirements 流程在 OpenSpec 里留下历史追溯；不允许"悄悄改"。
- **[风险] 顶层 spec 的 SHALL 粒度过粗、难以被下游精确引用** → 下游 change 可能只能笼统地 "参考 `gameplay-vision`" 而非指向具体 requirement。缓解：本 spec 按主题切成多条 requirement（约 12-14 条），每条聚焦一个决策，便于精确引用。
- **[风险] 与未来能力的边界划不清** → 作者可能把"距离限制的具体曲线"也写进本 spec。缓解：本文档在多处明确 **"具体实现 / 具体数值 / 具体算法属于后续 capability"**，作为编辑者的自我约束。
- **[权衡] 未为联机留接口（D6）** → 未来联机重构成本更高。接受：换来当前阶段的轻量与专注。
- **[权衡] 单道具限制（D8）让"全局花收集"需要多次往返** → 降低便利性。接受：这是 D9 自由穿梭存在的理由之一，亦是玩法张力的来源。
- **[权衡] 不讲故事（D3）让四关之间的主题跳跃缺乏情感粘合** → 依赖环境与任务趣味本身保持吸引力。接受：用户明确选择此方向。
