## Context

项目刚起步：`Assets/Scripts/` 下已存在两个 default asmdef（`Bananza.Runtime` / `Bananza.Editor`），但模块目录、命名空间、asmdef 拆分策略全部未约定。本次讨论中用户与 AI 助手已就以下三点达成结论：

1. 编辑器脚本统一走 `Assets/Scripts/Editor/`；空目录 `Assets/Editor/` 已手动删除。
2. SO 类型定义采用**就近式**（方案乙）——业务模块下直接放 `XxxData.cs`，不建立 `Data/` 代码根目录；`Assets/Data/` 仅承载 SO 资产实例。
3. 本阶段保留**两段式 asmdef**，通过明确的触发条件决定何时分裂，而非提前预设子 asmdef。

这些结论原本整理在 `Docs/ProjectLayout.md`，但该文档混装了"长期规范"和"决策记录 / 落地清单 / 变更记录"两类内容。用户希望把长期规范迁入 OpenSpec（AI 与开发者可读的权威源），Docs 回归简洁、纯人读定位。

**相关约束**：
- 受 [AGENTS.md](../../../AGENTS.md) §3 / §6 约束，其中 §3 关于编辑器脚本路径的表述已先行对齐本提案。
- `.meta` 不得手工创建；所有新增目录须由 Unity 导入资源时自动生成。
- 采用团结引擎 1.8.2 / Unity 2022.3.62t4，asmdef 行为与社区 Unity 一致。

## Goals / Non-Goals

**Goals:**
- 用 OpenSpec spec 权威表达"目录骨架 + 命名空间映射 + asmdef 当前划分 + 分裂触发条件 + 新增 asmdef 流程"。
- 让本 change 归档后，`openspec/specs/project-layout/` 成为新成员与 AI 助手查项目骨架的**唯一**入口。
- 通过 AGENTS.md 的**模糊引用**（不写具体路径），让硬规则文件保持稳定，不随 change 状态漂移。
- 清理临时讨论产物 `Docs/ProjectLayout.md`，避免双份维护。

**Non-Goals:**
- 不实际创建任何代码或子目录——规范归规范，落地等具体模块开发时按需建立。
- 不重新整理 `Assets/Settings/URP/` 资产位置（未来独立 change）。
- 不约束编码风格（命名、注释、日志）——归 `Docs/CodingGuidelines.md`。
- 不约束资产命名规范（未来独立 capability：`asset-naming`）。
- 不出"人读速查卡"——等首个 asmdef 实际落地、骨架稳定后再考虑。

## Decisions

### D1：规范的承载形式 —— OpenSpec spec，而非长期 Docs 文档

**选择**：所有骨架规范进 `openspec/specs/project-layout/spec.md`。

**理由**：
- 符合用户对 `Docs/` 与 `openspec/` 的定位划分（Docs = 人读区，OpenSpec = AI + 开发者权威源）。
- 未来骨架演进（拆 asmdef / 加模块）走 change 流程，每一次变更都有 proposal + spec delta 可追溯。
- 避免在 Docs 里堆积 changelog、决策记录，污染人读体验。

**替代方案**：
- **替代 A**：长期保留 `Docs/ProjectLayout.md` + 同步一份 OpenSpec spec。→ 拒绝，双份维护代价高，容易不一致。
- **替代 B**：只放 OpenSpec spec，完全不在 Docs 出现。→ **采纳**（本次做法）。
- **替代 C**：只放 Docs，不进 OpenSpec。→ 拒绝，失去 AI 权威规范源的好处。

### D2：asmdef 采用两段式起步 + 明确触发条件

**选择**：保留现有 `Bananza.Runtime` + `Bananza.Editor`，不提前分裂。通过触发条件（子模块 ≥15 文件 / 出现循环依赖 / 引入第三方 / 需要平台裁剪 / 编辑器工具 ≥10 / 开始写测试）来驱动分裂。

**理由**：
- 模块边界在早期不稳，过早固化 asmdef 会在后续重构中反复推翻。
- 两段式已能覆盖当前所有代码；分裂的价值要在代码量堆积到一定程度才显现（减少编译时间、强制依赖方向）。

**替代方案**：
- **预先拆出 Core / Data / Gameplay / UI / AI 共 5 个 asmdef**。→ 拒绝，过度设计，目前没有循环依赖风险，拆分只带来维护成本。

### D3：SO 类型定义就近放置（方案乙）

**选择**：`PropData.cs` 放 `Scripts/Runtime/Gameplay/Props/`，不建立独立 `Scripts/Runtime/Data/` 代码根目录。`Assets/Data/` 只放 `.asset` 资产实例。

**理由**：
- 数据和使用它的逻辑同目录，重构搬运成本低。
- 放弃未来独立 `Bananza.Data` asmdef 的便利，换取开发阶段的灵活性——当前阶段数据模型迭代频繁，集中式 Data 会在每次迭代时引发双向搬运。

