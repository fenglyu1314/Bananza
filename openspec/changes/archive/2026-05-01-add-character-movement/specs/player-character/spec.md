## ADDED Requirements

### Requirement: Ground Movement Driven by Player Input

主角 SHALL 支持由玩家输入驱动的地面移动：玩家在输入设备上给出一个方向矢量，主角 MUST 按该方向在水平面内移动，移动方向 MUST 相对于当前相机的朝向解算（即"按下前"代表相机正前方），而非世界坐标轴方向。

主角 MUST 自行承受重力作用，在失去地面支撑时 MUST 向下坠落直至再次落地；主角 MUST NOT 因正常移动操作而穿过测试场景中的墙壁、地面等静态几何体。

本 requirement 不要求实现跳跃、冲刺、蹲伏等进阶位移；这些动作由后续 change 按需引入。

#### Scenario: 玩家在平坦地面上推动摇杆前进

- **WHEN** 玩家在平坦的测试场景中，将手柄左摇杆或键盘 WASD 的前向输入推满
- **THEN** 主角 MUST 沿当前相机正前方在水平面内匀速前进
- **AND** 主角 MUST 持续贴合地面，不出现悬空或抖动
- **AND** 玩家松开输入后，主角 MUST 在可接受的短时间内停止移动

#### Scenario: 玩家沿斜方向输入

- **WHEN** 玩家同时输入"前 + 右"方向（例如摇杆指向右上 45°）
- **THEN** 主角的实际移动方向 MUST 对应"相机前方与相机右方的合成方向"
- **AND** 移动速率 MUST NOT 因为合成方向而超过仅推单一方向时的最大速率

#### Scenario: 主角尝试走向一堵墙

- **WHEN** 玩家持续输入一个会让主角直接撞向墙的方向
- **THEN** 主角 MUST 被墙阻挡而停止继续前进
- **AND** 主角 MUST NOT 穿入墙体或卡在墙内无法退出

#### Scenario: 主角从平台边缘走出

- **WHEN** 主角从一个有落差的平台边缘走出
- **THEN** 主角 MUST 受重力作用向下坠落
- **AND** 主角在落到下方地面后 MUST 恢复正常的地面移动响应

---

### Requirement: Character Facing Follows Movement Direction

当主角在水平面内产生位移时，主角的身体朝向 MUST 平滑地转向当前的移动方向，使玩家能够直观看到主角正"朝哪儿走"。当无移动输入且主角静止时，主角朝向 SHALL 保持上一次的朝向，MUST NOT 出现无意义的持续旋转。

本 requirement 关注的是"运动反馈的可读性"，不规定具体的旋转速率或插值方式，由实现在合理范围内决定。

#### Scenario: 玩家连续改变移动方向

- **WHEN** 玩家先按前进后立即改为按向右
- **THEN** 主角 MUST 在短时间内平滑旋转到朝向右方
- **AND** 主角 MUST NOT 出现瞬移式的朝向切换或反复抖动

#### Scenario: 玩家松开输入后静止

- **WHEN** 玩家松开所有移动输入且主角停止位移
- **THEN** 主角 MUST 保持当前朝向不动
- **AND** 主角 MUST NOT 因输入为零而发生自旋转或回中

---

### Requirement: Third-Person Follow Camera

测试场景中 MUST 存在一台主相机，其行为遵守如下规则：

- 相机 MUST 以"固定相对偏移 + 平滑跟随"的方式追随主角当前位置。
- 相机的视线 MUST 指向主角所在位置或其上方的固定观察点，使主角在大部分常规移动场景下位于画面可见区域内。
- 相机跟随 MUST 在 `LateUpdate`（或等价的每帧末阶段）中更新，以避免与角色位移产生一帧抖动。
- 相机 MUST NOT 以"紧贴角色、无平滑"的方式瞬时吸附，避免造成强烈晕动。

本 requirement 不要求相机具备碰撞检测、遮挡淡化、肩部切换、自由视角旋转等高级功能；这些作为后续 change 的候选扩展。

#### Scenario: 主角开始移动时相机行为

- **WHEN** 主角从静止开始向任意方向移动
- **THEN** 相机 MUST 平滑地跟随主角，保持大致恒定的相对偏移
- **AND** 主角 MUST 始终处于画面可见区域内

#### Scenario: 主角持续高速移动后急停

- **WHEN** 主角持续高速移动后玩家突然松开输入
- **THEN** 相机 MUST 在短时间内平滑收敛到"对准静止主角"的姿态
- **AND** 相机 MUST NOT 出现明显的过冲回弹或持续漂移

#### Scenario: 帧内更新顺序

- **WHEN** 某一帧内主角位移完成
- **THEN** 相机 MUST 在该帧稍后的阶段（如 `LateUpdate`）读取主角最新位置并跟随
- **AND** 相机 MUST NOT 使用上一帧的角色位置进行跟随（避免一帧延迟抖动）

---

### Requirement: Input Mapping via Unity Input System

