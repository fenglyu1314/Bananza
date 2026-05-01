## 1. 前置与项目设置

- [x] 1.1 在团结引擎编辑器中打开项目，通过 Package Manager 确认已安装 `com.unity.inputsystem`；若未安装则安装之
- [x] 1.2 在 `Project Settings → Player → Active Input Handling` 中切换到 `Input System Package (New)` 或 `Both`，并让编辑器重启生效
- [x] 1.3 在 `Docs/EngineNotes_Tuanjie.md` 中追加一条记录，说明本次启用 Input System 的设置项与团结引擎版本

## 2. 目录结构

- [x] 2.1 在团结引擎编辑器中创建目录 `Assets/Scripts/Runtime/Gameplay/Player/`（不要手工创建 `.meta`)
- [x] 2.2 如尚不存在，创建目录 `Assets/Scenes/`、`Assets/Prefabs/`、`Assets/Settings/Input/`
## 3. 输入动作表

- [x] 3.1 在 `Assets/Settings/Input/` 下通过编辑器创建一个 `InputActionAsset`，命名为 `PlayerInputActions.inputactions`
- [x] 3.2 在该 asset 中新增 Action Map `Player`，并定义 `Vector2` 动作 `Move`
- [x] 3.3 为 `Move` 绑定键鼠（WASD / 方向键）与常见手柄（左摇杆）两套 Binding
- [x] 3.4 勾选 `Generate C# Class` 并生成对应的包装类，确保包装类落在 `Bananza.Gameplay.Player` 命名空间下

## 4. 角色控制脚本

- [x] 4.1 新建 `Assets/Scripts/Runtime/Gameplay/Player/PlayerController.cs`，声明 `namespace Bananza.Gameplay.Player`
- [x] 4.2 在 `PlayerController` 中挂接 `CharacterController` 组件引用与 `Camera` 引用（可 Inspector 绑定或 `FindObjectOfType` 作为兜底）
- [x] 4.3 实现"读取 `Move` 动作 → 按相机朝向解算世界方向 → 调用 `CharacterController.Move()` 位移"的主循环，暴露 `moveSpeed` 字段
- [x] 4.4 在无地面支撑时对主角施加重力加速度；接地状态通过 `CharacterController.isGrounded` 判断
- [x] 4.5 根据当前帧位移方向平滑旋转主角 `transform` 的 Y 轴朝向，暴露 `turnSmoothTime` 字段；无位移时保持当前朝向
- [x] 4.6 确保脚本中不出现对旧版 `UnityEngine.Input.GetAxis` / `Input.GetKey` 的直接调用

## 5. 跟随相机脚本

- [x] 5.1 新建 `Assets/Scripts/Runtime/Gameplay/Player/PlayerFollowCamera.cs`，声明 `namespace Bananza.Gameplay.Player`
- [x] 5.2 暴露 `target`（`Transform`）、`offset`（`Vector3`）、`followSmoothTime`（`float`）、`lookAtHeight`（`float`）等 Inspector 字段
- [x] 5.3 在 `LateUpdate` 中用 `Vector3.SmoothDamp`（或等价平滑）把相机位置向 `target.position + offset` 收敛
- [x] 5.4 在 `LateUpdate` 末尾让相机朝向 `target.position + Vector3.up * lookAtHeight` 看齐
- [x] 5.5 处理 `target` 为 `null` 的防御分支：不进行跟随，不抛异常

## 6. 场景与 Prefab 搭建（在团结引擎编辑器中完成，不得手工生成 `.meta`）

- [x] 6.1 新建场景 `Assets/Scenes/Sandbox_P1.unity`，保存为默认打开场景
- [x] 6.2 在场景中放置一块足够大的平坦地面（Plane / Cube 均可）、若干作为参照物的墙体或箱体 primitive、至少一个有落差的小平台
- [x] 6.3 在场景中创建主角 GameObject：胶囊体视觉 + `CharacterController` + `PlayerController` + `PlayerInput`（引用 3.1 创建的 asset，Behavior = `Send Messages` 或 `Invoke Unity Events`，由 `PlayerController` 通过 `InputAction` 回调消费）
- [x] 6.4 将主角 GameObject 拖到 `Assets/Prefabs/` 下保存为 `Player.prefab`，并在场景中以 prefab 实例替换原 GameObject
- [x] 6.5 在场景中创建主相机（或复用默认 `Main Camera`），挂 `PlayerFollowCamera`，将 `target` 设为主角 prefab 实例
- [x] 6.6 在 `PlayerController` 上把相机引用指向该主相机，保证"按下前"解算为相机正前方
## 7. 冒烟验证（对照 spec 的 Scenario）

- [x] 7.1 在编辑器中进入 `Sandbox_P1` 场景 Play，手柄推满左摇杆"前"，主角沿相机前方匀速前进且松手后短时间内停止（对应 *Ground Movement* 场景 1）
- [x] 7.2 输入右上 45°，主角以"前 + 右"合成方向移动，且移动速率不超过单方向最大速率（场景 2）
- [x] 7.3 主角撞墙被阻挡且不穿模（场景 3）
- [x] 7.4 主角从小平台边缘走出后坠落并在落地后恢复正常移动（场景 4）
- [x] 7.5 连续改变方向时主角朝向平滑切换，无瞬移式旋转或抖动；松手静止后朝向保持不动（*Character Facing* 两个场景）
- [x] 7.6 相机全程平滑跟随主角，急停后短时间内收敛到对准静止主角，无明显过冲（*Third-Person Follow Camera* 三个场景）
- [x] 7.7 拔出手柄改用键鼠继续上述验证，主角移动表现一致（*Input Mapping* 两个场景）
## 8. OpenSpec 校验与提交

- [x] 8.1 运行 `openspec validate add-character-movement --strict`，确保 change 校验通过
- [x] 8.2 在团结引擎编辑器中 `File → Save Project`，让编辑器为本次新增的资源生成 `.meta` 文件
- [x] 8.3 用 `git status` 检查新增文件齐全：脚本、场景、prefab、InputActionAsset 与各自的 `.meta` 均由编辑器生成；无手工创建的 `.meta`
- [x] 8.4 使用 Conventional Commits 提交：`feat(player): add basic ground movement and follow camera`
