# documentation-conventions Specification

## Purpose

本 capability 定义 OpenSpec 产物（`openspec/changes/` 与 `openspec/specs/` 下的 `proposal.md` / `design.md` / `tasks.md` / `spec.md`）以及相关项目文档的**语言与结构约定**：默认使用简体中文撰写自然语言部分；同时列出必须保留英文原文的结构骨架、RFC 2119 关键词、代码标识符与行业缩写。

本 capability 与 `project-layout` 互为正交：

- `documentation-conventions`：规范"文档/产物用什么语言写、哪些标记与标识符保留英文"。
- `project-layout`：规范"源代码、资源、程序集在 Unity 项目中的物理布局"。

`openspec/config.yaml` 的 `context.Language` 与 `rules.*` 仅作为 AI skill 运行时的 prompt 执行层，规则的权威来源为本 spec；一旦 config 与本 spec 冲突，以本 spec 为准。

## Requirements
### Requirement: Default Language for OpenSpec Artifacts

所有由 OpenSpec 管理的书面产物 SHALL 默认使用简体中文撰写其自然语言部分（包括描述、解释、动机、权衡等）。

受本要求约束的产物 MUST 至少包括：

- `openspec/changes/<change>/proposal.md`
- `openspec/changes/<change>/design.md`
- `openspec/changes/<change>/tasks.md`
- `openspec/changes/<change>/specs/<capability>/spec.md`（delta spec）
- `openspec/specs/<capability>/spec.md`（归档后的 capability spec）

若某份产物因历史原因已以英文撰写并已归档（例如 `openspec/changes/archive/` 下的历史 change），MUST NOT 被追溯翻译；归档即视为快照。

新产物或尚未归档的 change 若以英文撰写，MUST 在合入前翻译为中文，或在 proposal 中给出明确豁免理由并获得团队评审通过。

#### Scenario: 新建 change 时生成的 proposal 默认中文

- **WHEN** AI 助手执行 `/opsx:propose` 或等价 skill 生成一份新的 `proposal.md`
- **THEN** proposal 的 Why / What Changes / Impact 节的正文 MUST 使用简体中文
- **AND** 节标题（`## Why`、`## What Changes`、`## Impact`）MUST 保留英文

#### Scenario: 历史英文 change 已被归档

- **WHEN** 某个 change（如 `establish-project-layout`）在本规范生效前已以英文完成归档
- **THEN** 其 `openspec/changes/archive/<...>/` 下的所有文件 MUST NOT 被追溯翻译或修改
- **AND** 后续若该 capability 需要演进，新 change 的中文产物与历史英文产物共存，不触发风格统一的返工

#### Scenario: 未归档的英文 change 合入前

- **WHEN** 某份仍在 `openspec/changes/` 下（未归档）的 proposal.md / design.md / tasks.md 主体为英文
- **THEN** 该 change 合入前 MUST 翻译为中文，或在 proposal.md 中明确写出豁免理由
- **AND** 豁免理由 MUST 接受团队评审

---

### Requirement: Preserved English Tokens and Identifiers

以下类别的内容 MUST 保留英文原文，不翻译，不因本规范的"默认中文"策略而被替换：

1. **OpenSpec 结构骨架**：由 OpenSpec CLI 识别并用于解析、校验、归档合并的标记。示例包括 `## Purpose`、`## Requirements`、`## ADDED Requirements`、`## MODIFIED Requirements`、`## REMOVED Requirements`、`## RENAMED Requirements`、`### Requirement:`、`#### Scenario:`、`- **WHEN**`、`- **THEN**`、`- **AND**`。该列表为示例而非穷举；凡 OpenSpec 官方文档中定义的 parser token，均属于本类别。
2. **RFC 2119 关键词**（按 RFC 2119 / RFC 8174 定义，全大写）：`MUST`、`MUST NOT`、`SHALL`、`SHALL NOT`、`SHOULD`、`SHOULD NOT`、`MAY`、`REQUIRED`、`RECOMMENDED`、`OPTIONAL`。这些词在规范语境中具有精确定义的法定含义，MUST NOT 被翻译为中文近义词（如"必须"、"应当"、"可以"）。
3. **代码标识符**：类名、方法名、字段名、变量名、命名空间、程序集名（包括 `.asmdef` 的 `name` 字段）、文件路径、文件名及扩展名、目录名、配置键名、CLI 命令与参数、Git 引用（commit hash / branch / tag）、Unity / 团结引擎 API、第三方库 API。
4. **行业专有名词与技术缩写**：协议名、数据格式、算法名、工业标准缩写（如 `URP`、`HDRP`、`GraphView`、`ScriptableObject`、`asmdef`、`JSON`、`YAML`、`UTF-8`、`LF`、`CRLF`）。首次出现时 MAY 采用 `中文（English）` 的形式辅助读者理解（例如 `渲染管线（URP）`），后续出现 MUST 统一使用其中一种（通常为英文缩写），不得中英混用。

属于上述类别的内容 MUST NOT 被翻译；不属于上述类别但出现频率高且有明确中文对应的术语，SHOULD 使用中文。

#### Scenario: Requirement 正文混排中文与 RFC 2119 关键词

- **WHEN** 撰写一条 Requirement 的正文描述
- **THEN** 正文 MUST 使用中文叙述
- **AND** 规范级约束词 MUST 保留英文大写（例如 `该 .asset 文件 MUST 放在 Assets/Data/ 下`）

