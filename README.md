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

## 目录结构（规划中）

```
Bananza/
├── Assets/           # Unity 资产目录
├── Packages/         # UPM 包清单
├── ProjectSettings/  # 项目设置（已启用 Force Text 序列化）
├── Docs/             # 项目文档
│   ├── EngineNotes_Tuanjie.md   # 引擎使用笔记
│   └── LegacyReferences.md      # 旧项目参考
├── .gitignore
├── .gitattributes    # 含 Git LFS 规则
├── LICENSE           # MIT
└── README.md
```

> **Assets 下的子目录组织、Scripts 的 asmdef 划分**等细节尚未确定，将在后续设计文档中明确。

---

## 文档索引

- [Docs/EngineNotes_Tuanjie.md](Docs/EngineNotes_Tuanjie.md) —— 团结引擎使用经验与注意事项
- [Docs/LegacyReferences.md](Docs/LegacyReferences.md) —— 旧项目探索阶段的参考与复用清单

---

## 开发约定（待细化）

- **序列化模式**：Force Text（已设置，便于 diff/merge）
- **行尾**：仓库统一 LF（详见 `.gitattributes`）
- **分支策略**：待定
- **提交信息规范**：待定（倾向 Conventional Commits）
- **编码规范**：待定（将单独落在 `Docs/CodingGuidelines.md`）

---

## License

本项目采用 [MIT License](LICENSE)。
