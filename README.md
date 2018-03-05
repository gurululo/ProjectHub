# ProjectHub 企业项目管理系统

ProjectHub 是一个面向企业内部的项目管理后台，用于集中维护项目信息、跟踪任务执行情况。系统采用 ASP.NET Core MVC 2.1 单体架构，页面通过 Razor Views 服务端渲染，数据以 JSON 文件形式保存在本地，不依赖数据库。

> 当前进度：项目基础框架与页面布局已完成，管理功能将在后续阶段逐步加入。

## 技术架构

| 层次 | 选型 |
| --- | --- |
| 后端 | C# / ASP.NET Core MVC 2.1 |
| 视图 | Razor Views（服务端渲染） |
| 前端 | Bootstrap 4、jQuery |
| 数据 | 本地 JSON 文件（无数据库） |

## 目录结构

```text
ProjectHub.sln
src/ProjectHub.Web/
├── Controllers/          # MVC 控制器
├── Models/               # 实体与视图模型
├── Views/                # Razor 页面
├── wwwroot/              # 静态资源（CSS、JS、Bootstrap、jQuery）
├── App_Data/             # 运行时 JSON 数据目录
├── Program.cs            # 应用入口
└── Startup.cs            # 启动配置
```

## 环境要求

- .NET Core SDK 2.1.x
- 现代浏览器（Chrome、Edge、Firefox 等）

## 本地启动

```bash
dotnet restore
dotnet run --project src/ProjectHub.Web
```

启动后访问：<http://localhost:5000>

## 说明

`App_Data` 目录中的 JSON 数据文件在运行过程中自动创建，并已在 `.gitignore` 中排除，请勿将实际运行数据提交到仓库。
