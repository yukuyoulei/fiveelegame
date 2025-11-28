# Unity 五行元素游戏迁移指南

## 概述

本项目已成功将网页版五行元素游戏迁移到Unity中，并实现了自动生成预设的功能。

## 核心组件

### 1. 单机游戏管理器 (OfflineGameManager)
- **文件**: `Assets/Scripts/Managers/OfflineGameManager.cs`
- **功能**: 管理游戏状态、玩家数据、场景切换
- **特性**: 
  - 玩家移动和场景切换
  - 任务系统管理
  - 资源采集和战斗系统
  - UI更新

### 2. 任务管理器 (TaskManager)
- **文件**: `Assets/Scripts/Managers/TaskManager.cs`
- **功能**: 生成和管理游戏任务
- **任务类型**:
  - 采集任务 (Collect)
  - 击杀任务 (Kill)
  - 对话任务 (Talk)

### 3. 技能管理器 (SkillManager)
- **文件**: `Assets/Scripts/Managers/SkillManager.cs`
- **功能**: 管理玩家技能系统
- **技能类型**:
  - 心法 (Mind): 增加元素获取
  - 外功 (Body): 增加战斗能力
  - 突破系统: 每5级需要突破

### 4. 场景管理器 (GameSceneManager)
- **文件**: `Assets/Scripts/Managers/SceneManager.cs`
- **功能**: 动态生成游戏场景
- **场景层次**:
  - 背景层: 远山
  - 中景层: 树木
  - 前景层: 草地纹理
- **特性**: 视差滚动效果

### 5. 玩家控制器 (PlayerController)
- **文件**: `Assets/Scripts/PlayerController.cs`
- **功能**: 处理玩家输入和行为
- **控制**:
  - WASD/方向键: 移动
  - 空格: 攻击
  - E: 采集
  - F: 对话
  - W/A/S/D (边界): 场景切换

### 6. 游戏对象
- **ResourceObject**: 资源采集对象
- **MonsterObject**: 怪物对象
- **NPCObject**: NPC对话对象
- **ProgressBar**: 进度条组件

### 7. 游戏引导器 (GameBootstrap)
- **文件**: `Assets/Scripts/GameBootstrap.cs`
- **功能**: 初始化整个游戏系统
- **职责**: 创建管理器、设置UI、生成场景

## 自动预设生成

### 预设生成器 (PrefabGenerator)
- **文件**: `Assets/Scripts/Editor/PrefabGenerator.cs`
- **菜单**: `Five Elements/Generate Prefabs`
- **功能**: 自动生成所有必需的游戏预设
- **生成内容**:
  - 玩家预设
  - 资源预设
  - 怪物预设
  - NPC预设
  - UI面板预设
  - 漂浮文字预设

## 使用方法

### 1. 基本设置
1. 在Unity中创建新场景
2. 添加空的GameObject
3. 添加`GameBootstrap`组件
4. 运行场景

### 2. 生成预设
1. 在Unity编辑器中点击菜单 `Five Elements/Generate Prefabs`
2. 选择要生成的预设类型
3. 点击"Generate All Prefabs"
4. 预设将保存到`Assets/Prefabs/`目录

### 3. 游戏控制
- **移动**: WASD或方向键
- **攻击**: 空格键
- **采集**: E键
- **对话**: F键
- **技能**: 点击技能按钮
- **场景切换**: 移动到场景边界并按相应方向键

## 游戏机制

### 五行元素系统
- **金**: 灰色资源
- **木**: 绿色资源
- **水**: 蓝色资源
- **火**: 红色资源
- **土**: 棕色资源

### 技能升级
- **心法**: 增加元素获取量
- **外功**: 增加攻击力和攻击速度
- **突破**: 每5级需要突破，消耗元素

### 任务系统
- 完成当前任务后才能切换场景
- 任务类型随机生成
- 任务难度随距离增加

### 场景生成
- 无限2D世界
- 场景内容根据世界坐标动态生成
- 视差滚动效果

## 技术特性

### 性能优化
- 对象池管理
- 动态场景生成
- 视差滚动

### 可扩展性
- 模块化设计
- 组件化架构
- 易于添加新功能

### 跨平台
- Unity引擎支持
- 支持多平台发布

## 文件结构
```
Assets/Scripts/
├── Managers/
│   ├── OfflineGameManager.cs
│   ├── TaskManager.cs
│   ├── SkillManager.cs
│   └── SceneManager.cs
├── Editor/
│   └── PrefabGenerator.cs
├── PlayerController.cs
├── ResourceObject.cs
├── MonsterObject.cs
├── NPCObject.cs
├── ProgressBar.cs
└── GameBootstrap.cs
```

## 注意事项

1. 需要Unity 2020.3或更高版本
2. 预设生成需要编辑器环境
3. 首次运行需要生成预设
4. 建议使用2D模板项目

## 后续开发

1. 添加音效和音乐
2. 增加更多游戏对象类型
3. 实现保存/加载系统
4. 添加多人模式
5. 优化性能和内存使用