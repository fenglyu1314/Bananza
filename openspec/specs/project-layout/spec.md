# project-layout Specification

## Purpose

本能力定义 Bananza Unity 项目的标准目录布局与程序集定义（`.asmdef`）策略。它规定源代码、ScriptableObject 类型定义、资源实例、场景、设置、第三方插件在 `Assets/` 下的位置，以及在什么条件下可以从两个基线程序集（`Bananza.Runtime`、`Bananza.Editor`）中拆分出新的 `.asmdef` 文件。下游文档（如 [AGENTS.md](../../AGENTS.md) 和 `README.md`）应引用本规范，而非复制其内容。

## Requirements
### Requirement: Top-level Asset Directory Layout

项目 SHALL 将 `Assets/` 组织为一组固定的顶层目录，每个目录职责单一且互不重叠。每个目录 MUST 仅在其包含文件时存在；MUST NOT 仅为满足本要求而创建空目录。

`Assets/` 下的标准顶层目录如下：

- `Art/` — 视觉资源（贴图、模型、材质、动画、VFX）
- `Audio/` — 音频资源
- `Prefabs/` — 预制体实例
- `Scenes/` — 场景文件
- `Scripts/` — C# 源代码（见 *Scripts Subtree Layout*）
- `Settings/` — URP 渲染管线资产和输入设置
- `Data/` — ScriptableObject 资源实例（`.asset`），按业务模块分组（不包含 C# 类型定义）
- `ThirdParty/` — 外部插件，每个隔离在自己的子目录中并携带独立的 `.asmdef`
- `Tests/` — 预留给测试程序集（可选，首次引入测试时创建）

#### Scenario: Adding a new SO asset instance

- **WHEN** 策划创建一个新的 `PropData` ScriptableObject 实例
- **THEN** `.asset` 文件 MUST 放在 `Assets/Data/Props/` 下，对应的 `.meta` MUST 由 Unity 编辑器生成

#### Scenario: Adding a third-party plugin

- **WHEN** 开发者导入一个新的第三方插件
- **THEN** 该插件 MUST 放在 `Assets/ThirdParty/<PluginName>/` 下，并 MUST 携带自己的 `.asmdef` 以与项目代码隔离

#### Scenario: Attempting to mix code and data in `Assets/Data/`

- **WHEN** 开发者试图将一个 `.cs` ScriptableObject 定义放在 `Assets/Data/` 下
- **THEN** 该放置 MUST 被拒绝——`Assets/Data/` 仅存放 `.asset` 实例；类型定义依照 *SO Type Definitions Co-located with Business Modules* 的要求存放在 `Assets/Scripts/Runtime/<Module>/` 下

---

### Requirement: Scripts Subtree Layout

所有 C# 源代码 SHALL 存放于 `Assets/Scripts/`，并划分为两个与项目两个 `.asmdef` 一一对应的根目录：

- `Assets/Scripts/Runtime/` — 由 `Bananza.Runtime.asmdef` 管理，`rootNamespace = Bananza`
- `Assets/Scripts/Editor/` — 由 `Bananza.Editor.asmdef` 管理，`rootNamespace = Bananza.Editor`

遗留的 `Assets/Editor/` 目录 MUST NOT 被重新创建；所有 editor-only 脚本 MUST 存放在 `Assets/Scripts/Editor/` 下。

在 `Runtime/` 内，认可的一级模块为 `Core`、`Gameplay`、`NPC`、`AI`、`UI`。在 `Editor/` 内，认可的一级模块为 `Inspectors`、`TaskGraph`、`Tools`。引入额外的一级模块 SHALL 通过变更提案（change proposal）更新本规范。

#### Scenario: Creating a new gameplay script

- **WHEN** 开发者为玩家模块新增一个 `PlayerController.cs`
- **THEN** 该文件 MUST 放在 `Assets/Scripts/Runtime/Gameplay/Player/PlayerController.cs`，并声明 `namespace Bananza.Gameplay.Player`

#### Scenario: Creating a new editor inspector

- **WHEN** 开发者为 `PropData` 添加一个自定义 Inspector
- **THEN** 该文件 MUST 放在 `Assets/Scripts/Editor/Inspectors/` 下，并使用以 `Bananza.Editor.Inspectors` 为根的命名空间

#### Scenario: Accidentally introducing `Assets/Editor/`

- **WHEN** 任何工具或开发者试图再次创建 `Assets/Editor/`
- **THEN** 该操作 MUST 被拒绝；editor 脚本 MUST 保留在 `Assets/Scripts/Editor/` 下

---

