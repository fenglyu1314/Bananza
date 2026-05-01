# Bananza

一个基于 **团结引擎 (Tuanjie) 1.8.2** 的 3D 动作游戏原型项目。

> 项目当前处于**基础框架搭建阶段**，详细的架构设计、模块划分、编码规范等将在 `Docs/` 目录中逐步补充。

---

## 环境要求

| 项 | 版本 / 说明 |
| --- | --- |
| 引擎 | 团结引擎 Tuanjie **1.8.2** |
| 渲染管线 | Universal Render Pipeline (URP) 14.x |
| 目标平台 | 暂定：PC / Android（后续确定） |
| Git LFS | **必须安装**（二进制资产使用 LFS 管理） |

### 首次克隆

```bash
# 1. 确保已安装 Git LFS（一次性）
git lfs install

# 2. 克隆仓库
git clone <repo-url> Bananza
cd Bananza

# 3. LFS 文件会随 clone 自动拉取；如未拉取可手动执行：
git lfs pull
```

---

## 目录结构

```
Bananza/
├── Assets/           # Unity 资产目录（Scripts / Editor / Data / Scenes / Settings …目前部分为占位）
├── Packages/         # UPM 包清单
├── ProjectSettings/  # 项目设置（已启用 Force Text 序列化）
├── Docs/             # 项目文档
│   ├── EngineNotes_Tuanjie.md   # 团结引擎笔记与差异
│   └── LegacyReferences.md      # 旧项目 Monkey_Demo 可复用设计参考
├── AGENTS.md          # 项目基础规则（面向 AI 助手与新成员）
├── .editorconfig      # 编辑器编码风格（缩进/行尾/编码）
├── .gitignore
├── .gitattributes     # 含 Git LFS 规则
├── LICENSE            # MIT
└── README.md
```

> `Assets/` 下子目录细分与 `asmdef` 划分以 OpenSpec `project-layout` spec 为准（运行 `openspec show project-layout` 查看）。

---

## 文档索引

- [Docs/EngineNotes_Tuanjie.md](Docs/EngineNotes_Tuanjie.md) —— 团结引擎使用经验与注意事项
- [Docs/LegacyReferences.md](Docs/LegacyReferences.md) —— 旧项目探索阶段的参考与复用清单
- 项目骨架与 `asmdef` 规范见 OpenSpec `project-layout` spec（`openspec show project-layout`）
- 项目玩法纲领（品类、玩家模式、核心动作、基调、目标结构、回访性）见 OpenSpec `gameplay-vision` spec（`openspec show gameplay-vision`）

---

## 开发约定

项目硬性约束（引擎版本、LFS、命名空间、`.meta` 规范、AI 助手行为等）统一见 [AGENTS.md](AGENTS.md)，本节仅列几项开发者日常需要注意的点：

- **序列化模式**：Force Text（已设置，便于 diff/merge）
- **行尾**：仓库统一 LF（详见 `.gitattributes`）
- **提交信息**：候选 Conventional Commits（`feat:` / `fix:` / `chore:` / `docs:` / `refactor:`），小步提交
- **分支策略**：待定
- **详细编码规范**：待定（将落在 `Docs/CodingGuidelines.md`）

---

## License

本项目采用 [MIT License](LICENSE)。
