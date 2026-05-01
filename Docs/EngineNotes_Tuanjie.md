
# EngineNotes · 团结引擎（Tuanjie Engine）项目规约

> 本文档用于记录本项目在 **团结引擎（Tuanjie Engine）** 下的版本信息、与 Unity 标准版的差异、已确认的技术选型，以及开发过程中逐步积累的踩坑与经验。
>
> **目的**：让所有协作者（包括 AI 协作工具）在给出建议前，先查阅本文档，避免基于"标准 Unity"的经验给出在团结下不适用的方案。
>
> **维护方式**：开发过程中遇到差异点、踩坑、版本升级等情况，随时回来追加一条记录。

---

## 1. 引擎与版本

| 项 | 值 |
| --- | --- |
| 引擎 | 团结引擎（Tuanjie Engine） |
| 当前版本 | **1.8.2** |
| 底座对应 Unity 版本 | Unity **2022.3.62t4**（LTS）；团结 1.x 线基于 2022.3 LTS fork |
| 维护方 | Unity 中国 |
| 编辑器语言 | 中文 / 英文可切换 |

> 后续升级到新版本时，请在本节更新版本号，并在第 6 节追加"版本升级记录"。

---

## 2. 目标平台

| 平台 | 优先级 | 说明 |
| --- | --- | --- |
| **PC（Windows）** | ⭐⭐⭐ 主力 | 首发目标平台，所有功能以 PC 体验为准 |
| **移动端（Android / iOS）** | ⭐ 低 | 未来可能支持，但非当前开发约束 |
| 小游戏（微信 / 抖音等） | ❌ 暂不考虑 | 如后续要做，框架需回来重新评估 |
| 主机 / 车机 / XR | ❌ 不考虑 | — |

### 平台策略对编码的影响（当前阶段）
- **以 PC 为第一公民**：可以放心使用 PC 上无压力的写法（如较重的反射、LINQ、UniTask、Addressables 等）。
- **为移动端留余地**：
  - 避免每帧分配大量 GC（`foreach` over `List`、字符串拼接等要注意）。
  - 避免过度依赖 PC 独有的 API（如某些 `System.IO` 深度用法在 Android/iOS 上行为不同）。
  - UI 设计尽量预留**可触屏操作**的空间（按钮大小、交互热区）。
- **不提前为移动端过度设计**：优先保证 PC 的开发效率，等真要做移动端时再专项优化。

---

## 3. 技术选型（已确认）

> 本节聚焦**团结引擎语境下的技术选型决策**。硬性规则（引擎版本、LFS、命名空间等）见 [AGENTS.md](../AGENTS.md)，仓库环境信息见 [README.md](../README.md)。

| 模块 | 选型 | 备注 |
| --- | --- | --- |
| **渲染管线** | URP（Universal Render Pipeline） | 可能根据项目需要做定制（自定义 Renderer Feature、后处理等） |
| **输入系统** | 新 Input System（Package） | 初步决定，实际使用中若遇到兼容问题可再评估 |
| **编程语言** | C# 9.0（Unity 2022.3 LTS 默认启用的子集）[^csharp] | 需要更高版本特性时再单独启用 |
| **脚本后端** | Mono（开发期） / IL2CPP（发布期） | 发布时切 IL2CPP 以便将来兼容移动端 |
| **.NET 兼容级别** | .NET Standard 2.1 | 默认即可 |
| **版本控制** | Git + Git LFS | 不依赖 Unity Cloud / Collab |

[^csharp]: Unity 2022.3 LTS 官方启用的是 C# 9.0 的部分特性（如 `record`、`init`、目标类型 `new` 等），并非完整 9.0。团结 1.8.2 下的实际启用范围待实战验证。

### 待在开发中决策的选型（多方案对比）

以下项目暂未定，等到对应需求出现时再决策；关注点是"**要不要用、用哪个**"：

- [ ] **资源管理**：Addressables vs YooAsset vs 原生 Resources/AssetBundle
- [ ] **异步方案**：UniTask vs 原生 Coroutine/Task
- [ ] **UI 框架**：UGUI vs UI Toolkit
- [ ] **DI / 架构框架**：是否引入 VContainer / Zenject，或自研轻量容器
- [ ] **序列化**：JsonUtility vs Newtonsoft.Json vs MemoryPack
- [ ] **Timeline / Cinemachine**：是否使用，以及主版本线（2.x / 3.x）