### Requirement: Directory-Namespace One-to-One Mapping

`Assets/Scripts/Runtime/` 和 `Assets/Scripts/Editor/` 下的每个子目录 SHALL 与一个 C# 命名空间段严格一一对应，这样未来拆分程序集时只需移动目录，无需重命名类型。

`Runtime/` 下的嵌套深度 MUST 不超过两级（`Runtime/<Module>/<SubModule>/`）。超过两级意味着受影响的模块应被拆分为独立的 `.asmdef`。

模块边界 MUST 被尊重：属于某一已认可模块的代码 MUST NOT 放在另一模块的目录下（例如 `Bananza.AI` 的代码 MUST NOT 放在 `Gameplay/` 下）。

#### Scenario: Namespace derived from path

- **WHEN** 一个文件位于 `Assets/Scripts/Runtime/AI/Brain/DecisionTree.cs`
- **THEN** 其命名空间 MUST 为 `Bananza.AI.Brain`

#### Scenario: Depth limit exceeded

- **WHEN** 开发者提议创建 `Assets/Scripts/Runtime/Gameplay/Props/Interactables/PickUp/PickUp.cs`（位于 `Runtime/` 下第三级）
- **THEN** 该提议 MUST 改为评估通过变更提案将 `Props`（或其兄弟模块）拆分为独立的 `.asmdef`，而不是加深目录树

#### Scenario: Code placed in the wrong module

- **WHEN** 一个 AI 系统脚本被误放到 `Assets/Scripts/Runtime/Gameplay/` 下
- **THEN** 在合入前，该脚本 MUST 被移动到 `Assets/Scripts/Runtime/AI/<SubModule>/`，并相应更新其命名空间

---

### Requirement: SO Type Definitions Co-located with Business Modules

ScriptableObject **类型定义**（`.cs`）SHALL 与使用它们的业务模块共处一地，而非放在专门的 `Scripts/Runtime/Data/` 根目录下。ScriptableObject **资源实例**（`.asset`）SHALL 存放在 `Assets/Data/<Module>/` 下，与其类型定义在组织上分离。

MUST NOT 规划专门的 `Bananza.Data` 程序集；SO 类型定义跟随其所属模块的程序集。

#### Scenario: Placing `PropData.cs`

- **WHEN** 开发者定义 `PropData` 这个 ScriptableObject 类型
- **THEN** `PropData.cs` MUST 放在 `Assets/Scripts/Runtime/Gameplay/Props/` 下（与 prop 运行时逻辑相邻）

#### Scenario: Placing the corresponding asset instance

- **WHEN** 制作 `PropData` 的第一个实例 `Apple.asset`
- **THEN** 该资源 MUST 保存在 `Assets/Data/Props/Apple.asset`

#### Scenario: NPC profile placement

- **WHEN** 定义同时被 NPC 运行时和 AI 大脑消费的 `NPCProfile.cs`
- **THEN** 该文件 MUST 根据主要消费方，放在 `Assets/Scripts/Runtime/NPC/` 或 `Assets/Scripts/Runtime/AI/Configs/` 下，并在 commit message 中写明选择理由

---

### Requirement: Assembly Definition Baseline

项目 SHALL 在基线状态下保持恰好两个程序集：

| asmdef | Path | rootNamespace | References |
|---|---|---|---|
| `Bananza.Runtime` | `Assets/Scripts/Runtime/` | `Bananza` | （无）|
| `Bananza.Editor`  | `Assets/Scripts/Editor/`  | `Bananza.Editor` | `Bananza.Runtime` |

除 *Assembly Split Triggers* 中定义的拆分触发条件外，MUST NOT 创建额外的 `.asmdef` 文件。

依赖方向严格且单向：

- Runtime 模块 MUST NOT 引用 editor 代码。
- `Editor` MAY 引用任何 `Runtime` 程序集，反向引用 MUST NOT 出现。

#### Scenario: Baseline preserved

- **WHEN** 检视项目的新鲜 checkout
- **THEN** MUST 恰好存在两个 `.asmdef` 文件：`Bananza.Runtime.asmdef` 和 `Bananza.Editor.asmdef`

#### Scenario: Runtime references Editor (forbidden)

- **WHEN** 某个 pull request 试图把 `Bananza.Editor` 加入 `Bananza.Runtime.asmdef` 的 `references` 列表
- **THEN** 该变更 MUST 被拒绝，视为违反单向依赖规则

---

### Requirement: Assembly Split Triggers

当且仅当满足以下触发条件之一时，SHALL 引入新的 `.asmdef`，且拆分 MUST 通过一个 OpenSpec change 提出（不得临时提交）：

