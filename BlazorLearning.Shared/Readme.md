# BlazorLearning - 权限框架学习项目

## 📖 项目概述

**BlazorLearning** 是一个完整的 Blazor 权限框架学习项目，通过20天的系统化学习，掌握现代Web应用的认证授权体系开发。

- **项目名称**：BlazorLearning
- **学习目标**：掌握Blazor权限框架开发
- **开始时间**：2025年6月26日
- **预计完成**：2025年7月15日

## 🏗️ 技术架构

### 核心技术栈
```
前端：Blazor Server/WebAssembly
后端：ASP.NET Core 9.0 Web API
数据库：PostgreSQL
ORM：FreeSql
认证：JWT + BCrypt
文档：OpenAPI + Scalar
日志：Serilog
```

### 架构图
```
┌─────────────────┐    HTTP/HTTPS    ┌──────────────────┐
│   Blazor Client │ ←──────────────→ │   Web API        │
│                 │                  │                  │
│ - UI Components │                  │ - Controllers    │
│ - State Mgmt    │                  │ - JWT Auth       │
│ - HTTP Client   │                  │ - Repositories   │
└─────────────────┘                  └──────────────────┘
                                               │
                                               ▼
                                      ┌──────────────────┐
                                      │   PostgreSQL     │
                                      │                  │
                                      │ - Users          │
                                      │ - Roles          │
                                      │ - Permissions    │
                                      └──────────────────┘
```

## 🎯 学习路线

### 阶段一：后端API基础（Day 1-4）✅
- [x] ASP.NET Core Web API项目搭建
- [x] FreeSql + PostgreSQL数据访问层
- [x] RESTful API设计和CRUD操作
- [x] 全局异常处理和日志系统
- [x] API文档和统一响应格式

### 阶段二：认证授权基础（Day 5-8）🔄
- [x] **Day 5**：JWT认证实现 ✅
- [ ] **Day 6**：角色权限系统
- [ ] **Day 7**：权限细化控制
- [ ] **Day 8**：认证优化和安全加固

### 阶段三：Blazor前端开发（Day 9-14）
- [ ] Blazor项目创建和基础配置
- [ ] API集成和数据展示
- [ ] 用户界面和CRUD操作
- [ ] 前后端认证集成
- [ ] 权限控制和路由保护

### 阶段四：权限框架完善（Day 15-18）
- [ ] 权限管理界面
- [ ] 高级权限功能
- [ ] 系统性能优化
- [ ] 操作日志和审计

### 阶段五：项目部署（Day 19-20）
- [ ] 容器化部署
- [ ] 生产环境配置
- [ ] 性能测试和文档

## 📊 当前进度

**总体进度**：5/20天 (25%)  
**当前阶段**：阶段二 - JWT认证实现 ✅ 已完成  
**下一步**：Day 6 - 角色权限系统

### 最新成就（Day 5）
- ✅ 完整的JWT认证系统
- ✅ 用户注册登录API
- ✅ BCrypt密码加密
- ✅ 受保护接口权限控制
- ✅ Claims用户信息解析
- ✅ 完整的认证测试流程

## 🚀 快速开始

### 环境要求
- .NET 9.0 SDK
- PostgreSQL 12+
- Visual Studio 2022 或 VS Code

### 安装步骤

1. **克隆项目**
```bash
git clone https://github.com/your-username/BlazorLearning.git
cd BlazorLearning
```

2. **配置数据库**
```bash
# 创建PostgreSQL数据库
createdb blazorlearning
```

3. **更新连接字符串**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blazorlearning;Username=postgres;Password=123456"
  }
}
```

4. **安装依赖包**
```bash
cd BlazorLearning.Api
dotnet restore
```

5. **启动项目**
```bash
dotnet run
```

6. **访问API文档**
```
https://localhost:7157/scalar/v1
```

## 🔧 技术文档

### API接口概览

#### 认证相关接口
```http
POST /api/auth/register    # 用户注册
POST /api/auth/login       # 用户登录
GET  /api/auth/profile     # 获取用户资料 [需要认证]
```

#### 测试接口
```http
GET /api/test/public       # 公开测试接口
GET /api/test/protected    # 受保护测试接口 [需要认证]
GET /api/test/user-info    # 用户信息接口 [需要认证]
```

#### 用户管理接口
```http
GET    /api/users          # 获取用户列表
GET    /api/users/{id}     # 获取单个用户
POST   /api/users          # 创建用户
PUT    /api/users/{id}     # 更新用户
DELETE /api/users/{id}     # 删除用户
```

### 数据库设计

#### Users 表
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT true
);
```

