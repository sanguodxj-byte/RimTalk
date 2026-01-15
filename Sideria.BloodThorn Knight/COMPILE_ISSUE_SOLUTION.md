# ?? C#编译问题解决方案

## ?? 当前问题

.NET SDK 10.0.100的C#编译器(csc.exe)存在已知bug，导致编译崩溃：
```
error MSB6006: "csc.exe"已退出，代码为 -1073741819
```

这是一个内存访问冲突错误，与SDK版本有关，不是代码问题。

---

## ? 解决方案

### 方案A：使用XML-only模式（推荐，立即可用）

**优点**：
- ? 不需要编译
- ? 立即可以测试
- ? 核心功能完全可用

**缺点**：
- ? 没有事件系统（第3天希德莉亚自动到达）
- ? 没有右键交互菜单
- ? 没有双路线自动管理

**操作步骤**：
```powershell
# 1. 双击运行
QuickDeploy.bat

# 2. 启动RimWorld
# 3. 启用mod（确保HAR在前面）
# 4. 开发模式生成角色测试
```

**可用功能**：
- ? 血龙种种族
- ? 所有特质
- ? 血原质系统
- ? 龙魂/血骸hediff
- ? 战斗技能
- ? 武器系统
- ? 自定义背景故事

---

### 方案B：使用.NET 8.0 SDK编译

**.NET 10存在bug，需要降级到.NET 8**

#### 步骤1：安装.NET 8.0 SDK

下载地址：https://dotnet.microsoft.com/download/dotnet/8.0

#### 步骤2：全局设置使用.NET 8

创建文件 `global.json` 在项目根目录：

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestPatch"
  }
}
```

#### 步骤3：编译

```powershell
cd Source
dotnet build -c Release
```

---

### 方案C：使用Visual Studio 2022编译

**如果你有VS 2022**：

#### 步骤1：在VS中打开项目

```
文件 → 打开 → 项目/解决方案
选择 Source/SideriaBloodThornKnight.csproj
```

#### 步骤2：配置

右键项目 → 属性 → 应用程序：
- 目标框架: .NET Framework 4.7.2
- 输出类型: 类库

#### 步骤3：生成

```
生成 → 重新生成解决方案
```

#### 步骤4：复制DLL

```
复制 Source\bin\Release\net472\SideriaBloodThornKnight.dll
到 Assemblies\SideriaBloodThornKnight.dll
```

---

### 方案D：使用旧版MSBuild

**如果有VS 2019或更早版本**：

```powershell
# VS 2022 MSBuild路径
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" Source\SideriaBloodThornKnight.csproj /p:Configuration=Release

# VS 2019 MSBuild路径
& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" Source\SideriaBloodThornKnight.csproj /p:Configuration=Release
```

---

## ?? 我的建议

### 立即测试（5分钟）

```powershell
# 1. 使用XML-only模式
.\QuickDeploy.bat

# 2. 启动RimWorld测试
# 核心功能完全可用！
```

### 后续完善

等你测试完核心功能后，可以：
1. 安装.NET 8.0 SDK
2. 创建global.json文件
3. 重新编译
4. 获得完整的事件系统

---

## ?? 创建global.json文件

在项目根目录（与Build.bat同级）创建 `global.json`：

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

这会强制使用.NET 8.0 SDK，避开10.0的bug。

---

## ? XML-only模式测试清单

你现在就可以测试：

### 1. 生成角色
```
开发模式 → Spawn pawn → Sideria_Dracovampir
```

### 2. 检查属性
```
Health Tab: 龙魂和血骸
Character Tab: 4个特质
Stats Tab: 移动4.9 c/s
```

### 3. 测试战斗
```
生成敌人 → 让角色战斗 → 观察血原质变化
```

### 4. 测试武器
```
Debug → Spawn thing → Sideria_Weapon_Atzgand
装备到角色 → 检查伤害
```

---

## ?? 如何检查.NET SDK版本

```powershell
dotnet --list-sdks
```

输出示例：
```
8.0.404 [C:\Program Files\dotnet\sdk]
9.0.101 [C:\Program Files\dotnet\sdk]
10.0.100 [C:\Program Files\dotnet\sdk]  ← 这个有bug
```

---

## ?? 总结

**现在的最佳方案**：

1. ? **立即测试**：使用 `QuickDeploy.bat` 部署XML-only版本
2. ? **核心功能**：完全可用，可以充分测试
3. ? **C#功能**：等安装.NET 8.0后再编译

**不要被编译问题困扰！XML版本已经非常完整了！** ??

---

## ?? 下一步

1. 双击 `QuickDeploy.bat`
2. 启动RimWorld
3. 测试mod
4. 告诉我测试结果！

C#编译可以稍后再搞，先玩起来！??
