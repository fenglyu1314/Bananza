## 1. 产出与自检

- [x] 1.1 起草 `proposal.md`，说明为何现在需要 `gameplay-vision` 以及影响范围
- [x] 1.2 起草 `design.md`，逐条记录 17 条顶层玩法决策及其替代方案
- [x] 1.3 起草 `specs/gameplay-vision/spec.md`，用 `## ADDED Requirements` 固化 18 条 requirement，每条至少一个 `#### Scenario:`
- [x] 1.4 运行 `openspec validate draft-gameplay-vision --strict`，确认结构、scenario 数、delta 操作均符合规范
- [x] 1.5 运行 `openspec show draft-gameplay-vision --json --deltas-only`，肉眼核对每条 requirement 的标题是否与 `design.md` 中的 "对应 requirement" 引用一致

## 2. 一致性审阅

- [x] 2.1 对照 17 条决策与 spec 中的 18 条 requirement，确认每条决策都被覆盖，或在 `design.md` 中说明为何不落为 requirement
- [x] 2.2 检查 spec 中所有 requirement 标题均为英文，正文与 scenario 使用中文，RFC 2119 关键词全大写
- [x] 2.3 检查 spec 中没有规定实现细节（类名、组件名、具体数值、具体算法），仅规定可观察行为与约束
- [x] 2.4 检查 `proposal.md` 的 `Capabilities` 段声明的新 capability 与 `specs/` 下目录名完全一致（`gameplay-vision`）
- [x] 2.5 检查 `design.md` 中引用的未来 capability 名（`world-structure`、`player-character`、`art-direction`、`task-system`、`world-state-persistence` 等）均使用 kebab-case 且相互拼写一致

## 3. 用户评审

- [x] 3.1 将本 change 的三份产物（proposal / design / spec）交由用户通读
- [x] 3.2 针对用户反馈逐条回应：若是**措辞调整** → 直接改；若是**决策调整** → 在 `design.md` 的对应 Decisions 小节与 spec 的相应 requirement 同步修订
- [x] 3.3 在用户最终确认前，MUST NOT 执行归档

## 4. 归档与落地

- [x] 4.1 用户确认通过后，运行 `openspec archive draft-gameplay-vision`，让 `specs/gameplay-vision/spec.md` 合入 `openspec/specs/gameplay-vision/`
- [x] 4.2 归档后运行 `openspec spec show gameplay-vision`，确认规范已正确落到 `openspec/specs/gameplay-vision/spec.md`
- [x] 4.3 在 `AGENTS.md` 和 `README.md` 中以回链形式（"参见 `openspec/specs/gameplay-vision/` 的 spec"）引入对本 capability 的引用，避免正文复制规范内容

## 5. 后续 change 铺垫（仅记录，不在本 change 内执行）

- [ ] 5.1 记录待起草的后续 change 候选清单：`define-world-structure`（关卡拓扑）、`define-player-character`（猴子控制器与相机细节）、`define-art-direction`（Low-poly 技术规格）、`define-task-system`（任务配置与触发机制）、`define-world-state-persistence`（跨关卡状态存储）
- [ ] 5.2 将该清单作为参考写入下一轮 explore 会话的起点，不作为本 change 的产出
