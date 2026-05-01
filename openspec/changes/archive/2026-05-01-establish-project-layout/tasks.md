## 1. 校验 spec 草案

- [x] 1.1 运行 `openspec validate establish-project-layout --strict` 确保 spec 合法、requirement 格式正确
- [x] 1.2 肉眼复核 `specs/project-layout/spec.md`：每个 requirement 至少 1 个 4-井号 scenario，无拼写/措辞问题

## 2. 迁移权威源至 OpenSpec

- [x] 2.1 修订 [AGENTS.md](../../AGENTS.md) §3：将对 `Docs/ProjectLayout.md` 的引用替换为对 OpenSpec `project-layout` spec 的模糊措辞（C 方案，不写具体路径）
- [x] 2.2 修订 [AGENTS.md](../../AGENTS.md) §6：同理，将 `Assets/` 子目录细分与 `asmdef` 策略两项后面挂着的"以 `Docs/ProjectLayout.md` 为准"改为 "以 OpenSpec `project-layout` spec 为准"
- [x] 2.3 删除 `Docs/ProjectLayout.md`（由用户执行 `git rm Docs/ProjectLayout.md`，AI 只提供命令）
- [x] 2.4 在 Unity 编辑器中刷新资源，确认 `Docs/ProjectLayout.md.meta` 被 Unity 自动清理，再一起提交（不要手工删 `.meta`）

## 3. 清扫对旧文档的其它引用

- [x] 3.1 `grep` 全仓库查找 `ProjectLayout.md` 文本，确认除 archive 外无残留
- [x] 3.2 检查 [README.md](../../README.md) 是否提及 `Docs/ProjectLayout.md`；如有，改为一句引导："项目骨架与 asmdef 规范见 OpenSpec `project-layout` spec（`openspec show project-layout`）"
- [x] 3.3 检查 `Docs/EngineNotes_Tuanjie.md`、`Docs/LegacyReferences.md` 是否交叉引用，视情况调整

## 4. 归档 change

- [ ] 4.1 用户确认所有 tasks 完成
- [ ] 4.2 运行 `openspec archive establish-project-layout`，将本 change 移入 `openspec/changes/archive/`，spec 生效到 `openspec/specs/project-layout/spec.md`
- [ ] 4.3 提交 commit：`chore(spec): establish project-layout capability`

## 5. 交付后校验

- [ ] 5.1 运行 `openspec list` 确认 `project-layout` 出现在 specs 列表中
- [ ] 5.2 运行 `openspec show project-layout` 确认内容可检索、格式完整
- [ ] 5.3 最后巡检：AGENTS.md 无断链、Docs 下无 `ProjectLayout.md` 残留、Unity 导入无 `.meta` 报错
