# Voice Tuner - TTS语音调节器

一个RimWorld mod，用于配置和调节AI叙事者人格的TTS语音参数。

## 功能特性

- 🎤 **多TTS后端支持**
  - Azure TTS（微软认知服务）- 支持完整SSML，情感风格，角色扮演
  - Fish Audio - 支持参考音色克隆
  - CosyVoice2（硅基流动）- 高质量中文语音
  - IndexTTS（本地/自托管）- 无需API密钥

- 🎛️ **可视化参数调节**
  - 语音选择（支持多语言）
  - 语速、音调、音量调节
  - 情感风格和强度（Azure专用）
  - 角色扮演模式（Azure专用）

- 🔊 **实时预览**
  - 一键测试当前配置
  - 自定义测试文本

- 📤 **配置导出**
  - 导出为NarratorPersonaDef兼容的XML格式
  - 直接注入到其他mod（如Sideria）

## 安装

1. 确保已安装 [Harmony](https://github.com/pardeike/HarmonyRimWorld)
2. 将 `VoiceTuner` 文件夹复制到 RimWorld 的 `Mods` 目录
3. 在游戏中启用mod

## 使用方法

### 打开调节器窗口

- **方式1**: 游戏设置 -> Mod设置 -> Voice Tuner -> "打开语音调节器窗口"
- **方式2**: 快捷键 `Ctrl+T`（可在设置中更改）

### 配置TTS

1. 在左侧人格列表中选择一个人格
2. 在右侧配置面板中选择TTS提供商
3. 填入API密钥（根据提供商要求）
4. 调节语音参数
5. 点击"播放测试"预览效果
6. 点击"保存配置"保存设置

### 导出配置

- **导出XML**: 将配置导出为独立XML文件（保存到桌面）
- **注入Mod**: 直接将TTS配置写入Sideria mod的XML文件

## TTS提供商配置指南

### Azure TTS

1. 注册 [Azure账户](https://azure.microsoft.com/)
2. 创建"语音服务"资源
3. 获取API密钥和区域
4. 在VoiceTuner中填入配置

支持的特殊功能：
- 情感风格（cheerful, sad, angry等）
- 情感强度（0.01-2.0）
- 角色扮演（Boy, Girl, OlderAdultMale等）

### Fish Audio

1. 注册 [Fish Audio](https://fish.audio/)
2. 获取API密钥
3. 获取参考音色ID（在网站上选择或上传音色）
4. 在VoiceTuner中填入配置

### CosyVoice2（硅基流动）

1. 注册 [硅基流动](https://siliconflow.cn/)
2. 获取API密钥
3. 在VoiceTuner中填入配置
4. 选择模型和语音

### IndexTTS（本地）

1. 部署IndexTTS服务（默认端口8080）
2. 在VoiceTuner中填入服务地址
3. 配置说话人ID（可选）

## 文件结构

```
VoiceTuner/
├── About/
│   └── About.xml
├── Assemblies/
│   └── VoiceTuner.dll
├── Languages/
│   ├── ChineseSimplified/
│   │   └── Keyed/
│   │       └── VoiceTuner_Keys.xml
│   └── English/
│       └── Keyed/
│           └── VoiceTuner_Keys.xml
├── Source/
│   └── VoiceTuner/
│       ├── Models/          # 数据模型
│       ├── Providers/       # TTS提供商实现
│       ├── Services/        # 导出服务
│       ├── UI/              # 游戏内窗口
│       └── *.cs             # 核心类
├── Build.ps1
└── README.md
```

## 编译

```powershell
# 编译Debug版本
.\Build.ps1

# 编译Release版本
.\Build.ps1 -Release

# 编译并部署到RimWorld
.\Build.ps1 -Release -Deploy
```

## 兼容性

- RimWorld 1.4, 1.5
- 需要 Harmony mod
- 可选依赖: The Second Seat（用于人格列表）

## 导出XML格式示例

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
    <defName>Sideria_Default</defName>
    
    <!-- TTS语音配置 -->
    <ttsVoiceName>zh-CN-XiaoxiaoNeural</ttsVoiceName>
    <ttsVoicePitch>1.0</ttsVoicePitch>
    <ttsVoiceRate>1.0</ttsVoiceRate>
    
    <!-- Azure TTS专用配置 -->
    <ttsAzureStyle>cheerful</ttsAzureStyle>
    <ttsAzureStyleDegree>1.20</ttsAzureStyleDegree>
    <ttsAzureRole>Girl</ttsAzureRole>
    
  </TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
</Defs>
```

## 许可证

MIT License

## 更新日志

### v1.0.0
- 初始版本
- 支持Azure TTS、Fish Audio、CosyVoice2、IndexTTS
- 游戏内配置UI
- XML导出和Mod注入功能