## Why

Bananza 项目已经完成目录布局与文档语言规范的基础建设，但**玩法层面仍是一张空白画布**：没有任何规范记录项目的品类定位、玩家模式、核心动作、情绪基调或任务系统形态。在此空白之上推进任何具体能力（抓取系统、任务系统、NPC AI、关卡等）都会缺乏共同参照，容易让讨论反复回到"我们到底在做什么游戏"这个元问题。

本 change 把通过多轮 explore 讨论沉淀下来的**最顶层玩法决策**落地为一份可被后续所有 change 引用的能力规范，作为项目玩法侧的**单一事实源**，为后续逐步展开的 `world-structure`、`player-character`、`art-direction`、`gameplay-core-loop` 等 capability 提供锚点。

## What Changes

- 新增 capability `gameplay-vision`，用 RFC 2119 语义固化项目最顶层的玩法定位与基调，覆盖：
  - **品类与基调**：3D 捣蛋沙盒、无失败机制、无故事驱动、无计时惩罚
  - **玩家模式**：本地 1-2 人手柄、共享屏幕、支持单人（2P 缺席）、硬距离限制，暂不做网络联机的架构预留
  - **主角与核心动作**：两只基本相同的猴子、仅支持移动与"单手单道具"的抓取投掷
  - **目标结构**：常规任务（列表显示）+ 隐藏任务（完成后显示）+ 全局跨关卡收集成就三层并存
  - **回访性**：玩家可自由穿梭已解锁的关卡，支持"捷径"解锁以缩短往返路径
- 范围**仅限**项目最顶层的纲领性约束，**不覆盖**具体关卡拓扑（景区/村庄/加油站/酒吧）、具体相机算法、美术风格细节、任务配置机制——这些留给后续独立 change（`world-structure`、`player-character`、`art-direction` 等）分别展开。
- 本 change 本身**不包含任何代码变更**，唯一产出为 OpenSpec 文档（proposal/design/spec/tasks），归档后进入 `openspec/specs/gameplay-vision/`。

## Capabilities

### New Capabilities

- `gameplay-vision`: 项目最顶层的玩法纲领，以 requirement 形式固化品类定位、玩家模式、核心动作、情绪基调、目标结构与回访性约束，作为所有后续玩法 change 的参照基线。

### Modified Capabilities

（无。本 change 不修改 `project-layout` 或 `documentation-conventions` 的任何 requirement。）

## Impact

- **OpenSpec specs**：归档后新增 `openspec/specs/gameplay-vision/spec.md`，与 `project-layout`、`documentation-conventions` 平级共存。
- **下游文档**：`README.md` 与 `AGENTS.md` 在后续更新时 SHOULD 改为引用 `gameplay-vision` spec，避免重复描述玩法基调。
- **未来 change**：后续所有玩法相关的 change 在 `design.md` 中 SHOULD 回链到 `gameplay-vision` 的具体 requirement，以证明其实现选择符合项目纲领。
- **代码/资源**：无直接影响。项目中尚未存在的玩法代码与资源将在后续 change 中按本 spec 的约束逐步引入。
- **风险**：本 spec 捕获的是**当前阶段**的决策快照；如未来项目方向调整（例如加入联机、改变品类），需通过标准的 MODIFIED/REMOVED Requirements 流程更新，并让引用本 spec 的下游 change 同步评审。