**替代方案**：
- **方案甲：集中式**（所有 SO 定义放 `Scripts/Runtime/Data/<Module>/`）。→ 拒绝，代码和使用方分离，且实际上并不便于拆 `Bananza.Data` asmdef（因为策划侧通常通过 Inspector 使用 SO，不涉及 asmdef 边界）。

### D4：编辑器脚本统一 `Assets/Scripts/Editor/`

**选择**：删除 `Assets/Editor/`（已由用户手动执行），编辑器脚本全部收敛到 `Assets/Scripts/Editor/`，由现有 `Bananza.Editor.asmdef` 管理。

**理由**：
- `Assets/Editor/` 和 `Assets/Scripts/Editor/` 两处并存会导致编辑器脚本分散、asmdef 作用域混乱。
- Unity 对 `Editor/` 文件夹名有特殊语义识别，但放在 `Scripts/` 下并不影响该识别——两者语义等价，但放 `Scripts/` 下分层更清晰。

### D5：AGENTS.md 的引用方式 —— C 方案（模糊引用）

**选择**：AGENTS.md 不写 `openspec/specs/project-layout/` 或 `openspec/changes/establish-project-layout/` 的具体路径，而是用"项目骨架与 asmdef 规范见 OpenSpec `project-layout` spec"这类模糊措辞。

**理由**：
- AGENTS.md 承载稳定硬规则，不应随 change 的生命周期（changes/ → archive/）而改路径。
- OpenSpec 有自己的检索机制（`openspec list` / `openspec show <capability>`），AI 与人都能找到。

**替代方案**：
- **A 方案**：写死 `openspec/specs/project-layout/`。→ 过渡期（spec 未归档前）链接会断。
- **B 方案**：写死 `openspec/changes/establish-project-layout/`。→ 归档后必须改一次。
- **C 方案**：不写具体路径，用 spec 名称。→ **采纳**。

### D6：目录最多两级 + 目录即命名空间

**选择**：`Scripts/Runtime/<Module>/<SubModule>/` 最多两级；命名空间与目录一一对应。超出两级视为"该拆 asmdef"的信号。

**理由**：
- 两级足以承载当前可预见的模块粒度（Gameplay/Props、AI/Brain、Core/Registry 等）。
- 一一对应可实现"拆 asmdef 只挪目录不改代码"的零改动迁移。

## Risks / Trade-offs

- **风险**：OpenSpec spec 首次使用，团队对 `openspec show` 命令不熟，可能仍习惯翻 Docs → 找不到。
  **缓解**：AGENTS.md §6 留下对 OpenSpec 的引导文字；tasks 中加一条在 `README.md` 文档索引区提一句 "项目骨架规范见 OpenSpec"。

- **风险**：两段式 asmdef 起步，若在模块量快速扩张时错过分裂时机，会积压编译时间。
  **缓解**：spec 中明确列出触发条件（文件数 / 循环依赖 / 测试引入等），任何开发者在达到阈值时都有依据提 change。

- **Trade-off**：SO 就近式换取开发灵活性，代价是未来若真要拆 `Bananza.Data` asmdef 会较麻烦。
  **评估**：可接受。当前单机原型阶段，Data 独立 asmdef 不是刚需；真需要时用一次 refactor change 即可。

- **风险**：删除 `Docs/ProjectLayout.md` 后，外部链接（如 AGENTS.md 里尚未修订的旧链接、README 索引）会断。
  **缓解**：tasks 中明确扫描并替换所有对 `Docs/ProjectLayout.md` 的引用。

## Migration Plan

1. 先创建 spec delta（本 change 的 `specs/project-layout/spec.md`），确保权威源就位。
2. 用 `openspec validate establish-project-layout --strict` 校验。
3. 更新 [AGENTS.md](../../../AGENTS.md) §3 / §6：移除对 `Docs/ProjectLayout.md` 的具体路径引用，改为对 OpenSpec `project-layout` spec 的模糊措辞。
4. 删除 [Docs/ProjectLayout.md](../../../Docs/ProjectLayout.md)（由用户执行 `git rm`，AI 只给命令不直接动 git）。
5. 扫描仓库其它位置（README、其它 Docs）是否有对 `Docs/ProjectLayout.md` 的引用，一并处理。
6. 归档 change：`openspec archive establish-project-layout` → 生效的 spec 进入 `openspec/specs/project-layout/spec.md`。

**回滚策略**：
如需回滚，从 `openspec/changes/archive/` 取出本 change 的 delta，反向应用；`Docs/ProjectLayout.md` 可从 git 历史恢复。

## Open Questions

- （本次无）所有关键决策点在本轮对话中已与用户确认：编辑器目录、SO 放置、AGENTS 引用方式、是否出人读速查卡、change 合成 vs 拆分。