主角的移动输入 MUST 通过团结引擎的 `Input System` 包（`com.unity.inputsystem`）定义，采用 `InputAction` / `InputActionAsset` 的形式；MUST NOT 直接调用旧版 `UnityEngine.Input.GetAxis` 或 `UnityEngine.Input.GetKey` 读取移动输入。

本 change 范围内 MUST 至少定义一个名为 `Move` 的 `Vector2` 动作，并为键鼠与常见手柄各提供一套绑定。动作表 MUST 以独立的 `InputActionAsset` 形式存放在 `Assets/Settings/Input/`（或与 `project-layout` 对 `Assets/Settings/` 的约定一致的位置）下，便于未来统一维护与扩展。

#### Scenario: 用手柄驱动移动

- **WHEN** 玩家连接一只常见手柄并推动左摇杆
- **THEN** `Move` 动作 MUST 接收到对应的 `Vector2` 值
- **AND** 主角 MUST 按该输入值进行移动

#### Scenario: 用键鼠驱动移动

- **WHEN** 玩家按下 W / A / S / D 或方向键
- **THEN** `Move` 动作 MUST 接收到等价的 `Vector2` 值
- **AND** 主角 MUST 按该输入值进行移动

#### Scenario: 有人试图加回旧版输入

- **WHEN** 某次提交中出现对 `UnityEngine.Input.GetAxis("Horizontal")` 等旧 API 的直接调用用于主角移动
- **THEN** 该提交 MUST 被视为违反本 requirement
- **AND** 修复方式 MUST 是将该输入迁移到当前的 `InputActionAsset` 中

---

### Requirement: Module Placement Under `Gameplay/Player`

本 capability 的运行时脚本 MUST 放置在 `Assets/Scripts/Runtime/Gameplay/Player/` 目录下，使用 `Bananza.Gameplay.Player` 为根的命名空间，遵循 `project-layout` 中 "Scripts Subtree Layout" 与 "Directory Hierarchy Mirrors Namespace" 的约束。

相机跟随脚本在本 capability 当前形态下 SHALL 与角色控制脚本同居 `Gameplay/Player/` 下；未来若引入多相机用例或独立相机模块，MUST 通过单独的 change 将相机迁至 `Gameplay/Camera/`（或独立 asmdef）并同步更新 `project-layout`。

#### Scenario: 新增角色控制脚本的落位

- **WHEN** 需要新增 `PlayerController.cs`（主角移动控制）
- **THEN** 该文件 MUST 位于 `Assets/Scripts/Runtime/Gameplay/Player/PlayerController.cs`
- **AND** 其命名空间 MUST 为 `Bananza.Gameplay.Player`

#### Scenario: 新增跟随相机脚本的落位

- **WHEN** 需要新增 `PlayerFollowCamera.cs`（跟随相机）
- **THEN** 该文件 MUST 位于 `Assets/Scripts/Runtime/Gameplay/Player/PlayerFollowCamera.cs`
- **AND** 其命名空间 MUST 为 `Bananza.Gameplay.Player`

#### Scenario: 试图把相机拆到独立模块而未走 change 流程

- **WHEN** 某开发者直接把 `PlayerFollowCamera.cs` 移到 `Assets/Scripts/Runtime/Gameplay/Camera/`，并修改其命名空间，但未提交对应的 OpenSpec change
- **THEN** 该操作 MUST 被拒绝
- **AND** 该开发者 MUST 先提出一份 OpenSpec change 用于修改本 requirement 与 `project-layout` 的相关规定

---

### Requirement: Alignment with Lighthearted Vision

主角的移动与相机能力 MUST 与 `gameplay-vision` 已固化的基线严格一致：

- MUST NOT 为主角引入生命值、体力值、耐力槽等可被耗尽并导致玩家失败的资源。
- MUST NOT 引入"走累了掉血"、"掉落一定高度即死亡"等会导致玩家进度损失或触发失败状态的机制。
- MUST NOT 为常规移动操作施加任何形式的通缉值 / 惩罚累积。
- 相机表现 MUST 维持 `gameplay-vision` 所追求的"轻松、无压力"情绪基调，MUST NOT 引入令人紧张的镜头抖动、频繁闪切、强制视角控制等。

#### Scenario: 某提案试图为"跑步消耗体力"设计机制

- **WHEN** 某后续 change 提议为主角新增"体力槽"，体力耗尽时强制停步或缓行若干秒
- **THEN** 该 change MUST 被视为与本 requirement 以及 `gameplay-vision` 的 "No Failure or Penalty Mechanics" 冲突
- **AND** 该 change 在未先修改上述两个 requirement 之前 MUST NOT 被合入

#### Scenario: 主角从高处跌落

- **WHEN** 主角从一处高台跌落到下方地面
- **THEN** 主角 MUST 直接恢复正常的地面移动
- **AND** 游戏 MUST NOT 触发任何形式的死亡、重生、进度回滚或扣减
