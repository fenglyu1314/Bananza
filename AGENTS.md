# Bananza – 项目基础规则（AGENTS.md）

本文件是 **Bananza** 项目面向 AI 编码助手与新成员的基础规则。内容刻意精简，只列硬性约束；细节规范后续放 `Docs/`。

---

## 1. 项目定位

- 基于 **团结引擎 Tuanjie 1.8.2**（Unity 2022.3.62t4）+ **URP 14.x** 的 3D 动作游戏原型。
- 项目当前阶段、目录演进等易变信息见 [README.md](README.md)，本文件只承载稳定规则。

## 2. 环境与工具

- 引擎版本锁定：`2022.3.62t4` / `Tuanjie 1.8.2`，勿擅自升级。
- **必须安装 Git LFS**（二进制资产走 LFS，由 `.gitattributes` 管理，不要随意修改该文件）。
- 编辑器格式规则遵循仓库根目录的 `.editorconfig`，不要在 IDE 中覆盖其中的缩进/换行/编码设置。
- **开发 IDE**：Visual Studio Code + Visual Studio（用于 Unity 调试）

## 3. 代码约束（C#）

- C# 代码统一放在 `Assets/Scripts/` 下；运行时脚本放 `Assets/Scripts/Runtime/`，编辑器脚本放 `Assets/Scripts/Editor/`。
- 顶层命名空间使用 `Bananza`，可按模块分子命名空间（如 `Bananza.Gameplay`）。
- 详细的目录分层、命名空间映射与 `asmdef` 策略以 OpenSpec `project-layout` spec 为准（通过 `openspec show project-layout` 查看）。
- 一个 `.cs` 文件只放一个顶层类型，文件名与类型名一致。
- 引用新第三方库前先与用户确认，避免引入与团结引擎不兼容的包。

## 4. 资产与版本控制

- 二进制资产（贴图/模型/音频/视频/动画等）走 LFS，不要直接塞入 Git。
- `.meta` 文件必须与对应资源**一起提交**，禁止产生 `.meta` 孤儿文件或缺 `.meta` 的资源。
- **`.meta` 文件一律由 Unity 编辑器自动生成**，严禁手工编写、复制或由脚本/AI 直接创建；如需新增资源，先放入 `Assets/` 下让 Unity 生成 `.meta`，再一起提交。
- `ProjectSettings/` 下的 `.asset` 已启用 **Force Text** 序列化，修改需谨慎、写清 commit 说明。
- 提交信息倾向 **Conventional Commits**（`feat:` / `fix:` / `chore:` / `docs:` / `refactor:`），小步提交。

## 5. AI 编码助手须知

- 遵守本文件所有规则；当规则与用户临时指令冲突时，以用户指令为准并提醒冲突。
- **严禁主动创建、伪造或修改 `.meta` 文件**：所有 `.meta` 必须由 Unity 编辑器自动生成。新增资源时，仅创建资源文件本身，随后提示用户在 Unity 中刷新以生成 `.meta`，再统一提交。
- 修改 `ProjectSettings/*.asset`、`.gitattributes`、`.gitignore` 前，先告知用户可能的影响。
- 批量改动、删除文件、调整目录结构前，必须先向用户说明方案并获得确认。
- **不要主动新建文档文件**（`*.md`、README 等），除非用户明确要求。
- 回答中引用项目文件请使用 Markdown 链接格式，便于跳转。

## 6. 本文件不约束的范围

以下内容不在本文件约束范围内，**不要擅自约定**，以用户决策、OpenSpec spec 或 `Docs/` 下文档为准：

- `Assets/` 下子目录的细分组织—— 以 OpenSpec `project-layout` spec 为准
- `asmdef` 划分策略—— 以 OpenSpec `project-layout` spec 为准
- 分支策略、发布流程
- 详细编码风格（命名、注释、日志等）—— 以 `Docs/CodingGuidelines.md` 为准
