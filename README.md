# Five Elements Game

一个基于五行理论的在线多人游戏，采用Unity客户端和ASP.NET Core服务器架构。

## 游戏概述

### 核心机制
- **五行系统**: 金、木、水、火、土五种元素，具有相生相克关系
- **体力系统**: 玩家移动和行动消耗体力，可自动恢复
- **无限地图**: 以原点(0,0)为中心的无限二维地图
- **实时多人**: 通过WebSocket实现的实时多人游戏

### 玩法要素
- **炼体**: 增加五行属性上限
- **炼心**: 增加五行属性值
- **采集**: 消耗体力采集五行矿物
- **战斗**: 消耗五行值攻击怪物
- **探索**: 移动到新区域自动生成内容

## 技术架构

### 前端 (Unity)
- **语言**: C#
- **网络**: WebSocketSharp
- **架构**: 组件化管理器模式
- **主要组件**:
  - GameManager: 游戏状态管理
  - NetworkManager: 网络通信
  - MapManager: 地图渲染
  - UIManager: 用户界面

### 后端 (ASP.NET Core)
- **语言**: C#
- **框架**: .NET 8.0
- **网络**: WebSocket
- **架构**: 分层服务架构
- **主要组件**:
  - ConnectionManager: WebSocket连接管理
  - GameWorldService: 游戏世界状态
  - GameLogicService: 游戏逻辑计算
  - MonsterMovementService: 怪物AI服务

### 共享库
- **位置**: `src/FiveElements.Shared/`
- **内容**: 游戏模型、消息定义、服务接口
- **目的**: 前后端共享游戏逻辑和数据结构

## 五行关系

### 相生关系
- 金生水
- 水生木
- 木生火
- 火生土
- 土生金

### 相克关系
- 金克木
- 木克土
- 土克水
- 水克火
- 火克金

## 游戏机制详解

### 主五行选择
- 玩家首次登录选择主五行
- 主五行炼体/炼心暴击率: 30%
- 其他五行暴击率: 5%

### 体力系统
- 初始体力: 100
- 移动消耗: 1体力
- 采集消耗: 2体力
- 自动恢复: 每5秒1点

### 地图生成
- 出生点: (0,0)
- 视野范围: 3x3格子
- 内容等级: 根据距离原点的距离计算
- 生成概率: 60%矿物, 40%怪物

### 怪物行为
- 移动周期: 每10秒
- 进化周期: 每5分钟
- 吸收机制: 移动到矿物格子会吸收并进化
- 进化逻辑: 基于五行相生相克

## 快速开始

### 服务器端
```cmd
cd src\FiveElements.Server
dotnet run
```

或者使用开发脚本：
```cmd
dev.bat server
```

### 客户端
1. 在Unity中打开`unity/`目录
2. 导入WebSocketSharp包
3. 配置服务器地址
4. 运行场景

## API接口

### WebSocket连接
- 端点: `ws://localhost:5000/ws`
- 协议: JSON消息格式

### REST API
- `GET /api/game/players` - 获取在线玩家列表
- `GET /api/game/world/stats` - 获取世界统计信息

## 消息类型

### 客户端到服务器
- `PlayerSelectMainElementMessage` - 选择主五行
- `PlayerMoveMessage` - 移动
- `PlayerHarvestMessage` - 采集
- `PlayerAttackMessage` - 攻击
- `PlayerTrainBodyMessage` - 炼体
- `PlayerTrainMindMessage` - 炼心

### 服务器到客户端
- `WelcomeMessage` - 欢迎消息
- `MapUpdateMessage` - 地图更新
- `PlayerStatsUpdateMessage` - 玩家状态更新
- `TrainingResultMessage` - 训练结果
- `HarvestResultMessage` - 采集结果
- `AttackResultMessage` - 攻击结果
- `PlayerJoinedMessage` - 玩家加入
- `PlayerLeftMessage` - 玩家离开
- `ErrorMessage` - 错误消息

## 部署说明

### 生产环境配置
1. 配置WebSocket安全连接(WSS)
2. 设置数据库持久化
3. 配置负载均衡
4. 设置监控和日志

### 性能优化
- 地图数据分页加载
- WebSocket连接池管理
- 怪物AI批量处理
- 消息压缩和缓存

## 开发计划

### 短期目标
- [ ] 完善Unity UI界面
- [ ] 添加音效和特效
- [ ] 实现玩家交易系统
- [ ] 添加成就系统

### 长期目标
- [ ] 公会和团队系统
- [ ] 装备和道具系统
- [ ] 副本和Boss战
- [ ] 移动端支持

## 开发工具

### Windows开发脚本

#### 批处理文件 (Command Prompt)
- `dev.bat setup` - 设置开发环境
- `dev.bat build` - 构建解决方案
- `dev.bat server` - 启动服务器
- `dev.bat test` - 运行测试
- `test-server.bat` - 测试服务器连接

#### PowerShell脚本
- `.\dev.ps1 setup` - 设置开发环境
- `.\dev.ps1 build` - 构建解决方案
- `.\dev.ps1 server` - 启动服务器
- `.\dev.ps1 test` - 运行测试
- `.\dev.ps1 help` - 显示帮助信息

### 构建命令
```cmd
dotnet build                    # 构建整个解决方案
dotnet run --project src/FiveElements.Server  # 启动服务器
dotnet test                     # 运行所有测试
```

## 贡献指南

1. Fork项目
2. 创建功能分支
3. 提交更改
4. 创建Pull Request

## 许可证

MIT License