#### Scenario: 试图翻译 `MUST` 为 `必须`

- **WHEN** 任何人（含 AI 助手）尝试将 RFC 2119 关键词替换为中文（如 `MUST` → `必须`、`SHALL` → `应当`）
- **THEN** 该修改 MUST 被拒绝
- **AND** 若是为了中文读者的可读性，MAY 在 `documentation-conventions` spec 中补充说明这些关键词的中文语义对照，但 Requirement 正文本身仍保留英文大写

#### Scenario: 翻译代码标识符

- **WHEN** 文档引用到 `Bananza.Runtime.asmdef`、`rootNamespace`、`includePlatforms`、`PlayerController.cs` 等标识符
- **THEN** 这些标识符 MUST 以原文形式出现
- **AND** 正文描述它们的作用时 MUST 使用反引号包裹以视觉上区分代码与散文

#### Scenario: 行业缩写首次出现

- **WHEN** `URP` 在一份文档中首次出现
- **THEN** MAY 写作 `渲染管线（URP）` 以辅助读者
- **AND** 同一文档后续出现 MUST 统一使用 `URP` 或 `渲染管线` 之一，不得混用

---

### Requirement: Requirement Title Language Policy

OpenSpec spec 中每一条 Requirement 的标题（即 `### Requirement:` 之后的短语）SHALL 保留英文。该策略适用于所有新写的 capability spec 及未来的 delta spec。

此策略的理由由 OpenSpec 归档机制决定：`openspec archive` 依赖 Requirement 标题作为 **delta 合并的匹配键**——`## MODIFIED Requirements` 或 `## REMOVED Requirements` 段中声明的标题必须与被修改的目标 spec 中的标题**完全一致**才能被正确合并。保留英文标题可消除中英切换导致的匹配失败风险，并在未来工具链演进中保持稳定锚点。

Requirement 标题以下的正文（描述、列表、Scenario 标题、Scenario 步骤）MUST 遵循 *Default Language for OpenSpec Artifacts* 的默认中文策略。

Scenario 标题（`#### Scenario:` 之后的短语）MAY 使用中文或英文，由作者根据可读性选择；但一旦选定，同一 capability spec 内 SHOULD 保持风格一致。

#### Scenario: 新建 Requirement 标题

- **WHEN** 作者在某份 spec 中新增一条 Requirement
- **THEN** `### Requirement:` 之后的标题 MUST 使用英文（例如 `### Requirement: Default Language for OpenSpec Artifacts`）
- **AND** 该 Requirement 下方的正文描述 MUST 使用中文

#### Scenario: 在 delta spec 中修改已有 Requirement

- **WHEN** 某 delta spec 在 `## MODIFIED Requirements` 段中引用一条已存在的 Requirement
- **THEN** 该 Requirement 的标题 MUST 与目标 spec 中的英文标题完全一致（仅允许空白字符的差异）
- **AND** 若目标 spec 的标题已历史性地为中文，delta 可继续使用中文标题匹配，但 SHOULD 在合适时机通过 RENAMED 操作迁移回英文

#### Scenario: Scenario 标题选用中文

- **WHEN** 作者认为中文 Scenario 标题更利于阅读（例如 `#### Scenario: 新建 change 时生成的 proposal 默认中文`）
- **THEN** 该选择 MUST 被接受
- **AND** 同一 spec 内的所有 Scenario 标题 SHOULD 保持中英风格一致

---

### Requirement: Commit, Branch, and External Interface Language

下列产物属于"与外部工具或生态交互"的接口性文本，SHALL 继续使用团队既有的英文惯例，不受本 capability 的默认中文策略约束：

- Git commit message（包括 Conventional Commits 的 type/scope/subject）。
- Git branch name、tag name。
- Pull request / Merge request 的标题与描述默认字段（如 GitHub / GitLab 的 PR title）。
- CI/CD pipeline 名称、job 名称、环境变量名。
- 代码注释中指向特定英文 issue tracker / ticket 的引用（如 `// See ISSUE-123`）。

本要求并不禁止上述产物使用中文——团队 MAY 在 commit body 或 PR description 的正文中使用中文补充说明；但**结构化/机器可读的字段**（type、scope、branch slug、CI job id 等）MUST 保持英文以确保工具链兼容。

代码注释中的自然语言 SHOULD 使用中文（与 `Conventions` 约定的 "编码时的注释等自然语言部分使用中文" 一致），但 MAY 根据上下文穿插英文术语或 API 名。

#### Scenario: Conventional Commit 提交

- **WHEN** 开发者提交一次代码变更，遵循 Conventional Commits 格式
- **THEN** 提交消息 MUST 形如 `feat(runtime): <subject>` / `chore(asmdef): <subject>`，type 和 scope MUST 为英文
- **AND** subject MAY 使用中文或英文，由作者选择；body MAY 使用中文展开说明

#### Scenario: Git branch 命名

- **WHEN** 为某个 OpenSpec change 创建功能分支
- **THEN** branch 名 MUST 为英文 kebab-case（例如 `establish-doc-conventions`）
- **AND** MUST NOT 使用中文或拼音（如 `建立文档规范` / `jianli-wendang-guifan`）

#### Scenario: 代码中的行注释

- **WHEN** 为一段 C# 代码补充解释性行注释
- **THEN** 注释 SHOULD 使用中文
- **AND** 涉及的 API 名、类型名、字段名 MUST 保留英文原形（通常自然就是英文）

