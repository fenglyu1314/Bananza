## Context

`project-layout` capability 上线后，团队在实际使用 OpenSpec 的过程中逐步沉淀出一套"产物用什么语言写"的约定。该约定目前以两种形式并存：

1. **`openspec/config.yaml` 的 `context.Language` + `rules.*`**：被 OpenSpec CLI 注入到 skill 的 prompt，对 AI 生成的 artifact 起**运行时**约束。
2. **散落在 `project-layout` spec 的某些 Scenario、`AGENTS.md` 的某些段落、历史对话中**：作为口头约定存在，无系统性文档。

这种"规则散在 config + 口头"的状态暴露了 3 个具体问题：

- **无法被 MUST/SHALL 级别引用**：`AGENTS.md` 如果说"所有 proposal 用中文写"，只能说"见 config.yaml"，但 config.yaml 本质是 YAML 配置文件，不是规范文档。
- **变更无审计**：`config.yaml` 里已经发现两个问题（`rules.spec` 拼写错误触发 `Unknown artifact ID in rules` 警告、YAML 引号嵌套触发 `YAMLParseError`），但因为 config 不经 OpenSpec change 流程，这两个 bug 是在起 `openspec new change` 时才被动发现的。
- **存在重复风险**：未来若再写 `README.md` 或 Docs/*.md 描述协作规范，会出现"同一规则三处抄写"的分歧源。

本设计把这套约定固化为 `documentation-conventions` capability，让它与 `project-layout` 一样进入 OpenSpec 的 "`specs/` 单一事实源 + `changes/` 变更可追溯" 体系。

## Goals / Non-Goals

**Goals:**

- 产出一个独立的 capability spec `documentation-conventions`，作为 OpenSpec 产物语言策略的权威来源。
- 明确四类规则边界：默认语言、保留英文清单、Requirement 标题策略、外部接口（commit/branch/PR）语言策略。
- 在本 change 内顺手修掉 `config.yaml` 的两个遗留问题（YAML 引号嵌套、`rules.spec` 拼写错误）。
- 建立 `documentation-conventions` 与 `project-layout` 之间的边界声明，避免未来二者规则互相蚕食。

**Non-Goals:**

- **不**追溯翻译已归档的 `establish-project-layout` 产物（proposal/design/tasks 保持英文原貌，作为历史快照）。
- **不**修改 `openspec/specs/project-layout/spec.md` 中已有的 "Spec as the Single Source of Truth for Project Layout" 这条 Requirement——它只适用于 `project-layout` 这一 capability 自身。
- **不**为 `Docs/*.md`、`README.md` 制定任何新的文档风格规范（那属于未来可能的 `writing-style` capability，本 change 不涉及）。
- **不**强制 Git commit message、branch name、PR 标题使用中文——外部工具惯例（Conventional Commits、GitHub/GitLab UI）保留英文。
- **不**触碰 RFC 2119 关键词本身——它们是行业标准，本 spec 只规定"保留英文大写"，不重新定义语义。

## Decisions

### Decision: 独立 capability 而非并入 `project-layout`

**选项：**

- **A.** 新建 `documentation-conventions` capability。（采纳）
- **B.** 作为一条 Requirement 加进 `project-layout`。
- **C.** 只用 `config.yaml` 持有规则，不写 spec。

**采纳 A 的理由：**

- `project-layout` 的语义是"源代码/资源/程序集在 Unity 项目中的物理布局"，与"文档语言策略"是两个正交关注点。强行合并会让 `project-layout` spec 变成杂物筐，未来很难重构。
- B 会导致 `project-layout` 承担跨 capability 的文档约定——比如未来如果再加一个 `testing-strategy` capability，它也需要遵循"默认中文"，那条规则应该由谁管？独立 capability 才能支撑正交扩展。
- C 的问题在 Context 里已经说清楚：`config.yaml` 是"运行时执行层"，不是"规范权威层"。拒绝 C 是本 change 的根本动机。

### Decision: Capability 命名 `documentation-conventions`

**选项：**

- **A.** `documentation-conventions`（采纳）。
- **B.** `spec-language-policy`。
- **C.** `i18n-policy` / `chinese-first`。

**采纳 A 的理由：**

- A 的范围最宽，未来可容纳 Markdown 风格、图表规范、术语表、文档归属策略等主题；B 把范围锁死在"语言"，C 把范围锁死在"中文"，都预设了错误的边界。
- `documentation-conventions` 与 `project-layout` 并列时，两者都以"-conventions"/"-layout" 这种"领域 + 形式"的方式命名，风格一致。
- 未来若 `documentation-conventions` 增长过大，可平滑拆出 `writing-style`、`terminology` 等子 capability，命名演进路径清晰。

### Decision: `config.yaml` 是"执行层"，不是"规范层"

**选项：**

- **A.** 规则以 `documentation-conventions` spec 为准，`config.yaml` 只保留"对 spec 的简短引用 + 必要的 prompt 执行细节"。（采纳）
- **B.** spec 和 config.yaml 双处并存完整规则，保持当前状态。

**采纳 A 的理由：**

- B 的重复会迅速演化成分歧——任何一方被修改而另一方忘了跟进，就是新 bug 的温床。
- OpenSpec `config.yaml` 的设计意图本来就是"prompt 注入"，不适合承担"规范持久化"职责。把规则搬到 spec，config.yaml 自然回归其本职。
- 本 change 的 tasks 里会明确：spec 归档后把 `config.yaml` 的 `context.Language` 压缩为 "以 `openspec/specs/documentation-conventions/spec.md` 为准" 的短说明，避免长期重复。

### Decision: Requirement 标题保留英文

**选项：**

- **A.** Requirement 标题（`### Requirement:` 后的短语）保留英文，正文中文。（采纳）
- **B.** 标题也翻译为中文。

**采纳 A 的理由：**

- OpenSpec `archive` 时会用 Requirement 标题作为 **delta 匹配 key**（delta spec 里的 `## MODIFIED Requirements` / `## REMOVED Requirements` 段必须用完全相同的标题才能精准替换）。一旦翻译为中文，未来所有 delta 都必须用中文标题匹配，失去了未来工具链复用的灵活性。
- Requirement 标题本身是**高密度短语**（如 `Top-level Asset Directory Layout`、`Assembly Split Triggers`），翻译后反而更冗长且歧义更多。
- 正文已经是中文，标题英文不会造成阅读负担；反而能在目录/大纲跳转时作为稳定锚点。

### Decision: 保留英文清单采用"白名单 + 语义化"而非"硬编码列表"

**选项：**

- **A.** 把保留英文的内容按**类别**归类（结构骨架、RFC 2119、代码标识符、行业专有名词），避免维护一份会过时的硬编码 token 列表。（采纳）
- **B.** 在 spec 里逐条列出所有保留英文的 token（如 `## Purpose`、`### Requirement:`、`MUST`、`SHALL`、...）。

**采纳 A 的理由：**

- B 会导致 OpenSpec 自身版本升级（比如引入新骨架 token）后，我们的 spec 立即过时。
- A 让 spec 聚焦在"语义规则"而非"字面清单"——未来只要某个 token 属于"OpenSpec 结构骨架"，就自动受保护。
- Scenario 里仍然可以举**代表性例子**（如 `### Requirement:`、`MUST`），帮助读者校准，但不承担穷举职责。

## Risks / Trade-offs

- **风险：中文排版与 RFC 2119 英文关键词混排的可读性** → 缓解：在 spec 的 Scenario 中给出标准混排示例（如 `**THEN** 该 `.asset` 文件 MUST 放在 ...`），让团队有参照；若半年内发现阅读负担明显，可在后续 change 中微调空格/标点规则。
- **风险：AI 工具不遵守 spec 规则** → 缓解：spec 只能约束走 OpenSpec skill 流程的 AI；对不读 config.yaml 的 AI，由 `AGENTS.md` 显式说明"请阅读 `documentation-conventions` spec"来兜底。验收时人工 review 产物语言即可捕获偏差。
- **权衡：中文默认 vs. 开源可读性** → 本项目主攻中文团队内部协作，暂不追求国际化。若未来项目转为开源或需要英文贡献者，需起新 change `switch-to-english-first` 或 `bilingual-conventions` 调整策略，**不在**本 change 预留开关。
- **风险：`config.yaml` 简化后 AI prompt 上下文变短，AI 可能忘掉细则** → 缓解：`config.yaml` 的 `context.Language` 保留一个明确的"参见 `documentation-conventions` spec"的指引，并在 `rules.*` 里保留最核心的 3~4 条执行层摘要（如"默认中文"、"保留骨架英文"），既避免重复全部规则，又保证 AI 基本遵循。

## Migration Plan

1. **本 change 实施（详见 tasks.md）**：
    1. 写 delta spec：`openspec/changes/establish-doc-conventions/specs/documentation-conventions/spec.md`。
    2. 修 `openspec/config.yaml`：
        - `rules.spec` → `rules.specs`（消除 OpenSpec 警告）。
        - 将 `context.Language` 与 `rules.*` 压缩为"以 spec 为准 + 核心执行摘要"。
    3. `openspec validate establish-doc-conventions --strict` 必须通过。
2. **归档**：`openspec archive establish-doc-conventions`，OpenSpec 自动生成 `openspec/specs/documentation-conventions/spec.md`。
3. **后续对齐（不属于本 change）**：下次修改 `AGENTS.md` 时，补一条"协作规范请参见 OpenSpec `documentation-conventions` spec"的指引；不为此单独起 change。

**回滚策略**：若上线后发现规则不实用，走正规 OpenSpec 流程起新 change（`revise-doc-conventions` 或 `remove-doc-conventions`）。不通过手改 spec 回滚。

## Open Questions

- 本 change 归档后，是否需要在 `AGENTS.md` 中加一条"OpenSpec 产物语言规则见 `documentation-conventions` spec"的指引？——**倾向加**，但作为独立小 change 处理，本 change 不含。
- 术语表（glossary）是否应属于 `documentation-conventions`？——**暂不**。术语表是"词汇 → 中文/英文的唯一选择"，与"语言策略"相关但不同；若未来需要，起新 change `add-glossary` 或升级本 capability。