1. 某子模块积累了 15 个或以上 `.cs` 文件。
2. 出现跨模块的循环依赖，且无法在不引入程序集边界的前提下解决。
3. 引入了第三方插件（插件始终在 `Assets/ThirdParty/<Plugin>/` 下拥有独立的 `.asmdef`）。
4. 某模块需要通过 `includePlatforms` / `excludePlatforms` 做平台限定（仅 Editor、仅 Standalone、仅移动端）。
5. 编辑器工具脚本（尤其是 GraphView 风格的编辑器）达到 10 个或以上文件。
6. 引入单元测试或集成测试——MUST 创建专用的 `Bananza.Tests.Runtime` / `Bananza.Tests.Editor` 程序集，并启用 Test Assemblies 标志。

当同一模块命中多个触发条件时，MAY 合并为一次变更提交。

#### Scenario: File count threshold crossed

- **WHEN** `Assets/Scripts/Runtime/AI/` 下的文件数达到 15
- **THEN** 在新增 AI 文件合入前，MUST 起草一份将 `Bananza.AI` 拆为独立程序集的 OpenSpec change

#### Scenario: Introducing a third-party plugin

- **WHEN** 团队决定引入 DOTween
- **THEN** 该插件 MUST 放在 `Assets/ThirdParty/DOTween/` 下，并 MUST 携带名为 `DOTween`（或供应商提供的名称）的独立 `.asmdef`

#### Scenario: First unit test added

- **WHEN** 创建第一个单元测试文件
- **THEN** MUST 通过一份 OpenSpec change 引入 `Bananza.Tests.Runtime.asmdef`（若需要 editor 测试则同时引入 `Bananza.Tests.Editor.asmdef`），并启用 Test Assemblies 标志

---

### Requirement: Standard Procedure for Adding a New `.asmdef`

当拆分触发条件被满足时，新的 `.asmdef` SHALL 按以下流程添加：

1. 通过 Unity 编辑器（Create → Assembly Definition）在目标目录中创建 `.asmdef`，以便 Unity 自动生成 `.meta`。AI 助手 MUST NOT 手工创建 `.asmdef` 或其 `.meta`。
2. 程序集命名为 `Bananza.<Module>`（runtime）或 `Bananza.Editor.<Tool>`（编辑器工具）。
3. 将 `rootNamespace` 设置为与目录命名空间一致（遵循 *Directory-Namespace One-to-One Mapping*）。
4. 只在 `references` 中声明**直接**依赖。MUST 拒绝循环引用。
5. 对于 editor-only 程序集，设置 `includePlatforms = ["Editor"]`。
6. 使用 Conventional Commits 提交：`chore(asmdef): split <Module> into Bananza.<Module>`。
7. 在同一 pull request 中通过一份 OpenSpec change（例如 `split-<module>-asmdef`）更新本规范。

#### Scenario: Splitting out `Bananza.Core`

- **WHEN** 团队决定从 `Bananza.Runtime` 中拆出 `Bananza.Core`
- **THEN** MUST 提出名为 `split-core-asmdef` 的 change，该 `.asmdef` MUST 通过 Unity 编辑器在 `Assets/Scripts/Runtime/Core/` 下创建，本规范 MUST 相应更新以反映新基线

#### Scenario: AI tries to hand-write `.meta`

- **WHEN** AI 助手被要求创建一个新的 `.asmdef`，并想要同时写出其 `.meta`
- **THEN** 该助手 MUST 拒绝创建 `.meta`，只创建 `.asmdef`，并指示用户刷新 Unity 编辑器以生成 `.meta`

---

### Requirement: Spec as the Single Source of Truth for Project Layout

本规范定义的目录与程序集约定 SHALL 作为项目的唯一权威参考。下游文档（如 [AGENTS.md](../../AGENTS.md)、`README.md`）MUST 引用本规范而非复制其内容。

临时性或长篇的人类可读指南 MUST NOT 复制本规范中的规范性内容；如需摘要，MAY 进行概述但 MUST 回链至本规范。

#### Scenario: AGENTS.md references layout rules

- **WHEN** AGENTS.md 需要描述代码放置规则
- **THEN** AGENTS.md MUST 引用 "the OpenSpec `project-layout` spec"，而非内嵌完整规则或指向 `openspec/` 下某个具体文件路径

#### Scenario: Conflict between docs and spec

- **WHEN** 发现本规范与任何 Docs/ 文件或 README 之间存在冲突
- **THEN** 本规范 MUST 被视为权威，另一文档 MUST 被更新以与之对齐（或改为引用本规范）

