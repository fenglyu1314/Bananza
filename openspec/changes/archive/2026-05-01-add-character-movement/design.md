## Context

本 change 是 Bananza 玩法能力的**第一块落地砖**，P1 阶段的起步工作。项目此前只有三条基线 capability（`gameplay-vision`、`project-layout`、`documentation-conventions`），还没有任何运行时玩法代码。本次要在团结引擎（Tuanjie）项目里引入主角的"地面移动"与"第三人称跟随相机"两项基础能力，为后续的拾取、投掷、任务等所有玩法动作提供立足点。

相关约束来自：

- `gameplay-vision`：3D 第三人称捣蛋沙盒、单机离线、禁止失败 / 惩罚机制、动词语义为"捣蛋 + 探索"。
- `project-layout`：只有 `Bananza.Runtime` 与 `Bananza.Editor` 两个 asmdef；运行时代码落在 `Assets/Scripts/Runtime/<Module>/<SubModule>/`，目录即命名空间；最多两级。
- `documentation-conventions`：OpenSpec 骨架与 RFC 2119 关键词保留英文，其余正文使用中文；代码标识符保留原文并用反引号包裹。
- 团结引擎注意事项见 `Docs/EngineNotes_Tuanjie.md`；`.meta` 严禁人工创建。

## Goals / Non-Goals

**Goals:**

- 在一个测试沙盒场景中，玩家可以用键鼠或手柄平滑操控主角在地面上行走与转向。
- 相机以稳定、无晕动的方式跟随主角，使主角在大部分时间处于画面可见区域内。
- 主角的移动行为接入团结引擎物理（或等价的角色控制器），在普通地形上不会穿模、不会卡进几何。
- 代码按 `project-layout` 的目录 / 命名空间约束落位，且仅依赖 `Bananza.Runtime` 单一 asmdef，未来扩展拾取 / 投掷时无需重排目录。

**Non-Goals:**

- 不做跳跃、冲刺、蹲伏、滑铲等进阶位移动作（留待后续 change）。
- 不做拾取、放下、投掷、互动（这些属于 P1 内但不在本 change）。
- 不做双人 / 分屏 / 第二玩家输入（P4）。
- 不做相机碰撞检测、相机遮挡淡化、肩部切换等高级相机特性（仅保留"跟随 + 看向主角"的最小形态）。
- 不接入 UI、HUD、任务提示、音频、存档。
- 不引入任何形式的生命值、耐力、体力槽、失败状态（由 `gameplay-vision` 硬约束）。

## Decisions

### Decision 1：模块落位采用 `Gameplay/Player/` 单一子模块

**方案**：本 change 新增的所有运行时脚本统一放在 `Assets/Scripts/Runtime/Gameplay/Player/`，命名空间 `Bananza.Gameplay.Player`。相机相关脚本与角色控制脚本同居该目录。

**理由**：

- `project-layout` 明确"目录即命名空间、Runtime 下最多两级"，且给出了 `Gameplay/Player/PlayerController.cs` 的范例，天然契合本场景。
- 当前只有"跟随主角的机位"这一种相机形态，把它作为 Player 模块的表现组件最简单；一旦未来引入双人分屏 / 自由视角 / 过场镜头，再通过单独的 change 拆出 `Gameplay/Camera/` 或独立 asmdef 不迟。
- 避免过早分层导致的空目录与跨模块耦合。

**考虑过的替代**：

- *方案 B*：将相机独立放在 `Assets/Scripts/Runtime/Gameplay/Camera/` 下。**放弃理由**：当前只有一个相机用例，独立一级模块收益极低，反而多出一个只含 1~2 个文件的模块。
- *方案 C*：将角色控制与相机分别放在不同命名空间但同一 asmdef。**放弃理由**：`project-layout` 约束下效果等同方案 B，且当前无需拆分。

### Decision 2：角色移动采用团结引擎内置 `CharacterController` 组件

**方案**：主角 GameObject 挂载团结引擎内置 `CharacterController`，由一个 `PlayerController` MonoBehaviour 从输入系统拿到移动意图，换算成相对相机方向的位移，驱动 `CharacterController.Move()`。重力自行施加（不依赖 Rigidbody）。

**理由**：

- `CharacterController` 自带胶囊碰撞与 slope / step 处理，对"捣蛋沙盒"这类无战斗、无复杂物理交互的位移需求已绰绰有余。
- 行为可预测、输入到位移是确定性的，便于后续拾取 / 投掷时的动作衔接（动作序列不会被物理抖动打断）。
- 无需调 Rigidbody 参数即可得到稳定的地面贴合感；移动端移植时也更稳定。

**考虑过的替代**：

