## Why

当前项目只有默认的 `Bananza.Runtime` / `Bananza.Editor` 两个 asmdef、`Assets/Scripts/Runtime|Editor` 两个空目录，缺乏对子模块目录、命名空间、asmdef 拆分时机的明确约定。趁代码量还是 0，必须先立下骨架规范，否则在 Gameplay/AI/UI 等模块并行落地后，会陷入循环依赖、模块边界糊、asmdef 反复重构的泥潭。

## What Changes

- 声明 `project-layout` 能力，承载目录骨架、命名空间映射、asmdef 划分策略、新增 asmdef 的标准流程。
- 明确当前阶段采用 **两段式 asmdef**（`Bananza.Runtime` + `Bananza.Editor`），**不提前分裂**；给出分裂触发条件与未来拆分蓝图。
- 确定 `Assets/Scripts/Runtime/<Module>/<SubModule>` 与 `Bananza.<Module>.<SubModule>` **目录即命名空间**的硬性约定；目录最多两级。
- 确定 **SO 类型定义就近放置**（方案乙）：`PropData.cs` 放业务模块目录，不建立独立 `Data/` 代码根目录；`Assets/Data/` 仅承载 SO 资产实例（`.asset`）。
- 确定编辑器脚本路径统一为 `Assets/Scripts/Editor/`（`Assets/Editor/` 已由用户手动删除）。
- **BREAKING**（仅对规则语境）：之前 [AGENTS.md](../../../AGENTS.md) §3 中"编辑器脚本放 `Assets/Editor/`"的表述被本提案覆盖。AGENTS.md 的同步修订已先行完成，本 change 负责把细节规范从临时文档迁入 OpenSpec。
- 取消历史草稿 [Docs/ProjectLayout.md](../../../Docs/ProjectLayout.md)：该文件本轮作为讨论载体产出，不作为长期文档保留；实施阶段由本 change 的 tasks 负责删除。

## Capabilities

### New Capabilities

- `project-layout`: 项目目录骨架、命名空间映射、asmdef 划分策略与新增 asmdef 的标准流程。作用域仅限 `Assets/` 下的源码与资产目录约定，不包含编码风格（归 `Docs/CodingGuidelines.md`）与资产命名（将来独立 capability）。

### Modified Capabilities

（无——这是首个 capability，当前 `openspec/specs/` 为空。）

## Impact

- **代码库**：短期内无实质代码改动；只确立规范。首次代码入库时按本规范创建子目录与命名空间。
- **`Assets/` 目录**：无破坏性动作；`Assets/Editor/` 空目录已由用户手动删除。
- **`Docs/`**：删除 `Docs/ProjectLayout.md`（临时讨论产物）。
- **`AGENTS.md`**：§3（编辑器脚本路径）、§6（引用方式）已先行对齐；本 change 的 tasks 补一步"把 §3/§6 里仍指向 `Docs/ProjectLayout.md` 的字样，改为对 OpenSpec `project-layout` spec 的模糊引用"（C 方案，不写具体路径）。
- **AI 助手行为**：新成员与 AI 助手今后查项目骨架规范，入口走 OpenSpec（`openspec list` / `openspec show project-layout`），不再查 Docs。
- **未来 asmdef 拆分**：本 change 归档后，后续每次拆 asmdef 都应走独立 change（如 `split-core-asmdef`），在本 spec 上做 MODIFIED/ADDED 变更。
