## Why

Bananza 进入 P1 阶段的第一步交付：要搭出一个"能跑起来"的最小可玩 demo，就必须先让主角在测试场景里可以被操控、并且有一台跟随镜头能看到他。没有这一层，后续的拾取、投掷、任务、双人、NPC 都没有立足点。本 change 的目标就是把"角色移动 + 相机跟随"作为项目的第一个玩法能力稳下来。

## What Changes

- 引入 `player-character` capability，本次仅覆盖"基础移动"与"跟随相机"两个子域；拾取、放下、投掷、跳跃等其余动作 **不在本 change 范围内**，将在后续 change 中增量添加。
- 在 Unity / 团结引擎测试场景中新增一个玩家角色 prefab（含角色控制组件与占位视觉），以及一个跟随该角色的第三人称相机。
- 玩家可以通过键鼠或手柄驱动角色在平面上移动并转向；相机以稳定方式跟随角色，使玩家始终能看到自身。
- 本阶段不接入任何 UI、任务、存档系统；也不引入生命值 / 体力 / 失败状态（遵循 `gameplay-vision` 中的 "No Failure or Penalty Mechanics" 与 "Single-Player Local Experience" 约束）。

## Capabilities

### New Capabilities
- `player-character`: 主角角色的运行时行为能力集合。本 change 只落地其中最小子集——地面移动与第三人称跟随相机；后续 change 会逐步加入拾取放下、投掷、跳跃、交互反馈等。

### Modified Capabilities
<!-- 本 change 不修改任何已有 capability 的 requirement。 -->

## Impact

- **新增代码**：`Assets/Scripts/PlayerCharacter/` 下的角色控制脚本与相机跟随脚本（具体类名与目录结构在 `design.md` 中给出）。
- **新增资源**：测试用场景（如 `Assets/Scenes/Sandbox_P1.scene`）、主角 prefab、占位模型或 primitive 作为视觉代理。资源本体由人工在团结引擎编辑器中创建；对应 `.meta` 文件严禁手工生成，必须由编辑器在导入时自动产出。
- **新增 spec**：`openspec/specs/player-character/spec.md`（通过本 change 的 delta 创建）。
- **依赖 / 约束**：严格遵守 `gameplay-vision` 已固化的品类定位、无失败机制、单机离线等基线；遵守 `project-layout` 中关于 `Assets/` 结构与 asmdef 的约束；遵守 `documentation-conventions` 中关于中英文混排的约束。
- **不涉及**：网络、存档、UI、音频、AI NPC、任务系统；这些能力在后续阶段单独立 change。