- *Rigidbody + 力 / 速度驱动*：更物理真实，但在"精确方向控制"与"与可互动物体并存"时调参成本高，且易出现漂移；暂不引入。
- *第三方控制器（如 Opsive、Kinematic Character Controller 等）*：属于 ThirdParty 插件，按 `project-layout` 需独立 asmdef 且要评估版权与维护性；本阶段不必要。

**Trade-off**：`CharacterController` 不参与物理碰撞响应（不会推动 Rigidbody 物体），一旦 P1 后续要做"撞到物体使其移动"的效果需额外处理；这在本 change 范围之外，留作未来改造点。

### Decision 3：输入采用团结引擎新版 Input System（`com.unity.inputsystem`）

**方案**：用 Input System 的 `InputAction` / `PlayerInput` 组件定义 `Move`（Vector2）与 `Look`（Vector2，预留）两个动作，按键鼠与手柄各绑一套。不引入旧版 `Input.GetAxis`。

**理由**：

- 新版 Input System 天然支持多设备 / 多玩家 / 设备热插拔，为 P4 的双人做好铺垫，避免届时推倒重来。
- 更好配合 Action Asset 做数据驱动绑定，便于后续在 change 中扩展动作表。

**考虑过的替代**：

- 旧版 `UnityEngine.Input`：上手简单但无法干净支持双人本地分屏，后期必然返工；放弃。

### Decision 4：相机采用"纯脚本驱动的跟随相机"，不引入 Cinemachine

**方案**：在 Player prefab 之外放一台主相机，挂一个 `PlayerFollowCamera` MonoBehaviour，在 `LateUpdate` 中以"固定偏移 + 平滑插值"的方式跟随主角位置，并朝主角看齐。相机参数（偏移、跟随平滑系数、观察点高度）通过 Inspector 暴露即可，本 change 不做 ScriptableObject 配置化。

**理由**：

- 目标是"最小可玩"，Cinemachine 的能力（Virtual Camera、Blending、Noise、Impulse 等）在本阶段完全用不上，引入反而增加学习与维护成本。
- 自写 ~50 行脚本足以覆盖本阶段需求，且对后续替换为 Cinemachine（如果真的需要）零阻力——届时只需替换一个组件。
- 保持依赖面足够小，易于在团结引擎上验证。

**考虑过的替代**：

- *Cinemachine*：功能丰富但属于额外包依赖，且部分版本在团结引擎上的兼容性还需验证；暂不引入，留待后续 change 按需评估。

### Decision 5：测试场景命名与位置

**方案**：新增一个沙盒场景 `Assets/Scenes/Sandbox_P1.scene`，含一块地面、若干参照物（墙、箱子等 primitive）、一个主角 prefab 实例与跟随相机。该场景作为 P1 各 change 共享的调试场景，后续 change（拾取、投掷等）可以在同一场景中增量验证。

**理由**：统一沙盒场景可减少"换 change 就换场景"的切换成本；同时场景路径遵守 `project-layout` 对 `Assets/Scenes/` 的约束。

## Risks / Trade-offs

- **[风险] 团结引擎在某些版本下 Input System 默认未启用** → **缓解**：在 `tasks.md` 中明确一步：在 Project Settings → Player → Active Input Handling 切到 "Input System Package (New)" 或 "Both"，并在 `Docs/EngineNotes_Tuanjie.md` 追加记录。
- **[风险] `CharacterController` 在陡坡 / 台阶上的表现依赖参数调优** → **缓解**：本 change 只保证在平坦测试场景下可用；陡坡与复杂地形的调优作为未来 change 的任务，不纳入当前验收。
- **[风险] 相机在高速移动或瞬移场景下可能出现抖动 / 穿墙** → **缓解**：本 change 仅覆盖"平缓跟随"，暂不处理遮挡与碰撞；明确写入 Non-Goals，避免被误要求。
- **[Trade-off] 不引入 Cinemachine 意味着未来如果需要复杂镜头（过场、肩切）要换实现** → 接受；届时通过独立 change 替换，当前脚本是临时方案的定位清楚。
- **[Trade-off] 将相机脚本与角色脚本共置于 `Gameplay/Player/`** 会在未来拆分 Camera 模块时需要移动文件 → 接受；移动成本低且会有对应 change 明确修订 `project-layout`。

## Open Questions

- 主角的占位视觉用 primitive（胶囊体）还是导入一个占位模型？建议先用 primitive，节省美术资源阻塞；后续 change 引入真实角色模型时替换。*（倾向：primitive）*
- 是否现在就把相机的关键参数（跟随偏移、平滑系数）抽到 ScriptableObject 以便未来 tuning？建议本 change 先用 Inspector 字段，等参数真的开始频繁调整时再在独立 change 中做配置化。*（倾向：暂不抽出）*
