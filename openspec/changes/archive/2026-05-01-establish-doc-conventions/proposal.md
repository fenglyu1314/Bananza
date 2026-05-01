## Why

`project-layout` 落地后，团队明确了"OpenSpec 产物默认使用中文、骨架与标识符保留英文"的工作方式，但该约定目前只以 `openspec/config.yaml` 的 prompt 注入形式存在，**没有对应的 capability spec**。这导致：

- 约定是否被执行、如何被违反、违反后以什么为准裁决，缺少可引用的权威来源。
- 未来 `config.yaml` 被改动（包括本 change 修复的 `rules.spec` → `rules.specs` 拼写错误），无法追溯"我们到底约定了什么"。
- 下游文档（`AGENTS.md`、`README.md`、Docs/*）只能凭感觉遵循，无法 MUST/SHALL 级别引用。

本 change 将把这套约定固化为一个独立的 capability `documentation-conventions`，使其与 `project-layout` 一样成为项目的"单一事实源"。

## What Changes

- 新增 capability `documentation-conventions`，沉淀 OpenSpec 产物的语言策略与文档协作守则。
- 新增 4 条 Requirement：OpenSpec 产物默认中文、保留英文的骨架与标识符清单、Requirement 标题语言策略、Commit / 分支 / 外部接口语言策略。
- 显式声明与 `project-layout` 的边界：`documentation-conventions` 只管"产物用什么语言写"，`project-layout` 只管"文件放在哪里"；二者互不覆盖，在"下游文档不得复制 spec 内容"这一交叉点上由 `project-layout` 独占所有权。
- 同步修正 `openspec/config.yaml` 中的两个遗留问题：
    - 将错误的 `rules.spec` 段更名为 `rules.specs`（与 OpenSpec schema 对齐，消除 `Unknown artifact ID in rules: "spec"` 警告）。
    - 用 YAML 单引号包裹含中文双引号的字符串，避免再次触发 `YAMLParseError`。
- 将 `openspec/config.yaml` 中 `context.Language` 与 `rules.*` 的内容从"权威规则"退化为"对 spec 的引用 + prompt 执行层"，消除与新 spec 的重复。

## Capabilities

### New Capabilities
- `documentation-conventions`: OpenSpec 产物与项目文档的语言、结构、归属策略；定义哪些内容必须中文、哪些必须保留英文、哪些文档必须引用而非复制 spec。

### Modified Capabilities
<!-- 本 change 不修改已有 capability 的 requirements。project-layout 中已有的 "Spec as the Single Source of Truth for Project Layout" 只适用于项目布局这一 capability 自身，不属于本次变更对象。 -->

## Impact

- **受影响文件**：
    - 新增 `openspec/specs/documentation-conventions/spec.md`（由本 change 归档时自动生成）。
    - 修改 `openspec/config.yaml`（修 YAML 错误 + 改 `spec` → `specs` + 精简为引用型说明）。
- **不受影响**：
    - 已归档的 `establish-project-layout` change 与 `openspec/specs/project-layout/spec.md` 不被追溯翻译或修改。
    - Unity 项目代码、`.editorconfig`、`Docs/EngineNotes_Tuanjie.md` 不受本 change 影响。
    - Git commit message、branch name、PR 标题继续沿用既有英文惯例。
- **对 AI 助手的影响**：未来执行 `/opsx:propose`、`/opsx:apply` 等 skill 时，生成的 proposal / design / tasks / delta spec 将默认遵循 `documentation-conventions` 定义的语言策略；`config.yaml` 只充当执行层，规则本身以 spec 为准。