> 注：上述候选方案在团结引擎下是否可用、版本是否齐备，属于"兼容性验证"问题，见第 5 节。

---

## 4. 与标准 Unity 的已知差异

> 这些是我们在评估阶段已知的差异点，随着开发深入会持续补充。

### 4.1 包管理（Package Manager）
- 团结使用**自己的 Package Registry**（团结镜像/私服），而非 Unity 官方 `packages.unity.com`。
- 大多数 Unity 官方包（URP、Input System、Cinemachine、Addressables 等）在团结下都可用，但**版本号可能与 Unity 官方不同**。
- 部分 Unity 海外特供包（如 Unity Gaming Services 某些模块）可能**缺失**。
- **实践建议**：添加包前先在 Package Manager 里确认实际可用版本，不要直接照抄 Unity 官方文档的版本号。

### 4.2 额外平台支持（当前项目暂不使用）
- 原生支持**微信小游戏 / 抖音小游戏 / 小红书小游戏**导出。
- 原生支持 **HarmonyOS Next（纯血鸿蒙）**。
- 原生支持**国产车机系统**。
- 📝 目前项目不涉及，但这是团结相比标准 Unity 的主要差异化方向。

### 4.3 云服务
- Unity Dashboard / Unity Gaming Services / Collab 等海外云服务**大概率不可用**。
- 团结可能有自己的开发者中心与 Asset Store 镜像。
- **实践建议**：不依赖 Unity 云服务做版本管理或远程配置，走 Git + 自建服务。

### 4.4 渲染
- 基于 URP 做了团结自研扩展（主要服务于小游戏端和车机场景的轻量渲染）。
- Shader Graph 基本通用，某些新 feature 可能跟得慢于 Unity 最新版。
- 📝 本项目以 URP 为主，若用到自研扩展再回来记录。
- **当前项目实际使用的 URP 版本**：14.x（以 `Packages/manifest.json` 为准，升级时同步更新此处）。

### 4.5 Editor
- 编辑器原生支持中文界面。
- 部分错误信息和文档有官方中文版。

---

## 5. 当前不确定 / 待实验的点（兼容性验证）

> 本节聚焦"**团结下能不能用、行为是否与标准 Unity 一致**"。选型决策（要不要用、用哪个）归档到第 3 节。

以下是目前暂未验证、需要在开发中确认的细节。遇到时请回来更新本节：

1. **Input System 包版本与稳定性**：团结 1.8.2 下 Input System 的实际包版本、已知 issue、与底座 Unity 2022.3 官方版本的行为差异。
2. **Burst / Jobs / ECS** 等 DOTS 相关模块在团结下的可用性。
3. **Profiler / Frame Debugger** 等调试工具是否有定制差异。
4. **C# 语言特性实际启用范围**：团结 1.8.2 下 C# 9.0 中哪些语法已启用、哪些未启用。
5. **团结 1.8 → 2.x 的升级路线**：是否需要在代码中规避某些即将废弃的 API。

---

## 6. 版本升级记录

> 每次升级团结版本时在此追加一条，记录升级原因、踩坑、API 变化等。

| 日期 | 原版本 | 新版本 | 变更说明 |
| --- | --- | --- | --- |
| — | — | 1.8.2 | 新工程建立，初始版本 |

---

## 7. 踩坑与经验记录

> 开发过程中遇到的团结特有问题、与标准 Unity 行为不一致的地方、临时解决方案等，都记录在这里。
>
> 格式建议：
> - **[日期] [标签] 问题简述**
>   - 现象：
>   - 原因：
>   - 解决方案：
>   - 相关链接/代码位置：

（暂无记录，首次踩坑时从此处开始追加。）

---

## 8. 参考资料

- 团结引擎官网：<https://unity.cn/tuanjie>
- 团结引擎文档中心：<https://docs.unity.cn/cn/tuanjie/>
- Unity 2022.3 LTS 文档（作为底座参考）：<https://docs.unity3d.com/2022.3/Documentation/Manual/>
- 项目内相关文档：
  - [AGENTS.md](../AGENTS.md) — 项目基础规则（面向 AI 助手与新成员）
  - [LegacyReferences.md](./LegacyReferences.md) — 旧 Demo 可参考设计
  - 待创建：`Framework.md`（项目框架设计）、`GDD.md`（游戏设计文档）、`CodingGuidelines.md`（详细编码规范）

---

_最后更新：2026-05-01_