#### Roles 表（规划中）
```sql
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### JWT配置

#### appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast256Bits",
    "Issuer": "BlazorLearning.Api",
    "Audience": "BlazorLearning.Client",
    "ExpireHours": 24
  }
}
```

#### JWT Claims结构
```json
{
  "nameidentifier": "用户ID",
  "name": "用户名",
  "email": "邮箱地址",
  "userId": "自定义用户ID",
  "username": "自定义用户名",
  "exp": "过期时间戳",
  "iss": "发行者",
  "aud": "受众"
}
```

### 认证使用示例

#### 用户注册
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "password": "123456",
  "email": "test@example.com"
}
```

#### 用户登录
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "123456"
}
```

#### 访问受保护接口
```http
GET /api/auth/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 项目结构
```
BlazorLearning/
├── BlazorLearning.Api/          # Web API项目
│   ├── Controllers/             # API控制器
│   ├── Services/               # 业务服务
│   ├── Extensions/             # 扩展方法
│   └── Program.cs              # 程序入口
├── BlazorLearning.Shared/       # 共享项目
│   ├── Models/                 # 数据模型
│   └── Services/               # 共享服务
└── BlazorLearning.Web/          # Blazor前端（待开发）
```

## 🔐 安全特性

### 已实现
- ✅ **BCrypt密码加密** - 行业标准密码哈希算法
- ✅ **JWT Token认证** - 无状态认证机制
- ✅ **Claims身份声明** - 用户信息安全传递
- ✅ **API权限控制** - `[Authorize]`特性保护
- ✅ **Token过期管理** - 24小时自动过期
- ✅ **统一错误处理** - 安全的错误信息返回

### 计划实现
- [ ] **角色权限控制** - 基于角色的访问控制
- [ ] **Token刷新机制** - 自动续期和安全退出
- [ ] **API限流保护** - 防止暴力攻击
- [ ] **操作日志审计** - 用户行为追踪
- [ ] **多设备登录控制** - 设备管理和安全策略

## 📈 性能指标

### 当前性能
- **API响应时间** < 100ms
- **JWT Token生成** < 10ms
- **密码加密时间** < 50ms
- **数据库查询** < 20ms

### 优化目标
- **并发支持** > 1000 req/s
- **内存使用** < 512MB
- **启动时间** < 3s

## 🧪 测试

### API测试
使用Scalar界面进行交互式测试：
1. 访问 `https://localhost:7157/scalar/v1`
2. 测试用户注册和登录
3. 获取JWT Token
4. 测试受保护接口

### 测试用例覆盖
- ✅ 用户注册（成功/失败场景）
- ✅ 用户登录（成功/失败场景）
- ✅ JWT Token验证
- ✅ 受保护接口访问控制
- ✅ Claims信息解析

## 📚 学习资源

### 官方文档
- [ASP.NET Core文档](https://docs.microsoft.com/aspnet/core)
- [JWT官方规范](https://jwt.io/)
- [FreeSql文档](https://freesql.net/)
- [Blazor文档](https://docs.microsoft.com/aspnet/core/blazor)

### 相关教程
- [ASP.NET Core Web API教程](https://docs.microsoft.com/learn/paths/create-web-api-with-aspnet-core/)
- [JWT认证最佳实践](https://auth0.com/blog/a-look-at-the-latest-draft-for-jwt-bcp/)

## 📋 代码规范

### Git提交格式
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type类型
- `feat`: 新功能
- `fix`: 修复bug
- `docs`: 文档修改
- `refactor`: 代码重构
- `test`: 测试相关

### 示例
```
feat(auth): 完成JWT认证系统实现

- 实现JWT Token生成和验证服务
- 添加用户注册和登录API
- 集成BCrypt密码加密
- 配置JWT认证中间件

Closes #5
```

## 🤝 贡献指南

### 开发流程
1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

### 代码质量
- 遵循 C# 编码规范
- 添加必要的单元测试
- 更新相关文档
- 确保所有测试通过

## 📄 许可证

本项目采用 [MIT License](LICENSE) 许可证。

## 📞 联系方式

- **项目地址**：[GitHub Repository](https://github.com/your-username/BlazorLearning)
- **问题反馈**：[Issues](https://github.com/your-username/BlazorLearning/issues)
- **学习记录**：查看 [PROGRESS.md](./PROGRESS.md)
- **问题解决**：查看 [PROBLEMS.md](./PROBLEMS.md)

---

**最后更新**：2025年6月30日  
**当前版本**：v0.2.0 (Day 5 - JWT认证完成)  
**下一个里程碑**：v0.3.0 (Day 8 - 完整认证授权系统)