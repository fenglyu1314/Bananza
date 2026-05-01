## 1. 起草 delta spec

- [x] 1.1 创建目录 `openspec/changes/establish-doc-conventions/specs/documentation-conventions/`（已在本 change 创建阶段完成）
- [x] 1.2 在上述目录下创建 `spec.md`，声明 4 条 Requirement（Default Language / Preserved English Tokens / Requirement Title Policy / Commit & Branch Language），每条至少 2 个 Scenario
- [x] 1.3 确认所有 Requirement 标题为英文、正文为中文、Scenario 使用 4 个 `#` 号、RFC 2119 关键词全大写

## 2. 修复 `openspec/config.yaml` 遗留问题

- [x] 2.1 把 `rules.spec` 段的键名改为 `rules.specs`（复数），消除 `Unknown artifact ID in rules: "spec"` 警告
- [x] 2.2 排查 `config.yaml` 中所有含中文全角引号或嵌套 `"..."` 的字符串，统一改为 YAML 单引号字符串（`'...'`）形式，避免 `YAMLParseError`
- [x] 2.3 在本地执行 `openspec status --change establish-doc-conventions --json`，确认不再输出 YAML 解析错误与未知 artifact ID 警告

## 3. 精简 `openspec/config.yaml` 的语言规则为执行层

- [x] 3.1 将 `context.Language` 段精简为：一段简短说明 + 一行 `参见 openspec/specs/documentation-conventions/spec.md`，保留"默认中文 / 骨架英文 / 标识符英文"三条核心执行摘要，删除与 spec 重复的完整规则清单
- [x] 3.2 将 `rules.proposal` / `rules.design` / `rules.tasks` / `rules.specs` 四段保留为"简短执行提示"（每段 1~2 条），不再承载规范性内容
- [x] 3.3 再次运行 `openspec status --change establish-doc-conventions --json`，确认无任何警告或解析错误

## 4. 校验本 change

- [x] 4.1 运行 `openspec validate establish-doc-conventions --strict`，要求输出 `Change 'establish-doc-conventions' is valid`
- [x] 4.2 运行 `openspec show establish-doc-conventions`，人工复核 4 条 Requirement 的 Scenario 是否全部被正确识别（未出现 "0 scenarios" 之类的静默失败）
- [x] 4.3 人工通读 `proposal.md` / `design.md` / `specs/documentation-conventions/spec.md`，确认无英文遗漏、无错别字、无与 `project-layout` spec 冲突的规则（AI 复核已通过，待用户最终确认）

## 5. 归档本 change

- [x] 5.1 运行 `openspec archive establish-doc-conventions`，确认 `openspec/specs/documentation-conventions/spec.md` 被自动创建
- [x] 5.2 运行 `openspec validate documentation-conventions --strict`，确认归档后的 capability spec 通过严格校验（已补写 Purpose 后再次通过）
- [x] 5.3 运行 `openspec list --specs` 与 `openspec list --archive`，确认 `documentation-conventions` 出现在 specs 列表、`establish-doc-conventions` 出现在 archive 列表（注：v1.3.1 CLI 无 `--archive` 选项，改用 `openspec list --changes` 确认无活跃 change + 文件系统核验 archive 目录）

## 6. 提交

- [ ] 6.1 按 Conventional Commits 格式提交 spec 与 config 修改，建议两个 commit：
    - `docs(openspec): add documentation-conventions capability`
    - `chore(openspec): fix config.yaml rules key and slim language rules`
- [x] 6.2 本 change 不修改 `AGENTS.md`；后续若需指引读者到新 spec，另起独立 change 处理
